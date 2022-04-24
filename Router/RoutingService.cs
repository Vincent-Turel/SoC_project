
using System.Threading.Tasks;

namespace Router
{
    internal class RoutingService : IRoutingService
    {
        public Task<string> FindPathway(string start, string end) => Finder.GetInstance().FindPathway(start, end);
    }
}
