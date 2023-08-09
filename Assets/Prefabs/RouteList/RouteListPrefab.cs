using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteListPrefab : MonoBehaviour
{

    public GameObject ScrollView;

    public GameObject Content;

    public GameObject ItemPrefab;

    public GameObject BlankState;

    public RouteItemEvent OnItemSelected;

    // Start is called before the first frame update
    void Awake()
    {
        Clearlist();
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
    }

    public void AddItem(Way w)
    {

        foreach (var route in w.Routes)
        {
            var neu = Instantiate(ItemPrefab, Content.transform);

            RouteItemPrefab item = neu.GetComponent<RouteItemPrefab>();
            item.OnSelected = OnItemSelected;
            item.FillWayRoute(w, route);
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
    }









}
