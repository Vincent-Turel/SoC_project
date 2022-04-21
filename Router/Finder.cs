using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ProxyService;
using Router.ProxyService;

namespace Router
{
    internal class Finder
    {
        static readonly HttpClient client = new HttpClient();

        public object ProxyCache { get; private set; }

        public Finder()
        {
        }

        public async Task<string> FindPathway(string start, string end)
        {
            (Coordinate startCoordinate, Coordinate endCoordinate) = await GetCoordinate(start, end);
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
            Data startEnd = JsonSerializer.Deserialize<Data>(await client.GetStringAsync(
                "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                start + "&end=" + end));
            Data startStartStation = JsonSerializer.Deserialize<Data>(await client.GetStringAsync(
                "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                start + "&end=" + end));
            Data startStarStation = JsonSerializer.Deserialize<Data>(await client.GetStringAsync(
                "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                start + "&end=" + end));
        }

        private async Task<Station> GetClosestStation(Coordinate coordinate)
        {
            var point = coordinate.ToGeoCoordinate();
            ProxyClient client = new ProxyClient();
            JCDecauxItem stations = await client.GetAllStationAsync();
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

        private async Task<(Coordinate, Coordinate)> GetCoordinate(string start, string end)
        {
            Data startData = JsonSerializer.Deserialize<Data>(await client.GetStringAsync(
                "https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&text=" +
                start));

            Data endData = JsonSerializer.Deserialize<Data>(await client.GetStringAsync(
                "https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&text=" +
                end));

            return (startData.features[0].geometry[0], endData.features[0].geometry[0]);
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
