using System;
using ServiceReference1;
using System.Diagnostics;

namespace HeavyClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting of the heavy client !");
            while (true)
            {
                Console.WriteLine("Press any key to start measures :");
                Console.WriteLine("Type quit to exit.");

                var choice = Console.ReadLine();
                if (choice == "quit") break;

                Console.Write("Current location : ");
                var loc = Console.ReadLine();
                Console.Write("\nDestination : ");
                var dest = Console.ReadLine();


                Console.WriteLine("\n ------- STARTING TESTS -------");


                Stopwatch stopwatch = new();
                RoutingServiceClient client = new();

                stopwatch.Start();
                var _ = await client.FindPathwayAsync(loc, dest);
                stopwatch.Stop();

                Console.WriteLine("Elapsed time without proxy is {0} s", (stopwatch.ElapsedMilliseconds / 1000.0).ToString("0.####"));

                for (int i = 0; i < 2; i++)
                {
                    stopwatch.Restart();
                    _ = await client.FindPathwayAsync(loc, dest);
                    stopwatch.Stop();
                    Console.WriteLine("Elapsed time with proxy is {0} s", (stopwatch.ElapsedMilliseconds / 1000.0).ToString("0.####"));
                }

                Console.WriteLine("\n ------- TESTS ARE OVER -------");
            }      
        }
    }
}
