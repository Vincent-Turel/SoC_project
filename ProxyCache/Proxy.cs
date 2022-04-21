namespace ProxyCache
{
    public class Proxy : IProxy
    {
        private readonly ProxyCache<JCDecauxItem> _proxyCache = new ProxyCache<JCDecauxItem>();
        
        public JCDecauxItem GetAllStation()
        {
            return _proxyCache.Get("all");
        }

        public JCDecauxItem GetStation(string key)
        {
            return _proxyCache.Get("key");
        }
    }
}
