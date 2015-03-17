// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Math;
    using PropertyChanged;
    using System.Collections.ObjectModel;
    using Workbench.Framework;
    using Workbench.Tools;

    /// <summary>
    ///   Main view model associated with the main application window.
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        ///   Gets the ViewModels for all distributions that can be selected.
        /// </summary>
        /// 
        public ObservableCollection<DistributionViewModel> Distributions { get; private set; }

        /// <summary>
        ///   Gets or sets the index of the current selected distribution.
        /// </summary>
        /// 
        public int SelectedDistributionIndex { get; set; }

        /// <summary>
        ///   Gets the current selected distribution.
        /// </summary>
        /// 
        public DistributionViewModel SelectedDistribution
        {
            get
            {
                Distributions[SelectedDistributionIndex].InitAsync();
                return Distributions[SelectedDistributionIndex];
            }
        }

        /// <summary>
        /// Gets the ViewModel for estimating distributions.
        /// </summary>
        public EstimateViewModel Estimate { get; private set; }

        public AnalysisViewModel Analysis { get ; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// 
        public MainViewModel()
        {
            // Create ViewModels for each statistical distribution
            var distributions = DistributionManager.GetDistributions(this);
            this.Distributions = new ObservableCollection<DistributionViewModel>(distributions);
            this.SelectedDistributionIndex = distributions.Find(x => x.Name == "Normal")[0];

            this.Estimate = new EstimateViewModel(this);
            this.Analysis = new AnalysisViewModel(this);
        }

    }
}
