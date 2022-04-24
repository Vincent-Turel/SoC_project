using System.ServiceModel;
using System;

namespace Router
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting of the routing service !");
            var service = new ServiceHost(typeof(RoutingService));
            service.Open();
        }
    }
}
