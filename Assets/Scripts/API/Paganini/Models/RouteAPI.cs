using System;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class RouteAPI : BaseAPI
{
    [JsonIgnore]
    public int erw_id;

    [JsonIgnore]
    public int way_id;

    public string erw_name;
	public string erw_date;
	public int erw_pin;
    public RouteStatusAPI status;
}

[System.Serializable]
public class RouteAPIResult : RouteAPI
{
    [JsonProperty]
    public int erw_id;

    [JsonProperty]
    public int way_id;
}

public class RouteAPIList
{
    public RouteAPIResult[] erw;
}


[System.Serializable]
public class RouteStatusAPI
{
    public int erw_status_id;
    public string erw_status_name;
}