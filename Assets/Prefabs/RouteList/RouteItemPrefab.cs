using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


[System.Serializable]
public class RouteItemEvent : UnityEvent<Way,Route>
{
}

public class RouteItemPrefab : MonoBehaviour
{

    [Header("Cell Settings")]
    public RouteItemCellPrefab TitleCell;
    public RouteItemCellPrefab StartCell;
    public RouteItemCellPrefab DestinatitonCell;
    public RouteItemCellPrefab DateCell;
    public RouteItemCellPrefab StatusCell;

    [Header("Row Settings")]
    public GameObject NewFlag;
    public Button RouteButton;

    [Header("Events")]
    public RouteItemEvent OnSelected;

    private Way WayItem;
    private Route RouteItem;


    // Start is called before the first frame update
    void Start()
    {
        RouteButton.onClick.AddListener(itemSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void FillWayRoute(Way way, Route route)
    {
        LandmarkIcon.LandmarkType startIcon = Enum.Parse<LandmarkIcon.LandmarkType>(way.StartType);
        LandmarkIcon.LandmarkType destIcon = Enum.Parse<LandmarkIcon.LandmarkType>(way.DestinationType);
        TitleCell.FillCell(way.Name);
        StartCell.FillCell(way.Start, startIcon);
        DestinatitonCell.FillCell(way.Destination, destIcon);
        DateCell.FillCell(route.Date.ToString("dd/MM/yyyy HH:mm"));
        StatusCell.FillCell(Route.RouteStatusDescriptions[route.Status], route.Status);

        NewFlag.SetActive(!route.FromAPI);

        WayItem = way;
        RouteItem = route;
    }


    private void itemSelected()
    {
        if (OnSelected != null)
        {
            OnSelected.Invoke(WayItem, RouteItem);
        }

        Debug.Log("Item RouteItem " + RouteItem.Id);
    }
}
