// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Statistics.Distributions;
    using PropertyChanged;
    using System.Collections.Generic;
    using System.Reflection;
    using Workbench.Tools;

    /// <summary>
    ///   Describes a distribution's property value, 
    ///   such as its Mean, Median or Variance.
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public class PropertyViewModel
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

        public DistributionViewModel Owner { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyViewModel(DistributionViewModel model, PropertyInfo property)
        {
            this.PropertyInfo = property;
            this.Name = DistributionManager.ToNormalCase(property.Name);
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


        public static bool TryParse(PropertyInfo prop, DistributionViewModel distribution,
            Dictionary<string, string> doc, out PropertyViewModel property)
        {
            property = null;

            if (prop.GetMethod.ReturnType != typeof(double))
                return false;

            property = new PropertyViewModel(distribution, prop);

            return true;
        }
    }
}
