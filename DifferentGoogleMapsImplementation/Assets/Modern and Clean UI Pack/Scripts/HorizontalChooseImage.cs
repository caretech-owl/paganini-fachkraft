using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalChooseImage : MonoBehaviour
{
    public GameObject ButtonRight, ButtonLeft;
    public TextMeshProUGUI indicator;
    public Image image;

    private int p_index = 0;
    public int index
    {
        get
        {
            return p_index;
        }
        set
        {
            p_index = value;
            StartCoroutine(DelayTextUpdate());
            return;
        }
    }
    IEnumerator DelayTextUpdate()
    {
        yield return new WaitForSeconds(0.3f);

        // fill list with given images
        if (objs != null && objs.Count > 0)
        {
            var _path = Application.streamingAssetsPath + "/" + objs[p_index];
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_path);
            www.SendWebRequest();
            while (!www.downloadHandler.isDone)
            {
            }

            image.sprite.texture.LoadImage(www.downloadHandler.data);

            indicator.text = (p_index + 1).ToString() + "/" + objs.Count;
        }
        else
        {
            indicator.text = "0";
        }
    }


    //You can access this property in different scripts
    //Example
    //public GameObject obj;
    //obj.GetComponent<HorizontalChoose>().value;
    public string value
    {
        get
        {
            return objs[p_index];
        }
    }

    public int DefaultValueIndex;
    public List<string> objs = new List<string>();
    void Start()
    {
        index = DefaultValueIndex;
        ButtonRight.GetComponent<Button>().onClick.AddListener(() =>
        {
            if ((index + 1) >= objs.Count)
            {
                index = 0;
            }
            else
            {

                index++;
            }
            return;
        });
        ButtonLeft.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (index == 0)
            {
                index = objs.Count - 1;
            }
            else
            {

                index--;
            }
            return;
        });
    }
}
