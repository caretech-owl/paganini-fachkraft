using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InternalDataModel
{
    public bool isLoggedIn { get; set; }

    public string videoFileName { get; set; }

    public List<TimeMarkerObject> timeMarkerObjects { get; set; }
}

