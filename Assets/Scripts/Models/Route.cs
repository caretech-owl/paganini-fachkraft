using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;


public class Route : BaseModel
{

    [PrimaryKey]
    public int Id { set; get; }
    public string Name { set; get; }
    public System.DateTime Date { set; get; }
    public int Pin { set; get; }
    public int Status { set; get; }

    [Indexed]
    public int WayId { get; set; }

    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<Pathpoint> Pathpoints { get; set; }

    public override string ToString()
    {
        return string.Format("[exploratory_route_walk: erw_id={0}, way_id={1}, erw_name={2},  erw_datum={3}, erw_pin={4}, erw_status=(5)]", Id, WayId, Name, Date, Pin, Status);
    }

    public enum RouteStatus
    {
        New,
        DraftPrepared,
        DraftNegotiated,
        Training,
        Completed,
        Discarded
    }

    public Route() { }
    public Route(RouteAPI erw)
    {
        this.Id = erw.erw_id;
        this.WayId = erw.way_id;
        this.Name = erw.erw_name;
        this.Date = System.DateTime.Parse(erw.erw_date);
        this.Pin = erw.erw_pin;
        this.Status = (int)Way.WayStatus.FromAPI;
    }

    public RouteAPI ToAPI()
    {
        RouteAPI erw = new RouteAPI
        {
            erw_id = this.Id,
            way_id = this.WayId,
            erw_name = this.Name,
            erw_date = this.Date.Year + "-" + this.Date.Month + "-" + this.Date.Day,
            erw_pin = this.Pin,
        };
        return erw;
    }
}