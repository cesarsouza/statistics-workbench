using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Fitting;
using PropertyChanged;
using Statistics_Workbench.ViewModels;

namespace Statistics_Workbench.Models
{
    [ImplementPropertyChanged]
    public class DistributionInfo
    {
        public string Name { get; private set; }

        public DistributionConstructorInfo Constructor { get; private set; }

        public ObservableCollection<DistributionPropertyInfo> Properties { get; private set; }

        public Type Type { get; private set; }

        public string Summary { get; private set; }

        public bool CanGenerate { get; private set; }



        public static bool TryParse(Type type, Dictionary<string, string> doc, out DistributionInfo distribution)
        {
            distribution = new DistributionInfo();

            if (!typeof(IUnivariateDistribution).IsAssignableFrom(type))
                return false;

            string name = DistributionManager.GetDistributionName(type);

            // Extract all properties with return value of double
            //
            var properties = new List<DistributionPropertyInfo>();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                DistributionPropertyInfo property;
                if (DistributionPropertyInfo.TryParse(prop, distribution, doc, out property))
                    properties.Add(property);
            }

            // Extract buildable constructors. A constructor is 
            // considered buildable if we can extract valid ranges
            // and default values from all of its parameters
            //
            var list = new List<DistributionConstructorInfo>();
            foreach (var ctor in type.GetConstructors())
            {
                DistributionConstructorInfo constructor;
                if (DistributionConstructorInfo.TryParse(ctor, distribution, out constructor))
                    list.Add(constructor);
            }

            if (list.Count == 0)
                return false;

            // For the time being, just consider the buildable 
            // constructor with the largest number of parameters.
            //
            var main = list.OrderByDescending(x => x.Parameters.Count).First();



            // Extract some documentation
            string summary = doc[type.Name];

            distribution.Constructor = main;
            distribution.Properties = new ObservableCollection<DistributionPropertyInfo>(properties);
            distribution.Type = type;
            distribution.Name = name;
            distribution.Summary = summary;
            distribution.CanGenerate = typeof(ISampleableDistribution<double>).IsAssignableFrom(type);

            return true;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
