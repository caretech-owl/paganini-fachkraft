using System;
using System.Collections.Generic;
using System.Linq;
using PaganiniRestAPI;
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

    public event EventHandler OnDataPartiallyDownloaded;
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

    private void FlagIfDraftUpdated(bool? updated)
    {
        if (CurrentWay.IsDirty || CurrentRoute.IsDirty)
        {
            updated = true;
        }

        CurrentRoute.IsDraftUpdated = updated;
        CurrentRoute.Insert();
    }


    /**********************
     *  Data processing  *
     **********************/

    #region Download Route Definition
    public void DownloadRouteDefinition()
    {
        Debug.Log("Downloading Route Definition.");
        CurrentWay = Way.Get(AppState.CurrentRoute.WayId);
        CurrentRoute = AppState.CurrentRoute;
        // Let's start the flag with no changes
        CurrentRoute.IsDraftUpdated = false;

        if (CurrentRoute.FromAPI)
        {
            DownloadPathpoints();
        }
        else // No need to download again
        {
            OnDataPartiallyDownloaded?.Invoke(this, EventArgs.Empty);
            OnDataDownloaded?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DownloadPathpoints()
    {
        Debug.Log("Downloading Pathpoints.");
        PaganiniRestAPI.Pathpoint.GetAll(AppState.CurrentRoute.Id, GetPathpointsSucceeded, LoadFailed);
    }

    private void DownloadPathpointPhotos()
    {
        Debug.Log("Downloading Pathpoint photos.");
        PaganiniRestAPI.PathpointPOI.GetAll(AppState.CurrentRoute.Id, GetPathpointPOIsSucceeded, LoadFailed);
    }

    private void DownloadPhotoData()
    {
        Debug.Log("Downloading Photo Data.");

        var lastUpdate = PhotoData.GetLastUpdateByRoute(AppState.CurrentRoute.Id);

        Dictionary<string, string> query = new Dictionary<string, string> { };
        if (lastUpdate != null)
        {
            var sinceDate = DateUtils.ConvertMillisecondsToUTCString(lastUpdate, "yyyy-MM-dd'T'HH:mm:ss");
            query = new Dictionary<string, string>
            {
                { "sinceDate", sinceDate }
            };
        }

        PaganiniRestAPI.PhotoData.GetAll(AppState.CurrentRoute.Id, query, GetPhotoDataSucceeded, LoadFailed);
    }

    #endregion

    #region Delete Definition
    public void DeleteWayDefinition()
    {
        // Delete Route
        var route = Route.Get(CurrentRoute.Id);
        if (route.FromAPI)
        {
            route.Status = Route.RouteStatus.Discarded;
            PaganiniRestAPI.Route.CreateOrUpdate(route.WayId, route.ToAPI(), DeleteRouteSucceeded, DeleteRouteFailed);            
        }
        else
        {
            DeleteRouteLocalContents();
        }

    }

    private void DeleteRouteLocalContents()
    {
        // Delete Way
        var way = Way.Get(CurrentWay.Id);
        if (!way.FromAPI)
        {
            // If there is only one route associated to the local way, we delete the way
            var routes = Route.GetRouteListByWay(way.Id);
            if (routes.Count <= 1)
            {
                Way.Delete(way.Id);
            }
        }
        // Delete route
        Route.Delete(CurrentRoute.Id);
        // Delete all photo data from this route
        PhotoData.DeleteFromRoute(CurrentRoute.Id);
        // Delete all photos from this route
        PathpointPhoto.DeleteFromPOIs(CurrentRoute.Id);
        // Delete all pathpoints from this route
        Pathpoint.DeleteFromRoute(CurrentRoute.Id, null, null);      

        OnDataUploaded?.Invoke(this, EventArgs.Empty);

    }

    /// <summary>
    /// This function is called when the CreateOrUpdate method to discard or 'delete' a route succeeds.
    /// </summary>
    /// <param name="route">Route returned from the API.</param>
    private void DeleteRouteSucceeded(RouteAPIResult route)
    {
        DeleteRouteLocalContents();
    }

    private void DeleteRouteFailed(string errorMessage)
    {
        Debug.Log(errorMessage);

        OnDataUploadError?.Invoke("Error discarding route: " + errorMessage);
    }

    #endregion

    private void InformStatus(string step)
    {
        Debug.Log(step);
        //UploadWayDefinition
        //UploadRouteDefinition
        //UploadNewAddedPathPhotos
        //UploadUpdatedPhotoData
        // UploadNewPathpointsPoints
        //UploadExistingPathpointsPoints
        // UploadNewPathpointsPOIs
    }

    public void UploadWayDefinition()
    {
        InformStatus("UploadWayDefinition");

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
        InformStatus("UploadRouteDefinition");

        // Before uploading the route, let's get rid of POIs we discarded that shouldn't be synced        
        DeleteDiscardedPOIsLocally(CurrentRoute.Id);

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

    // We delete information about the discarded POIs, so that they do not get uploaded
    private void DeleteDiscardedPOIsLocally(int parentRouteId)
    {
        var removeList = Pathpoint.GetPathpointListByRoute(parentRouteId, p => p.CleaningFeedback == Pathpoint.POIFeedback.No);

        foreach (var poi in removeList)
        {
            PhotoData.DeleteByPOI(poi.Id);
            PathpointPhoto.DeleteByPOI(poi.Id);
            // If the POI is from the API, we do update it as 'discarded'
            // If not, we delete it locally, so that it's never sent to the server
            if (!poi.FromAPI)
            {
                Pathpoint.Delete(poi.Id);
            }
            
        }

    }

    // upload the new pathpoints + photos in batch the first time
    public void UploadNewAddedPathPhotos(int parentRouteId)
    {
        InformStatus("UploadNewAddedPathPhotos");        

        // get new photos that belong to pathpoints (from the API) on the current route
        var photos = PathpointPhoto.GetListByRoute(parentRouteId, true, p => p.FromAPI == false);

        if (photos.Count > 0)
        {

            Dictionary<string, byte[]> pictures = new();

            List<IPathpointPhotoAPI> photoAPIs = new List<IPathpointPhotoAPI>();
            foreach (PathpointPhoto photo in photos)
            {
                var data = PhotoData.Get(photo.PhotoId);

                // internal photo reference
                string photoRef = string.Format("Pic{0}", Mathf.Abs(photo.Id));
                // photo metadata
                var photoAPI = photo.ToAPIBatchElement();
                photoAPI.photo_reference = photoRef;
                photoAPIs.Add(photoAPI);
                // photo files
                pictures.Add(photoRef, data.Photo);
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
        InformStatus("UploadUpdatedPathPhotos");
        // get updated photos that belong to pathpoints (from the API) on the current route
        // let's not update the photos
        var photos = PathpointPhoto.GetListByRoute(parentRouteId, true, p => p.FromAPI == true && p.IsDirty == true);

        if (photos.Count > 0)
        {

            //Dictionary<string, byte[]> pictures = new();

            List<IPathpointPhotoAPI> photoAPIs = new List<IPathpointPhotoAPI>();
            foreach (PathpointPhoto photo in photos)
            {
                //var data = PhotoData.Get(photo.PhotoId);
                // internal photo reference
                //string photoRef = string.Format("Pic{0}", Mathf.Abs(photo.Id));

                // photo metadata
                var photoAPI = photo.ToAPIBatchElement();

                // Do we update the picture as well?
                //if (data.IsDirty)
                //{
                //    pictures.Add(photoRef, data.Photo);
                //    photoAPI.photo_reference = photoRef;
                //}

                photoAPIs.Add(photoAPI);

            }

            var batch = new PathpointPhotoAPIBatch
            {
                photos = photoAPIs.ToArray()
            //    files = pictures
            };

            PaganiniRestAPI.PathpointPhoto.BatchUpdate(parentRouteId, batch, BatchUpdatePathpointPhotosSucceeded, CreateOrUpdateFailed);

            return;
        }


        UploadUpdatedPhotoData(parentRouteId);
    }

    // We upload the updated photoData, independently of whether the PathtpointPhoto was updated or not
    public void UploadUpdatedPhotoData(int parentRouteId)
    {
        InformStatus("UploadUpdatedPhotoData");

        var photos = PhotoData.GetListByRoute(parentRouteId, true, p => p.FromAPI == true && p.IsDirty == true);

        if (photos.Count > 0)
        {
            List<IPhotoDataAPI> dataAPI = new();
            Dictionary<string, byte[]> pictures = new();

            foreach (var photo in photos)
            {
                string photoRef = string.Format("Pic{0}", Mathf.Abs(photo.Id));
                pictures.Add(photoRef, photo.Photo);

                var photoAPI = photo.ToAPI();
                photoAPI.photo_reference = photoRef;
                dataAPI.Add(photoAPI);
            }

            var batch = new PhotoDataAPIBatch { data = dataAPI.ToArray(), files = pictures };

            PaganiniRestAPI.PhotoData.BatchUpdate(parentRouteId, batch, BatchUpdatePhotoDataSucceeded, CreateOrUpdateFailed);

            return;

        }
        UploadNewPathpointsPoints(parentRouteId);
    }


    public void UploadNewPathpointsPoints(int parentRouteId)
    {
        InformStatus("UploadNewPathpointsPoints");

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
        InformStatus("UploadExistingPathpointsPoints");
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
        InformStatus("UploadNewPathpointsPOIs");
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
                    var data = PhotoData.Get(photo.PhotoId);

                    // internal photo reference
                    string photoRef = string.Format("Pic{0}{1}", pathpoint.Timestamp, Mathf.Abs(photo.Id));
                    // photo metadata
                    var photoAPI = photo.ToAPI();
                    photoAPI.photo_reference = photoRef;
                    photoAPIs.Add(photoAPI);
                    // photo files
                    pictures.Add(photoRef, data.Photo);
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

        // Everything uploaded, nothing left to be updated
        CurrentRoute.IsDraftUpdated = false;
        CurrentRoute.Insert();

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
        Debug.Log("Processing Pathpoints.");
        // Delete local cache
        Pathpoint.DeleteNonDirtyCopies();

        int nDirty = 0;
        // Insert clean verion
        foreach (var point in res.pathpoints)
        {
            // Insert pathpoint only if it's not already here
            if (!CurrentRoute.FromAPI || !Pathpoint.CheckIfExists(p => p.IsDirty && p.Id == point.ppoint_id))
            {
                Pathpoint p = new Pathpoint(point);
                p.Insert();
            }
            else
            {
                nDirty++;
            }
        }

        if (nDirty >0)
        {
            Debug.Log($"There are some dirty pathpoints: {nDirty}");
            CurrentRoute.IsDraftUpdated = true;
        }

        DownloadPathpointPhotos();
    }

    private void GetPathpointPOIsSucceeded(PathpointPOIAPIList res)
    {
        Debug.Log("Processing Photos.");
        PathpointPhoto.DeleteNonDirtyCopies();

        int nDirty = 0;
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
                    else
                    {
                        nDirty++;
                    }
                }

            }
        }

        if (nDirty > 0)
        {
            Debug.Log($"There are some dirty pois: {nDirty}");
            CurrentRoute.IsDraftUpdated = true;
        }

        OnDataPartiallyDownloaded?.Invoke(this, EventArgs.Empty);
        DownloadPhotoData();

    }

    private void GetPhotoDataSucceeded(PhotoDataAPIList res)
    {
        int nUpdated = 0;
        int nDirty = 0;
        if (res != null && res.data != null)
        {
            foreach (var photo in res.data)
            {

                if (!CurrentRoute.FromAPI || !PhotoData.CheckIfExists(p => p.IsDirty && p.Id == photo.photo_id))
                {
                    PhotoData data = new PhotoData(photo);
                    data.Insert();
                    nUpdated++;
                }
                else
                {
                    nDirty++;
                }

            }
        }

        if (nDirty > 0)
        {
            Debug.Log($"There are some dirty photos: {nDirty}");
            CurrentRoute.IsDraftUpdated = true;
        }

        // Let's update the Route, and the isdraftupdated flag.
        FlagIfDraftUpdated(CurrentRoute.IsDraftUpdated);

        Debug.Log($"Done Downloading Route Definition. (Updated = {nUpdated} photos)");
        // Load editor
        OnDataDownloaded?.Invoke(this, EventArgs.Empty);
    }   

    /// <summary>
    /// This function is called when the CreateOrUpdate method of the Way class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="way">Way returned from to the API.</param>
    private void CreateOrUpdateWaySucceeded(WayAPIResult way)
    {
        InformStatus("-> CreateOrUpdateWaySucceeded");

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
        InformStatus("-> CreateOrUpdateRouteSucceeded");
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
        InformStatus("-> BatchCreatePathpointPhotosSucceeded");

        // Delete photos for the newly created POIs (routeId, pathpoint fromAPI, pathphoto fromAPI)
        PhotoData.DeleteFromRoute(CurrentRoute.Id, true, false);

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
        InformStatus("-> BatchUpdatePathpointPhotosSucceeded");
        // We do not delete photo data here, since we update this in a separate call
        // which is called at the end of this function

        // Delete Pathphotos for newly added pathpoints (routeId, pathpoint fromAPI, pathphoto fromAPI)
        PathpointPhoto.DeleteFromPOIs(CurrentRoute.Id, true, true);

        UploadUpdatedPhotoData(CurrentRoute.Id);
    }

    private void BatchUpdatePhotoDataSucceeded(PhotoDataAPIList photoDataAPIList)
    {
        InformStatus("-> BatchUpdatePhotoDataSucceeded");

        UploadNewPathpointsPoints(CurrentRoute.Id);

        //TODO: Update the last_update flag from the pictures

        foreach (var photoAPI in photoDataAPIList.data)
        {
            var photo = PhotoData.Get(photoAPI.photo_id);
            var uPhoto = new PhotoData(photoAPI); // contains the new lastUpdate reference

            photo.LastUpdate = uPhoto.LastUpdate;
            photo.IsDirty = false;
            photo.Insert();
        }

    }
    


    /// <summary>
    /// This function is called when the BatchCreate method of the Pathpoint class in the PaganiniRestAPI namespace succeeds.
    /// </summary>
    /// <param name="pathpointList">PathpointAPIList returned from the API.</param>
    private void BatchCreatePathpointsSucceeded(PathpointAPIList pathpointList)
    {
        InformStatus("-> BatchCreatePathpointsSucceeded");
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
        InformStatus("-> BatchUpdatePathpointsSucceeded");
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
        InformStatus("-> BatchCreatePathpointsPOISucceeded");
        // Delete photos for the newly created POIs (routeId, pathpoint fromAPI, pathphoto fromAPI)
        PhotoData.DeleteFromRoute(CurrentRoute.Id, false, false);

        // Delete Pathphotos for newly uploaded pathpoints (routeId, pathpoint fromAPI, pathphoto fromAPI)
        PathpointPhoto.DeleteFromPOIs(CurrentRoute.Id, false, false);

        // Delete the newly uploaded Pathpoints POIs 
        Pathpoint.DeleteFromRoute(CurrentRoute.Id, new bool[] { false }, new Pathpoint.POIsType[] { Pathpoint.POIsType.Landmark, Pathpoint.POIsType.Reassurance, Pathpoint.POIsType.WayStart, Pathpoint.POIsType.WayDestination });

        //Pathpoint.DeletePathpointListByRoute(CurrentRoute.Id, p => p.FromAPI == false && p.POIType == Pathpoint.POIsType.Point);

        Debug.Log(poiAPIList);

        // We signal that we submitted the draft, and nothing to update anymore
        CurrentRoute.IsDraftUpdated = false;
        CurrentRoute.Insert();

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

