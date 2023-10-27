using UnityEngine;
using UnityEngine.UI;

public class PinDetailsEdit : MonoBehaviour
{
    [Header("Pin Data")]
    public POITimelineItem POIMetadata;
    //public TMPro.TMP_InputField PinDescription;
    public RawImage POIVideoThumbnail;
    public RawImage POIPhotoThumbnail;
    public DirectionTypeToggle DirectionType;
    public POITypeToggle POIType;
    public POIFeedbackToggle POIFeedback;
    public POIFeedbackToggle POIReadOnlyFeedback;

    [Header("Edit modes")]
    public GameObject CleaningPanel;
    public GameObject DiscussionPanel;
    public GameObject ReadOnlyPanel;

    [Header("Gallery / Video switch")]
    public GameObject SwitchVideoPanel;
    public GameObject SwitchGalleryPanel;

    private Pathpoint CurrentPathpoint;
    private Way CurrentWay;
    private int CurrentIndex;
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
        CurrentWay = way;
        CurrentIndex = index;

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
            PopulateReadOnly(point);
        }

        // Render Navigation Thumbnnail
        if (point.Photos != null && point.Photos.Count > 0) {
            // render picture
            var previewPhoto = PathpointPhoto.GetDefaultPhoto(point.Photos);
            RenderThumbnail(previewPhoto.Data.Photo);
        }
        
    }

    public void EnableSwitchToGallery(bool activate) {
        SwitchVideoPanel.SetActive(!activate);
        SwitchGalleryPanel.SetActive(activate) ;
    }

    private void PopulateCleaning(Pathpoint point) {

        DiscussionPanel.SetActive(false);
        ReadOnlyPanel.SetActive(false);

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
        ReadOnlyPanel.SetActive(false);

        if (poi.POIType == Pathpoint.POIsType.WayStart ||
            poi.POIType == Pathpoint.POIsType.WayDestination)
        {
            DiscussionPanel.SetActive(false);
        }
        else
        {
            DiscussionPanel.SetActive(true);
            POIFeedback.FillDiscussionFeedback(poi);
        }        
    }

    private void PopulateReadOnly(Pathpoint poi)
    {
        CleaningPanel.SetActive(false);
        DiscussionPanel.SetActive(false);        

        if (poi.POIType == Pathpoint.POIsType.WayStart ||
            poi.POIType == Pathpoint.POIsType.WayDestination)
        {            
            ReadOnlyPanel.SetActive(false);
        }
        else
        {
            ReadOnlyPanel.SetActive(true);
            POIReadOnlyFeedback.FillDiscussionFeedback(poi);
        }

        
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
        // Destroy the old texture before creating a new one
        if (POIVideoThumbnail.texture != null)
        {
            Destroy(POIVideoThumbnail.texture);
        }

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        POIVideoThumbnail.texture = texture;
        POIVideoThumbnail.gameObject.SetActive(true);

        if (POIPhotoThumbnail.texture != null)
        {
            Destroy(POIPhotoThumbnail.texture);
        }

        POIPhotoThumbnail.texture = texture;
        POIPhotoThumbnail.gameObject.SetActive(true);
    }

    public void OnPOITypeValueChanged(Pathpoint.POIsType pOIType)
    {
        CurrentPathpoint.POIType = pOIType;
        CurrentPathpoint.InsertDirty();

        // Decision point? (landmark?)
        DirectionType.gameObject.SetActive(CurrentPathpoint.POIType == Pathpoint.POIsType.Landmark);

        Debug.Log($"OnPOITypeValueChanged: {pOIType}");

        PopulateMetadata(CurrentPathpoint, CurrentWay, CurrentIndex);
    }

    public void OnDirectionTypeValueChanged(string direction)
    {
        CurrentPathpoint.Instruction = direction;
        CurrentPathpoint.InsertDirty();

        Debug.Log($"OnDirectionTypeValueChanged: {direction}");

        FillPOIEditMode(CurrentPathpoint, CurrentWay, CurrentIndex);
    }



    private void DestroyTexture(Texture texture)
    {
        if (texture != null)
        {
            // Use DestroyImmediate to properly destroy the texture at runtime
            DestroyImmediate(texture, true);
        }
    }

    public void CleanupView()
    {
        // Clear references to objects
        CurrentPathpoint = null;
        CurrentWay = null;

        // Destroy textures to release memory
        DestroyTexture(POIVideoThumbnail.texture);
        DestroyTexture(POIPhotoThumbnail.texture);

        POIMetadata.CleanupView();

        // Remove event listeners or cleanup any other resources as needed
        // For example, if you have registered event listeners, unregister them here.

        // Deactivate or hide any game objects or UI elements that are no longer needed
        CleaningPanel.SetActive(false);
        DiscussionPanel.SetActive(false);
        ReadOnlyPanel.SetActive(false);
        SwitchVideoPanel.SetActive(false);
        SwitchGalleryPanel.SetActive(false);

        // Unload unused assets to free up memory
        Resources.UnloadUnusedAssets();
    }




}
