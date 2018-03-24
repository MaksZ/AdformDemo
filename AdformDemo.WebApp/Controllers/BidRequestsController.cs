using AdformDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdformDemo.WebApp.Controllers
{
    public class BidRequestsController : ApiController
    {
        // For demo purposes we store sensitive data as plain.
        const string clientId = "sellside.apiteam@tests.adform.com";
        const string clientSecret = "xPDUpHFZHuobERbKVjVxPujndfyg4C6KLDItwLwK";

        public IEnumerable<DateTime> GetAllItems()
        {
            return new Repository(clientId, clientSecret).BidRequests.Result.GetAnomalies(anomalyFactor: 3);
        }
    }
}
