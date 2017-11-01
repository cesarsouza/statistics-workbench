// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Math;
    using System;
    using System.Collections.ObjectModel;
    using Workbench.Framework;
    using Workbench.Tools;

    /// <summary>
    ///   Main view model associated with the main application window.
    /// </summary>
    /// 
    public class MainViewModel : ViewModelBase
    {
        private int selectedIndex = 0;


        /// <summary>
        ///   Occurs when the current selected distribution changes.
        /// </summary>
        /// 
        public event EventHandler SelectedDistributionChanged;

        /// <summary>
        ///   Gets the ViewModels for all distributions that can be selected.
        /// </summary>
        /// 
        public ObservableCollection<DistributionViewModel> Distributions { get; private set; }

        /// <summary>
        ///   Gets or sets the index of the current selected distribution.
        /// </summary>
        /// 
        public int SelectedDistributionIndex
        {
            get { return selectedIndex; }
            set { OnDistributionChanged(value); }
        }

        private void OnDistributionChanged(int value)
        {
            // Unwire previous distribution
            if (SelectedDistribution != null)
                SelectedDistribution.Initialized -= SelectedDistribution_Initialized;

            selectedIndex = value;
            SelectedDistribution = Distributions[selectedIndex];

            if (SelectedDistribution.IsInitialized)
            {
                this.Analysis.SelectedDistribution = SelectedDistribution;
            }
            else
            {
                SelectedDistribution.Initialized += SelectedDistribution_Initialized;
                SelectedDistribution.Activate();
            }

            if (SelectedDistributionChanged != null)
                SelectedDistributionChanged(this, EventArgs.Empty);
        }

        void SelectedDistribution_Initialized(object sender, EventArgs e)
        {
            this.Analysis.SelectedDistribution = SelectedDistribution;
        }

        /// <summary>
        ///   Gets the current selected distribution.
        /// </summary>
        /// 
        public DistributionViewModel SelectedDistribution { get; private set; }

        /// <summary>
        ///   Gets the ViewModel for estimating distributions.
        /// </summary>
        /// 
        public EstimateViewModel Estimate { get; private set; }

        /// <summary>
        ///   Gets the ViewModel for analyzing a distribution's density function.
        /// </summary>
        /// 
        public AnalysisViewModel Analysis { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// 
        public MainViewModel()
        {
            this.Estimate = new EstimateViewModel(this);
            this.Analysis = new AnalysisViewModel();

            // Create ViewModels for each statistical distribution
            var distributions = DistributionManager.GetDistributions(this);
            this.Distributions = new ObservableCollection<DistributionViewModel>(distributions);
            this.SelectedDistributionIndex = distributions.Find(x => x.Name == "Normal")[0];
        }

    }
}
