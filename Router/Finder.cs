using System;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Router.ProxyService;

namespace Router
{
    internal class Finder
    {
        private static Finder _instance;
        private static readonly HttpClient client = new();
        private const string URL = "https://api.openrouteservice.org/";
        private const string API_KEY = "5b3ce3597851110001cf624861912097812a436daaae0aca02220957";
        private static readonly ProxyClient proxyClient = new();

        private Finder()
        {
        }

        public static Finder GetInstance() => _instance ??= new Finder();


        public async Task<string> FindPathway(string start, string end)
        {
            var startCoordinate = (await GetCoordinate(start)).ToGeoCoordinate();
            var endCoordinate = (await GetCoordinate(end)).ToGeoCoordinate();

            (Station startStation, JObject startToStationData) = await GetClosestStation(startCoordinate);
            if (startStation is null)
            {
                return new JArray(await GetWalkingRoute(startCoordinate, endCoordinate)).ToString();
            }

            (Station endStation, JObject endToStationData) = await GetClosestStation(endCoordinate, false);
            if (endStation is null || startStation.Equals(endStation))
            {
                return new JArray(await GetWalkingRoute(startCoordinate, endCoordinate)).ToString();
            }

            var ridingData = await GetRidingRoute(startStation.position.ToGeoCoordinate(),
                endStation.position.ToGeoCoordinate());
            var fullWalkData = await GetWalkingRoute(startCoordinate, endCoordinate);

            return BikeIsUseless(startToStationData, ridingData, endToStationData, fullWalkData)
                ? new JArray(fullWalkData).ToString()
                : new JArray {startToStationData, ridingData, endToStationData}.ToString();
        }

        private bool BikeIsUseless(JObject start, JObject ride, JObject end, JObject full)
        {
            double startTime = start["features"][0]["properties"]["summary"]["duration"].Value<double>();
            double rideTime = ride["features"][0]["properties"]["summary"]["duration"].Value<double>();
            double endTime = end["features"][0]["properties"]["summary"]["duration"].Value<double>();
            double fullTime = full["features"][0]["properties"]["summary"]["duration"].Value<double>();

            return fullTime < startTime + rideTime + endTime;
        }

        private async Task<(Station, JObject)> GetClosestStation(GeoCoordinate coordinate, bool start = true)
        {
            Console.WriteLine("Looking for the closest station from the point : " + coordinate);
            Station[] stations = JsonConvert.DeserializeObject<Station[]>(await proxyClient.GetAllStationAsync())!
                .OrderBy(s => coordinate.GetDistanceTo(s.position.ToGeoCoordinate())).ToArray();

            for (var i = 0; i < stations.Length; i += 5)
            {
                var station = (await Task.WhenAll(stations.Skip(i).Take(5)
                    .Where(s =>
                    {
                        var station = GetStationAsync(s.contractName + "_" + s.number).Result;
                        return start
                            ? station.totalStands.availabilities.bikes > 0
                            : station.totalStands.capacity - station.totalStands.availabilities.bikes > 0;
                    })
                    .Select(async st => (st, 
                        await GetWalkingRoute(coordinate, st.position.ToGeoCoordinate())))))
                    .OrderBy(s => s.Item2["features"]![0]![
                        "properties"]!["summary"]!["distance"]!.Value<double>())
                    .FirstOrDefault(null!);
                if (station.st is not null && station.Item2 is not null)
                    return station;
            }

            Console.WriteLine("Did not found any stations");
            return (null, null);
        }

        private async Task<Station> GetStationAsync(string id)
        {
            return JsonConvert.DeserializeObject<Station>(await proxyClient.GetStationAsync(id));
        }

        private async Task<JObject> GetWalkingRoute(GeoCoordinate coordinate, GeoCoordinate stationPos)
        {
            return await GetRoute(coordinate, stationPos, "foot-walking");
        }

        private async Task<JObject> GetRidingRoute(GeoCoordinate coordinate, GeoCoordinate stationPos)
        {
            return await GetRoute(coordinate, stationPos, "cycling-regular");
        }

        private async Task<JObject> GetRoute(GeoCoordinate coordinate, GeoCoordinate stationPos,
            string meansOfTransportation)
        {
            return JsonConvert.DeserializeObject<JObject>(await client.GetStringAsync(
                URL + "v2/directions/" + meansOfTransportation + "?api_key=" + API_KEY + "&start=" + coordinate +
                "&end=" + stationPos));
        }

        private async Task<Coordinate> GetCoordinate(string location)
        {
            return (JsonConvert.DeserializeObject<JObject>(await client.GetStringAsync(
                    "https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&text=" +
                    location))!["features"]![0]!["geometry"]!["coordinates"] ?? throw new InvalidOperationException())
                .Value<Coordinate>();
        }
    }
}