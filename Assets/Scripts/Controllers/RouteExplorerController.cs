using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteExplorerController : MonoBehaviour
{
    public GameObject RouteListView;
    public TMPro.TMP_Text WelcomeText;
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

        WelcomeText.text = WelcomeText.text.Replace("[USER]", AppState.CurrentUser.Mnemonic_token);
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// Displays the list of routes
    /// </summary>
    public void DisplayRoutes()
    {
        var list = Way.GetWayListByUser(AppState.CurrentUser.Id);
        foreach (var way in list)
        {
            way.Routes = Route.GetRouteListByWay(way.Id);
            RouteList.AddItem(way);
        }
    }

    /// <summary>
    /// This function processes the selected route, and opens it in the editor
    /// scene.
    /// </summary>
    /// <param name="route">The selected route.</param>
    public void LoadRoute(Way way, Route route)
    {
        AppState.CurrentWay = way;
        AppState.CurrentRoute = route;
        SceneSwitcher.LoadRouteEditor();
    }




    /// <summary>
    /// This function is called when the GetAll method of the User class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="list">The list of users returned by the API.</param>
    private void GetWaySucceeded(WayAPIList list)
    {
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




    private void UpdateLocalRoutes(WayAPIResult [] list)
    {
        // Delete the current local copy of ways

        Way.DeleteNonDirtyCopies();
        Route.DeleteNonDirtyCopies();

        // Create a local copy of the API results
        foreach (WayAPIResult wres in list)
        {
            // Get the local copy first (not to overwrite local copy)
            if (!Way.CheckIfExists(w => w.Id == wres.way_id))
            {
                // Insert new way
                Way w = new Way(wres);
                w.UserId = AppState.CurrentUser.Id;
                w.Insert();
            }

            if (wres.routes != null)
            {
                foreach (RouteAPIResult rres in wres.routes)
                {
                    // check for local copy
                    if(!Route.CheckIfExists(r => r.Id == rres.erw_id))
                    {
                        // Insert associated route
                        Route r = new Route(rres);
                        r.Insert();
                    }
                }
            }            
        }
    }






}
