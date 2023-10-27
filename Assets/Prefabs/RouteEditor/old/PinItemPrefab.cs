using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class PinItemPrefab : MonoBehaviour
{
    public Button PinButton;

    [Header("Data components")]
    public LandmarkIcon PinIcon;
    public TMPro.TMP_Text PinTitle;
    public RawImage POIPhoto;        

    [Header("Events")]
    public PathpointItemEvent OnSelected;

    private Pathpoint PathpointItem;

    // Start is called before the first frame update
    void Start()
    {
        PinButton.onClick.AddListener(itemSelected);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void FillPathpoint(Pathpoint pathpoint)
    {
        // render picture
        byte[] preview = pathpoint.Photos[0].Data.Photo;        
        RenderPicture(preview);

        // render Icon
        if (pathpoint.POIType == Pathpoint.POIsType.Landmark)
        {
            PinIcon.SelectedLandmarkType = LandmarkIcon.LandmarkType.PinLandmark;
        }
        else if (pathpoint.POIType == Pathpoint.POIsType.Reassurance)
        {
            PinIcon.SelectedLandmarkType = LandmarkIcon.LandmarkType.PinReassurance;
        }

        // render description
        // TBD

        // Set current pathpoint
        PathpointItem = pathpoint;
    }

    private void RenderPicture(byte[] imageBytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        POIPhoto.texture = texture;
    }

    private void itemSelected()
    {
        if (OnSelected != null)
        {
            OnSelected.Invoke(PathpointItem, -1);
        }

        Debug.Log("Item PathpointItem " + PathpointItem.Id);
    }
}
