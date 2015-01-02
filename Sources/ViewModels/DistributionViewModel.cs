using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Accord.Math;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Univariate;
using AForge;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PropertyChanged;
using Statistics_Workbench.Models;

namespace Statistics_Workbench.ViewModels
{
    [ImplementPropertyChanged]
    public class DistributionViewModel
    {

        public IUnivariateDistribution Instance { get; private set; }

        public IFittingOptions Options { get; private set; }


        public DistributionInfo Distribution { get; private set; }

        public ObservableCollection<DistributionParameterInfo> Parameters { get; private set; }

        public ObservableCollection<DistributionPropertyInfo> Properties { get; private set; }


        public bool CanGenerate { get; private set; }

        public string Name { get; private set; }

        public string Documentation { get; private set; }

        public string ExternalDocUrl { get; private set; }

        public FunctionViewModel Functions { get; private set; }

        public PlotModel DensityFunction { get; private set; }

        public DistributionViewModel()
        {
            this.Parameters = new ObservableCollection<DistributionParameterInfo>();
            this.Properties = new ObservableCollection<DistributionPropertyInfo>();
            this.Functions = new FunctionViewModel();
            
        }

        public DistributionViewModel(MainViewModel parent, DistributionInfo distribution)
            : this()
        {
            this.Distribution = distribution;
            this.Name = Distribution.Name;
            this.Parameters = distribution.Constructor.Parameters;
            this.Properties = distribution.Properties;
            this.Documentation = distribution.Summary;
            this.CanGenerate = distribution.CanGenerate;

            // Get documentation page from the Accord.NET website
            this.ExternalDocUrl = DistributionManager.GetDocumentationUrl(distribution.Type);

            // Get the constructor parameters and their ranges (if possible)
            foreach (var parameter in distribution.Constructor.Parameters)
                parameter.PropertyChanged += OnParameterChanged;

            this.Options = DistributionManager.GetFittingOptions(distribution.Type);


            Task.Run((Action)Create);
        }

        void OnParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            Create(); // When a parameter has changed, we have to re-recreate the distribution.
        }

        public void Create()
        {
            IUnivariateDistribution instance;
            if (Distribution.Constructor.TryCreate(out instance))
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
