using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class PathpointItemEvent : UnityEvent<Pathpoint, int>
{
}

public class POITimelineItem : MonoBehaviour
{
    public Button PinButton;

    [Header("Data components")]
    //public LandmarkIcon PinIcon;
    public TMPro.TMP_Text PinTitle;
    public TMPro.TMP_Text PinSubtitle;
    public RawImage POIPhoto;
    public LandmarkIcon LocationIcon;
    public GameObject CheckmarkIcon;
    public GameObject EditIcon;
    public GameObject RemovedIcon;
    public GameObject NoData;

    [Header("Events")]
    public PathpointItemEvent OnSelected;

    private Pathpoint PathpointItem;
    private int CurrentIndex;

    private Color grayColor;
    private Color whiteColor;

    // Start is called before the first frame update
    void Start()
    {
        PinButton.onClick.AddListener(itemSelected);        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void FillPathpoint(Pathpoint pathpoint, int index)
    {
        FillPathpointData(pathpoint);
        PinTitle.text = "Pin " + index;

        LocationIcon.gameObject.SetActive(false);

        RenderIrrelevant(pathpoint.RelevanceFeedback == Pathpoint.POIFeedback.No);
        if (pathpoint.RelevanceFeedback != Pathpoint.POIFeedback.No) {
            RenderCompletedIcon(true);
        }
        


        CurrentIndex = index;
    }

    public void FillPathpointStart(Pathpoint pathpoint, Way way)
    {
        FillPathpointData(pathpoint);
        PinTitle.text = "Start";
        PinSubtitle.text = way.Start;

        EditIcon.gameObject.SetActive(false);
        LocationIcon.gameObject.SetActive(true);

        RenderIrrelevant(false);
        RenderLocationIcon(way.StartType);
        
    }

    public void FillPathpointDestination(Pathpoint pathpoint, Way way)
    {
        FillPathpointData(pathpoint);
        PinTitle.text = "Ziel";
        PinSubtitle.text = way.Destination;

        EditIcon.gameObject.SetActive(false);
        LocationIcon.gameObject.SetActive(true);

        RenderIrrelevant(false);
        RenderLocationIcon(way.DestinationType);        
    }

    public void FillPathpointData(Pathpoint pathpoint)
    {
        // render picture
        byte[] preview = pathpoint.Photos[0].Photo;
        RenderPicture(preview);

        // render description
        if (pathpoint.Description!= null)
        {
            PinSubtitle.text = pathpoint.Description;
        }   

        // Set current pathpoint
        PathpointItem = pathpoint;
    }

    private void RenderPOIType(Pathpoint.POIsType pOIsType)
    {

        //if (pOIsType == Pathpoint.POIsType.Landmark)
        //{
        //    PinIcon.SelectedLandmarkType = LandmarkIcon.LandmarkType.PinLandmark;
        //}
        //else if (pathpoint.POIType == Pathpoint.POIsType.Reassurance)
        //{
        //    PinIcon.SelectedLandmarkType = LandmarkIcon.LandmarkType.PinReassurance;
        //}

    }

    private void RenderIrrelevant(bool irrelevant) {
        // We gray out the color

        Color grayColor = new Color(0.5647f, 0.5647f, 0.5647f); // Values are in the range [0, 1]
        grayColor.a = 0.31f; // Set the alpha component

        POIPhoto.color = irrelevant ? grayColor : Color.white;

        RemovedIcon.SetActive(irrelevant);
        NoData.SetActive(!irrelevant);
        CheckmarkIcon.SetActive(!irrelevant);
        EditIcon.SetActive(!irrelevant);
    }


    private void RenderLocationIcon(string landmarkType)
    {
        LocationIcon.SetSelectedLandmark(Int32.Parse(landmarkType));
        CheckmarkIcon.SetActive(false);
        EditIcon.SetActive(false);
    }

    private void RenderCompletedIcon(bool completed)
    {        
        CheckmarkIcon.SetActive(completed);
        EditIcon.SetActive(!completed);

        POIPhoto.gameObject.SetActive(completed);
        
    }

    private void RenderPicture(byte[] imageBytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        POIPhoto.texture = texture;
        POIPhoto.gameObject.SetActive(true);
    }

    private void itemSelected()
    {
        if (OnSelected != null)
        {
            OnSelected.Invoke(PathpointItem, CurrentIndex);
        }

        Debug.Log("Item PathpointItem " + PathpointItem.Id + " label: " + PinTitle.text);
    }
}
