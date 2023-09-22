using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class RouteStepChange : MonoBehaviour
{

    [Header("UI States")]
    public GameObject DataState;
    public GameObject ErrorState;

    [Header("UI Controls")]
    public ToggleGroup ProcessStepGroup;
    public ButtonPrefab ButtonSave;

    private Route CurrentRoute;


    private RouteSharedData SharedData;

    void Awake()
    {
        SharedData = RouteSharedData.Instance;
        SharedData.OnDataUploadError.AddListener(RouteSharedData_OnDataUploadError);
    }


    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in ProcessStepGroup.GetComponentsInChildren<Toggle>())
        {
            item.onValueChanged.AddListener(delegate {
                if (item.isOn)
                {
                    SaveRouteChange();
                }
            });
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadRouteStepChange(Route route)
    {
        DataState.SetActive(true);
        ErrorState.SetActive(false);

        CurrentRoute = route;
        string step = "";
        
        if (route.Status == Route.RouteStatus.New)
        {
            step = "Cleaning";
        }
        else if (route.Status == Route.RouteStatus.DraftPrepared)
        {
            step = "Discussion";
        }
        else if (route.Status == Route.RouteStatus.Training)
        {
            step = "Training";
        }

        foreach (Toggle toggle in ProcessStepGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.isOn = toggle.name.Contains(step);           
        }
    }


    Route.RouteStatus GetRouteStatus()
    {

        foreach (Toggle toggle in ProcessStepGroup.ActiveToggles())
        {
            if (toggle.name.Contains("Cleaning"))
            {
                return Route.RouteStatus.New;
            }
            else if (toggle.name.Contains("Discussion"))
            {
                return Route.RouteStatus.DraftPrepared;
            }
            //TODO: In the future, we would actually need to do some personalisation
            // of the instructions in the adaptation interface before publishing
            else //if (toggle.name.Contains("Training"))
            {
                return Route.RouteStatus.Training;
            }

        }

        return Route.RouteStatus.New;
    }

    private void SaveRouteChange() {
        CurrentRoute.Status = GetRouteStatus();
        CurrentRoute.InsertDirty();

        // Send everything to the API
    }


    public void UploadRouteEditingDraft()
    {
        ButtonSave.RenderBusyState(true);
        SharedData.UploadWayDefinition();
    }

    private void RouteSharedData_OnDataUploadError(string message) {
        DataState.SetActive(false);
        ErrorState.SetActive(true);

    }

}
