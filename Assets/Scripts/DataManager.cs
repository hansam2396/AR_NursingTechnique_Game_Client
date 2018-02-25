﻿using System.Collections;
using UnityEngine;

/// <summary>
/// test ID : admin, pw : 123456, token : dtycveprbgjwhkqsfoal
/// </summary>
public enum RequestState
{
    None,
    listClinical,
    randomContent,
    randomItem,
    gameRecord
}

public class DataManager : MonoBehaviour
{
    public delegate void LoadingImageHandler(string success, Result result = null);
    public event LoadingImageHandler OnLoadingImage;

    public static DataManager instance = null;

    private RequestState requestState;
    private int requestCount=0;

    private string token = "dtycveprbgjwhkqsfoal";

    #region ClinicalURL Data & Property
    [SerializeField] private string signUpUrl = "http://52.78.158.73/user/signup.json?";
    [SerializeField] private string signInUrl = "http://52.78.158.73/user/signin.json?";
    [SerializeField] private string signOutUrl = "http://52.78.158.73/user/signout.json?token=";
    //간호술기 목차 보기 : "info" : {"listSize" : "목차 총 개수"}, "list" {"id": "순서","title":"제목","difficulty":"난이도:"}
    [SerializeField] private string listClinicalUrl = "http://52.78.158.73/game/list_clinical.json?token=";
    //"info" : {"id" : "해당 간호술기 번호(호출 번호)" ,"title" : "간호술기 제목" , "difficulty" : "난이도" } , " list" : { "index" : "순서" , "content" : "내용"}}}
    [SerializeField] private string randomContentUrl = "http://52.78.158.73/game/random_content.json?token";
    //"info" : {"id" : "해당 간호술기 번호(호출 번호)" ,"title" : "간호술기 제목" , "difficulty" : "난이도" } , " list" : { "name" : "아이템 명" , "rating" : "중요도"}}}
    //rating : 중요도, 필수-필수로 필요한 아이템, 항시-항시 준비되어 있는 물품, 혼동-헷갈리게 하는 물품
    [SerializeField] private string randomItemUrl = "http://52.78.158.73/game/random_item.json?token";
    [SerializeField] private string gameRecordUrl = "http://52.78.158.73/game/game_record.json?";

    public string Token
    {
        set
        {
            token = value;
        }
    }
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        requestState = RequestState.None;
       // SendListClinical();
    }
    
    public void SendSignUp(string id, string pw, string name, string job)
    {
        string url = signUpUrl + "uid=" + id + "&password=" + pw + "&name=" + name + "&age=" + 27;
        WWW www = new WWW(url);

        StartCoroutine(WaitForRequest(www));
    }

    public void SendSignIn(string id, string pw)
    {
        string url = signInUrl + "uid=" + id + "&password=" + pw;
        WWW www = new WWW(url);

        StartCoroutine(WaitForRequest(www));
    }

    public void SendListClinical()
    {
        requestState = RequestState.listClinical;

        string url = listClinicalUrl + token;
        WWW www = new WWW(url);

        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
            StartCoroutine(ReceiveData(www.text));
        }
        else
        {
            Debug.Log("WWW error: " + www.error);   // something wrong!
            if (GameObject.Find("LodingLayout").activeSelf)
            {
                if (OnLoadingImage != null)
                    OnLoadingImage(www.error);
            }
            else
                RequestAgain();
        }
    }

    private IEnumerator ReceiveData(string receiveData)
    {
        
        DataConfiguration config = JsonUtility.FromJson<DataConfiguration>(receiveData);

        if (receiveData.Contains("signup"))
        {
            yield return new WaitForSeconds(2f);
            if (OnLoadingImage != null)
                OnLoadingImage(config.Signup.Respon.Success);
        }
        else if(receiveData.Contains("signin"))
        {
            yield return new WaitForSeconds(2f);
            if (OnLoadingImage != null)
                OnLoadingImage(config.Signin.Respon.Success, config.Signin.Result);
        }
        else if(receiveData.Contains("list_clinical"))
        {
            if(config.List_clinical.Respon.Success.Equals("false"))
                StartCoroutine(LostToken());

        }
    }

    private void RequestAgain()
    {
        requestCount++;

        if (requestCount > 15)
        {
            string message = "서버가 불안정합니다. 잠시후 다시 시도하세요.";
            AlertViewController.Show("", message);
            return;
        }

        if (requestState == RequestState.listClinical)
            SendListClinical();

    }

    private IEnumerator LostToken()
    {
        string message = "계정 정보가 없습니다. 다시 로그인 해주세요.";
        AlertViewController.Show("", message);
        yield return new WaitForSeconds(1f);
        token = "";
        GameSceneManager.instance.ChangeScene(1);
    }
}

