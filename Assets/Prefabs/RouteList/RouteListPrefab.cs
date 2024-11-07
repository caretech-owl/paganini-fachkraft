using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteListPrefab : MonoBehaviour
{
    [Header("UI states")]
    public GameObject DataState;
    public GameObject BlankState;
    public GameObject LoadingState;

    [Header("Data UI")]
    public GameObject ScrollView;

    public GameObject Content;

    public GameObject ItemPrefab;

    [Header("Events")]
    public RouteItemEvent OnItemSelected;
    public RouteItemEvent OnItemEdit;

    private int dataCount;

    // Start is called before the first frame update
    void Awake()
    {
        Clearlist();
    }

    void Start() {
        dataCount = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clearlist()
    {
        if (Content== null) return;

        Transform content = Content.GetComponent<Transform>();

        // Remove all children
        for (int i = 0; i < content.childCount; i++)
        {
            // Get the child game object
            GameObject child = content.GetChild(i).gameObject;

            // Destroy the child game object
            Destroy(child);
        }
        dataCount = 0;
        ActivateStateView(LoadingState);
    }

    public void AddItem(Way w)
    {

        foreach (var route in w.Routes)
        {
            var neu = Instantiate(ItemPrefab, Content.transform);

            RouteItemPrefab item = neu.GetComponent<RouteItemPrefab>();
            item.OnSelected = OnItemSelected;
            item.OnRouteEdit = OnItemEdit;
            item.FillWayRoute(w, route);

            dataCount++;
        }        

    }


    public void AddItem(SynchronizationController.DetailedWayExport item)
    {

        Way w = new Way();
        w.Id = item.Id;
        w.Name = item.Name;
        w.StartType = item.StartType;
        w.DestinationType = item.DestinationType;
        w.Start = item.Start;
        w.Destination = item.Destination;
        w.Description = item.Description;
        w.UserId = item.UserId;

        Route r = new Route();
        r.Id = item.Id;
        r.Date = item.RecordingDate;
        r.Name = item.RecordingName;

        w.Routes = new List<Route>();
        w.Routes.Add(r);

        this.AddItem(w);

        dataCount++;
    }

    public void FinishLoading()
    {
        if (dataCount > 0) {
            ActivateStateView(DataState);
        } else {
            ActivateStateView(BlankState);
        }
    }

    private void ActivateStateView(GameObject view) {

        DataState.SetActive(view == DataState);
        BlankState.SetActive(view == BlankState);
        LoadingState.SetActive(view == LoadingState);
    }

}
