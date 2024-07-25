using System.Collections.Generic;
using MathNet.Numerics;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Linq.Expressions;
using NetTopologySuite.Triangulate;
using Unity.VisualScripting;
using static StatCompute;

public class RouteWalkStats : MonoBehaviour
{
    public GameObject POIStats;
    public GameObject SegmentStats;

    [Header("POI")]
    public BubbleIcon POIIcon;
    public StatViz POIDetailsViz;
    

    public GameObject POINoData;
    public GameObject POIData;

    public StatCard POIDurationCard;
    public StatCard POIPauseCard;

    public StatCard POIDecisionCard;
    public StatCard POIOfftrackCountCard;
    public StatCard POIInstructionModeCard;
    public StatCard POIRecoveryCard;

    public ToggleGroup POICardGroup;

    [Header("Segment")]
    public BubbleIcon SegmentStartIcon;
    public BubbleIcon SegmentEndIcon;
    public StatViz SegDetailsViz;    

    public GameObject SegmentNoData;
    public GameObject SegmentData;

    public StatCard SegDistanceCard;
    public StatCard SegDurationCard;
    public StatCard SegPaceCard;
    public StatCard SegPauseCard;
    public StatCard SegStopsCard;
    public StatCard SegNudgeCard;
    public StatCard SegOfftrackCountCard;
    public StatCard SegRecoveryCard;

    public ToggleGroup SegCardGroup;

    [Header("Adaptation")]
    public AdaptationStats AdaptModeStats;


    private int CurrentIndex;
    private Pathpoint CurrentPOI;
    private RouteWalkItemEvent.RouteWalkEventType CurrentEventType;
    private Way CurrentWay;
    private RouteWalkSharedData WalkSharedData;
    private RouteSharedData SharedData;
    private StatCompute WalkStatCompute;

    private StatsSharedData StatsData;

    // Start is called before the first frame update
    void Awake()
    {
        WalkSharedData = RouteWalkSharedData.Instance;
        SharedData = RouteSharedData.Instance;
        WalkStatCompute = StatCompute.Instance;
        StatsData = StatsSharedData.Instance;

        CurrentIndex = -1;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnRouteWalkItemSelected(Pathpoint poi, int poiIndex, RouteWalkItemEvent.RouteWalkEventType walkEventType)
    {
        // no change
        if (CurrentIndex == poiIndex && CurrentEventType == walkEventType)
            return;

        CurrentIndex = poiIndex;
        CurrentPOI = poi;
        CurrentEventType = walkEventType;
        CurrentWay = SharedData.CurrentWay;

        if (walkEventType == RouteWalkItemEvent.RouteWalkEventType.POISelected)
        {
            ShowPanel(POIStats);
            RenderPOIStats();
        }
        else if (walkEventType == RouteWalkItemEvent.RouteWalkEventType.SegmentSelected)
        {
            ShowPanel(SegmentStats);
            RenderSegmentStats();
        }
        else
        {

        }

        Debug.Log($"Item selected ({CurrentIndex}) POIId: {poi?.Id} Event: {walkEventType}");

        OnPOICardSelected(true);
    }


    public void OnPOICardSelected(bool value)
    {
        if (!value) return;

        foreach ( var activeToggle in POICardGroup.ActiveToggles())
        {
            var card = activeToggle.GetComponent<StatCard>();
            Debug.Log(activeToggle.name + " - " + card.GetCardTitle());
            RenderPOIDetails(activeToggle.name, card.GetCardTitle());
        }               
    }

    public void OnSegmentCardSelected(bool value)
    {
        if (!value) return;

        foreach (var activeToggle in SegCardGroup.ActiveToggles())
        {
            var card = activeToggle.GetComponent<StatCard>();
            Debug.Log(activeToggle.name + " - " + card.GetCardTitle());
            RenderSegmentDetails(activeToggle.name, card.GetCardTitle());
        }
    }

    private void RenderPOIDetails(string name, string title)
    {

        POIDetailsViz.SetCardTitle(title);

        if (name == "DecisionCard" || name == "InstructionCard")
        {            
            POIDetailsViz.RenderPicture(CurrentPOI);
        }
        else if (name == "DurationCard")
        {
            POIDetailsViz.RenderChart(StatsData.POIDurationHistory);
        }
        else if (name == "PauseCard")
        {            
            POIDetailsViz.RenderChart(StatsData.POIPauseHistory);            
        }
        else if (name == "OfftrackCard" || name == "RecoveryCard")
        {
            POIDetailsViz.RenderChartAggregated(StatsData.POIOfftrackCountHistory);
        }
    }

    private void RenderSegmentDetails(string name, string title)
    {

        SegDetailsViz.SetCardTitle(title);

        if (name == "DistanceCard")
        {
            SegDetailsViz.RenderChart(StatsData.SegDistanceHistory);
        }
        else if (name == "PauseCard")
        {
            SegDetailsViz.RenderChart(StatsData.SegPauseHistory);
        }
        else if (name == "DurationCard")
        {
            SegDetailsViz.RenderChart(StatsData.SegDurationHistory);
        }
        else if (name == "StopsCard")
        {
            SegDetailsViz.RenderChart(StatsData.SegStopsHistory);
        }
        else if (name == "WalkPaceCard")
        {
            SegDetailsViz.RenderChart(StatsData.SegWalkPaceHistory);
        }
        else if (name == "NudgesCard")
        {
            SegDetailsViz.RenderChart(StatsData.SegNudgesHistory);
        }
        else if (name == "OfftrackCard" || name == "RecoveryCard")
        {
            SegDetailsViz.RenderChartAggregated(StatsData.SegOfftrackCountHistory);
        }
    }


    public void RenderPOIStats()
    {
        POIIcon.FillPathpoint(CurrentPOI, CurrentWay, CurrentIndex);        
        var poiEvents = WalkSharedData.RouteWalkEventList.FindAll(e => e.TargetPOIId == CurrentPOI.Id);

        // Render Adaptation component
        AdaptModeStats.RenderAdaptPOIStats(CurrentPOI, poiEvents);

        //1. Select the main Decision Event
        RouteWalkEventLog decision = WalkStatCompute.FilterRelevantDecision(poiEvents);

        if (decision == null)
        {
            // reder no data
            ShowPOIData(POINoData);

            return;
        }

        ShowPOIData(POIData);

        var stats = WalkStatCompute.CalculateDecisionStats(WalkSharedData.CurrentRouteWalk, decision);        

        // time
        POIDurationCard.FillCardTime(decision.DurationEvent, stats.Duration);
        

        // decision
        POIDecisionCard.FillCardDecision(decision);        

        // 2. Aggregated metrics
        var errorList = poiEvents.FindAll(e => e.IsCorrectDecision == false);
        POIOfftrackCountCard.FillCardBadCountNumber(errorList.Count, stats.IncorrectDecisions);


        var eventsAtPOI = WalkSharedData.RouteWalkEventList.FindAll(e => e.StartTimestamp > decision.StartTimestamp &&
                                                                    e.StartTimestamp < decision.EndTimestamp);
        // pause
        var pauseList = eventsAtPOI.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Paused);
        var pauseTimeStats = WalkStatCompute.CalculateTimeAggregated(stats, eventsAtPOI, RouteWalkEventLogBase.RouteEvenLogType.Paused);
        POIPauseCard.FillCardTimeAggregated(pauseList, pauseTimeStats.Duration);        

        // recovery modes
        errorList = poiEvents.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Offtrack);
        if (errorList.Count > 0)
        {
            // we render the recovery mode
            List<string> recoveryModes = AggregateRecoveryModes(errorList);
            POIRecoveryCard.FillModes(recoveryModes);
        }
        else
        {
            // Not applicable, since there were no errors to recover from
            POIRecoveryCard.FillModes(new()); // empty list - no data
        }

        // instruction type
        var adaptationList = poiEvents.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Adaptation);
        if (decision.NavInstructionUsed != null & decision.NavInstructionUsed.Count > 0)
        {
            var instructionModes = decision.NavInstructionUsed.Select(nav => nav.ToString());
            POIInstructionModeCard.FillModes(instructionModes.ToList());
        }
        // we 
        else 
        {
             POIInstructionModeCard.FillModes(new());
        }


        // historical information

        StatsData.POIDecisionStats = stats;
        StatsData.POIDurationHistory = stats.DecisionEvents.Select(e => (e.walk, e.log == null ? (double?)null : e.log.DurationEvent / 1000))
                                                           .ToList();
        StatsData.POICorrectHistory = stats.DecisionEvents.Select(e => (e.walk, e.log == null ? (bool?)null : e.log.IsCorrectDecision)).ToList();
        StatsData.POIPauseHistory = pauseTimeStats.TimeEvents.Select(e => (e.walk, e.stat == null ? (double?)null : e.stat.Sum / 1000))
                                                           .ToList();
        var offtrackCountStats = WalkStatCompute.CalculateCountAggregated(stats, poiEvents, RouteWalkEventLogBase.RouteEvenLogType.Offtrack);
        StatsData.POIOfftrackCountHistory = offtrackCountStats.InstanceEvents;
    }


    public void RenderSegmentStats()
    {
        int nextIndex = CurrentIndex + 1;
        Pathpoint nextPOI = SharedData.POIList[nextIndex];
        SegmentStartIcon.FillPathpoint(CurrentPOI, CurrentWay, CurrentIndex);
        SegmentEndIcon.FillPathpoint(nextPOI, CurrentWay, nextIndex);

        //SegAdaptation.LoadAdaptation(nextPOI, isPOI: false);

        // We take the first walk over that path
        var segEvent = WalkSharedData.RouteWalkEventList.Find(e => e.SegPOIStartId == CurrentPOI.Id &&
                                                                    e.SegReachedPOIEndId == nextPOI.Id);
        
        if (segEvent == null)
        {
            ShowSegData(SegmentNoData);
            return;
        }

        var eventsAtSeg = WalkSharedData.RouteWalkEventList.FindAll(e => e.StartTimestamp > segEvent.StartTimestamp &&
                                                                    e.StartTimestamp < segEvent.EndTimestamp);

        AdaptModeStats.RenderSegmentStats(nextPOI, eventsAtSeg);
        ShowSegData(SegmentData);

        var stats = WalkStatCompute.CalculateSegmentStats(WalkSharedData.CurrentRouteWalk, segEvent);

        //time
        SegDurationCard.FillCardTime(segEvent.DurationEvent, stats.Duration);

        //distance
        double correctlyWalkedPerc = 0;
        if (segEvent.DistanceWalked >0)
        {
            correctlyWalkedPerc = 100 * (double)segEvent.DistanceCorrectlyWalked / (double)segEvent.DistanceWalked;
        }
        SegDistanceCard.FillCardGoodValueNumber(correctlyWalkedPerc, stats.CorrectDistance, valueUnit : "%");

        // pace
        var pace = WalkStatCompute.ConvertPace((double)segEvent.WalkingPace); // convert to min / km
        SegPaceCard.FillCardGoodValueNumber((double)segEvent.WalkingPace, stats.WalkPace, fmtText: pace);

        // offtrack


        var offtrackList = eventsAtSeg.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Offtrack);

        // pause
        var pauseList = eventsAtSeg.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Paused);
        var pauseTimeStats = WalkStatCompute.CalculateTimeAggregated(stats, eventsAtSeg, RouteWalkEventLogBase.RouteEvenLogType.Paused);
        SegPauseCard.FillCardTimeAggregated(pauseList, pauseTimeStats.Duration);

        // stop
        var stopList = eventsAtSeg.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Stopped);
        var stopTimeStats = WalkStatCompute.CalculateTimeAggregated(stats, eventsAtSeg, RouteWalkEventLogBase.RouteEvenLogType.Stopped);
        SegStopsCard.FillCardTimeAggregated(stopList, stopTimeStats.Duration);

        // attention nudges        
        var sleepList = eventsAtSeg.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Sleep);
        var sleepTimeStats = WalkStatCompute.CalculateTimeAggregated(stats, eventsAtSeg, RouteWalkEventLogBase.RouteEvenLogType.Sleep);
        SegNudgeCard.FillCardTimeAggregated(sleepList, sleepTimeStats.Duration, isLowerBetter: false);

        // offtrack
        var errorList = eventsAtSeg.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Offtrack);
        var errorStats = WalkStatCompute.CalculateCountAggregated(stats, eventsAtSeg, RouteWalkEventLogBase.RouteEvenLogType.Offtrack);
        SegOfftrackCountCard.FillCardBadCountNumber(errorList.Count, errorStats.Instances);

        // recovery
        if (errorList.Count > 0)
        {
            // we render the recovery mode
            List<string> recoveryModes = AggregateRecoveryModes(errorList);
            SegRecoveryCard.FillModes(recoveryModes);         
        }
        else
        {
            // Not applicable, since there were no errors to recover from
            SegRecoveryCard.FillModes(new()); // empty list - no data
        }

        StatsData.SegStats = stats;
        StatsData.SegDistanceHistory = stats.SegmentEvents.Select(e => (e.walk, e.log == null ? (double?)null :
                                        100 * (double)e.log.DistanceCorrectlyWalked / (double)e.log.DistanceWalked))
                                                           .ToList();
        StatsData.SegDurationHistory = stats.SegmentEvents.Select(e => (e.walk, e.log == null ? (double?)null : e.log.DurationEvent / 1000))
                                                           .ToList();
        StatsData.SegWalkPaceHistory = stats.SegmentEvents.Select(e => (e.walk, e.log == null ? (double?)null : e.log.WalkingPace))
                                                           .ToList();
        StatsData.SegPauseHistory = pauseTimeStats.TimeEvents.Select(e => (e.walk, e.stat == null ? (double?)null : e.stat.Sum / 1000))
                                                           .ToList();
        StatsData.SegStopsHistory = stopTimeStats.TimeEvents.Select(e => (e.walk, e.stat == null ? (double?)null : e.stat.Sum / 1000))
                                                           .ToList();
        StatsData.SegNudgesHistory = sleepTimeStats.TimeEvents.Select(e => (e.walk, e.stat == null ? (double?)null : e.stat.Sum / 1000))
                                                           .ToList();        
        StatsData.SegOfftrackCountHistory = errorStats.InstanceEvents;

    }

    private List<string> AggregateRecoveryModes(List<RouteWalkEventLog> errorList)
    {
        var recoveryModes = new HashSet<string>();

        // if interrupted, in any of them, this means that we did not recovery from this segment
        var interruptedEvent = errorList.Find(e => e.WasEventInterrupted == true);
        if (interruptedEvent != null)
        {
            // We did not recover from the mistake
            return new List<string> { RouteWalkEventLog.RecoveryInstructionType.NoRecovery.ToString() };
        }

        // we aggregate
        foreach (var e in errorList)
        {
            if (e.RecoveryInstructionUsed != null)
            {
                // if the user set "notIssue", it overwrites the other modes used in that off-track recovery
                if (e.RecoveryInstructionUsed.Contains(RouteWalkEventLog.RecoveryInstructionType.NoIssue))
                {
                    recoveryModes.Add(RouteWalkEventLog.RecoveryInstructionType.NoIssue.ToString());
                }
                else
                {
                    recoveryModes.UnionWith(e.RecoveryInstructionUsed.Select(mode => mode.ToString()));
                }
            }
            else
            {
                recoveryModes.Add(RouteWalkEventLog.RecoveryInstructionType.None.ToString());
            }
        }

        return recoveryModes.ToList();
    }


    public void ShowPanel(GameObject view)
    {
        POIStats.SetActive(POIStats == view);
        SegmentStats.SetActive(SegmentStats == view);
    }

    public void ShowPOIData(GameObject view)
    {
        POIData.SetActive(POIData == view);
        POINoData.SetActive(POINoData == view);
    }

    public void ShowSegData(GameObject view)
    {
        SegmentData.SetActive(SegmentData == view);
        SegmentNoData.SetActive(SegmentNoData == view);
    }
}
