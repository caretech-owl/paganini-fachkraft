using System.Collections;
using System.Collections.Generic;
using NinevaStudios.GoogleMaps;
using UnityEngine;

public class AppState 
{

    public static string APIToken = null;
    public static Observable<SocialWorker> CurrentSocialWorker = new Observable<SocialWorker>();

    public static int ScreenSleepTimeout = Screen.sleepTimeout;

    public static string CurrentMenuOption = null;
    public static string CurrentDeepLink = null;
    public static User CurrentUser = null;
    public static Route CurrentRoute = null;
    public static Way CurrentWay = null;
    public static RouteWalk CurrentRouteWalk = null;

    public static GoogleMapType DefaultMapType = GoogleMapType.Normal;

    public class MonitoringView
    {
        public static bool ShowPracticeModeInTimeline = true;
        public static bool UpdatedModeInTimeline = false;
        public static Pathpoint UpdatedModePOI = null;
    }    

    public static void ResetValues()
    {
        APIToken = null;
        CurrentSocialWorker.Reset();

        ScreenSleepTimeout = Screen.sleepTimeout;

        CurrentUser = null;
        CurrentRoute = null;
        CurrentWay = null;
        CurrentRouteWalk = null;

        MonitoringView.ShowPracticeModeInTimeline = true;
    }

}
