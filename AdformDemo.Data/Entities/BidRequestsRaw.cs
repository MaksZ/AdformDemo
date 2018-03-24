using Adform.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdformDemo.Data.Entities
{
    public class BidRequestsRaw
    {
        [Dimension("date")]
        public DateTime Date { get; set; }

        [Metric("bidRequests")]
        public int Value { get; set; }
    }
}
