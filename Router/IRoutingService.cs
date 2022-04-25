using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;


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
        Task<Stream> FindPathway(string start, string end);
    }
}
