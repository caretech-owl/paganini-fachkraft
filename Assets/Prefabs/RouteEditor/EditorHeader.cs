using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorHeader : MonoBehaviour
{
    public TMPro.TMP_Text HeaderText;

    private RouteSharedData SharedData;
    private Route.RouteStatus CurrentStatus;

    // Start is called before the first frame update
    void Awake()
    {
        SharedData = RouteSharedData.Instance;
    }

    private void Start()
    {
        UpdateHeader();
    }

    // Update is called once per frame
    void Update()
    {
        if (SharedData.CurrentRoute != null &&
            SharedData.CurrentRoute.Status != CurrentStatus) {
            UpdateHeader();
            CurrentStatus = SharedData.CurrentRoute.Status;
        }
    }


    void UpdateHeader() {

        HeaderText.text = "";
        if (SharedData.CurrentRoute == null)
        {
            return;
        }
        else if (SharedData.CurrentRoute.Status == Route.RouteStatus.New)
        {
            HeaderText.text = "Cleaning";
        }
        else if (SharedData.CurrentRoute.Status == Route.RouteStatus.DraftPrepared)
        {
            HeaderText.text = "Diskussion";
        }
    }
}
