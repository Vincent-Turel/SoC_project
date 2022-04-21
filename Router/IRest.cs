using System.ServiceModel;
using System.ServiceModel.Web;


namespace Router
{
    [ServiceContract]

    public interface IRest
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "pathway?start={start}&end={end}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string FindPathway(string start, string end);
    }
}
