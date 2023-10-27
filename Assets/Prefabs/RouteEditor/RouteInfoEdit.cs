using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RouteInfoEdit : MonoBehaviour
{

    [Header("UI Data")]
    public TMPro.TMP_InputField RouteNameInput;
    public TMPro.TMP_InputField WayStartInput;
    public TMPro.TMP_InputField WayDestinationInput;

    public IconDropdown StartDropdown;
    public IconDropdown DestinationDropdown;
  
    private Route CurrentRoute;
    private Way CurrentWay;

    private string newStartIcon;
    private string newDestinationIcon;

    void Awake()
    {
        StartDropdown.OnValueChanged.AddListener(StartDropdown_OnValueChanged);
        DestinationDropdown.OnValueChanged.AddListener(DestinationDropdown_OnValueChanged);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadRouteInfo(Way way, Route route)
    {
        CurrentRoute = route;
        CurrentWay = way;

        RouteNameInput.text = route.Name;
        WayStartInput.text = way.Start;
        WayDestinationInput.text = way.Destination;


        var startType = parseToLandmarkType(way.StartType);
        var endType = parseToLandmarkType(way.DestinationType);

        newStartIcon = way.StartType;
        newDestinationIcon = way.DestinationType;

        StartDropdown.SetDefaultOption(startType);
        DestinationDropdown.SetDefaultOption(endType);
    }


    public void SaveRouteChange() {

        Debug.Log("Save Route Changes.");

        CurrentRoute.IsDirty = CurrentRoute.Name != RouteNameInput.text;
        CurrentRoute.Name = RouteNameInput.text;        
        CurrentRoute.Insert();

        // We mark as 'dirty' if the values of the start / destination were changed
        CurrentWay.IsDirty = (CurrentWay.Start != WayStartInput.text || CurrentWay.Destination != WayDestinationInput.text)?
                              true : CurrentWay.IsDirty;
        CurrentWay.Start = WayStartInput.text;
        CurrentWay.Destination = WayDestinationInput.text;        
        CurrentWay.StartType = newStartIcon;
        CurrentWay.DestinationType = newDestinationIcon;
        CurrentWay.Insert();
    }


    private LandmarkIcon.LandmarkType parseToLandmarkType(string textType)
    {
        LandmarkIcon.LandmarkType parsedLandmarkType;

        if (Enum.TryParse<LandmarkIcon.LandmarkType>(textType, out parsedLandmarkType))
        {
            // Parsing successful, parsedLandmarkType now contains LandmarkType.Park
            Debug.Log("Parsed Landmark Type: " + parsedLandmarkType);
        }
        else
        {
            // Handle the case where the string doesn't match any enum value
            Debug.LogError("Invalid Landmark Type: " + textType);
        }

        return parsedLandmarkType;
    }

    private void StartDropdown_OnValueChanged(LandmarkIcon.LandmarkType value)
    {
        newStartIcon = ((int)value).ToString();
        CurrentWay.IsDirty = true;
    }

    private void DestinationDropdown_OnValueChanged(LandmarkIcon.LandmarkType value)
    {
        newDestinationIcon = ((int)value).ToString();
        CurrentWay.IsDirty = true;
    }

    private void OnDestroy()
    {
        StartDropdown.OnValueChanged.RemoveListener(StartDropdown_OnValueChanged);
        DestinationDropdown.OnValueChanged.RemoveListener(DestinationDropdown_OnValueChanged);
    }

}
