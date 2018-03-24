using AdformDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdformDemo.CommandLine
{
    class Program
    {
        private Data.Repository repo;

        // For demo purposes we store sensitive data as plain.
        const string clientId = "sellside.apiteam@tests.adform.com";
        const string clientSecret = "xPDUpHFZHuobERbKVjVxPujndfyg4C6KLDItwLwK";

        private Data.Repository Repository => repo ?? (repo = new Data.Repository(clientId, clientSecret));

        static void Main(string[] args)
        {
            new Program().Execute();
        }

        private void Execute()
        {
            Console.WriteLine($"Adform tech interview task implementation, {DateTime.Now}");
            Console.Write("-----------------------------------------------------");

            do
            {
                Console.WriteLine("\nChoose:\n\t(i) - to aggregate impressions by week\n\t(b) - to get anomalies in bidrequests\n\t(ESC) - to exit\n");
                var choice = Console.ReadKey(true);

                if (choice.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nLeaving... Have a nice day! Bye!");
                    Thread.Sleep(2500);
                    break;
                }

                if (choice.Key == ConsoleKey.I)
                {
                    Console.WriteLine(
@"Retrieves impressions split by date for this year
(Metric: ""impressions"", dimension: ""date"", filter: ""thisYear"").
Aggregates retrieved data by week and provide result how much 
impressions  was each week this year.");
                    Console.WriteLine();

                    AggregateImpressionsDemo();
                }

                if (choice.Key == ConsoleKey.B)
                {
                    Console.WriteLine(
@"Retrieves bid requests split by date for this year (Metric: ""bidRequests""). 
In retrieved result finds data anomalies when bidrequests increased or 
decreased  3 or more times compared to previous day.");
                    Console.WriteLine();

                    FindAnomaliesDemo();
                }
            } while (true);
        }

        private void AggregateImpressionsDemo()
        {
            foreach (var item in Repository.ImpressionsAsync.Result.GroupByWeek())
            {
                Console.WriteLine($"{{\"week\": {item.Week}, \"impressions\": {item.Total}}},");
            }
        }

        private void FindAnomaliesDemo()
        {
            foreach (var date in Repository.BidRequests.Result.GetAnomalies(anomalyFactor: 3))
            {
                Console.WriteLine($"\"{date.ToString("s").Replace("T00:00:00", string.Empty)}\"");
            }
        }
    }
}
