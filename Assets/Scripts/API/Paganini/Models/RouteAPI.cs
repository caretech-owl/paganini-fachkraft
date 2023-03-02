[System.Serializable]
public class RouteAPI
{
	public int erw_id;
	public int way_id;
    public string erw_name;
	public string erw_date;
	public int erw_pin;
    public RouteStatusAPI status;
}

[System.Serializable]
public class RouteStatusAPI
{
    public int erw_status_id;
    public int erw_status_name;
}

public class RouteAPIList
{
    public RouteAPI[] erw;
}