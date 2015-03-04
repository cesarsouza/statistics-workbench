// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Statistics.Distributions;
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;

    /// <summary>
    ///   Describes a distribution's constructor, which can use or not a number
    ///   of parameters in order to instantiate a new probability distribution.
    ///   After all distribution parameter's have been set, a new instance of
    ///   the distribution can be created by calling this class' Activate()
    ///   method.
    /// </summary>
    /// 
    public class ConstructorViewModel
    {

        /// <summary>
        ///   Gets the constructor's reflection information. This 
        ///   is the real Model underlying this ViewModel class.
        /// </summary>
        /// 
        public ConstructorInfo Constructor { get; private set; }

        /// <summary>
        ///   Gets the constructor's parameters and their current selected values.
        /// </summary>
        /// 
        public ObservableCollection<ParameterViewModel> Parameters { get; private set; }

        /// <summary>
        ///   Gets the parent distribution to whom this constructor belongs.
        /// </summary>
        /// 
        public DistributionViewModel Owner { get; private set; }


        private ConstructorViewModel(ConstructorInfo info, DistributionViewModel owner)
        {
            this.Owner = owner;
            this.Constructor = info;
            this.Parameters = new ObservableCollection<ParameterViewModel>();
        }


        /// <summary>
        ///   Creates a new distribution instance using the current selected
        ///   values for this constructor's parameters. If there is any problem
        ///   creating the object, this method returns null.
        /// </summary>
        /// 
        public IUnivariateDistribution Activate()
        {
            IUnivariateDistribution distribution = null;

            try
            {
                var parameters = new object[Parameters.Count];
                foreach (ParameterViewModel p in Parameters)
                    parameters[p.Parameter.Position] = Convert.ChangeType(p.Value, p.Parameter.ParameterType);

                distribution = (IUnivariateDistribution)Activator.CreateInstance(Owner.Type, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
                return null;
            }

            return distribution;
        }




        /// <summary>
        ///   Attempts to create a distribution's constructor. If the constructor 
        ///   parameter's aren't valid, this method fails and returns false.
        /// </summary>
        /// 
        /// <param name="info">The constructor's reflection information.</param>
        /// <param name="distribution">The distribution that owns this constructor.</param>
        /// <param name="constructor">The created distribution constructor.</param>
        /// 
        /// <returns>True if the constructor could be created; false otherwise.</returns>
        /// 
        public static bool TryParse(ConstructorInfo info, DistributionViewModel distribution, out ConstructorViewModel constructor)
        {
            constructor = new ConstructorViewModel(info, distribution);

            foreach (var param in info.GetParameters())
            {
                ParameterViewModel viewModel;
                if (!ParameterViewModel.TryParse(param, constructor, out viewModel))
                    return false;

                constructor.Parameters.Add(viewModel);
            }

            return true;
        }

    }
}
