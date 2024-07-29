using System;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static PathpointPIM;

[System.Serializable]
public class RouteWalkItemEvent : UnityEvent<Pathpoint, int, RouteWalkItemEvent.RouteWalkEventType>
{
    public enum RouteWalkEventType
    {        
        POISelected,
        SegmentSelected,
        NoneSelected
    }
}

public class RouteWalkTimelineSegment : MonoBehaviour
{
    [Header("Segment")]
    public GameObject OntrackSegmentPanel;    
    public GameObject OfftrackSegmentPanel;
    public GameObject CellPrefab;
    public Toggle SegmentToggle;

    [Header("Decision")]
    public GameObject WrongTurnPanel;
    public GameObject WrongDirectionPanel;
    public GameObject MissedTurnPanel;
    public Image CirclePanel;
    public Toggle CircleToggle;

    [Header("Adaptation")]
    public StatCard POIPracticedMode;
    public AdaptationPractice POIPracticeOutcome;
    public StatCard SegPracticedMode;
    public AdaptationPractice SegPracticeOutcome;
    public GameObject MuteOverlay;
    private SupportMode _atPOISupportMode;


    [Header("Events")]
    public RouteWalkItemEvent OnSelected;

    public Color OfftrackColor;
    public Color OntrackColor;
    public Color SelectedColor;
    private Color NoColor;

    private GridLayoutGroup gridLayout;
    private Pathpoint CurrentPOI;
    private int CurrentPOIIndex;

    // Start is called before the first frame update
    void Start()
    {
        SegmentToggle?.onValueChanged.AddListener(HandleSegmentSelected);
        CircleToggle?.onValueChanged.AddListener(HandlePOISelected);

        // transparent
        NoColor = Color.white;
        NoColor.a = 1;

        // hiding adaptation
        //POIPracticedMode.gameObject.SetActive(false);
        //SegPracticedMode.gameObject.SetActive(false);
        //MuteOverlay.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //RenderAdaptation();
    }

    public void SelectSegment()
    {
        if (SegmentToggle != null) {
            SegmentToggle.SetIsOnWithoutNotify(true);
            HandleSegmentSelected(true);
        }        
    }

    // Fills out a progressbar-like component representing a path with 
    public void RenderSubpath(List<Pathpoint> subpath, List<PathpointLog> logList)
    {
        int gridSize = subpath.Count;
        SetupSegmentGrid(gridSize, OntrackSegmentPanel, OntrackColor);
        SetupSegmentGrid(gridSize, OfftrackSegmentPanel, NoColor);


        // Handle offtrack visualization
        int i1 = logList.FindIndex(log => log.OnOffTrackEvent != null);
        int i2 = i1 >= 0 ? logList.FindLastIndex(log => log.OnOffTrackEvent != null) : -1;

        // Are there offtrack events?
        if (i1 >= 0)
        {
            int p1 = CalculateClosestPathpoint(logList[i1], subpath);
            int p2 = CalculateClosestPathpoint(logList[i2], subpath);

            // If there are offtrack events, we paint it so
            for (int i = p1; i <= p2; i++)
            {
                SetCellColor(OfftrackSegmentPanel, i, OfftrackColor);
                SetCellColor(OntrackSegmentPanel, i, NoColor);
            }

            //SetCellColor(OfftrackSegmentPanel, Math.Max(p1-1,0), OfftrackColor);
            //SetCellColor(OfftrackSegmentPanel, Math.Min(p2+1, gridSize-1), OfftrackColor);

        }
    }

    public void RenderLoop(List<PathpointLog> logList)
    {
        var log = logList.Find(log => log.OnOffTrackEvent != null);

        if (log != null)
        {
            // we do not do anything here?
            WrongDirectionPanel.SetActive(true);

        }           

        Debug.Log("Visualisating loop: OffTrackEVent: "+ log.OnOffTrackEvent);
    }

    public void RenderDecision(RouteWalkEventLog decisionMadeEvent)
    {
        // Correct decision
        if (decisionMadeEvent.IsCorrectDecision == true)
        {
            CirclePanel.color = OntrackColor;
            return;
        }

        // Incorrect decision
        CirclePanel.color = OfftrackColor;

        if (decisionMadeEvent.NavIssue == LocationTools.NavigationIssue.WrongDirection)
        {
            WrongDirectionPanel.SetActive(true);
        }
        else if (decisionMadeEvent.NavIssue == LocationTools.NavigationIssue.WrongTurn)
        {
            WrongTurnPanel.SetActive(true);
        }
        else if (decisionMadeEvent.NavIssue == LocationTools.NavigationIssue.MissedTurn)
        {
            MissedTurnPanel.SetActive(true);
        }

        Debug.Log("Offtrack event: " + decisionMadeEvent.NavIssue);
    }


    public void HideSegment()
    {
        SegmentToggle?.gameObject.SetActive(false);
    }

    public void SetupSegment(ToggleGroup group, Pathpoint poi, int poiIndex, bool isPOIClickable = true)
    {
        CircleToggle.interactable = isPOIClickable;
        if (isPOIClickable)
        {
            CircleToggle.group = group;
        }

        SegmentToggle.group = group;
        

        CurrentPOI = poi;
        CurrentPOIIndex = poiIndex;
    }

    public void RenderPracticedPOIAdaptation(RouteWalkEventLog adaptationLog)
    {
        RenderAdaptation(adaptationLog, POIPracticedMode, POIPracticeOutcome, isPOI: true);
    }

    public void RenderPracticedSegAdaptation(RouteWalkEventLog adaptationLog)
    {
        RenderAdaptation(adaptationLog, SegPracticedMode, SegPracticeOutcome, isPOI: false);
    }

    public void RenderCurrentSegAdaptation(Pathpoint point)
    {
        if (point.CurrentInstructionMode != null)
        {
            SegPracticedMode.FillModes(new List<string> { point.CurrentInstructionMode.ToPOIMode.ToString()});
        }
        
        SegPracticedMode.gameObject.SetActive(point.CurrentInstructionMode != null);
        SegPracticeOutcome.HideView();
    }

    public void RenderCurrentPOIAdaptation(Pathpoint point)
    {
        if (point.CurrentInstructionMode != null)
        {
            POIPracticedMode.FillModes(new List<string> { point.CurrentInstructionMode.AtPOIMode.ToString() });
        }

        POIPracticedMode.gameObject.SetActive(point.CurrentInstructionMode != null);
        POIPracticeOutcome.HideView();
    }

    public void RenderAdaptation(RouteWalkEventLog adaptationLog, StatCard statCard, AdaptationPractice practice, bool isPOI)
    {
        if (adaptationLog != null)
        {
            statCard.FillModes(new List<string> { adaptationLog.AdaptationSupportMode.ToString() });
            statCard.gameObject.SetActive(true);
            practice.RenderAdaptationPracticed(adaptationLog);

            if (isPOI)
            {
                MuteOverlay.gameObject.SetActive(adaptationLog.AdaptationSupportMode == SupportMode.Mute);
            }
        }
        else
        {
            statCard.gameObject.SetActive(false);
            MuteOverlay.gameObject.SetActive(false);
        }        
    }


    private void SetupSegmentGrid(int gridSize, GameObject segment, Color fillColor)
    {
        gridLayout = segment.GetComponent<GridLayoutGroup>();

        if (gridLayout == null)
        {
            gridLayout = segment.AddComponent<GridLayoutGroup>();
        }

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize; // Only one row

        // Calculate cell size
        RectTransform segmentPanelRect = segment.GetComponent<RectTransform>();
        float cellWidth = segmentPanelRect.rect.width / gridSize;

        gridLayout.cellSize = new Vector2(cellWidth, 10); // Adjust size as needed

        foreach (Transform child in segment.transform)
        {
            Destroy(child.gameObject);
        }

        // By default, all cells are onTrack
        for (int i = 0; i < gridSize; i++)
        {
            AddCell(segment, fillColor);
        }
    }

    private int CalculateClosestPathpoint(PathpointLog log, List<Pathpoint> pathpointList)
    {
        double minDistance = Double.MaxValue;
        int minIndex = -1;
        for (int i = 0; i < pathpointList.Count; i++)
        {
            Pathpoint pLog = new Pathpoint
            {
                Longitude = log.Longitude,
                Latitude = log.Latitude
            };
            var distance = LocationTools.GPSUtils.HaversineDistance(pLog, pathpointList[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }

        return minIndex;
    }

    private void HandleSegmentSelected(bool selected)
    {
        Debug.Log("Segment selected: " + selected);
        if (selected)
        {
            OnSelected?.Invoke(CurrentPOI, CurrentPOIIndex, RouteWalkItemEvent.RouteWalkEventType.SegmentSelected);
        }
        else
        {
            HandleNoSelection();
        }
    }

    private void HandlePOISelected(bool selected)
    {
        if (selected)
        {
            OnSelected?.Invoke(CurrentPOI, CurrentPOIIndex, RouteWalkItemEvent.RouteWalkEventType.POISelected);
        }
        else
        {
            HandleNoSelection();
        }
    }

    // Did we turn off the selection?
    private void HandleNoSelection()
    {
        if (!SegmentToggle.group.AnyTogglesOn()) // circle and segment have the same group
        {
            OnSelected.Invoke(null, -1, RouteWalkItemEvent.RouteWalkEventType.NoneSelected);
        }
    }

    private void AddCell(GameObject segment, Color color)
    {
        GameObject cell = Instantiate(CellPrefab, segment.transform);
        Image img = cell.GetComponent<Image>();
        img.color = color;
    }

    private void SetCellColor(GameObject segment, int index, Color color)
    {
        if (index >= 0 && index < segment.transform.childCount)
        {
            Transform child = segment.transform.GetChild(index);
            Image img = child.GetComponent<Image>();
            img.color = color;
        }
    }
}
