using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
//using System.Linq;
//using PaganiniRestAPI;
using UnityEngine;
using UnityEngine.Events;


public class RouteWalkSharedData : PersistentLazySingleton<RouteWalkSharedData>
{
    public RouteWalk CurrentRouteWalk;
    public Route CurrentRoute;
    public List<RouteWalk> PreviuosRouteWalks;

    public List<RouteWalkEventLog> RouteWalkEventList;
    public List<PathpointLog> PathpointLogList;
    public EditorMode CurrentEditorMode;

    // Route Editing variables
    public Pathpoint CurrentPOI;
    public int CurrentPOIIndex;

    public event EventHandler OnDataPartiallyDownloaded;
    public event EventHandler OnDataDownloaded;
    public event EventHandler OnDataUploaded;
    public event EventHandler<string> OnDataUploadError;
    public SharedDataErrorHandler OnDataDownloadError;

    public enum EditorMode
    {
        Cleaning = 0,
        Discussion = 1,
        ReadOnly = 2,
    }


    public RouteWalkSharedData()
    {
    }

    public void LoadRouteWalkFromDatabase()
    {
        CurrentRouteWalk = RouteWalk.Get(CurrentRoute.LastRouteWalkId);
        //CurrentRoute.LastRouteWalkId = 20;
        // Events

        RouteWalkEventList = RouteWalkEventLog.GetAll(p => p.RouteWalkId == CurrentRouteWalk.Id)
            .OrderBy(e=>e.StartTimestamp)
            .ToList();
        PathpointLogList = PathpointLog.GetAll(p => p.RouteWalkId == CurrentRouteWalk.Id)
            .OrderBy(p => p.Timestamp)
            .ToList();

        Debug.Log("Event count: " + RouteWalkEventList.Count);

        PrepareRouteWalkData();

    }


    /**********************
     *  Data processing  *
     **********************/

    #region Download Route Walk Definition
    public void DownloadRouteWalkDefinition()
    {
        Debug.Log("Downloading RouteWalk Definition.");

        CurrentRoute = AppState.CurrentRoute;
        //CurrentRoute.LastRouteWalkId = 20;

        DownloadCurrentRouteWalk();

    }

    private void DownloadCurrentRouteWalk()
    {
        Debug.Log("Downloading Current RouteWalk.");

        var list = RouteWalk.GetAll(p => p.Id == (int)AppState.CurrentRoute.LastRouteWalkId);

        // We have the route walk cached?
        if (list.Count == 0)
        {
            PaganiniRestAPI.RouteWalk.Get((int)AppState.CurrentRoute.LastRouteWalkId, GetRouteWalkSucceeded, LoadFailed);
        }
        else
        {
            DownloadRouteWalkEvents();
            DownloadPreviousRouteWalks();
        }        
    }

    private void DownloadPreviousRouteWalks()
    {
        Debug.Log("Downloading Previous RouteWalks.");

        PaganiniRestAPI.RouteWalk.GetAll((int)AppState.CurrentRoute.Id, GetPreviousRouteWalkSucceeded, LoadFailed);
    }

    private void DownloadRouteWalkEvents()
    { 
        Debug.Log("Downloading Route Walk Events.");

        var lastUpdate = CurrentRoute.LastUpdate; //TODO: Avoid deleting routes under training in route explorer

        Dictionary<string, string> query = new Dictionary<string, string> { };
        if (lastUpdate != null)
        {
            var sinceDate = DateUtils.ConvertMillisecondsToUTCString(lastUpdate, "yyyy-MM-dd'T'HH:mm:ss");
            query = new Dictionary<string, string>
            {
                { "sinceDate", sinceDate }
            };
        }

        PaganiniRestAPI.RouteWalkEvent.GetAll(CurrentRoute.Id, query, GetRouteWalkEventsSucceeded, LoadFailed);
    }

    public void PrepareRouteWalkData()
    {
        Debug.Log("Preparing RouteWalk data.");

        var pathLogs = PathpointLogList;
        var eventLogs = RouteWalkEventList;

        CurrentRouteWalk.EventLogList = eventLogs;

        foreach (var eventLog in eventLogs)
        {
            int startIndex = pathLogs.FindIndex(p => p.Id == eventLog.StartPathpointLogId);
            int destIndex = pathLogs.FindIndex(p => p.Id == eventLog.EndPathpointLogId);

            for (int i=startIndex; i<=destIndex; i++)
            {
                var pathpointLog = pathLogs[i];

                if (eventLog.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.POIReached)
                {
                    pathpointLog.OnTrackEvent = eventLog;
                }
                else if (eventLog.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Offtrack)
                {
                    pathpointLog.OnOffTrackEvent = eventLog;
                }
                else if (eventLog.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.DecisionMade)
                {
                    pathpointLog.OnPOIEvent = eventLog;
                }
            }
        }

        // load previous 3 route walks
        LoadPreviousEvents(3);

    }

    private void LoadPreviousEvents(int count)
    {
        // we take the previous 3
        PreviuosRouteWalks = RouteWalk.GetAll(r => r.StartDateTime < CurrentRouteWalk.StartDateTime &&
                                                   r.RouteId == CurrentRoute.Id)
                                .OrderByDescending(a => a.StartDateTime)
                                .Take(count).ToList();

        // we get the events
        foreach (var walk in PreviuosRouteWalks)
        {
            walk.EventLogList = RouteWalkEventLog.GetAll(e => e.RouteWalkId == walk.Id);
        }

        PreviuosRouteWalks.Reverse();

    }


    #endregion

    #region Delete Definition
    public void DeleteRouteWalkDefinition()
    {
        // Delete Route
        //var route = Route.Get(CurrentRouteWalk.Id);
        //if (route.FromAPI)
        //{
        //    route.Status = Route.RouteStatus.Discarded;
        //    PaganiniRestAPI.Route.CreateOrUpdate(route.WayId, route.ToAPI(), DeleteRouteSucceeded, DeleteRouteFailed);            
        //}
        //else
        //{
        //    DeleteRouteLocalContents();
        //}

    }

    #endregion

    #region Upload Pathpoint Instruction Modes

    public bool ArePIMChangesToUpload()
    {
        var pims = PathpointPIM.GetPIMListByRoute(CurrentRoute.Id, p => p.IsDirty);
        return pims.Count > 0;
    }

    public void UploadLocalPIM()
    {        
        var pims = PathpointPIM.GetPIMListByRoute(CurrentRoute.Id, p => p.IsDirty);

        Debug.Log($"Uploading local PIMS: ({pims.Count})");

        // send batch
        if (pims.Count > 0)
        {
            var batch = PreparePathpointPIMBatch(pims);
            PaganiniRestAPI.PathpointPIM.BatchCreate(CurrentRoute.Id, batch, BatchUploadSucceeded, CreateOrUpdateFailed);
            return;
        }
    }

    private void BatchUploadSucceeded(PathpointPIMAPIList res)
    {
        var pims = PathpointPIM.GetPIMListByRoute(CurrentRoute.Id, p => p.IsDirty);
        // delete uploaded ones
        foreach ( var pim in pims)
        {
            PathpointPIM.Delete(pim.Id);
        }

        OnDataUploaded.Invoke(this, EventArgs.Empty);
    }

    private PathpointPIMAPIBatch PreparePathpointPIMBatch(List<PathpointPIM> pimList)
    {
        // prepare batch
        List<PathpointPIMAPI> pimAPIList = new();
        foreach (var pim in pimList)
        {
            pimAPIList.Add(pim.ToAPI());
        }
        var batch = new PathpointPIMAPIBatch { pims = pimAPIList.ToArray() };
        return batch;
    }

    private void CreateOrUpdateFailed(string errorMessage)
    {
        Debug.Log(errorMessage);

        OnDataUploadError?.Invoke(this,errorMessage);
    }

    #endregion



    // Event handlers

    private void GetRouteWalkSucceeded(RouteWalkAPIResult res)
    {
        Debug.Log("Processing RouteWalk.");

        RouteWalk routeWalk = new RouteWalk(res);
        routeWalk.Insert();

        // insert events
        if (res.routewalk_events != null)
        {
            foreach (var eventRes in res.routewalk_events)
            {
                // lastUpdate? how do we manage that?
                RouteWalkEventLog walkEventLog = new RouteWalkEventLog(eventRes);
                walkEventLog.Insert();
            }
        }

        if (res.routewalk_paths != null)
        {
            // insert pathpointLog
            foreach (var pathRes in res.routewalk_paths)
            {
                PathpointLog pathpointLog = new PathpointLog(pathRes);
                pathpointLog.Insert();
            }
        }

        DownloadRouteWalkEvents();
        DownloadPreviousRouteWalks();
    }

    private void GetPreviousRouteWalkSucceeded(RouteWalkAPIList res)
    {
        Debug.Log("Processing previous RouteWalks.");

        int nRetreived = (res != null && res.routewalks != null) ? res.routewalks.Length : 0;

        if (nRetreived > 0)
        {
            foreach (var walk in res.routewalks)
            {
                RouteWalk routeWalk = new RouteWalk(walk);
                routeWalk.Insert();
            }
        }
    }

    private void GetRouteWalkEventsSucceeded(RouteWalkEventAPIList res)
    {
        int nRetreived = (res != null && res.routewalk_events != null) ? res.routewalk_events.Length : 0;

        Debug.Log($"Processing RouteWalkEvents. (Retreived {nRetreived})");
        
        if (nRetreived > 0)
        {
            long lastUpdate = 0;

            // Insert clean verion
            foreach (var eventRes in res.routewalk_events)
            {
                RouteWalkEventLog walkEventLog = new RouteWalkEventLog(eventRes);
                walkEventLog.Insert();

                if (walkEventLog.StartTimestamp > lastUpdate)
                {
                    lastUpdate = walkEventLog.StartTimestamp;
                }

                
            }

            // update last update date
            //CurrentRoute.LastUpdate = DateUtils.UTCMilliseconds();
            CurrentRoute.LastUpdate = lastUpdate;
            CurrentRoute.Insert();
        }

        OnDataDownloaded?.Invoke(this, EventArgs.Empty);

    }   


    /// <summary>
    /// This function is called when the Load methods fail.
    /// </summary>
    /// <param name="errorMessage">The error message returned by the API.</param>
    private void LoadFailed(string errorMessage)
    {
        Debug.Log(errorMessage);

        OnDataDownloadError?.Invoke(errorMessage);
    }




}

