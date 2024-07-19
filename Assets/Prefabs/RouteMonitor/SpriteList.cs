using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpriteList : MonoBehaviour
{
    [Header("Icons")]
    public List<MaskedIcon> Icons;



    // Start is called before the first frame update
    void Start()
    {    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RenderIcon(String iconName, Color color)
    {
        foreach (var icon in Icons)
        {
            icon.SelectIcon(icon.GetIconName() == iconName);
            if (icon.GetIconName() == iconName)
            {
                icon.ChangeFillColor(color);
            }
        }
    }

}
