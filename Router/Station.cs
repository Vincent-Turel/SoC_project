
namespace Router
{
    public class Station
    {
        public int number { get; set; }
        public string contractName { get; set; }
        public string name { get; set; }
        public Position position { get; set; }
        public Stands totalStands { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || GetType() != obj.GetType()) return false;
            var sta = (Station) obj;
            return number == sta.number && contractName == sta.contractName;
        }
    }
    
    public class Stands
    {
        public Availability availabilities { get; set; }
        public int capacity { get; set; }
    }

    public class Availability
    {
        public int bikes { get; set; }
        public int stands { get; set; }
        public int mechanicalBikes { get; set; }
        public int electricalBikes { get; set; }
    }
}