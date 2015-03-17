// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Tools
{
    using Accord;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using AForge;
    using NuDoq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Workbench.ViewModels;

    /// <summary>
    ///   Static class for performing actions related to distributions, such as obtaining all 
    ///   available distributions through reflection, querying its parameters, and normalizing 
    ///   their names.
    /// </summary>
    /// 
    public static class DistributionManager
    {
        private static string baseURL = "http://accord-framework.net/docs/html/";


        /// <summary>
        ///   Gets an array containing all distributions that can be dynamically build by
        ///   this application by inspecting Accord.NET assemblies using reflection.
        /// </summary>
        /// 
        public static DistributionViewModel[] GetDistributions(MainViewModel owner)
        {
            // This function iterates the Accord.Statistics assembly looking for
            // classes that are concrete (not abstract) and that implement the
            // IUnivariateDistribution interface. Then, it attempts to create a
            // DistributionViewModel from the distribution's type by using the
            // DistributionViewModel.TryParse method.

            var baseType = typeof(IUnivariateDistribution);

            var assembly = Assembly.GetAssembly(baseType);

            // Prepare and leave Accord.NET documentation parsed
            var doc = GetDocumentation(assembly);

            // Get all univariate distributions in Accord.NET:
            var distributions = assembly.GetTypes()
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);

            // Get only those that can be dynamically built:
            var buildable = new List<DistributionViewModel>();
            foreach (Type type in distributions)
            {
                DistributionViewModel distribution;
                if (DistributionViewModel.TryParse(owner, type, doc, out distribution))
                    buildable.Add(distribution);
            }

            buildable.Sort((a, b) => a.Name.CompareTo(b.Name));

            return buildable.ToArray();
        }


        /// <summary>
        ///   Gets the fitting options object that are expected by one distribution, if any. An
        ///   Accord.NET distribution object can be fitted to a set of observed values. However,
        ///   some distributions have different settings on how this fitting can be done. This
        ///   function creates an object that contains those possible settings that can be configured
        ///   for a given distribution type.
        /// </summary>
        /// 
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
   

        /// <summary>
        ///   Gets the online documentation URL for given an Accord.NET type.
        /// </summary>
        /// 
        public static string GetDocumentationUrl(Type type)
        {
            return baseURL + "T_" + type.FullName.Replace(".", "_") + ".htm";
        }

        /// <summary>
        ///   Gets the documentation URL for a code reference link 
        ///   contained in one of Accord.NET's documentation pages.
        /// </summary>
        /// 
        public static string GetDocumentationUrl(string cref)
        {
            string seeURL = cref.Replace(".", "_").Replace(":", "_");
            return baseURL + seeURL + ".htm";
        }

        /// <summary>
        ///   Parses through Accord.NET XML documentation files and generate XAML code for some 
        ///   selected types of documentation entries, such as Summary, Remarks and Examples.
        /// </summary>
        /// 
        private static Dictionary<string, DocumentationViewModel> GetDocumentation(Assembly assembly)
        {
            AssemblyMembers members = DocReader.Read(assembly);
            var visitor = new ClassToXamlVisitor(members.IdMap);
            members.Accept(visitor);
            return visitor.Texts;
        }

        /// <summary>
        ///   Gets the name of the distribution modeled by a given Accord.NET type. 
        ///   The name is returned in a normalized form (i.e. given a type whose name
        ///   is  NormalDistribution, the function would return "Normal").
        /// </summary>
        /// 
        public static string GetDistributionName(Type type)
        {
            // Extract the real distribution name from the class name
            string name = DistributionManager.Normalize(type.Name);

            if (name.Contains('`'))
                name = name.Remove(name.IndexOf("`"));

            // Remove the trailing "Distribution" from the name
            if (name.EndsWith("Distribution"))
                name = name.Remove(name.IndexOf("Distribution"));

            return name.Trim();
        }

        

        /// <summary>
        ///   Tries to get the valid range of a distribution's parameter.
        /// </summary>
        /// 
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

        /// <summary>
        ///   Tries to get the default value of a distribution's parameter.
        /// </summary>
        /// 
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

        /// <summary>
        ///   Normalizes the name of a distribution, parameter or property from this
        ///   distribution. The name  is returned in a normalized form, standardizing 
        ///   certain terms such as "Standard" and "Std" to a single common term "Std.",
        ///   for example.
        /// </summary>
        /// 
        public static string Normalize(string name)
        {
            string withSpaces = Regex.Replace(name, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

            name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(withSpaces);

            name = name.Replace("Standard", "Std.");
            name = name.Replace("Deviation", "Dev.");

            if (name.EndsWith("Dev"))
                name += ".";

            if (name.Contains("Std") && !name.Contains("Std."))
                name = name.Replace("Std", "Std.");

            return name;
        }

    }
}
