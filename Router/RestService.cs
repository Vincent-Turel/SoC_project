
using System.Threading.Tasks;

namespace Router
{
    internal class RestService : IRest
    {
        private readonly Finder _finder = new Finder();
        public Task<string> FindPathway(string start, string end) => new Finder().FindPathway(start, end);
    }
}
