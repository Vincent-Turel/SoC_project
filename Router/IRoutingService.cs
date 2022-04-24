using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;


namespace Router
{
    [ServiceContract]
    public interface IRoutingService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", 
            UriTemplate = "pathway?start={start}&end={end}",
            ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> FindPathway(string start, string end);
    }
}
