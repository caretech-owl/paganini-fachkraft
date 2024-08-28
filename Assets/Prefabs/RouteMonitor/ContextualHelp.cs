using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ContextualHelp : MonoBehaviour
{
    [Header("Help panels")]
    public GameObject OverviewHelp;
    public GameObject TimelineHelp;
    public GameObject MapHelp;

    [Header("Context views")]
    public GameObject OverviewView;
    public GameObject TimelineView;
    public RouteWalkMap MapHelpView;

    // Start is called before the first frame update
    void Start()
    {    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CloseHelp()
    {
        gameObject.SetActive(false);
        if (MapHelpView.gameObject.activeSelf)
        {
            MapHelpView.ToggleMapAsSnapshot(asSnapshot: false);
        }
    }

    public void RenderContextualHelp()
    {

        if (MapHelpView.gameObject.activeSelf)
        {
            MapHelpView.ToggleMapAsSnapshot(asSnapshot: true);
        }

        OverviewHelp.SetActive(OverviewView.activeSelf);
        TimelineHelp.SetActive(TimelineView.activeSelf);
        MapHelp.SetActive(MapHelpView.gameObject.activeSelf);

        gameObject.SetActive(true);
    }

}
