using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class RouteWalkOnboarding : MonoBehaviour
{
    [Header("UI States")]
    public GameObject LoadingState;
    public ButtonPrefab OkButton;

    [Header("Onboarding Screens")]    
    public GameObject OnboardNoData;
    public GameObject OnboardTraining;

    public UnityEvent OnUserConfirmed;


    // Start is called before the first frame update
    void Awake()
    {
        LoadView(LoadingState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadBusyView() {        
        gameObject.SetActive(true);
        LoadingState.SetActive(false);
        OkButton.RenderBusyState(true);
        //PopulateRouteOnboarding();
        
    }

    public void LoadNoData()
    {
        LoadView(OnboardNoData);
        LoadReadyView();
    }

    public void LoadOnboarding()
    {
        LoadView(OnboardTraining);
    }

    public void LoadReadyView()
    {
        OkButton.RenderBusyState(false);
    }

    public void UserConfirmed()
    {
        if (OnboardTraining.activeSelf)
        {
            OnUserConfirmed?.Invoke();
        }
        else
        {
            SceneSwitcher.LoadRouteExplorer();
        }
        
    }

    /// <summary>
    /// Populates the Route Onboarding view based on the status of the Route
    /// </summary>
    //public void PopulateRouteOnboarding()
    //{
    //    //SharedData current route

    //    OnboardCleaning.SetActive(SharedData.CurrentRoute.Status == Route.RouteStatus.New);
    //    OnboardDiscussion.SetActive(SharedData.CurrentRoute.Status == Route.RouteStatus.DraftPrepared);
    //    OnboardTraining.SetActive(SharedData.CurrentRoute.Status == Route.RouteStatus.Training);
    //}

    public void LoadView(GameObject view)
    {
        OnboardNoData.SetActive(OnboardNoData == view);
        OnboardTraining.SetActive(OnboardTraining == view);
        LoadingState.SetActive(LoadingState == view);
    }
}
