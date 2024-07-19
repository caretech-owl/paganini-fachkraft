using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POITimeline : MonoBehaviour
{
    public GameObject Content;
    public GameObject Viewport;
    public GameObject Padding;
    public GameObject Backline;

    public GameObject ItemPrefab;
    public PathpointItemEvent OnItemSelected;

    private int CurrentIndex;
    private float TimelineWidth = 0f;
    private List<POITimelineItem> _poiItems;

    // Start is called before the first frame update
    void Awake()
    {
        CurrentIndex = 0;
        _poiItems = new List<POITimelineItem>();
    }

    // Update is called once per frame
    float w;
    void Update()
    {
        //RectTransform contentRectTransform = Content.GetComponent<RectTransform>();

        //if (w != contentRectTransform.rect.width) {
        //    ResizePanel();
        //    w = contentRectTransform.rect.width;
        //}

    }

    public void RenderPOIChanges(int index)
    {
        Debug.Log($"Rendering Changes to POI [{index}]");
        _poiItems[index].RenderChanges();
    }
    
    public void AddPOI(Pathpoint p)
    {
        var item = SetupItem(p);
        item.FillPathpoint(p, CurrentIndex++ );

        AddToTimelineWidth(item.gameObject);
        ResizePanel();
    }

    public void AddStart(Pathpoint p, Way way)
    {
        var item = SetupItem(p);
        item.FillPathpointStart(p, way);

        AddToTimelineWidth(item.gameObject);

        ResizePanel();
    }

    public void AddDestination(Pathpoint p, Way way)
    {
        var item = SetupItem(p);
        item.FillPathpointDestination(p, way);

        AddToTimelineWidth(item.gameObject);
        ResizePanel();
    }

    private POITimelineItem SetupItem(Pathpoint p)
    {
        var neu = Instantiate(ItemPrefab, Content.transform);

        POITimelineItem item = neu.GetComponent<POITimelineItem>();
        item.OnSelected = OnItemSelected;

        _poiItems.Add(item);

        return item;
    }

    public void Clearlist()
    {        
        Transform content = Content.GetComponent<Transform>();

        // Remove all children
        for (int i = 0; i < content.childCount; i++)
        {            
            // Get the child game object
            GameObject child = content.GetChild(i).gameObject;

            if (child.name != "Padding") {
                Destroy(child);
            }            
        }
        TimelineWidth = 0;
        CurrentIndex = 1;
        w = -1;
        _poiItems.Clear();
    }

    void ResizePanel()
    {
        // Get the width of the content
        //RectTransform contentRectTransform = Content.GetComponent<RectTransform>();
        //float contentWidth = contentRectTransform.rect.width;

        // Get the width of the container of the content
        RectTransform viewportRectTransform = Viewport.GetComponent<RectTransform>();
        float viewportWidth = viewportRectTransform.rect.width;

        RectTransform padingRectTransform = Padding.GetComponent<RectTransform>();
        float currPaddingWidth = padingRectTransform.rect.width;

        // let's remove the current padding to the actual content
        //contentWidth = contentWidth - currPaddingWidth;


        float contentWidth = TimelineWidth;

        // Adding padding to center the contents
        float paddingWidth = contentWidth < viewportWidth ? (viewportWidth - contentWidth) / 2 : 1;
        Debug.Log($"Content width: {contentWidth} Current pad: {currPaddingWidth} Viewport: {viewportWidth} NewPad: {currPaddingWidth}");

        if (contentWidth == 0) return;

        padingRectTransform.sizeDelta = new Vector2(paddingWidth, padingRectTransform.sizeDelta.y);

        // Center the back line, standing behind the pins
        RectTransform lineRectTransform = Backline.GetComponent<RectTransform>();
        lineRectTransform.sizeDelta = new Vector2(contentWidth - 100, lineRectTransform.sizeDelta.y);
        
    }

    public void CleanupView()
    {
        Clearlist();
    }

    private void AddToTimelineWidth(GameObject item)
    {
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        TimelineWidth+= itemRectTransform.rect.width;
    }

}
