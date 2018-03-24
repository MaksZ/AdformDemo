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

            Assert.IsNotNull(result, "Empty array expected!");
            Assert.IsTrue(result.Length == 0, "Empty array expected!");
        }

        [TestMethod]
        public void GroupByWeekOnOneElementReturnsOneGroupWithTotalEqualToInitialValue()
        {
            var source = new[] { new ImpressionsRaw { Date = new DateTime(2018, 3, 26), Value = 100 } };

            var result = source.GroupByWeek();

            Assert.IsNotNull(result, "Empty array is not expected!");
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

            Assert.IsNotNull(result, "Empty array is not expected!");
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(weekNo, result[0].Week);
        }

        [TestMethod]
        public void GroupByWeekCountsTotalByAllValuesInGroup()
        {
            var values = new int[] { 213, 287 };
            var source = GetImpressionsOfTheSameWeek(5, (i) => values[i]);

            var result = source.GroupByWeek();

            Assert.IsNotNull(result, "Empty array is not expected!");
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

            Assert.IsNotNull(result, "Empty array is not expected!");
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
    }
}
