using UnityEngine;
using UnityEngine.Events;
using System;

public class RouteEditorController : MonoBehaviour
{
    [Header("Components")]
    public RouteOnboarding RouteOnboardingView;
    public RouteTimeline RouteTimelineView;
    public POIEdit POIEditView;
    public RouteStepChange StatusChange;
    public RouteInfoEdit RouteInfoEditView;


    //public VideoPlayerPrefab VideoManager;
    //public PinListPrefab PinList;
    public MapManager MapView;


    public UnityEvent OnWayDefinitionUploaded;

    private RouteSharedData SharedData;

    // Start is called before the first frame update
    void Start()
    {
        SharedData = RouteSharedData.Instance;
        SharedData.OnDataDownloaded += RouteSharedData_OnDataDownloaded;
        SharedData.OnDataPartiallyDownloaded += RouteSharedData_OnDataPartiallyDownloaded;
        SharedData.OnDataUploaded += SharedData_OnDataUploaded;

        SharedData.DownloadRouteDefinition();
    }

    /**********************
     *  Public UI events (and utilities)  *
     **********************/

    /// <summary>
    /// Toggle the route video representation, where active turns on video and inactive the map
    /// </summary>
    /// <param name="videoActive">Whether video should be active</param> 
    //public void ToggleRouteRepresentation(bool videoActive)
    //{
    //    if (videoActive)
    //    {
    //        GMap.DisableMap();
    //        // Activating the component, and then resuming to the last timestamp
    //        VideoManager.gameObject.SetActive(videoActive); // function to hide!        
    //        //VideoManager.ResumeVideo();
    //    }
    //    else // videoActive = false
    //    {
    //        GMap.EnableMap();
    //        // Pausing the component to get the current timestamp,
    //        // and then disabling the component
    //        //VideoManager.PauseVideo();
    //        VideoManager.gameObject.SetActive(videoActive);
    //    }
        
    //}


    //public void RenderPathpointTrace(PathpointTraceMessage traceMessage)
    //{
    //    Debug.Log($"RenderPathpointTrace {traceMessage.type} {traceMessage.eventType}");
    //    GMap.RenderMarker(traceMessage);        
    //}

    ///// <summary>
    ///// Safely terminate the editor
    ///// </summary>
    //public void TerminateEditor()
    //{
    //    GMap.DisableMap();
    //}


    public void LoadInfo() {

    }

    public void LoadMap()
    {
        HideAllButThisView(MapView.gameObject);
        MapView.LoadMap();
    }

    public void LoadChangeStatusAndSave()
    {
        HideAllButThisView(StatusChange.gameObject);
        StatusChange.LoadRouteStepChange(SharedData.CurrentRoute);
    }

    public void LoadEditRouteInfo()
    {
        HideAllButThisView(RouteInfoEditView.gameObject);
        RouteInfoEditView.LoadRouteInfo(SharedData.CurrentWay, SharedData.CurrentRoute);
    }

    public void LoadPOIEditor(Pathpoint poi, int index)
    {
        SharedData.CurrentPOI = poi;
        SharedData.CurrentPOIIndex = index;

        HideAllButThisView(POIEditView.gameObject);
        POIEditView.LoadView(index);        
    }

    public void LoadTimeline() {
        HideAllButThisView(RouteTimelineView.gameObject);
        RouteTimelineView.LoadView();        
    }

    public void BackToTimeline()
    {
        HideAllButThisView(RouteTimelineView.gameObject);
    }

    public void LoadOnboarding() {
        HideAllButThisView(RouteOnboardingView.gameObject);
        RouteOnboardingView.LoadBusyView();        
    }


    public void FlagChangesToDraft()
    {
        SharedData.CurrentRoute.IsDraftUpdated = true;
        SharedData.CurrentRoute.Insert();

    }

    // private functions


    private void RouteSharedData_OnDataDownloaded(object sender, EventArgs e)
    {
        RouteOnboardingView.LoadReadyView();
    }

    private void RouteSharedData_OnDataPartiallyDownloaded(object sender, EventArgs e)
    {
        SharedData.LoadRouteFromDatabase();
        LoadOnboarding();
    }

    private void SharedData_OnDataUploaded(object sender, EventArgs e)
    {
        OnWayDefinitionUploaded?.Invoke();
    }

    private void HideAllButThisView(GameObject view) {

        RouteOnboardingView.gameObject.SetActive(RouteOnboardingView.gameObject == view);
        RouteTimelineView.gameObject.SetActive(RouteTimelineView.gameObject == view);

        if (MapView.gameObject != view && MapView.gameObject.activeInHierarchy)
        {
            MapView.DisableMap();
        }
        else if (MapView.gameObject == view)
        {
            MapView.EnableMap();
        }

        if (RouteTimelineView.gameObject != view)
        {
            //RouteTimelineView.CleanupView();
        }

        POIEditView.gameObject.SetActive(POIEditView.gameObject == view);
        if (POIEditView.gameObject != view)
        {
            POIEditView.CleanupView();
        }

        MapView.gameObject.SetActive(MapView.gameObject == view);
        StatusChange.gameObject.SetActive(StatusChange.gameObject == view);

        RouteInfoEditView.gameObject.SetActive(RouteInfoEditView.gameObject == view);
    }

    private void OnDestroy()
    {
        SharedData.OnDataDownloaded -= RouteSharedData_OnDataDownloaded;
        SharedData.OnDataUploaded -= SharedData_OnDataUploaded;
        SharedData.OnDataPartiallyDownloaded -= RouteSharedData_OnDataPartiallyDownloaded;
    }



}