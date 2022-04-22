using System.ServiceModel;
using System.Threading.Tasks;

namespace ProxyCache
{
    [ServiceContract]
    public interface IProxy
    {
        [OperationContract]
        Task<string> GetAllStation();

        [OperationContract]
        Task<string> GetStation(string key);
    }
}
