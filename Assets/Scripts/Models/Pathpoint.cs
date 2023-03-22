using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using SQLite4Unity3d;
//using SQLiteNetExtensions.Attributes;
//using SQLiteNetExtensions.Extensions;

public class Pathpoint : BaseModel
{
    [PrimaryKey]
    public int Id { set; get; }
	public int RouteId { set; get; }
	public double Longitude { set; get; }
	public double Latitude { set; get; }
	public double Altitude { set; get; }
	public double Accuracy { set; get; }
	public POIsType POIType { set; get; }
	public long Timestamp { set; get; }
	public string Description { set; get; }
    public string PhotoFilename { set; get; }



	//[OneToMany(CascadeOperations = CascadeOperation.All)]
	[Ignore]
    public List<PathpointPhoto> Photos { get; set; }
    [Ignore]
    public List<string> PhotoFilenames { get; set; }


    public enum POIsType
    {
        [XmlEnum("-1")]
        Point = -1,
        [XmlEnum("1")]
        Reassurance = 1,
        [XmlEnum("2")]
        Landmark = 2
    }

    public Pathpoint() { }

    public Pathpoint(PathpointAPIResult pathpoint) {

        Id = pathpoint.ppoint_id;
        RouteId = pathpoint.erw_id;
        Longitude = pathpoint.ppoint_lon;
        Latitude = pathpoint.ppoint_lat;
        Altitude = pathpoint.ppoint_altitude;
        Accuracy = pathpoint.ppoint_accuracy;
        POIType = (POIsType) pathpoint.ppoint_poitype;
        Description = pathpoint.ppoint_description;

        var ts = DateTime.ParseExact(pathpoint.ppoint_timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        Timestamp = new DateTimeOffset(ts).ToUnixTimeMilliseconds();
    }

    public static List<Pathpoint> GetPathpointListByRoute(int routeId, Func<Pathpoint, bool> whereCondition = null)
    {
        List<Pathpoint> pathpoints;

        var conn = DBConnector.Instance.GetConnection();

        // Query all Ways and their related Routes using sqlite-net's built-in mapping functionality

        var pathpointQuery = conn.Table<Pathpoint>().Where(p => p.RouteId == routeId).AsEnumerable();

        if (whereCondition != null)
        {
            pathpointQuery = pathpointQuery.Where(whereCondition);
        }

        pathpoints = pathpointQuery.OrderBy(p => p.Timestamp).ToList();

        return pathpoints;
    }

    public static void ChangeParent(int olRouteId, int newRouteId)
    {
        // Open the SQLliteConnection
        var conn = DBConnector.Instance.GetConnection();

        // Create the SQL command with the update query and parameters
        string cmdText = "UPDATE Pathpoint SET RouteId = ? WHERE RouteId = ?";
        SQLiteCommand cmd = conn.CreateCommand(cmdText, newRouteId, olRouteId);

        // Execute the command
        cmd.ExecuteNonQuery();

    }



    public IPathpointAPI ToAPI()
    {
        IPathpointAPI pp;
        // For an update statement
        if (FromAPI)
        {
            pp = new PathpointAPIUpdate();
            pp.ppoint_id = Id;
            pp.IsNew = false;
        }
        // For a post statement
        else
        {
            pp = new PathpointAPI();
            pp.IsNew = true;
        }


        pp.ppoint_lon = Longitude;
        pp.ppoint_lat = Latitude;
        pp.ppoint_altitude = Altitude;
        pp.ppoint_accuracy = Accuracy;
        pp.ppoint_poitype = (int)POIType;
        pp.ppoint_timestamp = DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        pp.ppoint_description = Description;



        return pp;
    }



}
