using System.ServiceModel;
using System;

namespace ProxyCache
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Starting of the proxy service !");
            var service = new ServiceHost(typeof(Proxy));
            service.Open();
            Console.ReadLine();
        }
    }
}