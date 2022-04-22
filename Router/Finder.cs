using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Router.ProxyService;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Router
{
    internal class Finder
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly ProxyClient proxyClient = new ProxyClient();
        
        public Finder()
        {
        }

        public async Task<string> FindPathway(string start, string end)
        {
            Coordinate startCoordinate = await GetCoordinate(start);
            Coordinate endCoordinate = await GetCoordinate(end);
            Station startStation = await GetClosestStation(startCoordinate);
            Station endStation = await GetClosestStation(endCoordinate);
            
            if (BikeIsUseful(startCoordinate, endCoordinate, startStation, endStation))
            {

            }
            else
            {

            }
        }

        private async Task<bool> BikeIsUseful(Coordinate start, Coordinate end, Station startStation, Station endStation)
        {
            Data startEndStation = JsonConvert.DeserializeObject<JObject>(await client.GetStringAsync(
                "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                start + "&end=" + end));
            Data startEnd = JsonSerializer.Deserialize<Data>(await client.GetStringAsync(
                "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                start + "&end=" + end));
            
        }

        private async Task<Station> GetClosestStation(Coordinate coordinate)
        {
            var point = coordinate.ToGeoCoordinate();
            
            Station[] stations = (await proxyClient.GetAllStationAsync())._stations;
            
            var sortedStations = stations.OrderBy(s => coordinate.ToGeoCoordinate().GetDistanceTo(s.position
            ))
            Station? chosenOne = null;
            double bestDistance = Double.PositiveInfinity;
            foreach (var station in stations._stations)
            {
                double current = new GeoCoordinate(station.position.latitude, station.position.longitude).GetDistanceTo(point);
                if (current < bestDistance)
                {
                    bestDistance = current;
                    chosenOne = station;
                }
            }

            return chosenOne;
        }

        private async Task<Coordinate> GetCoordinate(string location)
        {
            return (JsonConvert.DeserializeObject<JObject>(await client.GetStringAsync(
                "https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&text=" +
                location))?["features"]?[0]?["geometry"]?["coordinates"] ?? throw new InvalidOperationException()).Value<Coordinate>();
        }

        public Way GetRoute(GeoCoordinate start, GeoCoordinate end, bool isOnBike)
        {
            string adresse =
                "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                (start.Longitude + "").Replace(',', '.') + "," + (start.Latitude + "").Replace(',', '.') + "&end=" +
                (end.Longitude + "").Replace(',', '.') + "," + (end.Latitude + "").Replace(',', '.');
            if (isOnBike)
            {
                adresse =
                    "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                    (start.Longitude + "").Replace(',', '.') + "," + (start.Latitude + "").Replace(',', '.') + "&end=" +
                    (end.Longitude + "").Replace(',', '.') + "," + (end.Latitude + "").Replace(',', '.');
            }

            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return new Way(resp.features[0].properties, resp.features[0].geometry);
        }
    }
}
