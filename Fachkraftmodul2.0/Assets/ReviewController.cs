using Assets;
using NinevaStudios.GoogleMaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ReviewController : MonoBehaviour
{
    //PUBLIC
    public TMPro.TMP_Text WayName;

    public VideoPlayer VideoManager;

    public Texture2D DefaultMarkerIcon;
    public Texture2D POILandmarkMarkerIcon;
    public Texture2D POIReassuranceMarkerIcon;

    //PRIVATE
    private static Texture2D StaticMarkerIcon;
    private InternalDataModel.DataOfExploritoryRouteWalks Erw;
    private GoogleMapsView Map;


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
            new LatLng(Erw.Pathpoints[Erw.Pathpoints.Count/2].Latitude, Erw.Pathpoints[Erw.Pathpoints.Count / 2].Longitude), 19, 0, 0);
            var options = new GoogleMapsOptions()
                .Camera(cameraPosition);

            Map = new GoogleMapsView(options);
            Map.Show(new Rect(150 + moveMapForPhone, 280, 1100 + moveMapForPhone, 750), OnMapReady);
            GameObject.Find("ButtonSmallRoute").GetComponent<Button>().Select();


            VideoManager.url = FileManagement.persistentDataPath + "/" + Erw.Folder + "/Video/" + Erw.Videos[0];

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

        foreach (var pathpoint in Erw.Pathpoints)
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
