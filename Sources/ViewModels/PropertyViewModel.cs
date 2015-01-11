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

        /// <summary>
        ///   Gets the distribution parameter's reflection information.
        /// </summary>
        /// 
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        ///   Gets the parent distribution to whom this property belongs.
        /// </summary>
        /// 
        public DistributionViewModel ParentDistribution { get; private set; }



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






        private PropertyViewModel(PropertyInfo prop, DistributionViewModel distribution)
        {
            this.PropertyInfo = prop;
            this.ParentDistribution = distribution;
            this.Name = DistributionManager.ToNormalCase(prop.Name);
            this.Name = DistributionManager.NormalizeTerms(Name);

            // this.ParentDistribution.InstanceChanged += ParentDistribution_InstanceChanged;
        }


        public void Update(IUnivariateDistribution instance)
        {
            try
            {
                if (instance != null)
                    Value = (double)PropertyInfo.GetValue(instance);
            }
            catch
            {
                Value = null;
            }
        }

    }
}
