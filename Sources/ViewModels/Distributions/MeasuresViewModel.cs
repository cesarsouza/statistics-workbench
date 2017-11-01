// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///   View Model for the distribution's function details page. Includes
    ///   charts models for each possible function associated with a distribution,
    ///   such as its PDF, CDF, HF, and more.
    /// </summary>
    /// 
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

        /// <summary>
        ///   Gets a suitable finite support interval for the distribution.
        /// </summary>
        /// 
        public DoubleRange Range { get { return range; } }

        /// <summary>
        ///   Gets a set of suitable input points in the distribution's domain.
        /// </summary>
        /// 
        public double[] XAxis { get { return supportPoints; } }

        /// <summary>
        ///   Gets the distribution probabilities (the output of the pdf)
        ///   for the input points in <see cref="XAxis"/>.
        /// </summary>
        /// 
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

            return createBaseModel(range, "PDF", supportPoints, pdf, instance is UnivariateDiscreteDistribution);
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

            return createBaseModel(range, "Log-PDF", supportPoints, y, instance is UnivariateDiscreteDistribution);
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

                return createBaseModel(unit, "IPDF", probabilities, y, false);
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
                try { y = supportPoints.Apply((x) => instance.DistributionFunction(x)); }
                catch
                {
                    var general = GeneralContinuousDistribution
                        .FromDensityFunction(instance.Support, instance.ProbabilityFunction);
                    y = supportPoints.Apply((x) => general.DistributionFunction(x));
                }

                return createBaseModel(range, "CDF", supportPoints, y, instance is UnivariateDiscreteDistribution);
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

            return createBaseModel(range, "CCDF", supportPoints, y, instance is UnivariateDiscreteDistribution);
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

            return createBaseModel(range, "CHF", supportPoints, y, instance is UnivariateDiscreteDistribution);
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

                return createBaseModel(unit, "QDF", probabilities, y, false);
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

                return createBaseModel(range, "HF", supportPoints, y, instance is UnivariateDiscreteDistribution);
            }
            catch
            {
                return null;
            }
        }





        private PlotModel createBaseModel(DoubleRange? range, string title, double[] x, double[] y, bool discrete)
        {
            var plotModel = new PlotModel();
            plotModel.Series.Clear();
            plotModel.Axes.Clear();

            double ymin = y.FirstOrDefault(a => !Double.IsNaN(a) && !Double.IsInfinity(a));
            double ymax = ymin;

            for (int i = 0; i < y.Length; i++)
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


            if (!discrete)
            {
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

                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);

                var lineSeries = new LineSeries
                {
                    YAxisKey = yAxis.Key,
                    XAxisKey = xAxis.Key,
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    MarkerStroke = OxyColor.FromRgb(0, 0, 0),
                    MarkerType = MarkerType.None,
                    Smooth = true,
                };

                for (int i = 0; i < x.Length; i++)
                {
                    if (Double.IsNaN(y[i]) || Double.IsInfinity(y[i]))
                        continue;

                    lineSeries.Points.Add(new DataPoint(x[i], y[i]));
                }

                plotModel.Series.Add(lineSeries);
            }
            else
            {
                var xAxis = new OxyPlot.Axes.CategoryAxis()
                {
                    Position = AxisPosition.Bottom,
                    Key = "xAxis",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                };

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

                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);

                var boxSeries = new ColumnSeries
                {
                    YAxisKey = yAxis.Key,
                    XAxisKey = xAxis.Key,
                    StrokeThickness = 2,
                    ColumnWidth = 1,
                };

                for (int i = 0; i < x.Length; i++)
                {
                    xAxis.Labels.Add(x[i].ToString("G2"));
                    var item = new ColumnItem(y[i]);
                    boxSeries.Items.Add(item);
                }

                plotModel.Series.Add(boxSeries);
            }

            var formattable = instance as IFormattable;
            if (formattable != null)
            {
                plotModel.Title = formattable.ToString("G3", CultureInfo.CurrentUICulture);
            }
            else
            {
                plotModel.Title = instance.ToString();
            }

            plotModel.TitlePadding = 2;
            plotModel.TitleFontSize = 15;
            plotModel.TitleFontWeight = 1;
            plotModel.TitlePadding = 2;


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

            double resolution = 100;
            this.unit = new DoubleRange(0, 1);
            this.probabilities = Vector.Range(0.0, 1.0, 1.0 / resolution);

            if (instance is UnivariateDiscreteDistribution)
            {
                this.supportPoints = Vector.Range(range.Min, range.Max, 1.0);
            }
            else
            {
                double min = range.Min - Math.Abs(range.Length) * 0.1;
                double max = range.Max + Math.Abs(range.Length) * 0.1;

                this.range = new DoubleRange(min, max);

                this.supportPoints = Vector.Range(range.Min, range.Max, range.Length / resolution);

                // make sure the support points include the important metrics

                concatenate(() => instance.Mean);
                concatenate(() => instance.Median);
                concatenate(() => instance.Mode);
                Array.Sort(supportPoints);
            }

        }

        private void concatenate(Func<double> property)
        {
            try
            {
                double value = property();
                if (!Double.IsNaN(value) && !Double.IsInfinity(value))
                    this.supportPoints = supportPoints.Concatenate(value);
            }
            catch { };
        }

    }
}
