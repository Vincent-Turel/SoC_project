using System.ServiceModel;
using System;

namespace Router
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting of the routing service !");
                var service = new ServiceHost(typeof(RoutingService));
                service.Open();
                Console.ReadLine();
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine("il y a une erreur..."); 
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
