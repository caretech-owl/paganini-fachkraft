using UnityEngine;
using UnityEngine.UI;

public class PinMetaEdit : MonoBehaviour
{
    [Header("Pin Data")]

    public TMPro.TMP_InputField PinDescription;
    public TMPro.TMP_InputField PinInstruction;

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

    public void PopulateMetadata(Pathpoint point, Way way)
    {
        CurrentPathpoint = point;
        if (point.POIType == Pathpoint.POIsType.WayStart)
        {
            PinDescription.text = way.Start;
            PinDescription.interactable = false;
        }
        else if (point.POIType == Pathpoint.POIsType.WayDestination)
        {
            PinDescription.text = way.Destination;
            PinDescription.interactable = false;
        }
        else
        {
            PinDescription.text = point.Description;
            PinDescription.interactable = true;
        }
            
        PinInstruction.text = point.Notes;
    }


    public void SaveChanges()
    {
        CurrentPathpoint.Description = PinDescription.text;
        CurrentPathpoint.Notes = PinInstruction.text;
        CurrentPathpoint.InsertDirty();
    }

    public void DeletePOI()
    {
        // Implement API call to delete POI
        // Add the function to SharedData
        // and remove it from the POIList and PathpointList
    }

    public void CleanupView()
    {
        // Clear references to objects
        CurrentPathpoint = null;

        // Unload unused assets to free up memory
        Resources.UnloadUnusedAssets();
    }


}
