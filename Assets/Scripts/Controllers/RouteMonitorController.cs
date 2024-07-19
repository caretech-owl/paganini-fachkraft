using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class RouteMonitorController : MonoBehaviour
{
    [Header("Main views")]
    public RouteWalkOverview OverviewView;
    public RouteWalkTimeline TimelineView;
    public RouteWalkMap MapView;
    public PIMPublish PublishView;
    public GameObject LoadingView;

    [Header("Components")]
    public ToggleGroup MenuToggle;
    public TMPro.TMP_Text HeaderText;

    public UnityEvent OnWayDefinitionUploaded;

    private RouteSharedData SharedData;
    private RouteWalkSharedData WalkSharedData;

    // Start is called before the first frame update
    void Start()
    {
        SharedData = RouteSharedData.Instance;
        SharedData.OnDataDownloaded += RouteSharedData_OnDataDownloaded;
        SharedData.OnDataPartiallyDownloaded += RouteSharedData_OnDataPartiallyDownloaded;

        WalkSharedData = RouteWalkSharedData.Instance;
        WalkSharedData.OnDataDownloaded += WalkSharedData_OnDataDownloaded;


        SharedData.DownloadRouteDefinition();
        HeaderText.text = AppState.CurrentRoute.Name;

        HideAllButThisView(LoadingView);

    }

    /**********************
     *  Public UI events (and utilities)  *
     **********************/

    public void LoadComponents()
    {
        TimelineView.OnViewLoaded.AddListener(OnTimelineViewLoaded);

        TimelineView.LoadView();
        OverviewView.LoadView();
    }

    public void UploadAdaptationChanges()
    {
        WalkSharedData.UploadLocalPIM();
    }

    private void OnTimelineViewLoaded()
    {
        SwitchView(true);
    }

    public void RenderMap()
    {
        HideAllButThisView(MapView.gameObject);
        MapView.LoadMap();
    }

    public void RenderTimeline()
    {
        HideAllButThisView(TimelineView.gameObject);
    }

    public void RenderOverview()
    {
        HideAllButThisView(OverviewView.gameObject);
    }

    public void RenderPublishOrClose()
    {
        if (WalkSharedData.ArePIMChangesToUpload())
        {
            HideAllButThisView(PublishView.gameObject);
            PublishView.LoadView();
        }
        else
        {
            SafelyCloseView();
        }        
    }

    public void SafelyCloseView()
    {
        MapView.DisableMap();
        SceneSwitcher.LoadRouteExplorer();
    }

    public void SwitchView( bool active)
    {
        // avoid triggering it more than once
        if (!active) return;

        foreach (var toggle in MenuToggle.ActiveToggles())
        {
            if (toggle.name.Contains("Overview"))
            {
                RenderOverview();
            }
            else if (toggle.name.Contains("Timeline"))
            {
                RenderTimeline();
            }
            else
            {
                RenderMap();
            }
        }
    }

    //public void FlagChangesToDraft()
    //{
    //    SharedData.CurrentRoute.IsDraftUpdated = true;
    //    SharedData.CurrentRoute.Insert();

    //}

    // private functions


    private void RouteSharedData_OnDataDownloaded(object sender, EventArgs e)
    {
        // Trigger to  Dowload the RouteWalk
        WalkSharedData.DownloadRouteWalkDefinition();

        // While waiting, cache information about the route definition in memory
        SharedData.LoadRouteFromDatabase();
    }

    private void RouteSharedData_OnDataPartiallyDownloaded(object sender, EventArgs e)
    {        
        //LoadOnboarding();
    }


    private void WalkSharedData_OnDataDownloaded(object sender, EventArgs e)
    {
        // Load route walk information in memory
        WalkSharedData.LoadRouteWalkFromDatabase();

        LoadComponents();
    }

    private void HideAllButThisView(GameObject view) {

        if (MapView.gameObject != view && MapView.gameObject.activeInHierarchy){
            MapView.DisableMap();
        }
        else if (MapView.gameObject == view)
        {
            MapView.EnableMap();
        }

        OverviewView.gameObject.SetActive(OverviewView.gameObject == view);
        TimelineView.gameObject.SetActive(TimelineView.gameObject == view);
        PublishView.gameObject.SetActive(PublishView.gameObject == view);
        MapView.gameObject.SetActive(MapView.gameObject == view);
        LoadingView.SetActive(LoadingView == view);
        



        //if (RouteTimelineView.gameObject != view)
        //{
        //    //RouteTimelineView.CleanupView();
        //}

        //POIEditView.gameObject.SetActive(POIEditView.gameObject == view);
        //if (POIEditView.gameObject != view)
        //{
        //    POIEditView.CleanupView();
        //}


        //StatusChange.gameObject.SetActive(StatusChange.gameObject == view);

        //RouteInfoEditView.gameObject.SetActive(RouteInfoEditView.gameObject == view);
    }

    private void OnDestroy()
    {
        SharedData.OnDataDownloaded -= RouteSharedData_OnDataDownloaded;
        SharedData.OnDataPartiallyDownloaded -= RouteSharedData_OnDataPartiallyDownloaded;

        WalkSharedData.OnDataDownloaded -= WalkSharedData_OnDataDownloaded;
    }



}