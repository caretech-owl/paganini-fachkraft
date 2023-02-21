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
        public const string BaseUrl = "https://infinteg-main.fh-bielefeld.de/paganini/api/";
       
        public const string Authenticate = BaseUrl + "sw/me/authentification";
        
        public const string SwProfile = BaseUrl + "sw/me/profile";

        public const string SwUsersList = BaseUrl + "sw/users";
        public const string SwUsers = SwUsersList + "/{0}";

        public const string SwWaysList =  SwUsers + "/ways";
        public const string SwWays = BaseUrl + "sw/ways/{1}";

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

    }

}