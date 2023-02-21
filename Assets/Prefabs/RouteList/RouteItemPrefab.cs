using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class RouteItemEvent : UnityEvent<Route>
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
        TitleCell.FillCell(way.Name);
        StartCell.FillCell(way.Start);
        DestinatitonCell.FillCell(way.Destination);
        DateCell.FillCell(route.Date.ToString());
        StatusCell.FillCell(route.Status.ToString());

        NewFlag.SetActive(!route.FromAPI);

        WayItem = way;
        RouteItem = route;

    }


    private void itemSelected()
    {
        if (OnSelected != null)
        {
            OnSelected.Invoke(RouteItem);
        }

        Debug.Log("Item RouteItem " + RouteItem.Id);
    }
}
