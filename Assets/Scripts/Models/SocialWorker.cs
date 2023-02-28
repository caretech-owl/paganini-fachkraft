using SQLite4Unity3d;

public class SocialWorker : BaseModel
{
    [PrimaryKey]
    public int Id { set; get; }
    public string Username { set; get; }
    public string Firstname { set; get; }
    public string Surname { set; get; }
    public string Photo { set; get; }
    public int WorkshopId { set; get; }
}
