using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InternalDataModel
{
    /// <summary>
    /// Local data model to handle user settings 
    /// and data of the exploritory route walks
    /// </summary>
  

    /// USER AND APPLICATION SETTINGS
    public bool isLoginRemembered { get; set; }
    public string lastSavedDate { get; set; }

    public string videoFileName { get; set; }

    public List<DataOfExploritoryRouteWalks> exploritoryRouteWalks { get; set; }

    /// CLASSES FOR DATA OF THE EXPLORITORY ROUTE WALKS
 
    public class DataOfExploritoryRouteWalks
    {
        public string Folder { get; set; }
        public List<string> Videos { get; set; }
        public List<string> Photos { get; set; }
        public List<PathpointAPI> Pathpoints { get; set; }
        public int Id { set; get; }
        public string Start { set; get; }
        public string Destination { set; get; }
        public string StartType { set; get; }
        public string DestinationType { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public int Status { set; get; }
    }
}

