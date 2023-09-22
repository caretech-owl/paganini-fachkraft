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

    public void FillPhoto(PathpointPhoto p)
    {
        RenderPicture(p.Photo);
        CurrentPathpointPhoto = p;

        // We de-select the pictures curated as 'deleted'
        SelectedToggle.isOn = p.CleaningFeedback != PathpointPhoto.PhotoFeedback.Delete;
    }

    private void RenderPicture(byte[] imageBytes)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        POIPhoto.texture = texture;
    }

    public void OnToggleSelectedChanged(bool isActive)
    {
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

}
