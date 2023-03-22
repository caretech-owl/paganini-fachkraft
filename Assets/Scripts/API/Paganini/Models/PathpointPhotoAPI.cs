using System;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class PathpointPhotoAPI : BaseAPI
{
    [JsonIgnore]
    public int pphoto_id;

    [JsonIgnore]
    public int ppoint_id;

	public string pphoto_description;

    public string photo;

    public string photo_reference;
}



[System.Serializable]
public class PathpointPhotoAPIResult : PathpointPhotoAPI
{
    [JsonProperty]
    public int pphoto_id;

    [JsonProperty]
    public int ppoint_id;

    [NonSerialized]
    public string photo_reference;
}

[System.Serializable]
public class PathpointPhotoAPIUpdate : PathpointPhotoAPI
{
    [JsonProperty]
    public int pphoto_id;

}


public class PathpointPhotoAPIList
{
    public PathpointPhotoAPIResult[] erw;
}


