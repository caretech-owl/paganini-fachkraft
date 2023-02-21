using SQLite;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

public class PathpointPhoto : BaseModel
{
    [PrimaryKey]
    public int Id { set; get; }
    public int PathpointId { set; get; }
    public string Description { set; get; }

    public byte[] Photo { set; get; }


    public PathpointPhoto() { }
}
