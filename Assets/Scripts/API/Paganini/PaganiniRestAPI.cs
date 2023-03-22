using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace PaganiniRestAPI
{
    public class Path
    {
        //Base URL for the Rest APi
        //public const string BaseUrl = "https://infinteg-main.fh-bielefeld.de/paganini/api/";
        public const string BaseUrl = "http://192.168.178.22:3000/";
       
       
        public const string Authenticate = BaseUrl + "sw/me/authentification";
        
        public const string SwProfile = BaseUrl + "sw/me/profile";

        public const string SwUsersList = BaseUrl + "sw/users";
        public const string SwUsers = SwUsersList + "/{0}";

        public const string SwWaysList =  SwUsers + "/ways";
        public const string SwWays = BaseUrl + "sw/ways/{0}";

        public const string SwRoutesList = SwWays + "/routes";
        public const string SwRoutes = BaseUrl + "sw/routes/{0}";

        public const string SwPOIList = SwRoutes + "/pois";

        public const string SwPathpointList = SwRoutes + "/pathpoints";
        public const string SwPathpoints = BaseUrl + "sw/pathpoints/{0}";

    }


    public class SocialWorker 
    {

        public static void Authenticate(string username, string password, UnityAction<AuthTokenAPI> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            RESTAPI.Instance.Get<AuthTokenAPI>(Path.Authenticate, successCallback, errorCallback, headers);
        }
    }

    public class User
    {

        public static void GetByID(Int32 id, UnityAction<UserAPI> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }                
            };

            string url = string.Format(Path.SwUsers, id);

            RESTAPI.Instance.Get<UserAPI>(url, successCallback, errorCallback, headers);
        }

        public static void GetAll(UnityAction<UserAPIList> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }
            };

            RESTAPI.Instance.Get<UserAPIList>(Path.SwUsersList, successCallback, errorCallback, headers);
        }

    }

    public class Way
    {

        public static void GetAll(Int32 userId, UnityAction<WayAPIList> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }
            };

            string url = string.Format(Path.SwWaysList, userId);

            RESTAPI.Instance.Get<WayAPIList>(url, successCallback, errorCallback, headers);
        }


        public static void CreateOrUpdate(Int32 userId, WayAPI way, UnityAction<WayAPIResult> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }
            };            

            if (way.IsNew)
            {
                string url = string.Format(Path.SwWaysList, userId);
                RESTAPI.Instance.Post<WayAPIResult>(url, way, successCallback, errorCallback, headers);
            }
            else
            {
                string url = string.Format(Path.SwWays, way.way_id);                
                RESTAPI.Instance.Put<WayAPIResult>(url, (WayAPIUpdate)way, successCallback, errorCallback, headers);
            }
        }

    }

    public class Route
    {


        public static void CreateOrUpdate(Int32 wayId, RouteAPI route, UnityAction<RouteAPIResult> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }
            };

            if (route.IsNew)
            {
                string url = string.Format(Path.SwRoutesList, wayId);
                RESTAPI.Instance.Post<RouteAPIResult>(url, route, successCallback, errorCallback, headers);
            }
            else
            {
                string url = string.Format(Path.SwRoutes, route.erw_id);
                RESTAPI.Instance.Put<RouteAPIResult>(url, route, successCallback, errorCallback, headers);
            }
        }

    }


    public class Pathpoint
    {
        public static void BatchCreate(Int32 routeId, PathpointAPIBatch batch, UnityAction<PathpointAPIList> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }
            };


            string url = string.Format(Path.SwPathpointList, routeId);
            RESTAPI.Instance.Post<PathpointAPIList>(url, batch, successCallback, errorCallback, headers);
        }

        public static void BatchUpdate(Int32 routeId, PathpointAPIBatch batch, UnityAction<PathpointAPIList> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }
            };


            string url = string.Format(Path.SwPathpointList, routeId);
            RESTAPI.Instance.Put<PathpointAPIList>(url, batch, successCallback, errorCallback, headers);
        }

    }


    public class PathpointPOI
    {
        public static void BatchCreate(Int32 routeId, PathpointPOIAPIBatch batch, UnityAction<PathpointPOIAPIList> successCallback, UnityAction<string> errorCallback)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "apitoken", AppState.APIToken }
            };

            // batch will need to have the list of byte[] files, with reference to the pathpoints (photo with the filename?)

            string url = string.Format(Path.SwPOIList, routeId);
            RESTAPI.Instance.PostMultipart<PathpointPOIAPIList>(url, batch, batch.files, successCallback, errorCallback, headers);

        }
    }



}