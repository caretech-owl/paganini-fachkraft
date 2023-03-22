using Assets;
using NinevaStudios.GoogleMaps;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PaganiniRestAPI;


public class RouteEditorController : MonoBehaviour
{
    [Header("Components")]
    public VideoPlayerPrefab VideoManager;
    public PinListPrefab PinList;
    public MapManager GMap;

    private Way CurrentWay;
    private Route CurrentRoute;
    private List<Pathpoint> PathpointList;
    private List<Pathpoint> POIList;

    private List<Pathpoint> DirtyPathpointPointList;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadRouteData();

        LoadMap();

        LoadPathpointList();

        LoadVideo();
    }

    /**********************
     *  Public UI events (and utilities)  *
     **********************/

    /// <summary>
    /// Moves the VideoPlayback to the pathpoint timestamp, so as to
    /// see the video around the current pathpoint
    /// </summary>
    /// <param name="pathpoint">Pathpoint to synchronize with</param> 
    public void SyncVideoToPathpoint(Pathpoint pathpoint)
    {
        double skipTime = (pathpoint.Timestamp - PathpointList[0].Timestamp) / 1000;

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(pathpoint.Photos[0].Photo);
        VideoManager.SkipToVideoFrame(skipTime, texture);
    }

    /// <summary>
    /// Toggle the route video representation, where active turns on video and inactive the map
    /// </summary>
    /// <param name="videoActive">Whether video should be active</param> 
    public void ToggleRouteRepresentation(bool videoActive)
    {
        if (videoActive)
        {
            GMap.DisableMap();
            // Activating the component, and then resuming to the last timestamp
            VideoManager.gameObject.SetActive(videoActive); // function to hide!        
            VideoManager.ResumeVideo();
        }
        else // videoActive = false
        {
            GMap.EnableMap();
            // Pausing the component to get the current timestamp,
            // and then disabling the component
            VideoManager.PauseVideo();
            VideoManager.gameObject.SetActive(videoActive);
        }
        
    }


    /// <summary>
    /// Safely terminate the editor
    /// </summary>
    public void TerminateEditor()
    {
        GMap.DisableMap();
    }


    private void LoadRouteData()
    {
        CurrentWay = Way.Get<Way>(AppState.CurrentRoute.WayId);
        CurrentRoute = AppState.CurrentRoute;
        PathpointList = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id);
        POIList = PathpointList.Where(item => item.POIType != Pathpoint.POIsType.Point).ToList();
    }

    private void LoadVideo()
    {
        VideoManager.LoadVideo(FileManagement.persistentDataPath + "/" + CurrentRoute.LocalVideoFilename);
    }

    private void LoadMap()
    {
        GMap.LoadMap(PathpointList);
        //GMap.LoadMap(POIList);
    }

    private void LoadPathpointList()
    {
        PinList.Clearlist();
        foreach (var item in POIList)
        {
            // We load photos
            item.Photos = PathpointPhoto.GetPathpointPhotoListByPOI(item.Id);
            //item.Photos

            // Add item to list
            PinList.AddItem(item);
        }
    }


    /**********************
     *  Data processing  *
     **********************/

    public void UpdatedWayDefinition()
    {
        // get latest definition from db
        var way = Way.Get<Way>(CurrentWay.Id);

        if (way.IsDirty)
        {
            PaganiniRestAPI.Way.CreateOrUpdate(way.UserId, way.ToAPI(), CreateOrUpdateWaySucceeded, CreateOrUpdateFailed);
            return;
        }

        UpdateRouteDefinition(way.Id);
    }

    public void UpdateRouteDefinition(int parentWayId)
    {
        // get latest definition from db
        var route = Route.Get<Route>(CurrentRoute.Id);

        if (route.IsDirty)
        {
            route.WayId = parentWayId;
            PaganiniRestAPI.Route.CreateOrUpdate(route.WayId, route.ToAPI(), CreateOrUpdateRouteSucceeded, CreateOrUpdateFailed);
            return;
        }

        UpdateNewPathpointsPoints(route.Id);
    }

    public void UpdateNewPathpointsPoints(int parentRouteId)
    {
        var pathpoints = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType == Pathpoint.POIsType.Point);

        // prepare batch
        List<PathpointAPI> pathpointAPIs = new();
        foreach (var pathpoint in pathpoints)
        {
            pathpointAPIs.Add((PathpointAPI)pathpoint.ToAPI());
        }
        var batch = new PathpointAPIBatch{ pathpoints = pathpointAPIs.ToArray() };

        if (pathpoints.Capacity > 0)
        {
            PaganiniRestAPI.Pathpoint.BatchCreate(parentRouteId, batch, BatchCreatePathpointsSucceeded, CreateOrUpdateFailed);
            return;
        }

        UpdateExistingPathpointsPoints(parentRouteId);
    }

    public void UpdateExistingPathpointsPoints(int parentRouteId)
    {
        //TODO: Here it would make sense to update all pathpoints, even those with POIs included

        var pathpoints = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id, p => p.FromAPI && p.IsDirty && p.POIType == Pathpoint.POIsType.Point);

        // prepare batch
        List<IPathpointAPI> pathpointAPIs = new ();
        foreach (var pathpoint in pathpoints)
        {
            pathpointAPIs.Add(pathpoint.ToAPI());
        }
        var batch = new PathpointAPIBatch { pathpoints = pathpointAPIs.ToArray() };

        // send batch
        if (pathpoints.Capacity > 0)
        {
            PaganiniRestAPI.Pathpoint.BatchUpdate(parentRouteId, batch, BatchUpdatePathpointsSucceeded, CreateOrUpdateFailed);
            return;
        }

        // POIs

        UpdatePathpointsPOIs(parentRouteId);
    }


    public void UpdatePathpointsPOIs(int parentRouteId)
    {
        var pathpoints = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType != Pathpoint.POIsType.Point);

        // dconstruct PathpointPOI

        if (pathpoints.Capacity > 0)
        {
            Dictionary<string, byte[]> pictures = new();

            List<PathpointPOIAPI> pathpointPOIAPIs = new List<PathpointPOIAPI>();
            foreach (Pathpoint pathpoint in pathpoints)
            {
                var poi = new PathpointPOIAPI();
                poi.pathpoint = (PathpointAPI)pathpoint.ToAPI();

                var photos = PathpointPhoto.GetPathpointPhotoListByPOI(pathpoint.Id);
                List<PathpointPhotoAPI> photoAPIs = new List<PathpointPhotoAPI>();
                foreach (var photo in photos)
                {
                    // internal photo reference
                    string photoRef = string.Format("Pic{0}{1}", pathpoint.Timestamp, Mathf.Abs(photo.Id));
                    // photo metadata
                    var photoAPI = photo.ToAPI();
                    photoAPI.photo_reference = photoRef;
                    photoAPIs.Add(photoAPI);
                    // photo files
                    pictures.Add(photoRef, photo.Photo);
                }
                poi.photos = photoAPIs.ToArray();
                pathpointPOIAPIs.Add(poi);
            }

            var batch = new PathpointPOIAPIBatch
            {
                pois = pathpointPOIAPIs.ToArray(),
                files = pictures
            };

            PaganiniRestAPI.PathpointPOI.BatchCreate(parentRouteId, batch, BatchCreatePathpointsPOISucceeded, CreateOrUpdateFailed);

            return;
        }

        //TODO: Here we should update photos
        // photos updated
        // new photos added to existing pictures
    }


    // Event handlers


    /// <summary>
    /// This function is called when the CreateOrUpdate method of the Way class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="way">Way returned from to the API.</param>
    private void CreateOrUpdateWaySucceeded(WayAPIResult way)
    {
        // Delete Current way
        Way.Delete<Way>(CurrentWay.Id);

        // Insert new definition
        var waydb = new Way(way);
        waydb.Insert();

        // We update reference of all the local routes
        // associated with the way
        Route.ChangeParent(CurrentWay.Id, waydb.Id);

        CurrentWay = waydb;

        // continue to updating the route
        UpdateRouteDefinition(waydb.Id);

        
    }

    /// <summary>
    /// This function is called when the CreateOrUpdate method of the Route class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="route">Route returned from the API.</param>
    private void CreateOrUpdateRouteSucceeded(RouteAPIResult route)
    {
        // Delete Current way
        Route.Delete<Route>(CurrentRoute.Id);

        // Insert new definition
        var routedb = new Route(route);
        routedb.Insert();

        // We update reference of all the local routes
        // associated with the way
        Pathpoint.ChangeParent(CurrentRoute.Id, routedb.Id);

        CurrentRoute = routedb;

        UpdateNewPathpointsPoints(routedb.Id);
    }

    /// <summary>
    /// This function is called when the BatchCreate method of the Pathpoint class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="pathpointList">PathpointAPIList returned from the API.</param>
    private void BatchCreatePathpointsSucceeded(PathpointAPIList pathpointList)
    {
        // Delete updated pathpoints

        //Pathpoint.DeletePathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType == Pathpoint.POIsType.Point);

        Debug.Log(pathpointList);

        UpdateExistingPathpointsPoints(CurrentRoute.Id);
    }

    /// <summary>
    /// This function is called when the BatchCreateUpdate method of the Pathpoint class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="pathpointList">PathpointAPIList returned from the API.</param>
    private void BatchUpdatePathpointsSucceeded(PathpointAPIList pathpointList)
    {
        // Delete updated pathpoints

        //Pathpoint.DeletePathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType == Pathpoint.POIsType.Point);

        Debug.Log(pathpointList);

        UpdatePathpointsPOIs(CurrentRoute.Id);
    }

    /// <summary>
    /// This function is called when the BatchCreate method of the PathpointPOI class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="poiAPIList">PathpointPOIAPIList returned from the API.</param>
    private void BatchCreatePathpointsPOISucceeded(PathpointPOIAPIList poiAPIList)
    {
        // Delete updated pathpoints

        //Pathpoint.DeletePathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType == Pathpoint.POIsType.Point);

        Debug.Log(poiAPIList);

        //UpdatePathpointsPOIs(CurrentRoute.Id);
    }


    /// <summary>
    /// This function is called when the GetAll method of the User class in the PaganiniRestAPI namespace fails.
    /// </summary>
    /// <param name="errorMessage">The error message returned by the API.</param>
    private void CreateOrUpdateFailed(string errorMessage)
    {
        Debug.Log(errorMessage);
    }



}
