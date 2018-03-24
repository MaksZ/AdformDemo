using Adform.Api;
using AdformDemo.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdformDemo.Data
{
    /// <summary>
    /// Repository of report data
    /// </summary>
    public class Repository
    {
        private readonly string clientId;
        private readonly string clientSecret;

        private Task<UserCredential> credentialTask;

        /// <summary>
        /// Filter used to manage scope of data
        /// </summary>
        public ReportFilter DefaultReportFilter { get; set; } = new ReportFilter { Date = "thisYear" };

        /// <summary>
        /// Returns Impressions
        /// </summary>
        public Task<IEnumerable<ImpressionsRaw>> ImpressionsAsync => GetEntitiesAsync<ImpressionsRaw>();

        /// <summary>
        /// Returns BidRequests
        /// </summary>
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

        /// <summary>
        /// Retrieves reports data by the given entity
        /// </summary>
        private async Task<IEnumerable<TEntity>> GetEntitiesAsync<TEntity>() where TEntity : new()
        {
            var credential = await GetCredential().ConfigureAwait(false);

            return await new ReportDataService(credential)
                .GetData<TEntity>(DefaultReportFilter)
                .ConfigureAwait(false);
        }
    }
}
