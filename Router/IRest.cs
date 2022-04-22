using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;


namespace Router
{
    [ServiceContract]

    public interface IRest
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "pathway?start={start}&end={end}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Task<string> FindPathway(string start, string end);
    }
}
