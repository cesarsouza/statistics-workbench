using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Accord.Statistics.Distributions;
using PropertyChanged;
using Statistics_Workbench.Models;
using Statistics_Workbench.ViewModels;

namespace Statistics_Workbench.Models
{
    /// <summary>
    ///   Describes a distribution's property value, 
    ///   such as its Mean, Median or Variance.
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public class DistributionPropertyInfo
    {
        /// <summary>
        ///   Gets the property's name.
        /// </summary>
        /// 
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the current value for the property.
        /// </summary>
        /// 
        public double? Value { get; private set; }

        public DistributionInfo Owner { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public DistributionPropertyInfo(DistributionInfo model, PropertyInfo property)
        {
            this.PropertyInfo = property;
            this.Name = Tools.ToNormalCase(property.Name);
            this.Owner = model;

            Name = Name.Replace("Standard", "Std.");
        }

        public void Update(IUnivariateDistribution instance)
        {
            Value = null;

            try
            {
                if (instance != null)
                    Value = (double)PropertyInfo.GetValue(instance);
            }
            catch
            {
            }
        }


        public static bool TryParse(PropertyInfo prop, DistributionInfo distribution,
            Dictionary<string, string> doc, out DistributionPropertyInfo property)
        {
            property = null;

            if (prop.GetMethod.ReturnType != typeof(double))
                return false;

            property = new DistributionPropertyInfo(distribution, prop);

            return true;
        }
    }
}
