using System.ServiceModel;

namespace ProxyCache
{
    [ServiceContract]
    public interface IProxy
    {
        [OperationContract]
        JCDecauxItem GetAllStation();

        [OperationContract]
        JCDecauxItem GetStation(string key);
    }
}
