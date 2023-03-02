using Assets;
using NinevaStudios.GoogleMaps;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RouteEditorController : MonoBehaviour
{
    [Header("Components")]
    public VideoPlayerPrefab VideoManager;
    public PinListPrefab PinList;

    public Texture2D DefaultMarkerIcon;
    public Texture2D POILandmarkMarkerIcon;
    public Texture2D POIReassuranceMarkerIcon;

    //PRIVATE
    private static Texture2D StaticMarkerIcon;
    private GoogleMapsView Map;

    private Way CurrentWay;
    private Route CurrentRoute;
    private List<Pathpoint> PathpointList;
    private List<Pathpoint> POIList;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadRouteData();

        LoadMap(19);

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
        VideoManager.SkipToVideoFrame(skipTime);
    }

    /// <summary>
    /// Toggle the route video representation, where active turns on video and inactive the map
    /// </summary>
    /// <param name="videoActive">Whether video should be active</param> 
    public void ToggleRouteRepresentation(bool videoActive)
    {
        if (videoActive)
        {
            // Activating the component, and then resuming to the last timestamp
            VideoManager.gameObject.SetActive(videoActive);
            VideoManager.ResumeVideo();
        }
        else // videoActive = false
        {
            // Pausing the component to get the current timestamp,
            // and then disabling the component
            VideoManager.PauseVideo();
            VideoManager.gameObject.SetActive(videoActive);
        }
        
    }


    private void LoadRouteData()
    {
        CurrentWay = Way.Get<Way>(AppState.CurrentRoute.WayId);
        CurrentRoute = AppState.CurrentRoute;
        PathpointList = Pathpoint.GetPathpointListByRoute(CurrentRoute.Id);
        POIList = PathpointList.Where(item => item.POIType != (int)Pathpoint.POIsType.Point).ToList();
    }

    private void LoadMap(int zoom)
    {
        StaticMarkerIcon = DefaultMarkerIcon;

        // start point
        Pathpoint startPoint = PathpointList[0];

        // initialize Map
        int moveMapForPhone = 0;
        var cameraPosition = new CameraPosition(
        new LatLng(startPoint.Latitude, startPoint.Longitude), zoom, 0, 0);
        var options = new GoogleMapsOptions()
            .Camera(cameraPosition);

        Map = new GoogleMapsView(options);
        Map.Show(new Rect(150 + moveMapForPhone, 280, 1100 + moveMapForPhone, 750), OnMapReady);
        //GameObject.Find("ButtonSmallRoute").GetComponent<Button>().Select();
        

    }

    private void LoadVideo()
    {
        VideoManager.LoadVideo(FileManagement.persistentDataPath + "/" + CurrentRoute.LocalVideoFilename);
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


    /// <summary>
    /// Event listener when map is ready for operations
    /// </summary>
    private void OnMapReady()
    {
        Debug.Log("The map is ready!");

        StaticMarkerIcon = DefaultMarkerIcon;

        var i = 0;

        foreach (var pathpoint in PathpointList)
        {
            if (i % 1 == 0)
            {
                if (pathpoint.POIType > 0)
                    StaticMarkerIcon = POILandmarkMarkerIcon;
                else
                    StaticMarkerIcon = DefaultMarkerIcon;

                var mo = new MarkerOptions()
                       .Position(new LatLng(pathpoint.Latitude, pathpoint.Longitude))
                       .Icon(NewCustomDescriptor());

                Map.AddMarker(mo);
            }

            i++;

            Debug.Log("OnMapReady -Pathpoint #: " + i);
        }

        GameObject.Find("ButtonSmallRoute").GetComponent<Button>().Select();
    }

    static ImageDescriptor NewCustomDescriptor()
    {
        return ImageDescriptor.FromTexture2D(StaticMarkerIcon);
    }


    // PUBLIC FUNCTIONS
    public void DisableMap()
    {
        Map.IsVisible = false;
    }

    public void EnableMap()
    {
        Map.IsVisible = true;
    }

}
