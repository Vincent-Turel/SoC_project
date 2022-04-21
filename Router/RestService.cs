
namespace Router
{
    internal class RestService : IRest
    {
        private readonly Finder _finder = new Finder();
        public string FindPathway(string start, string end)
        {
            return _finder.FindPathway(start, end);
        }
    }
}
