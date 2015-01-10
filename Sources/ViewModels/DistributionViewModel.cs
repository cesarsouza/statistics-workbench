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
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Workbench.Tools;

    [ImplementPropertyChanged]
    public class DistributionViewModel
    {

        public IUnivariateDistribution Instance { get; private set; }

        public IFittingOptions Options { get; private set; }

        public ObservableCollection<ParameterViewModel> Parameters { get; private set; }

        public ObservableCollection<PropertyViewModel> Properties { get; private set; }


        public bool CanGenerate { get; private set; }

        public string Name { get; private set; }

        public DocumentationViewModel Documentation { get; private set; }

        public string ExternalDocUrl { get; private set; }

        public Type Type { get; private set; }

        public ConstructorViewModel Constructor { get; private set; }

        public FunctionViewModel Functions { get; private set; }

        public PlotModel DensityFunction { get; private set; }

        public DistributionViewModel()
        {
            this.Parameters = new ObservableCollection<ParameterViewModel>();
            this.Properties = new ObservableCollection<PropertyViewModel>();
            this.Functions = new FunctionViewModel();
            
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
            distribution.CanGenerate = typeof(ISampleableDistribution<double>).IsAssignableFrom(type);
            

            // Get documentation page from the Accord.NET website
            distribution.ExternalDocUrl = DistributionManager.GetDocumentationUrl(distribution.Type);

            // Get the constructor parameters and their ranges (if possible)
            foreach (var parameter in distribution.Constructor.Parameters)
                parameter.PropertyChanged += distribution.OnParameterChanged;

            distribution.Options = DistributionManager.GetFittingOptions(distribution.Type);


            Task.Run((Action)distribution.Create);

            return true;
        }

        public override string ToString()
        {
            return Name;
        }

        void OnParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            Create(); // When a parameter has changed, we have to re-recreate the distribution.
        }

        public void Create()
        {
            IUnivariateDistribution instance;
            if (Constructor.TryCreate(out instance))
            {
                update(instance);
            }
        }

        public void Estimate(double[] values, double[] weights)
        {
            if (weights == null)
            {
                Instance.Fit(values, Options);
            }
            else
            {
                weights = weights.Divide(weights.Sum());
                Instance.Fit(values, weights, Options);
            }

            update(Instance);
        }

       
        private void update(IUnivariateDistribution instance)
        {
            this.Instance = instance;

            foreach (var prop in Properties)
                prop.Update(instance);

            Functions.Update(Instance);

            this.DensityFunction = Functions.CreatePDF();
        }
    }
}
