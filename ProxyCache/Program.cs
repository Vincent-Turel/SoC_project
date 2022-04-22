using System.ServiceModel;

namespace ProxyCache
{
    class Program
    {
        public static void Main()
        {
            var variable = new ServiceHost(typeof(Proxy));
            variable.Open();
        }
    }
}