using System;
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
        private const string API_KEY = "5b3ce3597851110001cf624813cdd2d934bc4b15add9f71c193a8d60";
        private static readonly ProxyClient proxyClient = new();

        private Finder() {}

        public static Finder GetInstance() => _instance ??= new Finder();


        public async Task<JArray> FindPathway(string start, string end)
        {
            Console.WriteLine("Finding pathway from {0} to {1}", start, end);
            var result = new JArray();
            JObject obj = new JObject {
                { "error", "false" },
                { "textError", "" }
            };

            try
            {
                Console.WriteLine("Retrieving coordinates...");
                var sCoordinate = await GetCoordinate(start);
                var eCoordinate = await GetCoordinate(end); 
                if (sCoordinate == null || eCoordinate == null)
                {
                    obj["error"] = "true";
                    if (sCoordinate == null && eCoordinate == null) 
                        obj["textError"] = "Both start and end location are invalid. Please try something else !";
                    else if (sCoordinate == null)
                        obj["textError"] = "Start location is invalid. Try something else !";
                    else if (eCoordinate == null)
                        obj["textError"] = "End location is invalid. Try something else !";
                } else
                {
                    var startCoordinate = sCoordinate.ToGeoCoordinate();
                    var endCoordinate = eCoordinate.ToGeoCoordinate();
                    Console.WriteLine("Coordination retreived : ");
                    Console.WriteLine("Start : {0}", startCoordinate);
                    Console.WriteLine("End : {0}", endCoordinate);
                    Console.WriteLine("Looking for bike stations...");

                    (Station startStation, JObject startToStationData) = await GetClosestStation(startCoordinate);
                    if (startStation is null)
                    {
                        Console.WriteLine("No stations found, returning walking itinerary.");
                        result.Add(await GetWalkingRoute(startCoordinate, endCoordinate));
                        result.Add(obj);
                        return result;  
                    }

                    (Station endStation, JObject endToStationData) = await GetClosestStation(endCoordinate, false);
                    if (endStation is null || startStation.Equals(endStation))
                    {
                        Console.WriteLine("No stations found, returning walking itinerary.");
                        result.Add(await GetWalkingRoute(startCoordinate, endCoordinate));
                        result.Add(obj);
                        return result;
                    }

                    var ridingData = await GetRidingRoute(startStation.position.ToGeoCoordinate(),
                        endStation.position.ToGeoCoordinate());
                    var fullWalkData = await GetWalkingRoute(startCoordinate, endCoordinate);
                    if (BikeIsUseless(startToStationData, ridingData, endToStationData, fullWalkData))
                    {
                        Console.WriteLine("Bike is useless for this itinerary.");
                        Console.WriteLine("Returning walking itinerary.");
                        result.Add(fullWalkData);
                    }
                    else
                    {
                        Console.WriteLine("Bike is useful for this itinerary.");
                        Console.WriteLine("Returning itinerary using bike.");
                        result.Add(startToStationData);
                        result.Add(ridingData);
                        result.Add(endToStationData);
                    }
                }
                result.Add(obj);
                return result;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("-------------- ERROR : TOO MANY REQUESTS --------------");
                obj["textError"] = "Too many requests have been done on OpenRouteService API. Please try again in a moment !";
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                obj["textError"] = "UNKNOWN ERROR ! Please contact an admin.";
            }
            obj["error"] = "true";
            result.Add(obj);
            return result;

        }

        private bool BikeIsUseless(JObject start, JObject ride, JObject end, JObject full)
        {
            double startTime = start["features"][0]["properties"]["summary"]["duration"].Value<double>();
            double rideTime = ride["features"][0]["properties"]["summary"]["duration"].Value<double>();
            double endTime = end["features"][0]["properties"]["summary"]["duration"].Value<double>();
            double fullTime = full["features"][0]["properties"]["summary"]["duration"].Value<double>();
            Console.WriteLine("Biking length : {0}", startTime + rideTime + endTime);
            Console.WriteLine("Walking length : {0}", fullTime);

            return fullTime < startTime + rideTime + endTime;
        }

        private async Task<(Station, JObject)> GetClosestStation(MyGeoCoordinate coordinate, bool start = true)
        {
            Console.WriteLine("Looking for the closest station from the point : " + coordinate);
            Station[] stations = (await GetAllStationAsync())
                .OrderBy(s => coordinate.GetDistanceTo(s.position.ToGeoCoordinate())).ToArray();

            for (var i = 0; i < stations.Length; i += 5)
            {
                var station = (await Task.WhenAll(stations.Skip(i).Take(5)
                        .Where(s => 
                        {
                            if (s.contractName != "jcdecauxbike")
                            {
                                var station = GetStationAsync(s.contractName + "_" + s.number).Result;
                                return (start
                                ? station.totalStands.availabilities.bikes > 0
                                : station.totalStands.capacity - station.totalStands.availabilities.bikes > 0);
                            }
                            return false;
                            
                        })
                        .Select(async st => (st,
                            await GetWalkingRoute(coordinate, st.position.ToGeoCoordinate())))))
                    .OrderBy(s => s.Item2["features"]![0]![
                        "properties"]!["summary"]!["distance"]!.Value<double>())
                    .FirstOrDefault();
                if (station.st is not null && station.Item2 is not null)
                {
                    Console.WriteLine("Found the best station : {0}_{1}", station.st.contractName, station.st.name);
                    return station;
                }
                Console.WriteLine("None of these 5 stations fit. Looking for others...");
            }

            Console.WriteLine("Did not find any station");
            return (null, null);
        }

        private async Task<Station> GetStationAsync(string id)
        {
            return JsonConvert.DeserializeObject<Station>(await proxyClient.GetStationAsync(id));
        }

        private async Task<Station[]> GetAllStationAsync()
        {
            return JsonConvert.DeserializeObject<Station[]>(await proxyClient.GetAllStationAsync());
        }

        private async Task<JObject> GetWalkingRoute(MyGeoCoordinate coordinate, MyGeoCoordinate stationPos)
        {
            return await GetRoute(coordinate, stationPos, "foot-walking");
        }

        private async Task<JObject> GetRidingRoute(MyGeoCoordinate coordinate, MyGeoCoordinate stationPos)
        {
            return await GetRoute(coordinate, stationPos, "cycling-regular");
        }

        private async Task<JObject> GetRoute(MyGeoCoordinate coordinate, MyGeoCoordinate stationPos,
            string meansOfTransportation)
        {
            var response = await client.GetAsync(
                URL + "v2/directions/" + meansOfTransportation + "?api_key=" + API_KEY + "&start=" + coordinate +
                "&end=" + stationPos);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
        }

        private async Task<Coordinate> GetCoordinate(string location)
        {
            try
            {
                var response = await client.GetAsync(URL + "geocode/search?api_key=" + API_KEY + "&text=" + location);
                response.EnsureSuccessStatusCode();
                return new Coordinate(JsonConvert.DeserializeObject<JObject>(
                    await response.Content.ReadAsStringAsync())["features"][0]["geometry"]!["coordinates"].ToObject<double[]>());
            } catch (ArgumentOutOfRangeException)
            {
                return null;
            } 
        }
    }
}