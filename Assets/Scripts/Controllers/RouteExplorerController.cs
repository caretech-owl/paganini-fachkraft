using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteExplorerController : MonoBehaviour
{
    public GameObject RouteListView;
    private RouteListPrefab RouteList;

    private void Awake()
    {
        DBConnector.Instance.Startup();
    }

    // Start is called before the first frame update
    void Start()
    {
        RouteList = RouteListView.GetComponent<RouteListPrefab>();

        PaganiniRestAPI.Way.GetAll(AppState.CurrentUser.Id, GetWaySucceeded, GetWayFailed);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DisplayRoutes()
    {
        var list = Way.GetAllWaysAndRoutes();
        foreach (var way in list)
        {
            RouteList.AddItem(way);
        }
    }

    /// <summary>
    /// This function is called when the GetAll method of the User class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="users">The list of users returned by the API.</param>
    private void GetWaySucceeded(WayAPIList list)
    {
        //List<Way> list = Way.ToModelList<WayAPI, Way>(ways.ways, api => new Way(api));

        //foreach (var item in list)
        //{
        //    RouteList.AddItem(item);
        //}

        //Debug.Log("Ways:" + list.Count);

        UpdateLocalRoutes(list.ways);
        DisplayRoutes();

    }

    /// <summary>
    /// This function is called when the GetAll method of the User class in the PaganiniRestAPI namespace fails.
    /// </summary>
    /// <param name="errorMessage">The error message returned by the API.</param>
    private void GetWayFailed(string errorMessage)
    {

    }

    /// <summary>
    /// This function processes the selected user, sets the current user in the user session, and load the next
    /// scene.
    /// </summary>
    /// <param name="user">The selected user.</param>
    public void LoadRoute(Way way)
    {
        //AppState.CurrentUser = user;
        //SceneSwitcher.LoadRouteExplorer();
    }



    private void UpdateLocalRoutes(WayAPI [] list)
    {
        // Delete the current local copy of ways

        Way.DeleteNonDirtyCopies<Way>();
        Route.DeleteNonDirtyCopies<Route>();

        // Create a local copy of the API results
        foreach (WayAPI wres in list)
        {
            // Insert new way
            Way w = new Way(wres);
            w.Insert();

            if (wres.routes != null)
            {
                foreach (RouteAPI rres in wres.routes)
                {
                    // Insert associated route
                    Route r = new Route(rres);
                    r.Insert();
                }
            }            
        }
    }






}
