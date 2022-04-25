using System.Device.Location;

namespace Router;

public class MyGeoCoordinate : GeoCoordinate
{
    public MyGeoCoordinate(double latitude, double longitude) : base(latitude, longitude)
    {
    }

    public override string ToString()
    {
        return base.Longitude.ToString().Replace(',', '.') + "," + base.Latitude.ToString().Replace(',', '.');
    }
}

public class Coordinate
{
    public double[] coordinates { get; }

    private MyGeoCoordinate _geoCoordinate;

    public Coordinate(double[] values)
    {
        coordinates = values;
    }

    public MyGeoCoordinate ToGeoCoordinate()
    {
        return _geoCoordinate ??= new MyGeoCoordinate(coordinates[1], coordinates[0]);
    }
}

public class Position
{
    public double latitude { get; set; }
    public double longitude { get; set; }

    private MyGeoCoordinate _geoCoordinate;

    public MyGeoCoordinate ToGeoCoordinate()
    {
        return _geoCoordinate ??= new MyGeoCoordinate(latitude, longitude);
    }

    public override string ToString()
    {
        return (latitude + "").Replace(',', '.') + "," + (longitude + "").Replace(',', '.');
    }
}