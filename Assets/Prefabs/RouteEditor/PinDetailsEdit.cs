using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinDetailsEdit : MonoBehaviour
{
    [Header("Pin Data")]
    public POITimelineItem POIMetadata;
    //public TMPro.TMP_InputField PinDescription;
    public RawImage POIVideoThumbnail;
    public DirectionTypeToggle DirectionType;
    public POITypeToggle POIType;
    public POIFeedbackToggle POIFeedback;

    [Header("Edit modes")]
    public GameObject CleaningPanel;
    public GameObject DiscussionPanel;

    [Header("Gallery / Video switch")]
    public GameObject SwitchVideoPanel;
    public GameObject SwitchGAlleryPanel;

    private Pathpoint CurrentPathpoint;
    public RouteSharedData.EditorMode EditMode { get; set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PopulateMetadata(Pathpoint point, Way way, int index)
    {
        CurrentPathpoint = point;

        // POI information
        FillPOIEditMode(point, way, index);

        // Information Editing
        if (EditMode == RouteSharedData.EditorMode.Cleaning)
        {
            PopulateCleaning(point);
        }
        else if (EditMode == RouteSharedData.EditorMode.Discussion)
        {
            PopulateDiscussion(point);
        }
        else
        {
            //TODO: ReadOnly
        }

        // Render Navigation Thumbnnail
        if (point.Photos != null && point.Photos.Count > 0) {
            RenderThumbnail(point.Photos[0].Photo);
        }
        
    }

    public void EnableSwitchToGallery(bool activate) {
        SwitchVideoPanel.SetActive(!activate);
        SwitchGAlleryPanel.SetActive(activate) ;
    }

    private void PopulateCleaning(Pathpoint point) {

        DiscussionPanel.SetActive(false);

        // If start / destination, user cannot edit POI Information (type, direction..)
        if (point.POIType == Pathpoint.POIsType.WayStart ||
            point.POIType == Pathpoint.POIsType.WayDestination)
        {
            CleaningPanel.SetActive(false);
        }
        else
        {
            CleaningPanel.SetActive(true);
            POIType.SetSelectedPOIType(point.POIType);
        }

        // Direction Type
        if (point.POIType == Pathpoint.POIsType.Landmark)
        {
            DirectionType.gameObject.SetActive(true);
            DirectionType.SetSelectedDirectionType(point.Instruction);
        }
        else
        {
            DirectionType.gameObject.SetActive(false);
        }
    }

    private void PopulateDiscussion(Pathpoint poi)
    {
        CleaningPanel.SetActive(false);
        DiscussionPanel.SetActive(true);

        POIFeedback.FillDiscussionFeedback(poi);
    }


    private void FillPOIEditMode(Pathpoint point, Way way, int index)
    {
        if (point.POIType == Pathpoint.POIsType.WayStart)
        {
            POIMetadata.FillPathpointStart(point, way);
        }
        else if (point.POIType == Pathpoint.POIsType.WayDestination)
        {
            POIMetadata.FillPathpointDestination(point, way);
        }
        else
        {
            POIMetadata.FillPathpoint(point, index);
        }
    }

    private void RenderThumbnail(byte[] imageBytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        POIVideoThumbnail.texture = texture;
        POIVideoThumbnail.gameObject.SetActive(true);
    }

    public void OnPOITypeValueChanged(Pathpoint.POIsType pOIType)
    {
        CurrentPathpoint.POIType = pOIType;
        CurrentPathpoint.InsertDirty();

        // Decision point? (landmark?)
        DirectionType.gameObject.SetActive(CurrentPathpoint.POIType == Pathpoint.POIsType.Landmark);

        Debug.Log($"OnPOITypeValueChanged: {pOIType}");
    }

    public void OnDirectionTypeValueChanged(string direction)
    {
        CurrentPathpoint.Instruction = direction;
        CurrentPathpoint.InsertDirty();

        Debug.Log($"OnDirectionTypeValueChanged: {direction}");
    }
}
