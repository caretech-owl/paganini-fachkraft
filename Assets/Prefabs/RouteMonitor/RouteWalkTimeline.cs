using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class RouteWalkTimeline: MonoBehaviour
{

    [Header("UI Elements")]
    public RouteWalkTimelineViz TimelineVizView;
    public GameObject NoDataView;

    private RouteSharedData SharedData;
    private RouteWalkSharedData WalkSharedData;


    [Header("Events")]
    public UnityEvent OnViewLoaded;

    bool _isViewJustLoaded = false;

    // Start is called before the first frame update
    void Awake()
    {
        SharedData = RouteSharedData.Instance;
        WalkSharedData = RouteWalkSharedData.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        if (_isViewJustLoaded)
        {
            TimelineVizView.SelectFirstSegment();
            _isViewJustLoaded = false;
        }        
    }

    public void LoadView()
    {
        gameObject.SetActive(true);
        _isViewJustLoaded = true;

        LoadPathpointList();

        SharedData.CurrentPOI = null;
        SharedData.CurrentPOIIndex = -1;
    }


    // <summary>
    // Populates the Route Onboarding view based on the status of the Route
    // </summary>
    private void LoadPathpointList()
    {
        TimelineVizView.Clearlist();
        int index = 0;
        foreach (var item in SharedData.POIList)
        {
            try {
                // We load photos                
                item.Photos = PathpointPhoto.GetPathpointPhotoListByPOI(item.Id);
                //item.Photos
            }
            catch (Exception e)
            {
                index++;
                Debug.Log($"Id: {item.Id} POIType: {item.POIType} --> Error loading photos");
                continue;
            }

            // Add item to list
            if (index == 0) {
                //TODO: Remove hack
                if (item.POIType != Pathpoint.POIsType.WayStart) { 
                    item.POIType = Pathpoint.POIsType.WayStart;
                    item.InsertDirty();
                }
                TimelineVizView.AddStart(item, SharedData.CurrentWay);
                LoadPOISegment(item, SharedData.POIList[index + 1]);
                LoadSegAdaptation(item);
                LoadPOIAdaptation(item); // hides

            } else if (index == SharedData.POIList.Count -1) {
                //TODO: Remove hack
                if (item.POIType != Pathpoint.POIsType.WayDestination)
                {
                    item.POIType = Pathpoint.POIsType.WayDestination;
                    item.InsertDirty();
                }
                TimelineVizView.AddDestination(item, SharedData.CurrentWay);
                LoadPOIAdaptation(item); // hides

            } else {
                TimelineVizView.AddPOI(item);
                LoadPOISegment(item, SharedData.POIList[index+1]);
                LoadSegAdaptation(item);
                LoadPOIAdaptation(item);
                //todo: segments in between
            }

            Debug.Log($"Index: {index} Id: {item.Id} POIType: {item.POIType}");

            index++;            
        }

        // No data to render
        if (index == 0) {
            LoadNoData();
        }

        Debug.Log("Number of POIs: "+ SharedData.POIList.Count);
        OnViewLoaded?.Invoke();
    }

    private void LoadPOISegment(Pathpoint item, Pathpoint nextItem)
    {
        // 'normal walk'
        var walkEvent = WalkSharedData.RouteWalkEventList.Find(e => e.SegPOIStartId == item.Id &&
                                                                    e.SegReachedPOIEndId == nextItem.Id);
       
        if (walkEvent != null)
        {
            int segPOIStartIndex = SharedData.PathpointList.FindIndex(p => p.Id == walkEvent.SegPOIStartId);
            int segPOIEndIndex = SharedData.PathpointList.FindIndex(p => p.Id == walkEvent.SegExpectedPOIEndId);

            var subpath = SharedData.PathpointList.GetRange(segPOIStartIndex, segPOIEndIndex - segPOIStartIndex + 1);
            var logList = WalkSharedData.PathpointLogList.FindAll(p => p.SegPOIStartId == walkEvent.SegPOIStartId &&
                                                                       p.Timestamp >= walkEvent.StartTimestamp &&
                                                                       p.Timestamp <= walkEvent.EndTimestamp);

            TimelineVizView.AddSegment(subpath, logList);
        }

        //  any loops? 
        walkEvent = WalkSharedData.RouteWalkEventList.Find(e => e.SegPOIStartId == item.Id &&
                                                                e.SegReachedPOIEndId == item.Id);

        if (walkEvent != null)
        {
            var logList = WalkSharedData.PathpointLogList.FindAll(p => p.SegPOIStartId == walkEvent.SegPOIStartId &&
                                                                       p.Timestamp >= walkEvent.StartTimestamp &&
                                                                       p.Timestamp <= walkEvent.EndTimestamp);

            TimelineVizView.AddLoop(logList);
        }

        // display decision
        var decisionEvent = WalkSharedData.RouteWalkEventList.FindAll(e => e.TargetPOIId == item.Id &&
                                                                        e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.DecisionMade);
        if (decisionEvent != null)
        {
            TimelineVizView.AddDecisionMade(decisionEvent);
        }

    }

    private void LoadPOIAdaptation(Pathpoint item)
    {
        var list = WalkSharedData.RouteWalkEventList.FindAll(e=> e.TargetPOIId == item.Id && e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Adaptation);
        var adaptationLog = GetRelevantAdaptation(list);

        TimelineVizView.LoadPOIAdaptation(adaptationLog);
    }

    private void LoadSegAdaptation(Pathpoint item)
    {
        var list = WalkSharedData.RouteWalkEventList.FindAll(e => e.SegPOIStartId == item.Id && e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Adaptation);
        var adaptationLog = GetRelevantAdaptation(list);
        TimelineVizView.LoadSegAdaptation(adaptationLog);
    }

    private RouteWalkEventLog GetRelevantAdaptation(List<RouteWalkEventLog> adapList)
    {        
        RouteWalkEventLog adaptation = null;
        if (adapList.Count > 0)
        {
            adaptation = adapList.OrderBy(a => a.StartTimestamp).ToList().First();
        }
        else if (adapList.Count == 1)
        {
            adaptation = adapList.First();
        }

        return adaptation;
    }

    private void LoadNoData() {
        TimelineVizView.gameObject.SetActive(false);
        NoDataView.SetActive(true);
        //SaveButton.SetActive(false);
    }

    public void CleanupView()
    {
        TimelineVizView.CleanupView();
    }

}
