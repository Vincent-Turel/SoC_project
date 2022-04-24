using System.Device.Location;
using System.Runtime.Serialization;

namespace Router
{
    [DataContract]
    public class Station
    {
        [DataMember] public int number { get; set; }
        [DataMember] public string contractName { get; set; }
        [DataMember] public string name { get; set; }
        [DataMember] public string address { get; set; }
        [DataMember] public Position position { get; set; }
        [DataMember] public bool banking { get; set; }
        [DataMember] public bool bonus { get; set; }
        [DataMember] public string status { get; set; }
        [DataMember] public string lastUpdate { get; set; }
        [DataMember] public bool connected { get; set; }
        [DataMember] public Stands totalStands { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || GetType() != obj.GetType()) return false;
            var sta = (Station) obj;
            return number == sta.number && contractName == sta.contractName;
        }
    }

    [DataContract]
    public class Position
    {
        [DataMember] public double latitude { get; set; }
        [DataMember] public double longitude { get; set; }

        private GeoCoordinate _geoCoordinate;

        public GeoCoordinate ToGeoCoordinate()
        {
            return _geoCoordinate ??= new GeoCoordinate(latitude, longitude);
        }

        public override string ToString()
        {
            return (latitude + "").Replace(',', '.') + "," + (longitude + "").Replace(',', '.');
        }
    }

    [DataContract]
    public class Stands
    {
        [DataMember] public Availability availabilities { get; set; }
        [DataMember] public int capacity { get; set; }
    }

    [DataContract]
    public class Availability
    {
        [DataMember] public int bikes { get; set; }
        [DataMember] public int stands { get; set; }
        [DataMember] public int mechanicalBikes { get; set; }
        [DataMember] public int electricalBikes { get; set; }
    }
}