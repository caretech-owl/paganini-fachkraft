using SQLite4Unity3d;
using System.Linq;
using System.Collections.Generic;
using System;
//using SQLiteNetExtensions.Attributes;

public class PathpointPhoto : BaseModel
{
    [PrimaryKey]
    public int Id { set; get; }
    public int PathpointId { set; get; }
    public string Description { set; get; }
    public PhotoFeedback Feedback { set; get; }

    //[Ignore]
    //public string PhotoFilename { set; get; }

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


    public PathpointPhotoAPI ToAPI()
    {

        PathpointPhotoAPI photo;
        // For an update statement
        if (FromAPI)
        {
            photo = new PathpointPhotoAPIUpdate();
            photo.pphoto_id = Id;
            photo.IsNew = false;
        }
        // For a post statement
        else
        {
            photo = new PathpointPhotoAPI();
            photo.IsNew = true;
        }


        photo.pphoto_description = Description;
        //photo.photo = Convert.ToBase64String(Photo);


        return photo;

    }
}
