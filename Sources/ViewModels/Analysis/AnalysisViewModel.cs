// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Statistics.Distributions.Univariate;
    using OxyPlot;
    using OxyPlot.Series;
    using System;
    using System.ComponentModel;
    using Workbench.Framework;

    /// <summary>
    ///   Shows details about a distribution's Probability Density Function. The user
    ///   can interact with the distribution's PDF and ask for different visualizations
    ///   given a range of inputs or probabilities.
    /// </summary>
    /// 
    public class AnalysisViewModel : ViewModelBase
    {
        /// <summary> Indicates X is between two values. </summary>
        public const string Between = " < X ≤ ";

        /// <summary> Indicates X is equal to a value. </summary>
        public const string EqualTo = "X = ";

        /// <summary> Indicates X is greater than a value. </summary>
        public const string GreaterThan = "X > ";

        /// <summary> Indicates X is less than a value. </summary>
        public const string LessThan = "X ≤ ";

        /// <summary> Indicates X is outside a value. </summary>
        public const string Outside = " < X ∪ X > ";

        private int selectedIndex;
        private double leftValue;
        private double rightValue;
        private double probability;
        private DistributionViewModel distribution;

        private bool automatic;


        /// <summary>
        ///   Gets a reference for the parent <see cref="MainViewModel"/>.
        /// </summary>
        /// 
        public DistributionViewModel SelectedDistribution
        {
            get { return distribution; }
            set { OnDistributionChanged(value); }
        }

        private void OnDistributionChanged(DistributionViewModel value)
        {
            if (!value.IsInitialized)
                throw new Exception();

            if (distribution != null)
                distribution.Updated -= distribution_DistributionUpdated;

            distribution = value;
            Probability = 0.5;
            updateInterval();
            updateChart();

            distribution.Updated += distribution_DistributionUpdated;
        }

        void distribution_DistributionUpdated(object sender, EventArgs e)
        {
            updateInterval();
            updateChart();
        }



        /// <summary>
        ///   Gets or sets the left hand side of the probability range 
        ///   specification, such as the 'A' in <c>p(A &lt; X &lt; B)</c>.
        /// </summary>
        /// 
        public double LeftValue
        {
            get { return leftValue; }
            set
            {
                if (leftValue != value)
                {
                    if (leftValue > rightValue)
                        value = rightValue;

                    leftValue = value;
                    updateProbability();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the right hand side of the probability range 
        ///   specification, such as the 'B' in <c>p(A &lt; X &lt; B)</c>.
        /// </summary>
        /// 
        public double RightValue
        {
            get { return rightValue; }
            set
            {
                if (rightValue != value)
                {
                    if (rightValue < leftValue)
                        LeftValue = value;

                    rightValue = value;
                    updateProbability();
                }
            }
        }

        /// <summary>
        ///   Gets whether the left hand side in the probability
        ///   range definition must be visible. This should be the
        ///   case only for between and outside, as in <c>a &lt; X &lt; b</c>
        ///   and <c>a > X + X > b</c>, but not in <c>X > a</c> or <c>X &lt; b</c>.
        /// </summary>
        /// 
        public bool LeftValueVisible
        {
            get
            {
                return Comparisons[selectedIndex] == Between
                    || Comparisons[selectedIndex] == Outside;
            }
        }

        /// <summary>
        ///   Gets or sets the probability of the selected range.
        /// </summary>
        /// 
        public double Probability
        {
            get { return probability; }
            set
            {
                if (probability != value)
                {
                    probability = value;
                    updateInterval();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the increment value for many controls that
        ///   accept inserting input points for the distribution's PDF.
        /// </summary>
        /// 
        public double ValueStep { get; set; }

        /// <summary>
        ///   Gets or sets which comparison operation should be done in the
        ///   probability calculation, such as '&lt;' in <c>p(X &lt; B)</c>.
        /// </summary>
        /// 
        public int ComparisonIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                updateProbability();
            }
        }

        /// <summary>
        ///   Gets the collection of possible comparisons that can be made.
        /// </summary>
        /// 
        public BindingList<string> Comparisons { get; private set; }

        /// <summary>
        ///   Gets the current plot for the distribution's probability density function.
        /// </summary>
        /// 
        public PlotModel DensityFunction { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MeasuresViewModel"/> class.
        /// </summary>
        /// 
        public AnalysisViewModel()
        {
            Comparisons = new BindingList<string>()
            {
                LessThan, GreaterThan, EqualTo, Between, Outside
            };

            selectedIndex = 0;
        }



        private void updateProbability()
        {
            if (automatic)
                return;

            try
            {
                automatic = true;
                var instance = SelectedDistribution.Instance;
                string comparison = Comparisons[selectedIndex];

                switch (comparison)
                {
                    case LessThan:
                        Probability = instance.DistributionFunction(RightValue);
                        break;
                    case GreaterThan:
                        Probability = instance.ComplementaryDistributionFunction(RightValue);
                        break;
                    case EqualTo:
                        Probability = instance.ProbabilityFunction(RightValue);
                        break;
                    case Between:
                        Probability = instance.DistributionFunction(LeftValue, RightValue);
                        break;
                    case Outside:
                        Probability = 1 - instance.DistributionFunction(LeftValue, RightValue);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                Probability = 0;
            }


            updateChart();

            automatic = false;
        }

        private void updateInterval()
        {
            if (automatic)
                return;

            try
            {
                automatic = true;
                var instance = SelectedDistribution.Instance;
                string comparison = Comparisons[selectedIndex];

                LeftValue = 0;
                RightValue = 0;

                switch (comparison)
                {
                    case Between:
                    case LessThan:
                        RightValue = instance.InverseDistributionFunction(Probability);
                        break;
                    case GreaterThan:
                        RightValue = instance.InverseDistributionFunction(1.0 - Probability);
                        break;
                    case EqualTo:
                        RightValue = instance.QuantileDensityFunction(Probability);
                        break;
                    default:
                        break;
                }

            }
            catch
            {
                RightValue = Double.NaN;
                LeftValue = Double.NaN;
            }

            updateChart();

            automatic = false;
        }

        private void updateChart()
        {
            var instance = SelectedDistribution.Instance;
            var plot = SelectedDistribution.Measures.CreatePDF();

            var colorTrue = OxyColor.FromRgb(250, 0, 0);
            var colorFalse = OxyColor.FromRgb(200, 200, 200);

            if (Double.IsNaN(RightValue))
                return;


            if (instance is UnivariateDiscreteDistribution)
            {
                ValueStep = 1;

                string comparison = Comparisons[selectedIndex];

                var x = SelectedDistribution.Measures.XAxis;
                var y = SelectedDistribution.Measures.YAxis;

                var series = plot.Series[0] as ColumnSeries;

                if (comparison == EqualTo)
                {
                    int index = Array.IndexOf(x, (int)RightValue);
                    if (index >= 0)
                        series.Items[index].Color = (index >= 0) ? colorTrue : colorFalse;
                }
                else
                {
                    switch (comparison)
                    {
                        case LessThan:
                            for (int i = 0; i < x.Length; i++)
                                series.Items[i].Color = (x[i] <= RightValue) ? colorTrue : colorFalse;
                            break;
                        case GreaterThan:
                            for (int i = 0; i < x.Length; i++)
                                series.Items[i].Color = (x[i] > RightValue) ? colorTrue : colorFalse;
                            break;
                        case Between:
                            for (int i = 0; i < x.Length; i++)
                                series.Items[i].Color = (x[i] > LeftValue && x[i] <= RightValue) ? colorTrue : colorFalse;
                            break;
                        case Outside:
                            for (int i = 0; i < x.Length; i++)
                                series.Items[i].Color = (x[i] <= LeftValue || x[i] > RightValue) ? colorTrue : colorFalse;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                ValueStep = SelectedDistribution.Measures.Range.Length / 100.0;

                string comparison = Comparisons[selectedIndex];

                var x = SelectedDistribution.Measures.XAxis;
                var y = SelectedDistribution.Measures.YAxis;

                if (comparison == EqualTo)
                {
                    var area = new LineSeries();
                    area.XAxisKey = "xAxis";
                    area.YAxisKey = "yAxis";
                    area.MarkerType = MarkerType.Circle;
                    area.MarkerSize = 10;
                    area.Points.Add(new DataPoint(RightValue, Probability));
                    plot.Series.Add(area);
                }
                else
                {
                    var left = new AreaSeries();
                    left.XAxisKey = "xAxis";
                    left.YAxisKey = "yAxis";
                    left.Fill = colorTrue;
                    left.Smooth = true;

                    var right = new AreaSeries();
                    right.XAxisKey = "xAxis";
                    right.YAxisKey = "yAxis";
                    right.Fill = colorTrue;
                    right.Smooth = true;

                    switch (comparison)
                    {
                        case LessThan:
                            for (int i = 0; i < x.Length; i++)
                                if (x[i] <= RightValue)
                                    left.Points.Add(new DataPoint(x[i], y[i]));
                            break;
                        case GreaterThan:
                            for (int i = 0; i < x.Length; i++)
                                if (x[i] > RightValue)
                                    left.Points.Add(new DataPoint(x[i], y[i]));
                            break;
                        case Between:
                            for (int i = 0; i < x.Length; i++)
                                if (x[i] >= LeftValue && x[i] < RightValue)
                                    left.Points.Add(new DataPoint(x[i], y[i]));
                            break;
                        case Outside:
                            for (int i = 0; i < x.Length; i++)
                            {
                                if (x[i] <= LeftValue)
                                    left.Points.Add(new DataPoint(x[i], y[i]));
                                if (x[i] >= RightValue)
                                    right.Points.Add(new DataPoint(x[i], y[i]));
                            }
                            break;
                        default:
                            break;
                    }

                    foreach (var point in left.Points)
                        left.Points2.Add(new DataPoint(point.X, 0));

                    foreach (var point in right.Points)
                        right.Points2.Add(new DataPoint(point.X, 0));

                    if (left.Points.Count > 0)
                        plot.Series.Add(left);

                    if (right.Points.Count > 0)
                        plot.Series.Add(right);
                }
            }

            this.DensityFunction = plot;
        }


    }
}
