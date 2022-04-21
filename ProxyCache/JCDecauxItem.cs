using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;


namespace ProxyCache
{
    [DataContract]
    public class JCDecauxItem
    {
        static readonly HttpClient client = new HttpClient();

        [DataMember]
        Station[] _stations { get; set; }

        public JCDecauxItem(string cacheItemName)
        {
            if (cacheItemName == "all")
            {
                string responseBody = client.GetStringAsync("https://api.jcdecaux.com/vls/v3/stations?apiKey=7109d50b09d48cbb216a7365c27620f25bea3d3c").Result;
                Station[] list = JsonSerializer.Deserialize<Station[]>(responseBody);
                _stations = list;
            }
            else
            {
                string[] val = cacheItemName.Split('_');
                string responseBody = client.GetStringAsync("https://api.jcdecaux.com/vls/v3/stations/" + val[0] + "?contract=" + val[1] + "&apiKey=7109d50b09d48cbb216a7365c27620f25bea3d3c").Result;
                Station station  = JsonSerializer.Deserialize<Station>(responseBody);
                _stations = new []{station};   
            }
        }
    }
    
    [DataContract]
    public class Contract
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string commercial_name { get; set; }
        [DataMember]
        public List<string> cities { get; set; }
        [DataMember]
        public string country_code { get; set; }

        public override String ToString()
        {
            if (cities == null)
            {
                return "Name : " + name + " Commercial : " + commercial_name + " Country code : " + country_code;
            }
            return "Name : " + name + ", Commercial : " + commercial_name + ", Country code : " + country_code + ", Cities : " + String.Join(";", cities) + "\n";
        }
    }

    [DataContract]

    public class Station
    {
        [DataMember]
        public int number { get; set; }
        [DataMember]
        public string contractName { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public Position position { get; set; }
        [DataMember]
        public bool banking { get; set; }
        [DataMember]
        public bool bonus { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string lastUpdate { get; set; }
        [DataMember]
        public bool connected { get; set; }
        [DataMember]
        public bool overflow { get; set; }
        [DataMember]
        public string shape { get; set; }

        [DataMember]
        public Stands totalStands { get; set; }
        [DataMember]
        public Stands mainStands { get; set; }
        [DataMember]
        public object? overflowStands { get; set; }

        public override string ToString()
        {
            string text = "number :" + number +
                     "\ncontractName : " + contractName +
                     "\nname : " + name +
                     "\naddress : " + address +
                     position.ToString() +
                     "banking : " + banking +
                     "\nbonus : " + bonus +
                     "\nstatus : " + status +
                     "\nlastUpdate : " + lastUpdate +
                     "\nconnected : " + connected +
                     "\noverflow : " + overflow +
                     "\nshape : " + shape + "\n";
            if (totalStands != null)
            {
                text += totalStands.ToString();
            }
            if (mainStands != null)
            {
                text += mainStands.ToString();
            }
            if (overflowStands != null)
            {
                text += overflowStands;
            }
            return text;
        }
    }

    [DataContract]

    public class Position
    {
        [DataMember]
        public double latitude { get; set; }
        [DataMember]
        public double longitude { get; set; }

        public override string ToString()
        {
            return "position : \n\tlatitude : " + latitude + "\n\tlongitude : " + longitude + "\n";
        }
    }

    [DataContract]

    public class Stands
    {
        [DataMember]
        public Availibilities availabilities { get; set; }
        [DataMember]
        public int capacity { get; set; }
        public override string ToString()
        {
            return "overflowStands : \n" + availabilities.ToString() + "\n\t capacity : " + capacity + "\n";
        }
    }

    [DataContract]

    public class Availibilities
    {
        [DataMember]
        public int bikes { get; set; }
        [DataMember]
        public int stands { get; set; }
        [DataMember]
        public int mechanicalBikes { get; set; }
        [DataMember]
        public int electricalBikes { get; set; }
        [DataMember]
        public int electricalInternalBatteryBikes { get; set; }
        [DataMember]
        public int electricalRemovableBatteryBikes { get; set; }
        public override string ToString()
        {
            return "\tavailabilities : " +
                    "\n\t\tbikes : " + bikes +
                    "\n\t\tstands : " + stands +
                    "\n\t\tmechanicalBikes : " + mechanicalBikes +
                    "\n\t\telectricalBikes : " + electricalBikes +
                    "\n\t\telectricalInternalBatteryBikes : " + electricalInternalBatteryBikes +
                    "\n\t\telectricalRemovableBatteryBikes : " + electricalRemovableBatteryBikes + "\n";
        }
    }

}
