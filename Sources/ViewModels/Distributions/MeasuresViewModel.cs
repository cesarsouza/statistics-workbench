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
    public class MeasuresViewModel
    {

        private IUnivariateDistribution instance;

        private DoubleRange range;
        private DoubleRange unit;
        private double[] supportPoints;
        private double[] probabilities;

        private double[] pdf;

        /// <summary>
        ///   Gets the current instance of the selected distribution
        ///   that is currently active in the application.
        /// </summary>
        /// 
        public void Update(IUnivariateDistribution instance)
        {
            update(instance);
        }

        public DoubleRange Range { get { return range; } }

        public double[] XAxis { get { return supportPoints; } }

        public double[] YAxis { get { return pdf; } }

        /// <summary>
        ///   Data-bindable plot for the distribution's Distribution Function (CDF).
        /// </summary>
        /// 
        public PlotModel DistributionFunction { get; private set; }

        /// <summary>
        ///   Data-bindable plot for the distribution's Density Function (PDF).
        /// </summary>
        /// 
        public PlotModel DensityFunction { get; private set; }

        /// <summary>
        ///   Data-bindable plot for the distribution's Log(PDF(x)) Function.
        /// </summary>
        /// 
        public PlotModel LogDensityFunction { get; private set; }

        /// <summary>
        ///   Data-bindable plot for the distribution's Quantile Function (QF).
        /// </summary>
        /// 
        public PlotModel InverseDistributionFunction { get; private set; }

        /// <summary>
        ///   Data-bindable plot for the distribution's Hazard Function (HF).
        /// </summary>
        /// 
        public PlotModel HazardFunction { get; private set; }

        /// <summary>
        ///   Data-bindable plot for the distribution's Cumulative Hazard Function (CHF).
        /// </summary>
        /// 
        public PlotModel CumulativeHazardFunction { get; private set; }

        /// <summary>
        ///   Data-bindable plot for the distribution's Quantile Density Function (QDF).
        /// </summary>
        /// 
        public PlotModel QuantileDensityFunction { get; private set; }

        /// <summary>
        ///   Data-bindable plot for the distribution's Complementary Distribution Function (CCDF).
        /// </summary>
        /// 
        public PlotModel ComplementaryDistributionFunction { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="MeasuresViewModel"/> class.
        /// </summary>
        /// 
        public MeasuresViewModel()
        {
            DensityFunction = new PlotModel();
        }




        /// <summary>
        ///   Creates a OxyPlot's graph for the Probability Density Function.
        /// </summary>
        ///
        public PlotModel CreatePDF()
        {
            try { pdf = supportPoints.Apply(instance.ProbabilityFunction); }
            catch
            {
                var general = GeneralContinuousDistribution
                    .FromDistributionFunction(instance.Support, instance.DistributionFunction);
                pdf = supportPoints.Apply(general.ProbabilityDensityFunction);
            }

            correctDiscrete(pdf, 0);

            return createBaseModel(range, "PDF", supportPoints, pdf);
        }

        /// <summary>
        ///   Creates a OxyPlot's graph for the Log Probability Density Function.
        /// </summary>
        ///
        public PlotModel CreateLPDF()
        {
            double[] y;
            try { y = supportPoints.Apply(instance.LogProbabilityFunction); }
            catch
            {
                var general = GeneralContinuousDistribution
                    .FromDistributionFunction(instance.Support, instance.DistributionFunction);
                y = supportPoints.Apply(general.LogProbabilityDensityFunction);
            }

            correctDiscrete(y, Double.NegativeInfinity);

            return createBaseModel(range, "Log-PDF", supportPoints, y);
        }

        /// <summary>
        ///   Creates a OxyPlot's graph for the Inverse Probability Density Function.
        /// </summary>
        /// 
        public PlotModel CreateIPDF()
        {
            try
            {
                double[] y;
                try { y = probabilities.Apply(instance.QuantileDensityFunction); }
                catch
                {
                    var general = GeneralContinuousDistribution
                        .FromDistributionFunction(instance.Support, instance.DistributionFunction);
                    y = probabilities.Apply(general.QuantileDensityFunction);
                }

                return createBaseModel(unit, "IPDF", probabilities, y);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///   Creates a OxyPlot's graph for the Cumulative Distribution Function.
        /// </summary>
        /// 
        public PlotModel CreateCDF()
        {
            try
            {
                double[] y;
                try { y = supportPoints.Apply(instance.DistributionFunction); }
                catch
                {
                    var general = GeneralContinuousDistribution
                        .FromDensityFunction(instance.Support, instance.ProbabilityFunction);
                    y = supportPoints.Apply(general.DistributionFunction);
                }

                correctDiscrete(y, 0);

                return createBaseModel(range, "CDF", supportPoints, y);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///   Creates a OxyPlot's graph for the Complementary Cumulative Distribution Function.
        /// </summary>
        /// 
        public PlotModel CreateCCDF()
        {
            double[] y;
            try { y = supportPoints.Apply(instance.ComplementaryDistributionFunction); }
            catch
            {
                var general = GeneralContinuousDistribution
                    .FromDensityFunction(instance.Support, instance.ProbabilityFunction);
                y = supportPoints.Apply(general.ComplementaryDistributionFunction);
            }

            correctDiscrete(y, 0);

            return createBaseModel(range, "CCDF", supportPoints, y);
        }

        /// <summary>
        ///   Creates a OxyPlot's graph for the Cumulative Hazard Function.
        /// </summary>
        /// 
        public PlotModel CreateCHF()
        {
            double[] y;
            try { y = supportPoints.Apply(instance.CumulativeHazardFunction); }
            catch
            {
                try
                {
                    var general = GeneralContinuousDistribution
                        .FromDensityFunction(instance.Support, instance.ProbabilityFunction);
                    y = supportPoints.Apply(general.CumulativeHazardFunction);
                }
                catch
                {
                    var general = GeneralContinuousDistribution
                        .FromDistributionFunction(instance.Support, instance.DistributionFunction);
                    y = supportPoints.Apply(general.CumulativeHazardFunction);
                }
            }

            return createBaseModel(range, "CHF", supportPoints, y);
        }

        /// <summary>
        ///   Creates a OxyPlot's graph for the Quantile Function.
        /// </summary>
        /// 
        public PlotModel CreateICDF()
        {
            try
            {
                double[] y;
                try { y = probabilities.Apply(instance.InverseDistributionFunction); }
                catch
                {
                    try
                    {
                        var general = GeneralContinuousDistribution
                            .FromDensityFunction(instance.Support, instance.ProbabilityFunction);
                        y = probabilities.Apply(general.InverseDistributionFunction);
                    }
                    catch
                    {
                        var general = GeneralContinuousDistribution
                            .FromDistributionFunction(instance.Support, instance.DistributionFunction);
                        y = probabilities.Apply(general.InverseDistributionFunction);
                    }
                }

                return createBaseModel(unit, "QDF", probabilities, y);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///   Creates a OxyPlot's graph for the Hazard Function.
        /// </summary>
        ///
        public PlotModel CreateHF()
        {
            try
            {
                double[] y;
                try { y = supportPoints.Apply(instance.HazardFunction); }
                catch
                {
                    try
                    {
                        var general = GeneralContinuousDistribution
                            .FromDensityFunction(instance.Support, instance.ProbabilityFunction);
                        y = supportPoints.Apply(general.HazardFunction);
                    }
                    catch
                    {
                        var general = GeneralContinuousDistribution
                            .FromDistributionFunction(instance.Support, instance.DistributionFunction);
                        y = supportPoints.Apply(general.HazardFunction);
                    }
                }

                return createBaseModel(range, "HF", supportPoints, y);
            }
            catch
            {
                return null;
            }
        }





        private PlotModel createBaseModel(DoubleRange? range, string title, double[] x, double[] y)
        {
            var plotModel = new PlotModel();
            plotModel.Series.Clear();
            plotModel.Axes.Clear();

            var xAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = range.Value.Min,
                Maximum = range.Value.Max,
                Key = "xAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80
            };

            plotModel.Axes.Add(xAxis);

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

            var yAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = ymin - minGrace,
                Maximum = ymax + maxGrace,
                Key = "yAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = title
            };

            plotModel.Axes.Add(yAxis);

            var lineSeries = new LineSeries
            {
                YAxisKey = yAxis.Key,
                XAxisKey = xAxis.Key,
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

            var formattable = instance as IFormattable;

            if (formattable != null)
            {
                plotModel.Title = formattable.ToString("G3", CultureInfo.CurrentUICulture);
            }
            else
            {
                plotModel.Title = instance.ToString();
            }

            plotModel.TitleFontSize = 15;
            plotModel.TitleFontWeight = 1;
            plotModel.TitlePadding = 2;

            plotModel.Series.Add(lineSeries);

            return plotModel;
        }

        private void correctDiscrete(double[] y, double zero)
        {
            if (instance is UnivariateDiscreteDistribution)
            {
                for (int i = 0; i < supportPoints.Length; i++)
                {
                    if (!supportPoints[i].IsInteger(1e-5))
                        y[i] = zero;
                }
            }
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
            double resolution = 100;

            range = new DoubleRange(0, 1);

            try
            {
                range = instance.Support;
                range = instance.GetRange(0.99);
            }
            catch
            {
            }

            if (range.Length == 0)
                range = new DoubleRange(instance.Mean - 1, instance.Mean + 1);


            double min = range.Min - Math.Abs(range.Length) * 0.1;
            double max = range.Max + Math.Abs(range.Length) * 0.1;

            this.range = new DoubleRange(min, max);
            this.unit = new DoubleRange(0, 1);
            this.probabilities = Matrix.Interval(0.0, 1.0, 1.0 / resolution);

            this.supportPoints = Matrix.Interval(range.Min, range.Max, range.Length / resolution);

            // make sure the support points include the important metrics
            try { this.supportPoints = supportPoints.Concatenate(instance.Mean); }
            catch { };
            try { this.supportPoints = supportPoints.Concatenate(instance.Median); }
            catch { };
            try { this.supportPoints = supportPoints.Concatenate(instance.Mode); }
            catch { };

            Array.Sort(supportPoints);
        }

    }
}
