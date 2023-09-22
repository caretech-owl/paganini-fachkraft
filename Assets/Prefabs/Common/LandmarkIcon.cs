using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandmarkIcon : MonoBehaviour
{
    public Color BackgroundColor = Color.white;

    public enum LandmarkType {
        Placeholder = -1,
        PinLandmark = -2,
        PinReassurance = -3,
        Train = 1,
        Coffee = 2,
        Work = 3,
        Home = 4,
        Bus = 5,
        Park = 6
    }
    public LandmarkType SelectedLandmarkType = LandmarkType.Placeholder;
    private LandmarkType activeLandmarkType;

    // Start is called before the first frame update
    void Start()
    {
        ApplyColorToBackground();
        displayLandmarkType(activeLandmarkType);
    }

    // Update is called once per frame
    void Update()
    {
        // If a new selected landmark has been set, we activate it
        if (activeLandmarkType != SelectedLandmarkType)
        {
            displayLandmarkType(SelectedLandmarkType);
        }

    }

    // activates the selected landmark type
    void displayLandmarkType(LandmarkType selected)
    {
        for (int i=0; i< gameObject.transform.childCount; i++)
        {
            Transform ch = gameObject.transform.GetChild(i);
            ch.gameObject.SetActive(ch.name == selected.ToString());
        }

        activeLandmarkType = selected;
    }

    public void SetSelectedLandmark(int typeCode)
    {
        SelectedLandmarkType = (LandmarkType)typeCode;
    }

    private void ApplyColorToBackground()
    {
        // Find the "Background" components within the prefab
        //Transform[] backgroundComponents = gameObject.GetComponentsInChildren<Transform>(true);

        //foreach (Transform component in backgroundComponents)
        //{
        //    if (component.name == "Background")
        //    {
        //        Renderer renderer = component.GetComponent<Renderer>();

        //        if (renderer != null)
        //        {
        //            renderer.material.color = BackgroundColor;
        //        }
        //    }
        //}

        Image[] backgroundComponents = gameObject.GetComponentsInChildren<Image>(true);

        foreach (var component in backgroundComponents)
        {
            if (component.name == "Background")
            {
                component.color = BackgroundColor;                
            }
        }
    }

}
