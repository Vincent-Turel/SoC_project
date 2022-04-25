using System.Threading.Tasks;
using System.IO;
using System.Text;


namespace Router
{
    internal class RoutingService : IRoutingService
    {
        public async Task<Stream> FindPathway(string start, string end) =>
            new MemoryStream(Encoding.UTF8.GetBytes((await Finder.GetInstance().FindPathway(start, end)).ToString()));
    }
}
