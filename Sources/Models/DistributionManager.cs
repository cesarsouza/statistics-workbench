using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Fitting;
using AForge;
using NuDoq;

namespace Statistics_Workbench.Models
{
    public class DistributionManager
    {
        private static string baseURL = "http://accord-framework.net/docs/html/T_";


        public static IFittingOptions GetFittingOptions(Type type)
        {
            // Try to create a fitting options object
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFittableDistribution<,>));

            foreach (var i in interfaces)
            {
                foreach (var arg in i.GetGenericArguments())
                {
                    var argType = arg.GetTypeInfo();

                    if (typeof(IFittingOptions).IsAssignableFrom(argType) && argType != typeof(IFittingOptions))
                    {
                        return (IFittingOptions)Activator.CreateInstance(argType);
                    }
                }
            }

            return null;
        }

        public static DistributionInfo[] GetDistributions()
        {
            var baseType = typeof(IUnivariateDistribution);

            var assembly = Assembly.GetAssembly(baseType);

            // Prepare and leave Accord.NET documentation parsed
            Dictionary<string, string> doc = GetDocumentation(assembly);

            // Get all univariate distributions in Accord.NET:
            var distributions = assembly.GetTypes()
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);

            // Get only those that can be dynamically built:
            var buildable = new List<DistributionInfo>();
            foreach (Type type in distributions)
            {
                DistributionInfo distribution;
                if (DistributionInfo.TryParse(type, doc, out distribution))
                    buildable.Add(distribution);
            }

            buildable.Sort((a, b) => a.Name.CompareTo(b.Name));

            return buildable.ToArray();
        }

        public static string GetDocumentationUrl(Type type)
        {
            string docPage = baseURL + type.FullName.Replace(".", "_") + ".htm";
            return docPage;
        }

        private static Dictionary<string, string> GetDocumentation(Assembly assembly)
        {
            var members = DocReader.Read(assembly);
            ClassToXamlVisitor visitor = new ClassToXamlVisitor(members.IdMap);
            members.Accept(visitor);
            Dictionary<string, string> doc = visitor.Texts.ToDictionary(x => x.Key, y => y.Value.ToString());
            return doc;
        }


        public static string GetDistributionName(Type type)
        {
            // Extract the real distribution name from the class name
            string name = Tools.ToNormalCase(type.Name);

            if (name.Contains('`'))
                name = name.Remove(name.IndexOf("`"));

            // Remove the trailing "Distribution" from the name
            if (name.EndsWith("Distribution"))
                name = name.Remove(name.IndexOf("Distribution"));

            return name.Trim();
        }

        public static string GetParameterName(ParameterInfo parameterInfo)
        {
            string name = Tools.ToNormalCase(parameterInfo.Name);
            name = name.Replace("Std ", "Std. ");
            name = name.Replace("Standard", "Std.");
            return name;
        }


        public static bool IsInteger(ParameterInfo parameter)
        {
            return parameter.ParameterType == typeof(int);
        }

        public static bool TryGetRange(ParameterInfo parameter, out DoubleRange range)
        {
            range = new DoubleRange(0, 0);

            var attrb = parameter.GetCustomAttribute<RangeAttribute>();
            if (attrb == null)
                return false;

            double min = (double)Convert.ChangeType(attrb.Minimum, typeof(double));
            double max = (double)Convert.ChangeType(attrb.Maximum, typeof(double));

            range = new DoubleRange(min, max);

            return true;
        }

        public static bool TryGetDefault(ParameterInfo parameter, out double value)
        {
            var attrb = parameter.GetCustomAttribute<DefaultValueAttribute>();

            if (attrb != null)
            {
                value = (double)Convert.ChangeType(attrb.Value, typeof(double));
                return true;
            }

            DoubleRange range;
            if (!TryGetRange(parameter, out range))
            {
                value = 0;
                return false;
            }

            var a = parameter.GetCustomAttribute<RangeAttribute>();

            value = 0;

            if (a is PositiveAttribute || a is PositiveIntegerAttribute)
                value = 1;

            else if (a is NegativeAttribute || a is NegativeIntegerAttribute)
                value = -1;

            else if (a is UnitAttribute)
                value = 0.5;


            if (value < range.Min)
                value = range.Min;

            if (value > range.Max)
                value = range.Max;

            return true;
        }

    }
}
