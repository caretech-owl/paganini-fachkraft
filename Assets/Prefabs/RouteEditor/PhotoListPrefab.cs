using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhotoListPrefab : MonoBehaviour
{

    public GameObject Content;

    public GameObject ItemPrefab;

    public GameObject BlankState;

    private RectTransform ContentRectTransform;
    private float CurrentContentHeight;

    // Start is called before the first frame update
    void Start()
    {
        ContentRectTransform = Content.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (ContentRectTransform.rect.height != CurrentContentHeight)
        {
            CurrentContentHeight = ContentRectTransform.rect.height;
            Debug.Log("Component size has changed: " + CurrentContentHeight);

            ResizePanel();
        }
    }

    public void Clearlist()
    {
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

    public void AddItem(PathpointPhoto p)
    {
        var neu = Instantiate(ItemPrefab, Content.transform);

        var item = neu.GetComponent<PhotoElementPrefab>();
        //item.OnSelected = OnItemSelected;
        item.FillPhoto(p);

    }

    void ResizePanel()
    {
        // Get the height of the grid panel
        RectTransform gridRectTransform = Content.GetComponent<RectTransform>();
        float gridHeight = gridRectTransform.rect.height;

        // Set the height of the photo panel to match the height of the grid panel
        RectTransform photoRectTransform = GetComponent<RectTransform>();
        photoRectTransform.sizeDelta = new Vector2(photoRectTransform.sizeDelta.x, gridHeight);
    }

}
