// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Statistics.Distributions;
    using System.Reflection;
    using Workbench.Tools;

    /// <summary>
    ///   Describes a distribution's property value, 
    ///   such as its Mean, Median or Variance.
    /// </summary>
    /// 
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

        /// <summary>
        ///   Gets the distribution parameter's reflection information.
        /// </summary>
        /// 
        public PropertyInfo Property { get; private set; }

        /// <summary>
        ///   Gets the parent distribution to whom this property belongs.
        /// </summary>
        /// 
        public DistributionViewModel Owner { get; private set; }



        /// <summary>
        ///   Attempts to create a distribution's property. If the property doesn't
        ///   qualify as a valid property to be shown in the automatic distribution
        ///   description, the method fails and returns false.
        /// </summary>
        /// 
        /// <param name="info">The property's reflection information.</param>
        /// <param name="owner">The distribution that should own this property.</param>
        /// <param name="property">The created distribution property.</param>
        /// 
        /// <returns>True if the property could be created; false otherwise.</returns>
        /// 
        public static bool TryParse(PropertyInfo info, DistributionViewModel owner, out PropertyViewModel property)
        {
            property = null;

            if (info.GetMethod.ReturnType != typeof(double))
                return false;

            property = new PropertyViewModel(info, owner);

            return true;
        }


        /// <summary>
        ///   Updates the property value by querying
        ///   the underlying distribution model.
        /// </summary>
        /// 
        public void Update()
        {
            try
            {
                var instance = Owner.Instance;
                if (instance != null)
                    Value = (double)Property.GetValue(instance);
            }
            catch
            {
                Value = null;
            }
        }


        private PropertyViewModel(PropertyInfo prop, DistributionViewModel distribution)
        {
            this.Property = prop;
            this.Owner = distribution;
            this.Name = DistributionManager.Normalize(prop.Name);
        }

    }
}
