// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Statistics.Testing;
    using OxyPlot;
    using OxyPlot.Series;
    using PropertyChanged;
    using System.ComponentModel;
    using Workbench.Framework;


    [ImplementPropertyChanged]
    public class AnalysisViewModel : ViewModelBase
    {

        private const string Between = "≤ X <";
        private const string EqualTo = "= X";
        private const string GreaterThan = "> X";
        private const string LessThanOrEqualThan = "≤ X";

        private int selectedIndex;
        private double leftValue;
        private double rightValue;
        private double probability;

        private bool automatic;

        /// <summary>
        ///   Gets a reference for the parent <see cref="MainViewModel"/>.
        /// </summary>
        /// 
        public MainViewModel Owner { get; private set; }


        public DistributionTail Tail { get; set; }

        public double LeftValue
        {
            get { return leftValue; }
            set
            {
                if (leftValue != value)
                {
                    leftValue = value;
                    intervalChanged();
                }
            }
        }

        public double RightValue
        {
            get { return rightValue; }
            set
            {
                if (rightValue != value)
                {
                    if (rightValue < leftValue)
                        value = leftValue;
                    rightValue = value;
                    intervalChanged();
                }
            }
        }

        public bool RightValueVisible
        {
            get { return Comparisons[selectedIndex] == Between; }
        }

        public double Probability
        {
            get { return probability; }
            set
            {
                if (probability != value)
                {
                    probability = value;
                    probabilityChanged();
                }
            }
        }

        public double ValueStep { get; set; }

        public int ComparisonIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                intervalChanged();
            }
        }

        public BindingList<string> Comparisons { get; private set; }


        public PlotModel DensityFunction { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MeasuresViewModel"/> class.
        /// </summary>
        /// 
        public AnalysisViewModel(MainViewModel owner)
        {
            this.Owner = owner;

            Comparisons = new BindingList<string>()
            {
                LessThanOrEqualThan, Between, EqualTo, GreaterThan
            };

            selectedIndex = 0;
        }

        private void intervalChanged()
        {
            if (automatic)
                return;

            try
            {
                automatic = true;
                var instance = Owner.SelectedDistribution.Instance;
                string comparison = Comparisons[selectedIndex];

                switch (comparison)
                {
                    case LessThanOrEqualThan:
                        Probability = instance.DistributionFunction(LeftValue);
                        break;
                    case GreaterThan:
                        Probability = instance.ComplementaryDistributionFunction(LeftValue);
                        break;
                    case EqualTo:
                        Probability = instance.ProbabilityFunction(LeftValue);
                        break;
                    case Between:
                        Probability = instance.DistributionFunction(LeftValue, RightValue);
                        break;
                    default:
                        break;
                }

                Update();
            }
            finally
            {
                automatic = false;
            }
        }

        private void probabilityChanged()
        {
            if (automatic)
                return;

            try
            {
                automatic = true;
                var instance = Owner.SelectedDistribution.Instance;
                string comparison = Comparisons[selectedIndex];

                RightValue = 0;

                switch (comparison)
                {
                    case Between:
                    case LessThanOrEqualThan:
                        LeftValue = instance.InverseDistributionFunction(Probability);
                        break;
                    case GreaterThan:
                        LeftValue = instance.InverseDistributionFunction(1.0 - Probability);
                        break;
                    case EqualTo:
                        LeftValue = instance.QuantileDensityFunction(Probability);
                        break;
                    default:
                        break;
                }

                Update();
            }
            finally
            {
                automatic = false;
            }
        }

        public void Update()
        {
            var instance = Owner.SelectedDistribution.Instance;
            var plot = Owner.SelectedDistribution.Measures.CreatePDF();

            ValueStep = Owner.SelectedDistribution.Measures.Range.Length / 100.0;

            string comparison = Comparisons[selectedIndex];

            var x = Owner.SelectedDistribution.Measures.XAxis;
            var y = Owner.SelectedDistribution.Measures.YAxis;

            if (comparison == EqualTo)
            {
                var area = new LineSeries();
                area.XAxisKey = "xAxis";
                area.YAxisKey = "yAxis";
                area.MarkerType = MarkerType.Circle;
                area.MarkerSize = 10;
                area.Points.Add(new DataPoint(LeftValue, Probability));
                plot.Series.Add(area);
            }
            else
            {
                var area = new AreaSeries();
                area.XAxisKey = "xAxis";
                area.YAxisKey = "yAxis";
                area.Fill = OxyColor.FromRgb(250, 0, 0);

                switch (comparison)
                {
                    case LessThanOrEqualThan:
                        for (int i = 0; i < x.Length; i++)
                            if (x[i] <= LeftValue)
                                area.Points.Add(new DataPoint(x[i], y[i]));
                        break;
                    case GreaterThan:
                        for (int i = 0; i < x.Length; i++)
                            if (x[i] > LeftValue)
                                area.Points.Add(new DataPoint(x[i], y[i]));
                        break;
                    case Between:
                        for (int i = 0; i < x.Length; i++)
                            if (x[i] >= LeftValue && x[i] < RightValue)
                                area.Points.Add(new DataPoint(x[i], y[i]));
                        break;
                    default:
                        break;
                }

                foreach (var point in area.Points)
                    area.Points2.Add(new DataPoint(point.X, 0));

                if (area.Points.Count > 0)
                    plot.Series.Add(area);
            }

            DensityFunction = plot;
        }
    }
}
