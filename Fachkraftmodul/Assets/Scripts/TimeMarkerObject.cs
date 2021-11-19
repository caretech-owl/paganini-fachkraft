using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMarkerObject : MonoBehaviour
{
    public Vector2 LatLng { get; set; }
    public double Timestamp { get; set; }
    public OnlineMapsMarker Marker { get; set; }

    public TimeMarkerObject()
    {

    }
}
