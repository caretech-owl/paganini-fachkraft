﻿[System.Serializable]
public class WayAPI
{
	public int way_id;
	public AddressAPI way_start;
	public AddressAPI way_destination;
	public string way_name;
	public string way_description;

	public RouteAPI [] routes;
}

public class WayAPIList
{
	public WayAPI[] ways;
}