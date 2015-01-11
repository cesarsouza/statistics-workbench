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
    using Accord.Statistics.Distributions.Univariate;
    using AForge;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using PropertyChanged;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///   View Model for the distribution's function details page. Includes
    ///   charts models for each possible function associated with a distribution,
    ///   such as its PDF, CDF, HF, and more.
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public class FunctionViewModel
    {

        private IUnivariateDistribution instance;

        private DoubleRange range;
        private DoubleRange unit;
        private double[] supportPoints;
        private double[] probabilities;


        /// <summary>
        ///   Gets the current instance of the selected distribution
        ///   that is currently active in the application.
        /// </summary>
        /// 
        public IUnivariateDistribution Instance
        {
            get { return instance; }
            set { update(value); }
        }




        public PlotModel DistributionFunction { get; private set; }

        public PlotModel DensityFunction { get; private set; }

        public PlotModel LogDensityFunction { get; private set; }

        public PlotModel InverseDistributionFunction { get; private set; }

        public PlotModel HazardFunction { get; private set; }

        public PlotModel CumulativeHazardFunction { get; private set; }

        public PlotModel QuantileDensityFunction { get; private set; }

        public PlotModel ComplementaryDistributionFunction { get; private set; }


        public FunctionViewModel()
        {
            DensityFunction = new PlotModel();
        }







        public PlotModel CreatePDF()
        {
            string title = "PDF";

            double[] x = supportPoints;
            double[] y;

            try { y = x.Apply(Instance.ProbabilityFunction); }
            catch
            {
                var general = GeneralContinuousDistribution
                    .FromDistributionFunction(Instance.Support, Instance.DistributionFunction);
                y = x.Apply(general.ProbabilityDensityFunction);
            }

            return createBaseModel(range, title, supportPoints, y);
        }

        public PlotModel CreateLPDF()
        {
            string title = "Log-PDF";

            double[] x = supportPoints;
            double[] y;

            try { y = x.Apply(Instance.LogProbabilityFunction); }
            catch
            {
                var general = GeneralContinuousDistribution
                    .FromDistributionFunction(Instance.Support, Instance.DistributionFunction);
                y = x.Apply(general.LogProbabilityDensityFunction);
            }

            return createBaseModel(range, title, supportPoints, y);
        }

        public PlotModel CreateIPDF()
        {
            try
            {
                string title = "IPDF";

                double[] x = probabilities;
                double[] y;

                try { y = x.Apply(Instance.QuantileDensityFunction); }
                catch
                {
                    var general = GeneralContinuousDistribution
                        .FromDistributionFunction(Instance.Support, Instance.DistributionFunction);
                    y = x.Apply(general.QuantileDensityFunction);
                }

                return createBaseModel(unit, title, x, y);
            }
            catch
            {
                return null;
            }
        }

        public PlotModel CreateCDF()
        {
            try
            {
                string title = "CDF";

                double[] x = supportPoints;

                double[] y;
                try { y = x.Apply(Instance.DistributionFunction); }
                catch
                {
                    var general = GeneralContinuousDistribution
                        .FromDensityFunction(Instance.Support, Instance.ProbabilityFunction);
                    y = x.Apply(general.DistributionFunction);
                }

                return createBaseModel(range, title, x, y);
            }
            catch
            {
                return null;
            }
        }

        public PlotModel CreateCCDF()
        {
            string title = "CCDF";

            double[] x = supportPoints;

            double[] y;
            try { y = x.Apply(Instance.ComplementaryDistributionFunction); }
            catch
            {
                var general = GeneralContinuousDistribution
                    .FromDensityFunction(Instance.Support, Instance.ProbabilityFunction);
                y = x.Apply(general.ComplementaryDistributionFunction);
            }

            return createBaseModel(range, title, x, y);
        }

        public PlotModel CreateCHF()
        {
            string title = "CHF";

            double[] x = supportPoints;

            double[] y;
            try { y = x.Apply(Instance.CumulativeHazardFunction); }
            catch
            {
                try
                {
                    var general = GeneralContinuousDistribution
                        .FromDensityFunction(Instance.Support, Instance.ProbabilityFunction);
                    y = x.Apply(general.CumulativeHazardFunction);
                }
                catch
                {
                    var general = GeneralContinuousDistribution
                        .FromDistributionFunction(Instance.Support, Instance.DistributionFunction);
                    y = x.Apply(general.CumulativeHazardFunction);
                }
            }

            return createBaseModel(range, title, x, y);
        }

        public PlotModel CreateICDF()
        {
            try
            {
                string title = "QDF";

                double[] x = probabilities;

                double[] y;
                try { y = x.Apply(Instance.InverseDistributionFunction); }
                catch
                {
                    try
                    {
                        var general = GeneralContinuousDistribution
                            .FromDensityFunction(Instance.Support, Instance.ProbabilityFunction);
                        y = x.Apply(general.InverseDistributionFunction);
                    }
                    catch
                    {
                        var general = GeneralContinuousDistribution
                            .FromDistributionFunction(Instance.Support, Instance.DistributionFunction);
                        y = x.Apply(general.InverseDistributionFunction);
                    }
                }

                return createBaseModel(unit, title, x, y);
            }
            catch
            {
                return null;
            }
        }

        public PlotModel CreateHF()
        {
            try
            {
                string title = "HF";

                double[] x = supportPoints;

                double[] y;
                try { y = x.Apply(Instance.HazardFunction); }
                catch
                {
                    try
                    {
                        var general = GeneralContinuousDistribution
                            .FromDensityFunction(Instance.Support, Instance.ProbabilityFunction);
                        y = x.Apply(general.HazardFunction);
                    }
                    catch
                    {
                        var general = GeneralContinuousDistribution
                            .FromDistributionFunction(Instance.Support, Instance.DistributionFunction);
                        y = x.Apply(general.HazardFunction);
                    }
                }

                return createBaseModel(range, title, x, y);
            }
            catch
            {
                return null;
            }
        }





        private PlotModel createBaseModel(DoubleRange range, string title, double[] x, double[] y)
        {
            var plotModel = new PlotModel();
            plotModel.Series.Clear();
            plotModel.Axes.Clear();

            var dateAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = range.Min,
                Maximum = range.Max,
                Key = "xAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80
            };

            plotModel.Axes.Add(dateAxis);

            double ymin = y.FirstOrDefault(a => !Double.IsNaN(a) && !Double.IsInfinity(a));
            double ymax = ymin;

            for (int i = 0; i < x.Length; i++)
            {
                if (Double.IsNaN(y[i]) || Double.IsInfinity(y[i]))
                    continue;

                if (y[i] > ymax)
                    ymax = y[i];
                if (y[i] < ymin)
                    ymin = y[i];
            }

            double maxGrace = ymax * 0.1;
            double minGrace = ymin * 0.1;

            var valueAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = ymin - minGrace,
                Maximum = ymax + maxGrace,
                Key = "yAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = title
            };

            plotModel.Axes.Add(valueAxis);

            var lineSeries = new LineSeries
            {
                YAxisKey = valueAxis.Key,
                XAxisKey = dateAxis.Key,
                StrokeThickness = 2,
                MarkerSize = 3,
                MarkerStroke = OxyColor.FromRgb(0, 0, 0),
                MarkerType = MarkerType.None,
                CanTrackerInterpolatePoints = true,
                Smooth = true,
            };



            for (int i = 0; i < x.Length; i++)
            {
                if (Double.IsNaN(y[i]) || Double.IsInfinity(y[i]))
                    continue;

                lineSeries.Points.Add(new DataPoint(x[i], y[i]));
            }

            plotModel.TitlePadding = 2;

            var formattable = Instance as IFormattable;

            if (formattable != null)
            {
                plotModel.Title = formattable.ToString("G3", CultureInfo.CurrentUICulture);
            }
            else
            {
                plotModel.Title = Instance.ToString();
            }

            plotModel.TitleFontSize = 15;
            plotModel.TitleFontWeight = 1;
            plotModel.TitlePadding = 2;

            plotModel.Series.Add(lineSeries);

            return plotModel;
        }



        private void update(IUnivariateDistribution instance)
        {
            this.instance = instance;
            this.updateRange();
            this.DensityFunction = CreatePDF();
            this.DistributionFunction = CreateCDF();
            this.ComplementaryDistributionFunction = CreateCCDF();
            this.CumulativeHazardFunction = CreateCHF();
            this.HazardFunction = CreateHF();
            this.InverseDistributionFunction = CreateICDF();
            this.LogDensityFunction = CreateLPDF();
            this.QuantileDensityFunction = CreateIPDF();
        }

        private void updateRange()
        {
            DoubleRange range = new DoubleRange(0, 1);

            try
            {
                range = Instance.Support;
                range = Instance.GetRange(0.99);
            }
            catch
            {
            }

            this.range = new DoubleRange(range.Min - 5, range.Max + 5);
            this.unit = new DoubleRange(0, 1);
            this.supportPoints = Matrix.Interval(range.Min, range.Max, (range.Length) / 1000.0);
            this.probabilities = Matrix.Interval(0.0, 1.0, 1000);
        }

    }
}
