using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SharedDataErrorHandler : UnityEvent<string>
{
}

public class RouteSharedData : PersistentLazySingleton<RouteSharedData>
{
    public Way CurrentWay;
    public Route CurrentRoute;
    public List<Pathpoint> PathpointList;
    public List<Pathpoint> POIList;
    public EditorMode CurrentEditorMode;

    // Route Editing variables
    public Pathpoint CurrentPOI;
    public int CurrentPOIIndex;

    private List<Pathpoint> DirtyPathpointPointList;

    public event EventHandler OnDataDownloaded;
    public event EventHandler OnDataUploaded;
    public SharedDataErrorHandler OnDataUploadError;
    public SharedDataErrorHandler OnDataDownloadError;

    public enum EditorMode
    {
        Cleaning = 0,
        Discussion = 1,
        ReadOnly = 2,
    }


    public RouteSharedData()
    {
    }

    public void LoadRouteFromDatabase()
    {
        PathpointList = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id);
        POIList = PathpointList.Where(item => item.POIType != Pathpoint.POIsType.Point).ToList();
    }


    /**********************
     *  Data processing  *
     **********************/

    public void DownloadRouteDefinition()
    {
        Debug.Log("Downloading Route Definition.");
        CurrentWay = Way.Get(AppState.CurrentRoute.WayId);
        CurrentRoute = AppState.CurrentRoute;

        if (CurrentRoute.FromAPI)
        {
            DownloadPathpoints();
        }
        else // No need to download again
        {
            OnDataDownloaded?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DownloadPathpoints()
    {
        PaganiniRestAPI.Pathpoint.GetAll(AppState.CurrentRoute.Id, GetPathpointsSucceeded, LoadFailed);
    }

    private void DownloadPhotos()
    {
        PaganiniRestAPI.PathpointPOI.GetAll(AppState.CurrentRoute.Id, GetPathpointPOIsSucceeded, LoadFailed);
    }


    public void UploadWayDefinition()
    {
        // get latest definition from db
        var way = Way.Get(CurrentWay.Id);

        if (way.IsDirty)
        {
            PaganiniRestAPI.Way.CreateOrUpdate(way.UserId, way.ToAPI(), CreateOrUpdateWaySucceeded, CreateOrUpdateFailed);
            return;
        }

        UploadRouteDefinition(way.Id);
    }

    public void UploadRouteDefinition(int parentWayId)
    {
        // get latest definition from db
        var route = Route.Get(CurrentRoute.Id);

        if (route.IsDirty)
        {
            route.WayId = parentWayId;
            PaganiniRestAPI.Route.CreateOrUpdate(route.WayId, route.ToAPI(), CreateOrUpdateRouteSucceeded, CreateOrUpdateFailed);
            return;
        }

        UploadNewAddedPathPhotos(route.Id);


    }

    // upload the new pathpoints + photos in batch the first time
    public void UploadNewAddedPathPhotos(int parentRouteId)
    {
        // get new photos that belong to pathpoints (from the API) on the current route
        var photos = PathpointPhoto.GetListByRoute(parentRouteId, true, p => p.FromAPI == false);

        if (photos.Count > 0)
        {

            Dictionary<string, byte[]> pictures = new();

            List<IPathpointPhotoAPI> photoAPIs = new List<IPathpointPhotoAPI>();
            foreach (PathpointPhoto photo in photos)
            {
                // internal photo reference
                string photoRef = string.Format("Pic{0}", Mathf.Abs(photo.Id));
                // photo metadata
                var photoAPI = photo.ToAPIBatchElement();
                photoAPI.photo_reference = photoRef;
                photoAPIs.Add(photoAPI);
                // photo files
                pictures.Add(photoRef, photo.Photo);
            }

            var batch = new PathpointPhotoAPIBatch
            {
                photos = photoAPIs.ToArray(),
                files = pictures
            };

            PaganiniRestAPI.PathpointPhoto.BatchCreate(parentRouteId, batch, BatchCreatePathpointPhotosSucceeded, CreateOrUpdateFailed);

            return;
        }

        UploadUpdatedPathPhotos(parentRouteId);
    }

    public void UploadUpdatedPathPhotos(int parentRouteId)
    {
        // get updated photos that belong to pathpoints (from the API) on the current route
        var photos = PathpointPhoto.GetListByRoute(parentRouteId, true, p => p.FromAPI == true && p.IsDirty == true);

        if (photos.Count > 0)
        {

            Dictionary<string, byte[]> pictures = new();

            List<IPathpointPhotoAPI> photoAPIs = new List<IPathpointPhotoAPI>();
            foreach (PathpointPhoto photo in photos)
            {
                // internal photo reference
                string photoRef = string.Format("Pic{0}", Mathf.Abs(photo.Id));
                // photo metadata
                var photoAPI = photo.ToAPIBatchElement();
                photoAPI.photo_reference = photoRef;
                photoAPIs.Add(photoAPI);
                // photo files
                pictures.Add(photoRef, photo.Photo);
            }

            var batch = new PathpointPhotoAPIBatch
            {
                photos = photoAPIs.ToArray(),
                files = pictures
            };

            PaganiniRestAPI.PathpointPhoto.BatchUpdate(parentRouteId, batch, BatchUpdatePathpointPhotosSucceeded, CreateOrUpdateFailed);

            return;
        }


        UploadNewPathpointsPoints(parentRouteId);
    }



    public void UploadNewPathpointsPoints(int parentRouteId)
    {
        var pathpoints = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType == Pathpoint.POIsType.Point);
        var batch = PreparePathpointBatch(pathpoints);

        if (pathpoints.Count > 0)
        {
            PaganiniRestAPI.Pathpoint.BatchCreate(parentRouteId, batch, BatchCreatePathpointsSucceeded, CreateOrUpdateFailed);
            return;
        }

        UploadExistingPathpointsPoints(parentRouteId);
    }

    public void UploadExistingPathpointsPoints(int parentRouteId)
    {
        //TODO: Here it would make sense to update all pathpoints, even those with POIs included

        var pathpoints = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id, p => p.FromAPI && p.IsDirty);

        var batch = PreparePathpointBatch(pathpoints);

        // send batch
        if (pathpoints.Count > 0)
        {
            PaganiniRestAPI.Pathpoint.BatchUpdate(parentRouteId, batch, BatchUpdatePathpointsSucceeded, CreateOrUpdateFailed);
            return;
        }

        // POIs

        UploadNewPathpointsPOIs(parentRouteId);
    }


    // upload the new pathpoints + photos in batch the first time
    public void UploadNewPathpointsPOIs(int parentRouteId)
    {
        var pathpoints = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType != Pathpoint.POIsType.Point);

        // dconstruct PathpointPOI

        if (pathpoints.Count > 0)
        {
            Dictionary<string, byte[]> pictures = new();

            List<PathpointPOIAPI> pathpointPOIAPIs = new List<PathpointPOIAPI>();
            foreach (Pathpoint pathpoint in pathpoints)
            {
                var poi = new PathpointPOIAPI();
                poi.pathpoint = (PathpointAPI)pathpoint.ToAPI();

                var photos = PathpointPhoto.GetPathpointPhotoListByPOI(pathpoint.Id);
                List<IPathpointPhotoAPI> photoAPIs = new List<IPathpointPhotoAPI>();
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

        OnDataUploaded?.Invoke(this, EventArgs.Empty);
    }



    private PathpointAPIBatch PreparePathpointBatch(List<Pathpoint> pathpoints)
    {
        // prepare batch
        List<IPathpointAPI> pathpointAPIs = new();
        foreach (var pathpoint in pathpoints)
        {
            pathpointAPIs.Add(pathpoint.ToAPI());
        }
        var batch = new PathpointAPIBatch { pathpoints = pathpointAPIs.ToArray() };
        return batch;
    }


    // Event handlers

    private void GetPathpointsSucceeded(PathpointAPIList res)
    {
        // Delete local cache
        Pathpoint.DeleteNonDirtyCopies();

        // Insert clean verion
        foreach (var point in res.pathpoints)
        {
            // Insert pathpoint only if it's not already here
            if (!CurrentRoute.FromAPI || !Pathpoint.CheckIfExists(p => p.IsDirty && p.Id == point.ppoint_id))
            {
                Pathpoint p = new Pathpoint(point);
                p.Insert();
            }

        }

        DownloadPhotos();
    }

    private void GetPathpointPOIsSucceeded(PathpointPOIAPIList res)
    {
        PathpointPhoto.DeleteNonDirtyCopies();

        if (res != null && res.pois != null)
        {
            // Insert clean verion
            foreach (var poi in res.pois)
            {
                foreach (var photo in poi.photos)
                {
                    // Insert pathpoint only if it's not already here
                    if (!CurrentRoute.FromAPI || !PathpointPhoto.CheckIfExists(p => p.IsDirty && p.Id == photo.pphoto_id))
                    {
                        PathpointPhoto p = new PathpointPhoto(photo);
                        p.Insert();
                    }
                }

            }
        }

        // Load editor
        OnDataDownloaded?.Invoke(this, EventArgs.Empty);
    }


    /// <summary>
    /// This function is called when the CreateOrUpdate method of the Way class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="way">Way returned from to the API.</param>
    private void CreateOrUpdateWaySucceeded(WayAPIResult way)
    {
        // Delete Current way
        Way.Delete(CurrentWay.Id);

        // Insert new definition
        var waydb = new Way(way);
        waydb.UserId = CurrentWay.UserId;
        waydb.Insert();

        // We update reference of all the local routes
        // associated with the way
        Route.ChangeParent(CurrentWay.Id, waydb.Id);

        CurrentWay = waydb;
        AppState.CurrentWay = CurrentWay;

        // continue to updating the route
        UploadRouteDefinition(waydb.Id);
    }

    /// <summary>
    /// This function is called when the CreateOrUpdate method of the Route class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="route">Route returned from the API.</param>
    private void CreateOrUpdateRouteSucceeded(RouteAPIResult route)
    {
        // Delete Current way
        Route.Delete(CurrentRoute.Id);

        // Insert new definition
        var routedb = new Route(route);
        routedb.WayId = CurrentWay.Id;
        routedb.Insert();

        // We update reference of all the local routes
        // associated with the way
        Pathpoint.ChangeParent(CurrentRoute.Id, routedb.Id);

        CurrentRoute = routedb;
        AppState.CurrentRoute = CurrentRoute;

        UploadNewAddedPathPhotos(routedb.Id);
    }

    /// <summary>
    /// This function is called when the BatchCreate method of the PathpointPhoto class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="photoAPIList">PathpointPhotoAPIList returned from the API.</param>
    private void BatchCreatePathpointPhotosSucceeded(PathpointPhotoAPIList photoAPIList)
    {
        // Delete Pathphotos for newly added pathpoints (routeId, pathpoint fromAPI, pathphoto fromAPI)
        // TODO: This is not working
        PathpointPhoto.DeleteFromPOIs(CurrentRoute.Id, true, false);

        UploadUpdatedPathPhotos(CurrentRoute.Id);
    }

    /// <summary>
    /// This function is called when the BatchUpdate method of the PathpointPhoto class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="photoAPIList">PathpointPhotoAPIList returned from the API.</param>
    private void BatchUpdatePathpointPhotosSucceeded(PathpointPhotoAPIList photoAPIList)
    {
        // Delete Pathphotos for newly added pathpoints (routeId, pathpoint fromAPI, pathphoto fromAPI)
        PathpointPhoto.DeleteFromPOIs(CurrentRoute.Id, true, true);

        UploadNewPathpointsPoints(CurrentRoute.Id);
    }

    /// <summary>
    /// This function is called when the BatchCreate method of the Pathpoint class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="pathpointList">PathpointAPIList returned from the API.</param>
    private void BatchCreatePathpointsSucceeded(PathpointAPIList pathpointList)
    {
        // Delete updated pathpoints
        // We delete the local pathpoints (fromAPI=false) for the CurrentRoute, specifically those that are Point
        Pathpoint.DeleteFromRoute(CurrentRoute.Id, new bool[] { false }, new Pathpoint.POIsType[] { Pathpoint.POIsType.Point });

        Debug.Log(pathpointList);

        UploadExistingPathpointsPoints(CurrentRoute.Id);
    }

    /// <summary>
    /// This function is called when the BatchCreateUpdate method of the Pathpoint class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="pathpointList">PathpointAPIList returned from the API.</param>
    private void BatchUpdatePathpointsSucceeded(PathpointAPIList pathpointList)
    {
        // Delete updated pathpoints
        // We delete the local caches (fromAPI=true) of the pathpoints for the CurrentRoute, all pathpointtypes
        Pathpoint.DeleteFromRoute(CurrentRoute.Id, new bool[] { true }, null);

        Debug.Log(pathpointList);

        UploadNewPathpointsPOIs(CurrentRoute.Id);
    }

    /// <summary>
    /// This function is called when the BatchCreate method of the PathpointPOI class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="poiAPIList">PathpointPOIAPIList returned from the API.</param>
    private void BatchCreatePathpointsPOISucceeded(PathpointPOIAPIList poiAPIList)
    {
        // Delete updated pathpoints

        // Delete Pathphotos for newly uploaded pathpoints (routeId, pathpoint fromAPI, pathphoto fromAPI)
        PathpointPhoto.DeleteFromPOIs(CurrentRoute.Id, false, false);

        // Delete the newly uploaded Pathpoints POIs 
        Pathpoint.DeleteFromRoute(CurrentRoute.Id, new bool[] { false }, new Pathpoint.POIsType[] { Pathpoint.POIsType.Landmark, Pathpoint.POIsType.Reassurance });

        //Pathpoint.DeletePathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType == Pathpoint.POIsType.Point);

        Debug.Log(poiAPIList);

        OnDataUploaded?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// This function is called when the Upload methods fail.
    /// </summary>
    /// <param name="errorMessage">The error message returned by the API.</param>
    private void CreateOrUpdateFailed(string errorMessage)
    {
        Debug.Log(errorMessage);

        OnDataUploadError?.Invoke(errorMessage);
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

