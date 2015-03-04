// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using OxyPlot;
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Workbench.Tools;

    /// <summary>
    ///   Describes a probability distribution, such as the Gaussian, the Bernoulli,
    ///   or the Gamma. This class can be data-bound to a user interface so the user
    ///   can select the distribution parameter's and instantiate or estimate it.
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public class DistributionViewModel
    {
        /// <summary>
        ///   Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// 
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether this instance is initializing.
        /// </summary>
        /// 
        public bool IsInitializing { get; private set; }

        /// <summary>
        ///   Gets the current active probability distribution, if any.
        /// </summary>
        /// 
        public IUnivariateDistribution Instance { get; private set; }

        /// <summary>
        ///   Gets the parameters that can be set to create the distribution.
        /// </summary>
        /// 
        public ObservableCollection<ParameterViewModel> Parameters { get; private set; }

        /// <summary>
        ///   Gets the properties that can be extract from a distribution, 
        ///   such as its mean, standard deviation, variance, mode, and so on.
        /// </summary>
        /// 
        public ObservableCollection<PropertyViewModel> Properties { get; private set; }

        /// <summary>
        ///   Gets the fitting options that be selected to estimate this distribution.
        /// </summary>
        /// 
        public IFittingOptions Options { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether it is possible to configure the way
        ///   this distribution estimates its values when analyzing a data sample.
        /// </summary>
        /// 
        public bool HasOptions { get { return Options != null; } }

        /// <summary>
        ///   Gets the name of this distribution, such as "Normal" or "Bernoulli".
        /// </summary>
        /// 
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the type of this distribution.
        /// </summary>
        /// 
        public Type Type { get; private set; }

        /// <summary>
        ///   Gets the distribution's constructor view model, that allows the parameters needed
        ///   to create this distribution to be shown and set in the applications's right sidebar.
        /// </summary>
        /// 
        public ConstructorViewModel Constructor { get; private set; }

        /// <summary>
        ///   Gets the functions view model for this distribution, that contains the many
        ///   functions (such as PDF, CDF, QF) that are shown in the Measures page of the
        ///   main application.
        /// </summary>
        /// 
        public MeasuresViewModel Measures { get; private set; }

        /// <summary>
        /// Gets the documentation view model for this distribution, that contains, among
        /// other things, the XAML codes generated from its documentation page so they can
        /// be data-bound to the Summary page in the main application.
        /// </summary>
        /// 
        public DocumentationViewModel Documentation { get; private set; }

        /// <summary>
        ///   Gets the density function that is displayed on top of the application's right sidebar.
        /// </summary>
        /// 
        public PlotModel DensityFunction { get; private set; }



        /// <summary>
        ///   Initializes a new instance of the <see cref="DistributionViewModel"/> class.
        /// </summary>
        /// 
        public DistributionViewModel()
        {
            this.Parameters = new ObservableCollection<ParameterViewModel>();
            this.Properties = new ObservableCollection<PropertyViewModel>();
            this.Measures = new MeasuresViewModel();
        }

        /// <summary>
        ///   Asynchronously begin to initialize this instance. This method immediately returns.
        /// </summary>
        /// 
        public void InitAsync()
        {
            if (!IsInitialized && !IsInitializing)
            {
                IsInitializing = true;
                Task.Factory.StartNew(() => this.update());
            }
        }

        /// <summary>
        ///   Estimates valid values for the distributions' parameters that fit the specified sample.
        /// </summary>
        /// 
        public void Estimate(double[] sample, double[] weights)
        {
            if (weights == null)
            {
                Instance.Fit(sample, Options);
            }
            else
            {
                weights = weights.Divide(weights.Sum());
                Instance.Fit(sample, weights, Options);
            }

            update(Instance);
        }


        /// <summary>
        /// Attempts to create a new DistributionViewModel from a given type.
        /// </summary>
        public static bool TryParse(Type type, Dictionary<string, DocumentationViewModel> doc, out DistributionViewModel distribution)
        {
            distribution = new DistributionViewModel();

            if (!typeof(IUnivariateDistribution).IsAssignableFrom(type))
                return false;

            string name = DistributionManager.GetDistributionName(type);

            // Extract all properties with return value of double
            //
            var properties = new List<PropertyViewModel>();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                PropertyViewModel property;
                if (PropertyViewModel.TryParse(prop, distribution, out property))
                    properties.Add(property);
            }

            // Extract buildable constructors. A constructor is 
            // considered buildable if we can extract valid ranges
            // and default values from all of its parameters
            //
            var list = new List<ConstructorViewModel>();
            foreach (var ctor in type.GetConstructors())
            {
                ConstructorViewModel constructor;
                if (ConstructorViewModel.TryParse(ctor, distribution, out constructor))
                    list.Add(constructor);
            }

            if (list.Count == 0)
                return false;

            // For the time being, just consider the buildable 
            // constructor with the largest number of parameters.
            //
            var main = list.OrderByDescending(x => x.Parameters.Count).First();



            // Extract some documentation
            var documentation = doc[type.Name];
            documentation.Name = name;

            distribution.Constructor = main;
            distribution.Properties = new ObservableCollection<PropertyViewModel>(properties);
            distribution.Parameters = main.Parameters;
            distribution.Type = type;
            distribution.Name = name;
            distribution.Documentation = documentation;


            foreach (var parameter in distribution.Constructor.Parameters)
                parameter.ValueChanged += distribution.distribution_OnParameterChanged;

            distribution.Options = DistributionManager.GetFittingOptions(distribution.Type);

            return true;
        }


        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return Name;
        }



        private void distribution_OnParameterChanged(object sender, EventArgs e)
        {
            update(); // When a parameter has changed, we have to re-recreate the distribution.
        }

        private void update()
        {
            var instance = Constructor.Activate();

            if (instance != null)
                update(instance);

            IsInitializing = false;
            IsInitialized = true;
        }

        private void update(IUnivariateDistribution instance)
        {
            this.Instance = instance;
            this.Measures.Instance = instance;
            this.DensityFunction = this.Measures.CreatePDF();

            foreach (var property in Properties)
                property.Update();

            foreach (var param in Parameters)
            {
                param.ValueChanged -= distribution_OnParameterChanged;
                param.Sync();
                param.ValueChanged += distribution_OnParameterChanged;
            }
        }

    }
}
