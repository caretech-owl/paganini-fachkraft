using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SQLite4Unity3d;

namespace Assets.Scripts
{

	public class Pathpoint
	{
		public int Id { set; get; }
		public int Erw_id { set; get; }
		public double Longitude { set; get; }
		public double Latitude { set; get; }
		public double Altitude { set; get; }
		public double Accuracy { set; get; }
		public int POIType { set; get; }
		public long Timestamp { set; get; }
		public string Description { set; get; }

		public Pathpoint() { }
		
	}
}
