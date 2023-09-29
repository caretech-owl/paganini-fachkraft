using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class PhotoElementEvent : UnityEvent<PhotoElementPrefab>
{
}

public class PhotoElementPrefab : MonoBehaviour
{
    public RawImage POIPhoto;
    public Toggle SelectedToggle;
    public Toggle KeepItToggle;
    public Button OpenPhoto;
    public PathpointPhoto CurrentPathpointPhoto;


    public bool IsSelected
    {
        get
        {
            return SelectedToggle == null ? false : SelectedToggle.isOn;
        }
    }

    [Header("Events")]
    public PhotoElementEvent OnSelectedChanged;
    public PhotoElementEvent OnPhotoOpened;
    
    //public RouteItemEvent OnSelected;

    // Start is called before the first frame update
    void Start()
    {
        SelectedToggle.onValueChanged.AddListener(OnToggleSelectedChanged);
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void FillPhoto(PathpointPhoto p, bool enableSelection)
    {
        RenderPicture(p.Photo);
        CurrentPathpointPhoto = p;

        EnableSelection(enableSelection);

        // We de-select the pictures curated as 'deleted'
        SelectedToggle.isOn = p.CleaningFeedback != PathpointPhoto.PhotoFeedback.Delete;

        // Display user feedback?
        if (p.DiscussionFeedback != PathpointPhoto.PhotoFeedback.None)
        {
            KeepItToggle.isOn = p.DiscussionFeedback == PathpointPhoto.PhotoFeedback.Keep;
            KeepItToggle.gameObject.SetActive(!enableSelection);
        }
        else
        {
            KeepItToggle.gameObject.SetActive(false);
        }
        


        ApplyDeselectionEffect(SelectedToggle.isOn);
    }

    private void RenderPicture(byte[] imageBytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        POIPhoto.texture = texture;
    }

    public void OnToggleSelectedChanged(bool isActive)
    {
        ApplyDeselectionEffect(isActive);


        if (OnSelectedChanged != null)
        {
            OnSelectedChanged.Invoke(this);
        }

        Debug.Log("Photo selected: " +isActive);
    }

    public void OnPhotoSelected()
    {
        if (OnPhotoOpened != null)
        {
            OnPhotoOpened.Invoke(this);
        }

        Debug.Log("Photo opened: " + CurrentPathpointPhoto.Id);
    }

    private void ApplyDeselectionEffect(bool isActive)
    {
        Color grayColor = Color.white;//new Color(0.5647f, 0.5647f, 0.5647f); // Values are in the range [0, 1]
        grayColor.a = 0.4f; // Set the alpha component

        POIPhoto.color = !isActive ? grayColor : Color.white;
    }


    private void EnableSelection(bool enable)
    {
        SelectedToggle.gameObject.SetActive(enable);
    }

}
