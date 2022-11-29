using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaganiniRestAPI
{
    //Base URL for the Rest APi
    public const string serverURL = "https://infinteg-main.fh-bielefeld.de/paganini/api/sw/";

    //getAuthToken  param: apitoken: ""
    public const string getAuthToken = serverURL + "me/authentification";
    //getUserProfile  param: apitoken: ""
    public const string getUserProfile = serverURL + "me/profile";

}
