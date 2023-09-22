using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class GalleryEvent : UnityEvent<PathpointPhoto, int>
{
}

public class PhotoGallery : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject Content;
    public GameObject ItemPrefab;
    public GameObject BlankState;

    public RouteSharedData.EditorMode EditMode { get; set; }

    private List<PathpointPhoto> CurrentPhotos;

    [Header("Events")]
    public GalleryEvent OnPhotoOpened;

    // Start is called before the first frame update
    void Start()
    {
        Clearlist();

        if (CurrentPhotos != null)
            LoadPhotos(CurrentPhotos);
    }

    // Update is called once per frame
    void Update()
    {
        //if (ContentRectTransform.rect.height != CurrentContentHeight)
        //{
        //    CurrentContentHeight = ContentRectTransform.rect.height;
        //    Debug.Log("Component size has changed: " + CurrentContentHeight);

        //    ResizePanel();
        //}
    }


    /* UI and events */

    public void Clearlist()
    {
        if (!Content) return;

        Transform content = Content.GetComponent<Transform>();

        // Remove all children
        for (int i = 0; i < content.childCount; i++)
        {
            // Get the child game object
            GameObject child = content.GetChild(i).gameObject;

            // Destroy the child game object
            Destroy(child);
        }
    }

    public void LoadPhotos(List<PathpointPhoto> photos)
    {
        //fix: To safely load, due to problem with initialisation
        CurrentPhotos = photos;
        if (!Content) return;        

        foreach (var photo in photos)
        {
            AddItem(photo);
        }

        //TODO: Prepare the Gallery based on the EditMode
        // Edit during cleaning
        // Feedback during discussion
    }

    public void AddItem(PathpointPhoto p)
    {
        var neu = Instantiate(ItemPrefab, Content.transform);

        var item = neu.GetComponent<PhotoElementPrefab>();
        item.OnPhotoOpened.AddListener(OnPhotoOpenedHandler);
        item.OnSelectedChanged.AddListener(OnPhotoSelectedHandler);
        item.FillPhoto(p);

    }

    private void OnPhotoOpenedHandler(PhotoElementPrefab prefab) {
        OnPhotoOpened?.Invoke(prefab.CurrentPathpointPhoto,0);
    }

    private void OnPhotoSelectedHandler(PhotoElementPrefab prefab)
    {
        var photo = prefab.CurrentPathpointPhoto;
        if (prefab.IsSelected) {
            photo.CleaningFeedback = PathpointPhoto.PhotoFeedback.Keep;
        } else {
            photo.CleaningFeedback = PathpointPhoto.PhotoFeedback.Delete;
        }
        photo.InsertDirty();        
    }

}
