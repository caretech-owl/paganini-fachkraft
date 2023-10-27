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

    public void PopulateMetadata(Pathpoint point)
    {
        CurrentPathpoint = point;

        PinDescription.text = point.Description;
        PinInstruction.text = point.Instruction;
    }


    public void SaveChanges()
    {
        CurrentPathpoint.Description = PinDescription.text;
        CurrentPathpoint.Instruction = PinInstruction.text;
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
