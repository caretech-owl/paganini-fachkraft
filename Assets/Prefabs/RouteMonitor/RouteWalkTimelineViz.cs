using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Triangulate;
using UnityEngine;
using UnityEngine.UI;

public class RouteWalkTimelineViz : MonoBehaviour
{
    public GameObject Content;
    public GameObject Viewport;
    public GameObject Padding;
    public GameObject Backline;

    public GameObject ItemPrefab;
    public ToggleGroup SelectionToggleGroup;
    public RouteWalkItemEvent OnItemSelected;

    private int CurrentIndex;
    private Pathpoint CurrentPOI;
    private float TimelineWidth = 0f;

    private POITimelineItem _poiItem;
    private POITimelineItem _firstPoiItem;


    // Start is called before the first frame update
    void Awake()
    {
        CurrentIndex = 0;
    }

    // Update is called once per frame

    void Update()
    {

    }

    public void SelectFirstSegment()
    {
        RouteWalkTimelineSegment segment = _firstPoiItem.gameObject.GetComponent<RouteWalkTimelineSegment>();
        segment.SelectSegment();
    }
    
    public void AddPOI(Pathpoint p)
    {
        CurrentIndex = CurrentIndex+1;
        var item = SetupItem(p);
        item.FillPathpoint(p, CurrentIndex );        

        AddToTimelineWidth(item.gameObject);
        ResizePanel();
    }

    public void AddStart(Pathpoint p, Way way)
    {
        var item = SetupItem(p, isClickable: false);
        item.FillPathpointStart(p, way);

        _firstPoiItem = item;

        AddToTimelineWidth(item.gameObject);

        ResizePanel();
    }

    public void AddDestination(Pathpoint p, Way way)
    {
        CurrentIndex = CurrentIndex + 1;
        var item = SetupItem(p, isClickable: false);
        item.FillPathpointDestination(p, way);

        // Destination POI does not have a segement afterwards
        RouteWalkTimelineSegment segment = item.gameObject.GetComponent<RouteWalkTimelineSegment>();
        segment.HideSegment();

        AddToTimelineWidth(item.gameObject);
        ResizePanel();
    }

    private POITimelineItem SetupItem(Pathpoint p, bool isClickable = true)
    {
        var neu = Instantiate(ItemPrefab, Content.transform);

        _poiItem = neu.GetComponent<POITimelineItem>();
        //poiItem.OnSelected = OnItemSelected;

        CurrentPOI = p;

        SetupDefaultSegment(isClickable);

        return _poiItem;
    }

    public void AddSegment(List<Pathpoint> subpath, List<PathpointLog> logList)
    {        
        RouteWalkTimelineSegment segment = _poiItem.gameObject.GetComponent<RouteWalkTimelineSegment>();
        //item.OnSelected = OnSegmentSelected;

        segment.RenderSubpath(subpath, logList);

        //AddDefaultSegment(segment);
    }

    public void SetupDefaultSegment(bool isClickable = true)
    {
        RouteWalkTimelineSegment segment = _poiItem.gameObject.GetComponent<RouteWalkTimelineSegment>();

        if (SelectionToggleGroup != null)
        {            
            segment.SetupSegment(SelectionToggleGroup, CurrentPOI, CurrentIndex, isPOIClickable: isClickable);
        }

        segment.OnSelected = OnItemSelected;
    }

    public void AddLoop(List<PathpointLog> logList)
    {
        RouteWalkTimelineSegment segment = _poiItem.gameObject.GetComponent<RouteWalkTimelineSegment>();

        segment.RenderLoop(logList);
    }

    public void AddDecisionMade(List<RouteWalkEventLog> eventList)
    {
        if (eventList.Count == 0) return;

        var segment = _poiItem.GetComponent<RouteWalkTimelineSegment>();

        var decisionMade = eventList.FirstOrDefault(e => e.IsCorrectDecision == false) ?? eventList[0];

        segment.RenderDecision(decisionMade);
    }

    public void LoadPOIAdaptation(RouteWalkEventLog adaptationLog)
    {
        var segment = _poiItem.GetComponent<RouteWalkTimelineSegment>();
        segment.RenderPracticedPOIAdaptation(adaptationLog);
    }

    public void LoadSegAdaptation(RouteWalkEventLog adaptationLog)
    {
        var segment = _poiItem.GetComponent<RouteWalkTimelineSegment>();
        segment.RenderPracticedSegAdaptation(adaptationLog);
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
        CurrentIndex = 0;
        //w = -1;
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
        // force to update size, necessary as it is controlled by the parent
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();        
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemRectTransform);

        TimelineWidth += itemRectTransform.rect.width;

        //Debug.Log("POI Size: " + itemRectTransform.rect.width);
    }

}
