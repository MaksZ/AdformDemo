using Adform.Api;
using AdformDemo.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdformDemo.Data
{
    public class Repository
    {
        private readonly string clientId;
        private readonly string clientSecret;

        private Task<UserCredential> credentialTask;

        public ReportFilter DefaultReportFilter { get; set; } = new ReportFilter { Date = "thisYear" };

        public Task<IEnumerable<ImpressionsRaw>> ImpressionsAsync => GetEntitiesAsync<ImpressionsRaw>();

        public Task<IEnumerable<BidRequestsRaw>> BidRequests => GetEntitiesAsync<BidRequestsRaw>();

        public Repository(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        /// <remark>
        /// For demo purposes we don't take into account that token can expire
        /// </remark>
        private Task<UserCredential> GetCredential() => credentialTask ?? (credentialTask = AuthorizationBroker.AuthorizeAsync(clientId, clientSecret));

        private async Task<IEnumerable<TEntity>> GetEntitiesAsync<TEntity>() where TEntity : new()
        {
            var credential = await GetCredential().ConfigureAwait(false);

            return await new ReportDataService(credential).GetData<TEntity>(DefaultReportFilter).ConfigureAwait(false);
        }
    }
}
