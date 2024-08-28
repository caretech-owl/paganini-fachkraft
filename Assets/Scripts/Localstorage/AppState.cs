using System.Collections;
using System.Collections.Generic;
using NinevaStudios.GoogleMaps;
using UnityEngine;

public class AppState 
{

    public static string APIToken = null;
    public static SocialWorker CurrenSocialWorker = null;

    public static int ScreenSleepTimeout = Screen.sleepTimeout;

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
        CurrenSocialWorker = null;

        ScreenSleepTimeout = Screen.sleepTimeout;

        CurrentUser = null;
        CurrentRoute = null;
        CurrentWay = null;
        CurrentRouteWalk = null;

        MonitoringView.ShowPracticeModeInTimeline = true;
    }

}
