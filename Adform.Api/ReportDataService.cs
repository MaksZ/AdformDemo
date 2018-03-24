using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Adform.Api
{
    /// <summary>
    /// Implements logic for accessing of report data
    /// </summary>
    public class ReportDataService
    {
        private readonly UserCredential credential;

        public ReportDataService(UserCredential credential)
        {
            this.credential = credential;
        }

        /// <summary>
        /// Retrieves report data; 
        /// given type should contain public properties marked 
        /// with attributes [Dimension] or [Metric],
        /// those will be passed to service within request
        /// </summary>
        /// <typeparam name="T">Type containing desired dimensions and metrics</typeparam>
        /// <param name="filter">Data filter</param>
        public async Task<IEnumerable<T>> GetData<T>(ReportFilter filter) where T : new ()
        {
            var parser = new ReportDataParser<T>();

            var request = new ReportDataRequest
            {
                Filter = filter,
                Dimensions = parser.Dimensions,
                Metrics = parser.Metrics
            };

            var raw = await GetRawData(request).ConfigureAwait(false);

            return parser.Parse(raw);
        }

        private async Task<string> GetRawData(ReportDataRequest request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(credential.TokenType, credential.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(request));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var respond = await client
                    .PostAsync(new Uri("https://api.adform.com/v1/reportingstats/publisher/reportdata"), content)
                    .ConfigureAwait(false);

                return await respond.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);
            }
        }

        private class ReportDataRequest
        {
            [JsonProperty("filter")]
            public ReportFilter Filter { get; set; }

            [JsonProperty("dimensions")]
            public string[] Dimensions { get; set; }

            [JsonProperty("metrics")]
            public string[] Metrics { get; set; }
        }
    }
}
