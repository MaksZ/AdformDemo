using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adform.Api
{
    /// <summary>
    /// Helper class to deserialize data from service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ReportDataParser<T> where T : new()
    {
        private readonly Dictionary<PropertyInfo, PropertyFeatureAttribute> propMap;
        private readonly Lazy<string[]> dimensions;
        private readonly Lazy<string[]> metrics;

        public string[] Dimensions => dimensions.Value;

        public string[] Metrics => metrics.Value;

        public ReportDataParser()
        {
            propMap = GetPropertyMap();

            dimensions = GetPropertyFeatureNames<DimensionAttribute>();

            metrics = GetPropertyFeatureNames<MetricAttribute>();
        }

        /// <summary>
        /// Deserializes raw json to a sequence of objects of a given type
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public IEnumerable<T> Parse(string raw)
        {
            var json = JObject.Parse(raw);

            var jtReportData = json.GetValue("reportData");

            // get properties in order they come in respond
            var propSchema =
                jtReportData["columnHeaders"]
                    .ToObject<string[]>()
                    .Select(columnHeader => propMap.Single(kv => kv.Value.Name == columnHeader).Key)
                    .ToArray();

            var objectBuilder = new ObjectBuilder(propSchema);

            var jtRows = jtReportData["rows"];

            return jtRows.Select(jtRow => objectBuilder.Build(jtRow));
        }

        private Dictionary<PropertyInfo, PropertyFeatureAttribute> GetPropertyMap()
        {
            var result =
                typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Aggregate(
                        new Dictionary<PropertyInfo, PropertyFeatureAttribute>(),
                        (acc, prop) =>
                        {
                            var attr = prop.GetCustomAttribute<PropertyFeatureAttribute>();

                            if (attr != null)
                                acc.Add(prop, attr);

                            return acc;
                        }
                    );

            return result;
        }

        private Lazy<string[]> GetPropertyFeatureNames<TAttr>() where TAttr : PropertyFeatureAttribute
            =>
                new Lazy<string[]>(() => propMap.Values.OfType<TAttr>().Select(x => x.Name).ToArray(), false);

        /// <summary>
        /// Helper class to initialize object's properties with values from object array
        /// </summary>
        private class ObjectBuilder
        {
            private readonly PropertyInfo[] propSchema;

            public ObjectBuilder(PropertyInfo[] propSchema) { this.propSchema = propSchema; }

            public T Build(JToken values)
            {
                var obj = Enumerable
                        .Range(0, propSchema.Length)
                        .Aggregate(
                            new T(),
                            (seed, i) =>
                            {
                                var prop = propSchema[i];
                                prop.SetValue(seed, values[i].ToObject(prop.PropertyType));

                                return seed;
                            }
                        );

                return obj;
            }
        }
    }

    /// <summary>
    /// Base class for object property feature
    /// </summary>
    public abstract class PropertyFeatureAttribute : Attribute
    {
        public string Name { get; }

        protected PropertyFeatureAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Marked with this attribute property is treated as dimension
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DimensionAttribute : PropertyFeatureAttribute
    {
        public DimensionAttribute(string name) : base(name) { }
    }

    /// <summary>
    /// Marked with this attribute property is treated as metric
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MetricAttribute : PropertyFeatureAttribute
    {
        public MetricAttribute(string name) : base(name) { }
    }
}
