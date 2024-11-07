using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastItem : MonoBehaviour
{
    public TMPro.TMP_Text ToastTitle;
    public TMPro.TMP_Text ToastDescription;
    

    // Start is called before the first frame update
    void Start()
    {  
    }

    public void FillToast(string title, string description)
    {
        ToastTitle.text = title;
        ToastDescription.text = description;
    }


}
