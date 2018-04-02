﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Content : MonoBehaviour
{

    [SerializeField]
    private Image fill;

    [SerializeField]
    private float fillAmount = 1;

    [SerializeField]
    private GameObject button;
    [SerializeField]
    private GameObject GameStart;
    [SerializeField]
    private GameObject GameUI;

    [SerializeField]
    private Text timerText;
    [SerializeField] private Image[] lifeImg;
    [SerializeField] private Sprite[] lifeImgSource;

    private Image[] checkImg;
    [SerializeField] private Sprite[] checkImageSource;
    [SerializeField] private Sprite contentImage;

    private int life = 5;
    private Draggable draggable;

    [SerializeField] private Droppable droppable;
    private List<Image> baseItemImage = new List<Image>();
    private List<Image> necessaryItemImage = new List<Image>();
    private List<Image> confusionItemImage = new List<Image>();

    void Start()
    {
        GameUI.SetActive(true);
        droppable.OnSuccess += onSuccess;
        droppable.OnFaile += onFaile;
        droppable.OnNothing += onNothing;

        if (DataManager.instance.RequestState == RequestState.randomItem)
            SetRandoomItem();
        else
        {
            checkImg = new Image[DataManager.instance.OriginList.Count];
            for (int i=0; i< DataManager.instance.OriginList.Count; i++)
            {
                GameObject obj = new GameObject("marker");
                Image img = obj.AddComponent<Image>();
                obj.transform.SetParent(GameObject.Find("Item").transform);
                img.sprite = checkImageSource[0];
                checkImg[i] = img;
            }

            foreach(int index in ShuffleList(DataManager.instance.OriginList))
            {
                var r = UnityEngine.Random.Range(0, 2);
                if(r == 0)
                {
                    GameObject obj = new GameObject(index.ToString());
                    Image img = obj.AddComponent<Image>();
                    img.preserveAspect = true;
                    obj.AddComponent<Draggable>();
                    obj.transform.SetParent(GameObject.Find("Items").transform);
                    img.sprite = contentImage;
                }
                else
                {
                    GameObject obj = new GameObject(index.ToString());
                    Image img = obj.AddComponent<Image>();
                    img.preserveAspect = true;
                    obj.AddComponent<Draggable>();
                    obj.transform.SetParent(GameObject.Find("Itemss").transform);
                    img.sprite = contentImage;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fillAmount >= 0)
        {
            fillAmount = 1 - Time.time / 100;
            UpdateBar();
            //Debug.Log (fillAmount);

            string minutes = ((int)(fillAmount * 100) / 60).ToString();
            string seconds = ((int)(fillAmount * 100 - 60)).ToString();

            timerText.text = "00:0" + minutes + ":" + seconds;
        }
        else
            button.SetActive(true);
    }

    private void UpdateBar()
    {
        if (fillAmount != fill.fillAmount)
            fill.fillAmount = fillAmount;
    }

    public void onSuccess(GameObject obj)
    {
        draggable = obj.GetComponent<Draggable>();
        draggable.Success();
        if (DataManager.instance.RequestState == RequestState.randomItem)
        {
            if (DataManager.instance.NecessaryRating.Count == 0 && life > 0)
            {
                Debug.Log("성공");
            }
        }
        else
        {
            checkImg[droppable.markImageIndex - 1].sprite = checkImageSource[1];
            if(droppable.index == DataManager.instance.RandomContentList.Count)
            {
                Debug.Log("성공");
            }
        }
    }

    public void onFaile(GameObject obj = null)
    {
        draggable = obj.GetComponent<Draggable>();
        life--;
        draggable.Faile();
        SetLife();
    }

    public void onNothing(GameObject obj)
    {
        draggable = obj.GetComponent<Draggable>();
        draggable.Nothing();
    }

    public void SetLife()
    {
        if (life == 4)
        {
            lifeImg[0].sprite = lifeImgSource[1];
        }
        else if (life == 3)
        {
            lifeImg[1].sprite = lifeImgSource[1];
        }
        else if (life == 2)
        {
            lifeImg[2].sprite = lifeImgSource[1];
        }
        else if (life == 1)
        {
            lifeImg[3].sprite = lifeImgSource[1];
        }
        else if (life == 0)
        {
            lifeImg[4].sprite = lifeImgSource[1];
            StartCoroutine(GameSceneManager.instance.ChangeScene(2));
        }
    }

    private void SetRandoomItem()
    {
        int i = 0; int j = 0; int l = 0;

        for (int k = 0; k < DataManager.instance.BaseRating.Count; k++)
        {
            GameObject obj = new GameObject("item");
            Image img = obj.AddComponent<Image>();
            obj.transform.SetParent(GameObject.Find("Item").transform);
            baseItemImage.Add(img);
        }
        for (int o = 0; o < DataManager.instance.NecessaryRating.Count; o++)
        {
            GameObject obj = new GameObject("items");
            Image img = obj.AddComponent<Image>();
            obj.AddComponent<Draggable>();
            obj.transform.SetParent(GameObject.Find("Items").transform);
            necessaryItemImage.Add(img);
        }
        for (int p = 0; p < DataManager.instance.ConfusionRating.Count; p++)
        {
            GameObject obj = new GameObject("itemss");
            Image img = obj.AddComponent<Image>();
            obj.AddComponent<Draggable>();
            obj.transform.SetParent(GameObject.Find("Itemss").transform);
            confusionItemImage.Add(img);
        }

        foreach (string name in DataManager.instance.BaseRating)
        {
            baseItemImage[i].preserveAspect = true;
            baseItemImage[i].sprite = Resources.Load<Sprite>(name);
            i++;
        }
        foreach (string name in DataManager.instance.NecessaryRating)
        {
            necessaryItemImage[j].preserveAspect = true;
            necessaryItemImage[j].sprite = Resources.Load<Sprite>(name);
            j++;
        }
        foreach (string name in DataManager.instance.ConfusionRating)
        {
            confusionItemImage[l].preserveAspect = true;
            confusionItemImage[l].sprite = Resources.Load<Sprite>(name);
            l++;
        }
    }

    private List<int> ShuffleList(List<int> inputList)
    {
        var count = inputList.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = inputList[i];
            inputList[i] = inputList[r];
            inputList[r] = tmp;
        }
        return inputList;
    }
}