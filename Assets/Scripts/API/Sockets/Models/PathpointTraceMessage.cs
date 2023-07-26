using System;
using Newtonsoft.Json;
using UnityEngine;


public class PathpointTraceMessage : BaseMessage
{
    public int seq;
    public PathpointAPI pathpoint;
    public string eventType;
}
