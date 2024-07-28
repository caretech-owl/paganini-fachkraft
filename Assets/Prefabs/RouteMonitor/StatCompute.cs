using System;
using System.Collections.Generic;
using System.Drawing;
using MathNet.Numerics;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate;
using UnityEngine;
using UnityEngine.UI;
using static StatCompute;


public class StatCompute : PersistentLazySingleton<StatCompute>
{

    private RouteWalkSharedData WalkSharedData;
    private RouteSharedData SharedData;

    private List<PathpointPIM.SupportMode> _atPOIModesScore = new List<PathpointPIM.SupportMode> {
        PathpointPIM.SupportMode.Trivia,
        PathpointPIM.SupportMode.Challenge,
        PathpointPIM.SupportMode.Mute
    };

    private List<PathpointPIM.SupportMode> _toPOIModesScore = new List<PathpointPIM.SupportMode> {
        PathpointPIM.SupportMode.Trivia,
        PathpointPIM.SupportMode.Challenge
    };

    private List<PathpointPIM.SupportMode> _atPOIReassuranceModesScore = new List<PathpointPIM.SupportMode> {
        PathpointPIM.SupportMode.Mute
    };

    private void Start()
    {
        WalkSharedData = RouteWalkSharedData.Instance;
        SharedData = RouteSharedData.Instance;
    }

    public class StatResults
    {
        public double Sum;
        public double Mean { get { return Sum / Count; } }
        public int Count;

        public StatResults()
        {
            Sum = 0;
            Count = 0;
        }

        public double GetValueChange(double refValue)
        {
            return (refValue - Mean) / Mean;
        }

    }

    public class DecisionStats
    {
        public List<(RouteWalk walk, RouteWalkEventLog log)> DecisionEvents;
        public StatResults Duration;
        public StatResults IncorrectDecisions;

        public DecisionStats()
        {
            DecisionEvents = new() { };
            Duration = new() { };
            IncorrectDecisions = new() { };
        }
        public void AddEvent(RouteWalk walk, RouteWalkEventLog eventLog)
        {
            DecisionEvents.Add((walk,eventLog));
            Duration.Count++;
            IncorrectDecisions.Count++;
        }

    }

    public class SegmentStats
    {
        public List<(RouteWalk walk, RouteWalkEventLog log)> SegmentEvents;
        public StatResults Duration;
        public StatResults CorrectDistance;
        public StatResults WalkPace;
        public StatResults Stops;
        public StatResults Pauses;
  

        public SegmentStats()
        {
            SegmentEvents = new() { };
            Duration = new() { };
            CorrectDistance = new() { };
            WalkPace = new() { };
            Stops = new() { };
            Pauses = new() { };

        }
        public void AddEvent(RouteWalk walk, RouteWalkEventLog eventLog)
        {
            SegmentEvents.Add((walk, eventLog));
            Duration.Count++;
            CorrectDistance.Count++;
            WalkPace.Count++;
            Stops.Count++;
            Pauses.Count++;
        }
    }

    public class TimeStats
    {
        // segment, duration, 
        public List<(RouteWalk walk, StatResults stat)> TimeEvents;
        public StatResults Duration;

        public int Count { get; set; }

        public TimeStats()
        {
            TimeEvents = new() { };
            Duration = new() { };
        }
        public void AddEvent((RouteWalk walk, StatResults stat) eventLog)
        {
            TimeEvents.Add(eventLog);
        }
    }

    public class CountStats
    {
        // segment, duration, 
        public List<(RouteWalk, StatResults)> InstanceEvents;
        public StatResults Instances;

        public int Count { get; set; }

        public CountStats()
        {
            InstanceEvents = new() { };
            Instances = new() { };
        }
        public void AddEvent((RouteWalk, StatResults) eventLog)
        {
            InstanceEvents.Add(eventLog);
        }
    }

    /*PKI compute*/
    public double CalculatePKITrainingProgress()
    {
        double atPoiRaPoints = 0;
        double atPoiLmPoints = 0;
        double toPoiPoints = 0;

        int raCount = 0;
        int lmCount= 0;
        int segCount = SharedData.POIList.Count -1;

        // to properly score segment after a muted poi   (poi)-mute-(poi)
        bool wasMuted = false;

        foreach (var poi in SharedData.POIList)
        {
            if (poi.CurrentInstructionMode!= null)
            {
                (int topoi, int atpoiLandmark, int atpoiReassurance) = GetPerformancePoints(poi.CurrentInstructionMode, poi.POIType);
                atPoiLmPoints = atPoiLmPoints + atpoiLandmark;
                atPoiRaPoints = atPoiRaPoints + atpoiReassurance;
                

                if (wasMuted) // maxout points for this segment
                {
                    toPoiPoints = toPoiPoints + _toPOIModesScore.Count;
                }
                else // according to the mode set for the segment
                {
                    toPoiPoints = toPoiPoints + topoi;
                }

                wasMuted = poi.CurrentInstructionMode.AtPOIMode == PathpointPIM.SupportMode.Mute; 
            }

            raCount+= poi.POIType == Pathpoint.POIsType.Reassurance? 1: 0;
            lmCount += poi.POIType == Pathpoint.POIsType.Landmark ? 1 : 0;
        }

        double score = atPoiLmPoints / (lmCount * _atPOIModesScore.Count) +
                       atPoiRaPoints / (raCount * _atPOIReassuranceModesScore.Count) +
                        toPoiPoints / (segCount * _toPOIModesScore.Count);

        return score / 3;

    }

    public double CalculatePKITrainingPerformance()
    {
        return 0;
    }

    public double CalculatePKITrainingCompleteness()
    {
        if (WalkSharedData.CurrentRouteWalk == null)
            return double.NaN;


        if (WalkSharedData.CurrentRouteWalk.WalkCompletion == RouteWalk.RouteWalkCompletion.Completed)
        {
            return 1;
        }

        return WalkSharedData.CurrentRouteWalk.WalkCompletionPercentage;
    }

    public double CalculatePKITrainingAccuracy()
    {
        double poiErrorCount = 0;
        double segErrorCount = 0;
        var errorList = WalkSharedData.RouteWalkEventList.FindAll(e => e.EvenLogType == RouteWalkEventLogBase.RouteEvenLogType.Offtrack);
        foreach (var poi in SharedData.POIList)
        {
            if (poi.POIType == Pathpoint.POIsType.WayDestination)
                break;

            int poiIndex = errorList.FindIndex(e => e.TargetPOIId == poi.Id);
            int segIndex = errorList.FindIndex(e => e.SegPOIStartId == poi.Id);

            poiErrorCount = poiErrorCount + (poiIndex < 0 ? 0 : 1);
            segErrorCount = segErrorCount + (segIndex < 0 ? 0 : 1);
        }

        double n_decisions = SharedData.POIList.Count -1 + // pois where decisions are made (arrival doesn't count
                             SharedData.POIList.Count -1;  // segments where user walks

        double score = 1- (poiErrorCount + segErrorCount) / n_decisions;

        return score;
    }

    private (int, int, int) GetPerformancePoints(PathpointPIM pim, Pathpoint.POIsType poiType)
    {
        int topoi = 0;
        int atpoiLandmark = 0;
        int atpoiReassurance = 0;

        topoi = _toPOIModesScore.IndexOf(pim.ToPOIMode) + 1;

        if (poiType == Pathpoint.POIsType.Landmark)
        {
            atpoiLandmark = _atPOIModesScore.IndexOf(pim.AtPOIMode) + 1;
        }
        else
        {
            atpoiReassurance = _atPOIReassuranceModesScore.IndexOf(pim.AtPOIMode) + 1;
        }
        

        return (topoi, atpoiLandmark, atpoiReassurance);
    }

    /*Stat compute*/

    public DecisionStats CalculateDecisionStats(RouteWalk currentWalk, RouteWalkEventLog currentDecionEvent)
    {
        // filter the envets of the poi
        var lists = GetPathpointEvents((int)currentDecionEvent.TargetPOIId, RouteWalkEventLogBase.RouteEvenLogType.DecisionMade);

        DecisionStats stats = new() { };        

        foreach ((var walk, var eventList) in lists)
        {
            var decision = FilterRelevantDecision(eventList);

            // no data on that routeWalk
            if (decision == null)
            {
                // Add without counting towards the statistic
                stats.DecisionEvents.Add((walk, null));
                continue;
            }
                

            stats.AddEvent(walk, decision);
            stats.Duration.Sum += decision.DurationEvent;


            //var errorList = eventList.FindAll(e => e.IsCorrectDecision == false);
            //stats.IncorrectDecisions.Sum += errorList.Count;
            stats.IncorrectDecisions.Count += decision.IsCorrectDecision == false ? 1 : 0;
            stats.IncorrectDecisions.Sum += decision.IsCorrectDecision == false ? 1 : 0;
        }

        // Add the current event at the end. This one doesn't count towards historical statistics
        stats.DecisionEvents.Add((currentWalk, currentDecionEvent));

        return stats;
    }

    public SegmentStats CalculateSegmentStats(RouteWalk currentWalk, RouteWalkEventLog currentSegmentEvent)
    {
        var lists = GetSegmentEvents((int)currentSegmentEvent.SegPOIStartId, RouteWalkEventLogBase.RouteEvenLogType.POIReached);
        SegmentStats stats = new() { };

        foreach ((var walk, var eventList) in lists)
        {
            var segment = FilterRelevantSegment(eventList, (int)currentSegmentEvent.SegExpectedPOIEndId);

            // no data on that routeWalk
            if (segment == null)
            {
                stats.SegmentEvents.Add((walk, null));
                continue;
            }
                

            stats.AddEvent(walk, segment);
            stats.Duration.Sum += segment.DurationEvent;
            //calculate percentage of distance walked
            stats.CorrectDistance.Sum += 100 * (double)segment.DistanceCorrectlyWalked / (double)segment.DistanceWalked;
            stats.WalkPace.Sum += ConvertPaceToMmh((double)segment.WalkingPace)??0;            

            //var errorList = eventList.FindAll(e => e.IsCorrectDecision == false);
            //stats.IncorrectDecisions.Sum += errorList.Count;
            //stats.IncorrectDecisions.Sum += decision.IsCorrectDecision == false ? 1 : 0;
        }

        // Add the current event at the end. This one doesn't count towards historical statistics
        stats.SegmentEvents.Add((currentWalk, currentSegmentEvent));

        return stats;
    }

    public TimeStats CalculateTimeAggregated(SegmentStats segmentStats, List<RouteWalkEventLog> eventsAtSeg, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        var list = eventsAtSeg.FindAll(e => e.EvenLogType == evenLogType);

        TimeStats stats = new () { };

        // go over previous segment events
        foreach ((var walk, var segment) in segmentStats.SegmentEvents)
        {
            var eventList = GetEventsAtSegment(walk, segment, evenLogType);

            StatResults duration = new() { };
            foreach(var e in eventList)
            {
                duration.Sum += e.DurationEvent;
                duration.Count++;
            }

            //TODO: Fix the new format
            stats.AddEvent((walk, duration));

            // last stat is the current stat, and doesn't count towards statistics
            if (stats.TimeEvents.Count == segmentStats.SegmentEvents.Count)
            {
                continue;
            }

            if (eventList.Count > 0)
            {
                stats.Count++;
                stats.Duration.Sum += duration.Sum;                
            }

            // we only count towards the statistic, those segments that were
            // actually reached
            if (segment != null)
            {
                stats.Duration.Count++;
            }
            

        }

        return stats;
    }


    public TimeStats CalculateTimeAggregated(DecisionStats decisionStats, List<RouteWalkEventLog> eventsAtPOI, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        var list = eventsAtPOI.FindAll(e => e.EvenLogType == evenLogType);

        TimeStats stats = new() { };

        // go over previous poi events
        foreach ((var walk, var decision) in decisionStats.DecisionEvents)
        {
            var eventList = GetEventsAtPOI(walk, decision, evenLogType);

            StatResults duration = new() { };
            foreach (var e in eventList)
            {
                duration.Sum += e.DurationEvent;
                duration.Count++;
            }
            stats.AddEvent((walk, duration));

            // last stat is the current stat, and doesn't count towards statistics
            if (stats.TimeEvents.Count == decisionStats.DecisionEvents.Count)
            {
                continue;
            }

            
            if (eventList.Count > 0)
            {
                stats.Count++;
                stats.Duration.Sum += duration.Sum;
            }
            
            // we only count towards the statistic, those poi that were
            // actually reached
            if (decision != null)
            {
                stats.Duration.Count++;
            }

        }

        return stats;
    }


    public CountStats CalculateCountAggregated(SegmentStats segmentStats, List<RouteWalkEventLog> eventsAtSeg, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        var list = eventsAtSeg.FindAll(e => e.EvenLogType == evenLogType);

        CountStats stats = new() { };

        // go over previous segment events
        foreach ((var walk, var segment) in segmentStats.SegmentEvents)
        {
            var eventList = GetEventsAtSegment(walk, segment, evenLogType);

            StatResults instanceEvent = new() { };
            foreach (var e in eventList)
            {
                instanceEvent.Sum++ ;
                instanceEvent.Count++;
            }
            
            stats.AddEvent((walk, instanceEvent));
            // last stat is the current stat, and doesn't count towards statistics
            if (stats.InstanceEvents.Count == segmentStats.SegmentEvents.Count)
            {
                continue;
            }

            if (eventList.Count > 0)
            {
                stats.Count++;
                stats.Instances.Sum++;
            }

            // we only count towards the statistic, those segments that were
            // actually reached
            if (segment!= null)
            {
                stats.Instances.Count++;
            }
            

        }

        return stats;
    }

    public CountStats CalculateCountAggregated(DecisionStats decisionStats, List<RouteWalkEventLog> eventsAtPOI, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        var list = eventsAtPOI.FindAll(e => e.EvenLogType == evenLogType);

        CountStats stats = new() { };

        // go over previous poi events
        foreach ((var walk, var decision) in decisionStats.DecisionEvents)
        {
            var eventList = GetEventsAtPOI(walk, decision, evenLogType);

            StatResults instanceEvent = new() { };
            foreach (var e in eventList)
            {
                instanceEvent.Sum++;
                instanceEvent.Count++;
            }

            stats.AddEvent((walk, instanceEvent));
            // last stat is the current stat, and doesn't count towards statistics
            if (stats.InstanceEvents.Count == decisionStats.DecisionEvents.Count)
            {
                continue;
            }

            if (eventList.Count > 0)
            {
                stats.Count++;
                stats.Instances.Sum++;
            }

            // we only count towards the statistic, those poi that were
            // actually reached
            if (decision!= null)
            {
                stats.Instances.Count++;
            }
            

        }

        return stats;
    }




    public string ConvertPace(double metersPerSecond)
    {

        double? minutesPerKm = ConvertPaceToMmh(metersPerSecond);
        if (minutesPerKm == null)
        {
            return "N/A"; // Handle division by zero (though this scenario shouldn't happen for pace)
        }

        // Format the result nicely
        int minutes = (int)minutesPerKm;
        int seconds = (int)((minutesPerKm - minutes) * 60);

        return $"<size=90%>{minutes}</size><size=50%>Min</size> <size=90%>{seconds}</size><size=50%>Sek</size><br><size=50%>pro km</size>";        
    }

    public double? ConvertPaceToMmh(double metersPerSecond)
    {
        double kmPerHour = metersPerSecond * 3.6; // 1 m/s = 3.6 km/h

        return kmPerHour == 0? null : 60 / kmPerHour;
    }


    /*Data filtering*/

    private List<RouteWalkEventLog> GetEventsAtSegment(RouteWalk walk, RouteWalkEventLog segmentEvent, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        if (segmentEvent == null)
        {
            return new();
        }

        //var walk = WalkSharedData.PreviuosRouteWalks.Find(w => w.Id == segmentEvent.RouteWalkId);
        var list = walk.EventLogList.FindAll(e => e.StartTimestamp > segmentEvent.StartTimestamp &&
                                       e.StartTimestamp < segmentEvent.EndTimestamp &&
                                       e.EvenLogType == evenLogType);
        return list;
    }

    private List<RouteWalkEventLog> GetEventsAtPOI(RouteWalk walk, RouteWalkEventLog poiEvent, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        if (poiEvent == null)
        {
            return new ();
        }

        //var walk = WalkSharedData.PreviuosRouteWalks.Find(w => w.Id == poiEvent.RouteWalkId);
        var list = walk.EventLogList.FindAll(e => e.StartTimestamp > poiEvent.StartTimestamp &&
                                       e.StartTimestamp < poiEvent.EndTimestamp &&
                                       e.EvenLogType == evenLogType);
        return list;
    }

    public List<(RouteWalk, List<RouteWalkEventLog>)> GetPathpointEvents(int poiId, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        List<(RouteWalk, List<RouteWalkEventLog>)> lists = new() { };

        foreach (var walk in WalkSharedData.PreviuosRouteWalks)
        {
            var eventList = walk.EventLogList.FindAll(e => e.TargetPOIId == poiId && e.EvenLogType == evenLogType);
            lists.Add((walk, eventList));
        }

        return lists;
    }


    public List<(RouteWalk, List<RouteWalkEventLog>)> GetSegmentEvents(int startPOIId, RouteWalkEventLogBase.RouteEvenLogType evenLogType)
    {
        List<(RouteWalk, List<RouteWalkEventLog>)> lists = new() { };

        foreach (var walk in WalkSharedData.PreviuosRouteWalks)
        {
            var eventList = walk.EventLogList.FindAll(e => e.SegPOIStartId == startPOIId && e.EvenLogType == evenLogType);
            lists.Add((walk, eventList));
        }

        return lists;
    }



    public RouteWalkEventLog FilterRelevantDecision(List<RouteWalkEventLog> poiEvents)
    {
        RouteWalkEventLog decision = null;

        // If we have many events, we take the incorrect one
        if (poiEvents.Count > 1)
        {
            decision = poiEvents.Find(e => e.IsCorrectDecision == false);
            if (decision == null)
            {
                decision = poiEvents.Find(e => e.IsCorrectDecision == true);
            }
            else
            {
                //TODO: decision was not made. it ended there?
            }
        }
        else if (poiEvents.Count == 1)
        {
            decision = poiEvents[0];
        }

        return decision; // render no data
    }

    public RouteWalkEventLog FilterRelevantSegment(List<RouteWalkEventLog> segEvents, int segPOIEndId)
    {
        RouteWalkEventLog segment = null;

        if (segEvents.Count > 1)
        {
            // normal segment
            segment = segEvents.Find(e => e.SegReachedPOIEndId == segPOIEndId);
            if (segment == null)
            {
                // no loops
                segment = segEvents.Find(e => e.SegReachedPOIEndId != e.SegPOIStartId);
            }

            // if at this point is still null, we just keep it like this
            // we do not consider loops as meaningful way to compare
        }
        else if (segEvents.Count == 1)
        {
            segment = segEvents[0];
        }

        return segment;
    }
}
