using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorHeaderPrefab : MonoBehaviour
{
    public TMPro.TMP_Text WayName;

    public LandmarkIcon IconStart;
    public LandmarkIcon IconDestination;

    // Start is called before the first frame update
    void Start()
    {
        WayName.text = AppState.CurrentWay.Name;
        IconStart.SetSelectedLandmark(Int32.Parse(AppState.CurrentWay.StartType));
        IconDestination.SetSelectedLandmark(Int32.Parse(AppState.CurrentWay.DestinationType));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
