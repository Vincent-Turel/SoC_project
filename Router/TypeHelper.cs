using System.Device.Location;

namespace Router
{
    public class Data
    {
        public Feature[] features { get; set; }
    }

    public class Feature
    {
        public Coordinate geometry { get; set; }
        public Property properties { get; set; }
    }

    public class Property
    {
        public Summary summary { get; set; }
    }

    public class Summary
    {
        public double distance { get; set; }
        public int duration { get; set; }
    }
    public class Coordinate
    {
        public double[] coordinates { get; set; }

        private GeoCoordinate _geoCoordinate;

        public GeoCoordinate ToGeoCoordinate()
        {
            return _geoCoordinate ??= new GeoCoordinate(coordinates[0], coordinates[1]);
        }

        public override string ToString()
        {
            return (coordinates[0] + "").Replace(',', '.') + "," + (coordinates[1] + "").Replace(',', '.');
        }
    }
}