using System.Collections.Generic;
using System.Linq;
using SQLite4Unity3d;
//using SQLiteNetExtensions.Attributes;
//using SQLiteNetExtensions.Extensions;

public class Pathpoint : BaseModel
{
    [PrimaryKey]
    public int Id { set; get; }
	// [Indexed]
	public int RouteId { set; get; }
	public double Longitude { set; get; }
	public double Latitude { set; get; }
	public double Altitude { set; get; }
	public double Accuracy { set; get; }
	public int POIType { set; get; }
	public long Timestamp { set; get; }
	public string Description { set; get; }
    public string PhotoFilename { set; get; }



	//[OneToMany(CascadeOperations = CascadeOperation.All)]
	[Ignore]
    public List<PathpointPhoto> Photos { get; set; }

    public enum POIsType
    {
        Point = -1,
        Reassurance = 1,
        Landmark = 2
    }

    public Pathpoint() { }

    public static List<Pathpoint> GetPathpointListByRoute(int routeId)
    {
        List<Pathpoint> pathpoints;

        var conn = DBConnector.Instance.GetConnection();

        // Query all Ways and their related Routes using sqlite-net's built-in mapping functionality

        pathpoints = conn.Table<Pathpoint>().Where(p => p.RouteId == routeId)
                                            .OrderBy(p => p.Timestamp).ToList();

        return pathpoints;
    }

}
