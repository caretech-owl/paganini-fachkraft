using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LocationUtils;
using NinevaStudios.GoogleMaps;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Texture2D DefaultMarkerIcon;
    public Texture2D POILandmarkMarkerIcon;
    public Texture2D POIReassuranceMarkerIcon;

    //PRIVATE
    private static Texture2D StaticMarkerIcon;
    private GoogleMapsView Map;
    private List<Pathpoint> PathpointList;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**********************
     *  Public UI events (and utilities)  *
     **********************/

    /// <summary>
    /// Load the map component
    /// </summary>
    /// <param name="pathpoints">Pathpoints to render in the map</param> 
    public void LoadMap(List<Pathpoint> pathpoints)
    {
        PathpointList = pathpoints;
        LoadMap(19);
    }

    /// <summary>
    /// Disable Map
    /// </summary>
    public void DisableMap()
    {
        Map.IsVisible = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Enable Map component
    /// </summary>
    public void EnableMap()
    {
        Map.IsVisible = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Remove markers from the map
    /// </summary>
    public void ClearMapMarkers()
    {
        Map.Clear();
    }

    public void DisplayMarkers(List<Pathpoint> pathpoints)
    {

        foreach (var pathpoint in pathpoints)
        {

            if (pathpoint.POIType == Pathpoint.POIsType.Point)
            {
                StaticMarkerIcon = DefaultMarkerIcon;
            }
            else if (pathpoint.POIType == Pathpoint.POIsType.Landmark)
            {
                StaticMarkerIcon = POILandmarkMarkerIcon;
            }               
            else
            {
                StaticMarkerIcon = POIReassuranceMarkerIcon;
            }

            var mo = new MarkerOptions()
                    .Position(new LatLng(pathpoint.Latitude, pathpoint.Longitude))
                    .Icon(NewCustomDescriptor());

            Map.AddMarker(mo);
        }
    }

    //public void FixRoute()
    //{
    //    var pipeline = new LocationUtils.GPSCleaningPipeline
    //    {
    //        ToleranceSimplify = ToleranceSimplify,
    //        MaxAccuracyRadio = MaxAccuracyRadio,
    //        DistanceOutlier = DistanceOutlier,
    //        SegmentSplit = SegmentSplit,
    //        OutlierFactor = OutlierFactor,
    //        MinEvenly = MinEvenly,
    //        MaxEvenly = MaxEvenly,
    //        POIClusterDistance = POIClusterDistance
    //    };
    //    var ppList = pipeline.CleanRoute(PathpointList);
    //    DisplayMarkers(ppList);
    //}


    private void LoadMap(int zoom)
    {
        StaticMarkerIcon = DefaultMarkerIcon;

        // start point
        Pathpoint startPoint = PathpointList[0];

        // initialize Map
        var cameraPosition = new CameraPosition(
        new LatLng(startPoint.Latitude, startPoint.Longitude), zoom, 0, 0);
        var options = new GoogleMapsOptions()
        .Camera(cameraPosition);

        Map = new GoogleMapsView(options);
       
        Map.Show(GetComponentSize(), OnMapReady);
    }

    public Rect GetComponentSize()
    {
        // Get the RectTransform component
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        // return rectTransform.rect;


        // Calculate the absolute position of the rect within the canvas
        Vector2 absolutePosition = GetCanvasPosition();
        // Calculate the final position and size of the rect

        Rect rect = new Rect(absolutePosition, rectTransform.rect.size);
        return rect;
    }

    public Vector2 GetCanvasPosition()
    {
        // Get the RectTransform of the GameObject
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        // Get the absolute position of the RectTransform in world space
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // Convert the bottom-left corner to screen coordinates
        Vector2 canvasPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[0]);
        canvasPosition.y += 35;

        return canvasPosition;
    }


    /// <summary>
    /// Event listener when map is ready for operations
    /// </summary>
    private void OnMapReady()
    {
        Debug.Log("The map is ready!");

        DisplayMarkers(PathpointList);
    }



    static ImageDescriptor NewCustomDescriptor()
    {
        return ImageDescriptor.FromTexture2D(StaticMarkerIcon);
    }
}
