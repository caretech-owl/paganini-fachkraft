using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

public class Pathpoint : BaseModel
{
    [PrimaryKey]
    public int Id { set; get; }
	public int RouteId { set; get; }
	public double Longitude { set; get; }
	public double Latitude { set; get; }
	public double Altitude { set; get; }
	public double Accuracy { set; get; }
	public int POIType { set; get; }
	public long Timestamp { set; get; }
	public string Description { set; get; }
    public string PhotoFilename { set; get; }

    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<PathpointPhoto> Photos { get; set; }

    public Pathpoint() { }
		
}
