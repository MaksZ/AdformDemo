using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdformDemo.Data.Entities;
using AdformDemo.Data;

namespace AdformDemo.Tests.Data
{
    [TestClass]
    public class AggregatorsTests
    {
        [TestMethod]
        public void GroupByWeekReturnsEmptyArrayOnEmptySource()
        {
            var source = Enumerable.Empty<ImpressionsRaw>();

            var result = source.GroupByWeek();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 0, "Empty array is expected!");
        }

        [TestMethod]
        public void GroupByWeekOnOneElementReturnsOneGroupWithTotalEqualToInitialValue()
        {
            var source = new[] { new ImpressionsRaw { Date = new DateTime(2018, 3, 26), Value = 100 } };

            var result = source.GroupByWeek();

            Assert.IsNotNull(result);
            Assert.AreEqual(source.Length, result.Length);
            Assert.AreEqual(source[0].Date.GetWeekOfYear(), result[0].Week);
            Assert.AreEqual(source[0].Value, result[0].Total);
        }

        [TestMethod]
        public void GroupByWeekReturnsOneGroupIfItemsBelongToTheSameWeek()
        {
            var weekNo = 5;
            var source = GetImpressionsOfTheSameWeek(weekNo);

            var result = source.GroupByWeek();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(weekNo, result[0].Week);
        }

        [TestMethod]
        public void GroupByWeekCountsTotalByAllValuesInGroup()
        {
            var values = new int[] { 213, 287 };
            var source = GetImpressionsOfTheSameWeek(5, (i) => values[i]);

            var result = source.GroupByWeek();

            Assert.IsNotNull(result);
            Assert.AreEqual(values.Sum(), result[0].Total);
        }

        [TestMethod]
        public void GroupByWeekReturnsSeveralGroupsIfItemsBelongToSeveralWeeks()
        {
            var weekNos = new List<int> { 5, 6 };
            var values = new int[] { 213, 287 };

            var source = new List<ImpressionsRaw>();
            weekNos.ForEach(n => source.AddRange(GetImpressionsOfTheSameWeek(n, (i) => values[i])));

            var result = source.GroupByWeek();

            Assert.IsNotNull(result);
            Assert.IsTrue(weekNos.Zip(result.Select(x => x.Week), (exp, act) => exp == act).All(x => x));
        }

        //[TestMethod]
        //public void Jan01andJan02of2018BelongToWeek1()
        //{
        //    var seed = new DateTime(2018, 1, 1);

        //    Assert.AreEqual(1, seed.GetWeekOfYear());
        //    Assert.AreEqual(1, seed.AddDays(1).GetWeekOfYear());
        //}
        
        private ImpressionsRaw[] GetImpressionsOfTheSameWeek(int weekNo, Func<int, int> valueSetter = null)
        {
            if (valueSetter == null)
                valueSetter = (int i) => 0;
            
            var seed = new DateTime(2018, 1, 1);

            var date1 = seed.AddDays((weekNo - 1) * 7);
            var date2 = date1.AddDays(1);

            var source = new[]
            {
                new ImpressionsRaw { Date = date1, Value = valueSetter(0) },
                new ImpressionsRaw { Date = date2, Value = valueSetter(1) }
            };

            return source;
        }

        [TestMethod]
        public void GetAnomaliesReturnsEmptyArrayOnEmptySource()
        {
            var source = Enumerable.Empty<BidRequestsRaw>();

            var result = source.GetAnomalies(3);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 0, "Empty array is expected!");
        }

        [TestMethod]
        public void GetAnomaliesReturnsEmptyArrayOnOneElementSource()
        {
            var source = GetBidRequests(new[] { 0 });

            var result = source.GetAnomalies(3);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 0, "Empty array is expected!");
        }

        [TestMethod]
        public void GetAnomaliesFindsAnomalyWhenValueIncreased()
        {
            const int anomalyFactor = 3;

            var values = new int[] { 10, anomalyFactor * 10 };

            var source = GetBidRequests(values);

            var result = source.GetAnomalies(anomalyFactor);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length, "One one item is expected!");
            Assert.AreEqual(source[1].Date, result[0], "Anomaly date is expected!");
        }

        [TestMethod]
        public void GetAnomaliesFindsAnomalyWhenValueDecreased()
        {
            const int anomalyFactor = 3;

            var values = new int[] { anomalyFactor * 10, 10 };

            var source = GetBidRequests(values);

            var result = source.GetAnomalies(anomalyFactor);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length, "One one item is expected!");
            Assert.AreEqual(source[1].Date, result[0], "Anomaly date is expected!");
        }

        [TestMethod]
        public void GetAnomaliesDoesNotFindAnomalyWhenValuesLessThanRequired()
        {
            const int anomalyFactor = 3;

            var values = new int[] 
            {
                anomalyFactor * 10 - 5,
                10,
                anomalyFactor * 10 - 1
            };

            var source = GetBidRequests(values);

            var result = source.GetAnomalies(anomalyFactor);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length, "Empty array is expected!");
        }

        [TestMethod]
        public void GetAnomaliesDoesNotFailOnZeroValues()
        {
            const int anomalyFactor = 3;

            var values = new int[] { 7, 0, 0, 5 };

            var source = GetBidRequests(values);

            var result = source.GetAnomalies(anomalyFactor);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(source[1].Date, result[0]);
            Assert.AreEqual(source[3].Date, result[1]);
        }

        private BidRequestsRaw[] GetBidRequests(int[] values, DateTime seed = default(DateTime))
        {
            if (seed == default(DateTime)) seed = new DateTime(2018, 3, 26);

            return values
                .Select((v, i) => new BidRequestsRaw { Date = seed.AddDays(i), Value = v })
                .ToArray();
        }
    }
}
