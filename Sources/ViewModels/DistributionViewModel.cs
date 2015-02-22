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
    using System.Threading;
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

        public bool IsInitialized { get; private set; }

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
        public IFittingOptions EstimationOptions { get; private set; }

        public bool HasOptions { get { return EstimationOptions != null; } }


        public string Name { get; private set; }


        public string ExternalDocUrl { get; private set; }

        public Type Type { get; private set; }

        public ConstructorViewModel Constructor { get; private set; }

        public FunctionViewModel Functions { get; private set; }

        public DocumentationViewModel Documentation { get; private set; }


        public PlotModel DensityFunction { get; private set; }



        public DistributionViewModel()
        {
            this.Parameters = new ObservableCollection<ParameterViewModel>();
            this.Properties = new ObservableCollection<PropertyViewModel>();
            this.Functions = new FunctionViewModel();
        }


        public void InitAsync()
        {
            if (!IsInitialized && !IsInitializing)
            {
                IsInitializing = true;
                Task.Factory.StartNew(() => this.Update());
            }
        }

        private void Update()
        {
            var instance = Constructor.Activate();

            if (instance != null)
                update(instance);

            IsInitializing = false;
            IsInitialized = true;
        }

        public void Estimate(double[] values, double[] weights)
        {
            if (weights == null)
            {
                Instance.Fit(values, EstimationOptions);
            }
            else
            {
                weights = weights.Divide(weights.Sum());
                Instance.Fit(values, weights, EstimationOptions);
            }

            update(Instance);
        }


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


            // Get documentation page from the Accord.NET website
            distribution.ExternalDocUrl = DistributionManager.GetDocumentationUrl(distribution.Type);

            foreach (var parameter in distribution.Constructor.Parameters)
                parameter.ValueChanged += distribution.OnParameterChanged;

            distribution.EstimationOptions = DistributionManager.GetFittingOptions(distribution.Type);

            // Task.Run((Action)distribution.Update);

            return true;
        }



        private void OnParameterChanged(object sender, EventArgs e)
        {
            Update(); // When a parameter has changed, we have to re-recreate the distribution.
        }




        public override string ToString()
        {
            return Name;
        }




        private void update(IUnivariateDistribution instance)
        {
            this.Instance = instance;
            this.Functions.Instance = instance;
            this.DensityFunction = this.Functions.CreatePDF();

            foreach (var property in Properties)
                property.Update();

            foreach (var param in Parameters)
            {
                param.ValueChanged -= OnParameterChanged;
                param.Sync();
                param.ValueChanged += OnParameterChanged;
            }
        }


    }
}
