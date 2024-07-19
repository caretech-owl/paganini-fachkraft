using System;
using System.Collections.Generic;
using System.Linq;
using PaganiniRestAPI;
using UnityEngine;
using UnityEngine.Events;
using static StatCompute;

public class StatsSharedData : PersistentLazySingleton<StatsSharedData>
{

    // computed statistics
    public DecisionStats POIDecisionStats { get; set; }
    public TimeStats POIPauseStats { get; set; }

    public List<(RouteWalk log, double? value)> POIDurationHistory { get; set; }
    public List<(RouteWalk log, bool? value)> POICorrectHistory { get; set; }
    public List<(RouteWalk log, StatResults value)> POIOfftrackCountHistory { get; set; }
    public List<(RouteWalk log, double? value)> POIPauseHistory { get; set; }

    public SegmentStats SegStats;
    public List<(RouteWalk log, double? value)> SegDistanceHistory { get; set; }
    public List<(RouteWalk log, double? value)> SegDurationHistory { get; set; }
    public List<(RouteWalk log, double? value)> SegWalkPaceHistory { get; set; }
    public List<(RouteWalk log, double? value)> SegPauseHistory { get; set; }
    public List<(RouteWalk log, double? value)> SegStopsHistory { get; set; }
    public List<(RouteWalk log, double? value)> SegNudgesHistory { get; set; }
    public List<(RouteWalk log, StatResults value)> SegOfftrackCountHistory { get; set; }


    public StatsSharedData()
    {
    }


}

