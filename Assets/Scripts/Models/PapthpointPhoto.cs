using SQLite4Unity3d;
using System.Linq;
using System.Collections.Generic;
//using SQLiteNetExtensions.Attributes;

public class PathpointPhoto : BaseModel
{
    [PrimaryKey]
    public int Id { set; get; }
    public int PathpointId { set; get; }
    public string Description { set; get; }
    public PhotoFeedback Feedback { set; get; }

    public byte[] Photo { set; get; }

    public enum PhotoFeedback
    {
        None = 0,
        Delete = 1,
        Keep = 2,
    }

    public PathpointPhoto() { }


    public static List<PathpointPhoto> GetPathpointPhotoListByPOI(int pathpointId)
    {
        List<PathpointPhoto> photos;

        var conn = DBConnector.Instance.GetConnection();

        // Query all Ways and their related Routes using sqlite-net's built-in mapping functionality

        photos = conn.Table<PathpointPhoto>().Where(p => p.PathpointId == pathpointId).ToList();

        return photos;
    }
}
