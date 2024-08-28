using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using SQLite4Unity3d;
using Unity.VisualScripting;
using static StatCompute;


public class PathpointPIM : BaseModel<PathpointPIM>
{

    [PrimaryKey]
    public int Id { set; get; }
    public System.DateTime ActiveSinceDateTime { set; get; }
    public bool IsAtPOINewToUser { set; get; }
    public bool IsToPOINewToUser { set; get; }
    public int PathpointId { set; get; }     
    public SupportMode AtPOIMode { set; get; }
    public SupportMode ToPOIMode { set; get; }
    public int RouteId { set; get; }


    [System.Serializable]
    public enum SupportMode
    {
        None = 0,
        Instruction = 1,
        Trivia = 2,
        Challenge = 3,
        Mute = 4
    }


    public PathpointPIM() { }

    public PathpointPIM(IPathpointPIMAPI instructionModeAPI)
    {
        Id = instructionModeAPI.pim_id;
        ActiveSinceDateTime = (DateTime)DateUtils.ConvertUTCStringToUTCDate(instructionModeAPI.pim_timestamp, "yyyy-MM-dd'T'HH:mm:ss"); 
        IsAtPOINewToUser = instructionModeAPI.pim_atpoi_isnew;
        IsToPOINewToUser = instructionModeAPI.pim_topoi_isnew;
        PathpointId = instructionModeAPI.ppoint_id;
        AtPOIMode = (SupportMode)instructionModeAPI.pim_atpoi_mode;
        ToPOIMode = (SupportMode)instructionModeAPI.pim_topoi_mode;
    }


    public PathpointPIMAPI ToAPI()
    {
        PathpointPIMAPI pimAPI = new ();

        pimAPI.pim_atpoi_isnew = IsAtPOINewToUser;
        pimAPI.pim_topoi_isnew = IsToPOINewToUser;
        pimAPI.ppoint_id = PathpointId;
        pimAPI.pim_atpoi_mode = (int)AtPOIMode;
        pimAPI.pim_topoi_mode = (int)ToPOIMode;

        pimAPI.pim_timestamp = DateUtils.ConvertUTCDateToUTCString(ActiveSinceDateTime, "yyyy-MM-dd'T'HH:mm:ss");

        return pimAPI;
    }


    public static List<PathpointPIM> GetPIMListByRoute(int routeId, Func<PathpointPIM, bool> whereCondition = null)
    {
        List<PathpointPIM> pimList;

        var conn = DBConnector.Instance.GetConnection();

        // Query all Ways and their related Routes using sqlite-net's built-in mapping functionality

        var pathpointQuery = conn.Table<PathpointPIM>().Where(p => p.RouteId == routeId).AsEnumerable();

        if (whereCondition != null)
        {
            pathpointQuery = pathpointQuery.Where(whereCondition);
        }

        pimList = pathpointQuery.ToList();

        return pimList;
    }

    public static void DeleteFromRoute(int routeId, bool[] fromAPI)
    {
        // Open the SQLliteConnection
        var conn = DBConnector.Instance.GetConnection();

        // Prepare the base DELETE command text
        string cmdText = "DELETE FROM PathpointPIM WHERE RouteId = ?";

        List<object> parameters = new List<object> { routeId };

        // Add conditions for FromAPI
        if (fromAPI != null && fromAPI.Length > 0)
        {
            var fromAPIConditions = string.Join(" OR ", fromAPI.Select((val, idx) => $"FromAPI = ?"));
            cmdText += $" AND ({fromAPIConditions})";
            parameters.AddRange(fromAPI);
        }
        

        // Prepare the SQLiteCommand with the command text and parameters
        SQLiteCommand cmd = conn.CreateCommand(cmdText, parameters.ToArray());

        // Execute the command
        cmd.ExecuteNonQuery();
    }

}