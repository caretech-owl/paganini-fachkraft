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
    public GameObject NoData;

    [Header("Icons")]
    public LandmarkIcon LocationIcon;
    //public GameObject CheckmarkIcon;
    public GameObject EditIcon;
    public GameObject RemovedIcon;
    public GameObject LandmarkIcon;
    public GameObject ReassuranceIcon;

    [Header("Direction Icons")]
    public GameObject TurnLeftIcon;
    public GameObject TurnRightIcon;
    public GameObject StraightIcon;


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

        RenderIrrelevant(pathpoint.RelevanceFeedback == Pathpoint.POIFeedback.No || pathpoint.CleaningFeedback == Pathpoint.POIFeedback.No);
        if (pathpoint.RelevanceFeedback != Pathpoint.POIFeedback.No && pathpoint.CleaningFeedback != Pathpoint.POIFeedback.No) {
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
        var previewPhoto = PathpointPhoto.GetDefaultPhoto(pathpoint.Photos);
        if (previewPhoto != null)
        {            
            RenderPicture(previewPhoto.Data.Photo);
        }
        

        // render description
        if (pathpoint.Description!= null)
        {
            PinSubtitle.text = pathpoint.Description;
        }   

        // Set current pathpoint
        PathpointItem = pathpoint;
    }

    private void RenderPOIType()
    {

        ReassuranceIcon.SetActive(PathpointItem.POIType == Pathpoint.POIsType.Reassurance);
        LandmarkIcon.SetActive(PathpointItem.POIType == Pathpoint.POIsType.Landmark);

        bool isLandmark = PathpointItem.POIType == Pathpoint.POIsType.Landmark;
        StraightIcon.SetActive(isLandmark && PathpointItem.Instruction== "Straight");
        TurnLeftIcon.SetActive(isLandmark && PathpointItem.Instruction == "LeftTurn");
        TurnRightIcon.SetActive(isLandmark && PathpointItem.Instruction == "RightTurn");    
    }

    private void RenderIrrelevant(bool irrelevant) {
        // We gray out the color

        Color grayColor = new Color(0.5647f, 0.5647f, 0.5647f); // Values are in the range [0, 1]
        grayColor.a = 0.31f; // Set the alpha component

        POIPhoto.color = irrelevant ? grayColor : Color.white;

        RemovedIcon.SetActive(irrelevant);
        NoData.SetActive(!irrelevant);
        ReassuranceIcon.SetActive(!irrelevant);
        LandmarkIcon.SetActive(!irrelevant);
        EditIcon.SetActive(!irrelevant);
        StraightIcon.SetActive(false);
        TurnLeftIcon.SetActive(false);
        TurnRightIcon.SetActive(false);
    }


    private void RenderLocationIcon(string landmarkType)
    {
        LocationIcon.SetSelectedLandmark(Int32.Parse(landmarkType));
        LandmarkIcon.SetActive(false);
        ReassuranceIcon.SetActive(false);
        EditIcon.SetActive(false);
    }

    private void RenderCompletedIcon(bool completed)
    {
        if (completed)
        {
            RenderPOIType();
        }
        
        EditIcon.SetActive(!completed);

        POIPhoto.gameObject.SetActive(completed);
        
    }

    private void RenderPicture(byte[] imageBytes)
    {
        if (POIPhoto.texture != null)
        {
            Destroy(POIPhoto.texture);
        }

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


    public void CleanupView()
    {
        // Remove any event listeners or cleanup any other resources as needed.
        // For example, if you have registered event listeners, unregister them here.
        PinButton.onClick.RemoveAllListeners();

        // Clear references to objects
        if (POIPhoto.texture != null)
        {
            Destroy(POIPhoto.texture);
        }

        // Deactivate or hide any game objects or UI elements that are no longer needed
        LocationIcon.gameObject.SetActive(false);
        // You can add other GameObjects/UI elements that need to be deactivated or reset here.

        // Clear any text or data
        PinTitle.text = string.Empty;
        PinSubtitle.text = string.Empty;

        // Reset other variables or states as needed
        // For example, if you have other variables that need to be reset, do so here.
    }

    private void OnDestroy()
    {
        CleanupView();
    }

}
