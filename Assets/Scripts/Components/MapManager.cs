using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LocationUtils;
using NinevaStudios.GoogleMaps;
using PaganiniRestAPI;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject MapContainer;
    public Texture2D DefaultMarkerIcon;
    public Texture2D POILandmarkMarkerIcon;
    public Texture2D POIReassuranceMarkerIcon;

    //PRIVATE
    private static Texture2D StaticMarkerIcon;
    private GoogleMapsView Map;
    private List<Pathpoint> PathpointList;
    private RouteSharedData SharedData;


    // Start is called before the first frame update
    void Awake()
    {
        SharedData = RouteSharedData.Instance;
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
    public void LoadMap()
    {
        PathpointList = SharedData.PathpointList;
        LoadMap(19);
    }

    /// <summary>
    /// Disable Map
    /// </summary>
    public void DisableMap()
    {
        Map.IsVisible = false;
        MapContainer.SetActive(false);
    }

    /// <summary>
    /// Enable Map component
    /// </summary>
    public void EnableMap()
    {
        Map.IsVisible = true;
        MapContainer.SetActive(true);
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
        int i = 0;

        foreach (var pathpoint in pathpoints)
        {
            Debug.Log("DisplayMarkers: "+ pathpoint.Id);

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
                    .Icon(NewCustomDescriptor(StaticMarkerIcon))
                    .Title($"Marker {i} Lat: {pathpoint.Latitude} Lon: {pathpoint.Longitude}");

            Map.AddMarker(mo);
            i++;
        }
    }

    int count = 0;
    public void RenderMarker(PathpointTraceMessage traceMessage)
    {
        Texture2D icon = DefaultMarkerIcon;

        if (traceMessage.eventType == SocketsAPI.POIState.OnPOI.ToString())
        {
            icon = ChangeIconColor(icon, 0.8f, 0.8f, 1.2f);
        }
        else if (traceMessage.eventType == SocketsAPI.POIState.OffTrack.ToString())
        {
            icon = ChangeIconColor(icon, 1.2f, 0.8f, 0.8f);
        }
        else if (traceMessage.eventType == SocketsAPI.POIState.Invalid.ToString())
        {
            icon = ChangeIconColor(icon, 1.2f, 0.6f, 0.2f);
        }
        else if (traceMessage.eventType == SocketsAPI.POIState.OnTrack.ToString())
        {
            icon = ChangeIconColor(icon, 0.8f, 1.2f, 0.8f);            
        }
        else
        {
            icon = ChangeIconColor(icon, 1.2f, 0.8f, 1.2f);
        }

        

        var pathpoint = traceMessage.pathpoint;

        var mo = new MarkerOptions()
        .Position(new LatLng(pathpoint.ppoint_lat, pathpoint.ppoint_lon))
        .Icon(NewCustomDescriptor(icon))
        .Title($"Seq: {traceMessage.seq} Lat: {pathpoint.ppoint_lat} Lon: {pathpoint.ppoint_lon}"); 

        Map.AddMarker(mo);
        count++;
    }



    private void LoadMap(int zoom)
    {
        StaticMarkerIcon = DefaultMarkerIcon;

        // initialize Map

        var options = new GoogleMapsOptions();        

        if (PathpointList != null && PathpointList.Capacity > 0)
        {
            // start point
            Pathpoint startPoint = PathpointList[0];

            // setup camera
            var cameraPosition = new CameraPosition(
                new LatLng(startPoint.Latitude, startPoint.Longitude), zoom, 0, 0);
            options = options.Camera(cameraPosition);

        }

        options.MapType(GoogleMapType.Hybrid);

        Map = new GoogleMapsView(options);
       
        Map.Show(GetComponentSize(), OnMapReady);
    }

    public Rect GetComponentSize()
    {
        // Get the RectTransform component
        RectTransform rectTransform = MapContainer.GetComponent<RectTransform>();
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
        RectTransform rectTransform = MapContainer.GetComponent<RectTransform>();

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

    private Texture2D ChangeIconColor(Texture2D icon, float r, float g, float b)
    {
        // Create a new Texture2D object with the same size as the original texture
        Texture2D newIcon = new Texture2D(icon.width, icon.height);

        // Get the pixel data from the original texture
        Color[] pixels = icon.GetPixels();

        // Modify the color values to give the texture a green hue
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = pixels[i].r * r;
            pixels[i].g = pixels[i].g * g;
            pixels[i].b = pixels[i].b * b;
        }

        // Apply the modified pixel data to the new texture
        newIcon.SetPixels(pixels);
        newIcon.Apply();

        return newIcon;

    }

    static ImageDescriptor NewCustomDescriptor(Texture2D icon)
    {
        return ImageDescriptor.FromTexture2D(icon);
    }


}
