using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LandmarkIcon;
using UnityEngine.UI;
using PaganiniRestAPI;

public class POIEdit : MonoBehaviour
{
    [Header("UI Elements")]
    public PinDetailsEdit PinEdit;
    public PhotoGallery Gallery;
    public VideoGallery Video;
    public PhotoSlideShow SlideShow;


    private RouteSharedData SharedData;   


    // Start is called before the first frame update
    void Awake()
    {
        SharedData = RouteSharedData.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadView(int index)
    {
        gameObject.SetActive(true);

        // Editor mode (Cleaning, Discussion)
        SetupEditorMode();

        //PinEdit
        LoadPinEdit(index);

        // Gallery
        LoadGallery();
    }

    public void LoadGallery()
    {
        HideAllButThisView(Gallery.gameObject);

        Gallery.Clearlist();
        Gallery.LoadPhotos(SharedData.CurrentPOI.Photos);
        PinEdit.EnableSwitchToGallery(false);
    }

    public void LoadSlideShow()
    {
        HideAllButThisView(SlideShow.gameObject);
        SlideShow.EditMode = SharedData.CurrentEditorMode;
        SlideShow.LoadSlideShow(SharedData.CurrentPOI.Photos);
        PinEdit.EnableSwitchToGallery(false);
    }


    public void LoadVideo()
    {
        HideAllButThisView(Video.gameObject);


        Video.LoadVideo(SharedData.POIList[0]);

        Pathpoint pointNext = SharedData.CurrentPOI;
        if (SharedData.CurrentPOI.POIType != Pathpoint.POIsType.WayDestination &&
            SharedData.CurrentPOIIndex + 1 < SharedData.POIList.Count) {
            pointNext = SharedData.POIList[SharedData.CurrentPOIIndex + 1];
        }

        Video.LimitPlaybackTimeframe(SharedData.CurrentPOI, pointNext);
        PinEdit.EnableSwitchToGallery(true);
    }

    private void LoadPinEdit(int index) {
        PinEdit.EditMode = SharedData.CurrentEditorMode;
        PinEdit.PopulateMetadata(SharedData.CurrentPOI, SharedData.CurrentWay, index);
    }

    private void HideAllButThisView(GameObject view)
    {
        SlideShow.gameObject.SetActive(SlideShow.gameObject == view);
        Gallery.gameObject.SetActive(Gallery.gameObject == view);
        Video.gameObject.SetActive(Video.gameObject == view);
    }

    private void SetupEditorMode() {
        if (SharedData.CurrentRoute.Status == Route.RouteStatus.New)
        {
            SharedData.CurrentEditorMode = RouteSharedData.EditorMode.Cleaning;
        }
        else if (SharedData.CurrentRoute.Status == Route.RouteStatus.DraftPrepared)
        {
            SharedData.CurrentEditorMode = RouteSharedData.EditorMode.Discussion;
        }
        else
        {
            SharedData.CurrentEditorMode = RouteSharedData.EditorMode.ReadOnly;
        }

    }

}
