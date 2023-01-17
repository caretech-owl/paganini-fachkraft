using Assets;
using NinevaStudios.GoogleMaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using static ServerCommunication;
using LocationUtils;
using TMPro;

public class ReviewLabController : MonoBehaviour
{
    //PUBLIC
    public TMPro.TMP_Text WayName;

    public VideoPlayer VideoManager;

    public Texture2D DefaultMarkerIcon;
    public Texture2D DefaultInaccurateMarkerIcon;
    public Texture2D POILandmarkMarkerIcon;
    public Texture2D POIReassuranceMarkerIcon;

    public TMPro.TMP_Dropdown FixTechniqueDropdown;
    public TMPro.TMP_InputField SmoothWindowInput;
    public TMPro.TMP_InputField MinEvenlyInput;
    public TMPro.TMP_InputField MaxEvenlyInput;
    public TMPro.TMP_InputField OutlierDistanceInput;
    public TMPro.TMP_InputField MaxAccuracyRadioInput;
    public TMPro.TMP_InputField ToleranceSimplifyInput;
    public TMPro.TMP_InputField SegmentSplitInput;
    public TMPro.TMP_InputField OutlierFactorInput;
    public Toggle KeepChangesToggle;


    //PRIVATE
    private static Texture2D StaticMarkerIcon;
    private InternalDataModel.DataOfExploritoryRouteWalks Erw;
    private GoogleMapsView Map;

    private List<Assets.Scripts.Pathpoint> CurrentPathPointList;


    private Texture2D RedDefaultMarkerIcon;
    private Texture2D GreenDefaultMarkerIcon;
    private Texture2D PurpleDefaultMarkerIcon;



    // Start is called before the first frame update
    void Start()
    {
        Erw = InternalDataModelController.GetInternalDataModelController().idm.exploritoryRouteWalks.FindLast(x => x.Id == InternalDataModelController.GetInternalDataModelController().idm.currentIdOfWay);

        if (Erw == null)
            ErrorHandlerSingleton.GetErrorHandler().AddNewError("Error in ReviewController", "Could not find exploritory route walk!");
        else
        {
            StaticMarkerIcon = DefaultMarkerIcon;

            WayName.text = Erw.Name;

            // initialize Map
            int moveMapForPhone = 0;
            var cameraPosition = new CameraPosition(
            new LatLng(Erw.Pathpoints[Erw.Pathpoints.Count / 2].Latitude, Erw.Pathpoints[Erw.Pathpoints.Count / 2].Longitude), 19, 0, 0);
            var options = new GoogleMapsOptions()
                .MapType(GoogleMapType.Satellite)
                .Camera(cameraPosition);

            Map = new GoogleMapsView(options);
            Map.Show(new Rect(150 + moveMapForPhone, 280, 1100 + moveMapForPhone, 750), OnMapReady);
            GameObject.Find("ButtonSmallRoute").GetComponent<Button>().Select();


            VideoManager.url = FileManagement.persistentDataPath + "/" + Erw.Folder + "/Video/" + Erw.Videos[0];

        }

        RedDefaultMarkerIcon = ChangeTextureHue(DefaultMarkerIcon, 1.2f, 0.8f, 0.8f);
        GreenDefaultMarkerIcon = ChangeTextureHue(DefaultMarkerIcon, 0.8f, 1.2f, 0.8f);
        PurpleDefaultMarkerIcon = ChangeTextureHue(DefaultMarkerIcon, 1.2f, 0.8f, 1.2f);

    }

    /// <summary>
    /// Event listener when map is ready for operations
    /// </summary>
    private void OnMapReady()
    {
        Debug.Log("The map is ready!");


        //DisplayMarkers(Erw.Pathpoints, DefaultMarkerIcon, config);
        FixRoute();

        GameObject.Find("ButtonSmallRoute").GetComponent<Button>().Select();

    }

    private void DisplayMarkers(List<Assets.Scripts.Pathpoint> pathpoints, Texture2D defaultIcon, Dictionary<string, double> config)
    {

        foreach (var pathpoint in pathpoints)
        {

            if (pathpoint.POIType > 0)
                StaticMarkerIcon = POILandmarkMarkerIcon;
            else if (pathpoint.Accuracy >= config["MaxAccuracyRadio"])
                StaticMarkerIcon = DefaultInaccurateMarkerIcon;
            else
                StaticMarkerIcon = defaultIcon;

            var mo = new MarkerOptions()
                    .Position(new LatLng(pathpoint.Latitude, pathpoint.Longitude))
                    .Icon(NewCustomDescriptor());

            Map.AddMarker(mo);

        }

    }

    private Texture2D ChangeTextureHue(Texture2D originalTexture, float r, float g, float b)
    {
        Texture2D texture = new Texture2D(originalTexture.width, originalTexture.height);

        // Get the pixel data from the original texture
        Color[] pixels = originalTexture.GetPixels();

        // Modify the color values to give the texture a reddish hue
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = pixels[i].r * r;
            pixels[i].g = pixels[i].g * g;
            pixels[i].b = pixels[i].b * b;
        }

        // Apply the modified pixel data to the new texture
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    public void FixRoute()
    {
        string option = FixTechniqueDropdown.options[FixTechniqueDropdown.value].text;
        Texture2D texture = DefaultMarkerIcon;
        Dictionary<string, double> config = GetConfigValues();

        Debug.Log("Fix technique to apply: " + option);

        var ppList = Erw.Pathpoints;
        if (KeepChangesToggle.isOn && CurrentPathPointList != null)
        {
            ppList = CurrentPathPointList;
        }

        // Identify jumps in GPS
        // cluster landmarks and their photos


        var tolerance = GPSSmooth.EstimateSimplicationDistanceTolerance(ppList, config["ToleranceSimplify"]);


        // Check what FixRoute action to apply
        if (option == "Original")
        {
            texture = DefaultMarkerIcon;
        }
        else if (option == "Pipeline")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePoints(ppList, config["MaxAccuracyRadio"]);
            ppList = LocationUtils.GPSUtils.RemoveOutliers(ppList, config["DistanceOutlier"]);
            ppList = LocationUtils.GPSUtils.RemoveSelfIntersections(ppList);
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePointsBasedOnData(ppList, (int)config["SegmentSplit"], config["OutlierFactor"]);
            ppList = LocationUtils.GPSSmooth.SimplifyTrackVWSimplifier(ppList, tolerance);
            ppList = LocationUtils.GPSUtils.EvenlySpaced(ppList, config["MinEvenly"], config["MaxEvenly"]);

            texture = PurpleDefaultMarkerIcon;
        }
        else if (option == "FilterOnly")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePoints(ppList, config["MaxAccuracyRadio"]);
            ppList = LocationUtils.GPSUtils.RemoveOutliers(ppList, config["DistanceOutlier"]);
            ppList = LocationUtils.GPSUtils.RemoveSelfIntersections(ppList);
            texture = DefaultInaccurateMarkerIcon;
        }
        else if (option == "Accuracy")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePointsBasedOnData(ppList, (int)config["SegmentSplit"], config["OutlierFactor"]);
            texture = PurpleDefaultMarkerIcon;
        }
        else if (option == "SimplifyPT")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePoints(ppList, config["MaxAccuracyRadio"]);
            ppList = LocationUtils.GPSUtils.RemoveOutliers(ppList, config["DistanceOutlier"]);
            ppList = LocationUtils.GPSSmooth.SimplifyTrack(ppList, tolerance);
            texture = PurpleDefaultMarkerIcon;
        }
        else if (option == "SimplifyVW")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePoints(ppList, config["MaxAccuracyRadio"]);
            ppList = LocationUtils.GPSUtils.RemoveOutliers(ppList, config["DistanceOutlier"]);
            ppList = LocationUtils.GPSSmooth.SimplifyTrackVWSimplifier(ppList, tolerance);
            texture = PurpleDefaultMarkerIcon;
        }
        else if (option == "SimplifyDP")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePoints(ppList, config["MaxAccuracyRadio"]);
            ppList = LocationUtils.GPSUtils.RemoveOutliers(ppList, config["DistanceOutlier"]);
            ppList = LocationUtils.GPSSmooth.SimplifyTrackDouglasPeucker(ppList, tolerance);
            texture = PurpleDefaultMarkerIcon;
        }
        else if (option == "Smooth")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePoints(ppList, config["MaxAccuracyRadio"]);
            ppList = LocationUtils.GPSUtils.RemoveOutliers(ppList, config["DistanceOutlier"]);
            ppList = LocationUtils.GPSSmooth.Smooth(ppList, (int)config["WindowSmooth"]);
            texture = RedDefaultMarkerIcon;
        }
        else if (option == "Evenly")
        {
            ppList = LocationUtils.GPSUtils.RemoveInnacuratePoints(ppList, config["MaxAccuracyRadio"]);
            ppList = LocationUtils.GPSUtils.RemoveOutliers(ppList, config["DistanceOutlier"]);
            ppList = LocationUtils.GPSUtils.EvenlySpaced(ppList, config["MinEvenly"], config["MaxEvenly"]);
            texture = GreenDefaultMarkerIcon;
        }

        CurrentPathPointList = ppList;

        Debug.Log("Original #: " + Erw.Pathpoints.Count + " Pathpoints: " + ppList.Count);
        //foreach (var p in ppList)
        //{
        //    Debug.Log(p.Latitude + " " + p.Longitude);
        //}

        DisplayMarkers(ppList, texture, config);
    }

    private Dictionary<string, double> GetConfigValues()
    {
        Dictionary<string, double> inputFieldValues = new Dictionary<string, double>();

        inputFieldValues.Add("WindowSmooth", Int32.Parse(SmoothWindowInput.text));
        inputFieldValues.Add("MinEvenly", Double.Parse(MinEvenlyInput.text));
        inputFieldValues.Add("MaxEvenly", Double.Parse(MaxEvenlyInput.text));
        inputFieldValues.Add("DistanceOutlier", Double.Parse(OutlierDistanceInput.text));
        inputFieldValues.Add("MaxAccuracyRadio", Double.Parse(MaxAccuracyRadioInput.text));
        inputFieldValues.Add("ToleranceSimplify", Double.Parse(ToleranceSimplifyInput.text));
        inputFieldValues.Add("SegmentSplit", Int32.Parse(SegmentSplitInput.text));
        inputFieldValues.Add("OutlierFactor", Double.Parse(OutlierFactorInput.text));


        return inputFieldValues;
    }

    public void ClearMapMarkers()
    {
        Map.Clear();
    }

    //public void SnapToFootRoad()
    //{
    //    //SnapToFootRoadAsync().Wait();

    //    Debug.Log("Preparing pathpoints to snap!");

    //    List<double[]> list = new List<double[]>();

    //    foreach (var pathpoint in Erw.Pathpoints)
    //    {

    //        var point = new double[] {  pathpoint.Longitude, pathpoint.Latitude };
    //        list.Add(point);
    //    }

    //    ServerCommunication.Instance.SnapToPedestrianPath(SnapSucceed, SnapFailed, list);

    //}

    //private void SnapSucceed(GraphHopperResponse response)
    //{
    //    Debug.Log(response);
    //}

    //private void SnapFailed(string msg)
    //{
    //    Debug.Log(msg);
    //}

    //private async Task SnapToFootRoadAsync()
    //{
    //    Debug.Log("Preparing pathpoints to snap!");

    //    List<GLatLng> list = new List<GLatLng>();

    //    foreach (var pathpoint in Erw.Pathpoints)
    //    {

    //        var point = new GLatLng(pathpoint.Latitude, pathpoint.Longitude);
    //        list.Add(point);                       
    //    }

    //    List <GLatLng> snapList = await GraphAPI.SnapToPedestrianPath(list);

    //}



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
