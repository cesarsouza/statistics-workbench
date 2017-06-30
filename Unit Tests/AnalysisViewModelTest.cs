using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.ViewModels;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Threading;
using OxyPlot;
using OxyPlot.Series;
using Accord.Statistics.Distributions.Univariate;

namespace Unit_Tests
{
    [TestClass]
    public class AnalysisViewModelTest
    {
        // tests based on examples available at
        // http://wiki.stat.ucla.edu/socr/index.php/SOCR_EduMaterials_Activities_Normal_Probability_examples

        [TestMethod]
        public void ConstructorTest_Visibility()
        {
            var main = new MainViewModel();
            main.SetDistribution("Normal");

            Assert.IsTrue(main.SelectedDistribution.Instance is NormalDistribution);

            AnalysisViewModel target = main.Analysis;
            Assert.IsFalse(target.LeftValueVisible);

            target.ComparisonIndex = target.Comparisons.IndexOf(AnalysisViewModel.EqualTo);
            Assert.IsFalse(target.LeftValueVisible);

            target.ComparisonIndex = target.Comparisons.IndexOf(AnalysisViewModel.Outside);
            Assert.IsTrue(target.LeftValueVisible);

            target.ComparisonIndex = target.Comparisons.IndexOf(AnalysisViewModel.GreaterThan);
            Assert.IsFalse(target.LeftValueVisible);

            target.ComparisonIndex = target.Comparisons.IndexOf(AnalysisViewModel.Between);
            Assert.IsTrue(target.LeftValueVisible);

            target.ComparisonIndex = target.Comparisons.IndexOf(AnalysisViewModel.LessThan);
            Assert.IsFalse(target.LeftValueVisible);
        }

        [TestMethod]
        public void ConstructorTest_Example01()
        {
            var main = new MainViewModel();

            main.SetDistribution("Normal");
            main.GetParameter("Mean").Value = 4.62;
            main.GetParameter("Std. Dev.").Value = 0.23;

            AnalysisViewModel target = main.Analysis;

            Assert.IsNotNull(target);

            compute(target, 4.35, AnalysisViewModel.Between, 4.85);
            Assert.AreEqual(0.721129, target.Probability, 1e-6);
        }

        [TestMethod]
        public void ConstructorTest_Example02()
        {
            var main = new MainViewModel();

            main.SetDistribution("Normal");
            main.GetParameter("Mean").Value = 43.3;
            main.GetParameter("Std. Dev.").Value = 4.6;

            AnalysisViewModel target = main.Analysis;

            Assert.IsNotNull(target);

            probability(target, AnalysisViewModel.GreaterThan, 0.05);
            Assert.AreEqual(0, target.LeftValue, 1e-6);
            Assert.AreEqual(50.86, target.RightValue, 1e-2);
        }

        [TestMethod]
        public void ConstructorTest_Example03()
        {
            var main = new MainViewModel();

            main.SetDistribution("Normal");
            main.GetParameter("Mean").Value = 62;
            main.GetParameter("Std. Dev.").Value = 8;

            AnalysisViewModel target = main.Analysis;

            Assert.IsNotNull(target);

            probability(target, AnalysisViewModel.LessThan, 0.30);
            Assert.AreEqual(0, target.LeftValue, 1e-6);
            Assert.AreEqual(57.804, target.RightValue, 1e-2);

            probability(target, AnalysisViewModel.GreaterThan, 0.05);
            Assert.AreEqual(0, target.LeftValue, 1e-6);
            Assert.AreEqual(75.158, target.RightValue, 1e-2);
        }

        [TestMethod]
        public void ConstructorTest_Example04()
        {
            var main = new MainViewModel();

            main.SetDistribution("Binomial");
            main.GetParameter("Trials").Value = 3;
            main.GetParameter("Probability").Value = 0.02;

            AnalysisViewModel target = main.Analysis;

            Assert.IsNotNull(target);

            compute(target, 0, AnalysisViewModel.EqualTo, 3);
            Assert.AreEqual(0.000008, target.Probability, 1e-10);
        }

        [TestMethod]
        public void ConstructorTest_Example07()
        {
            var main = new MainViewModel();

            main.SetDistribution("Binomial");
            main.GetParameter("Trials").Value = 2000;
            main.GetParameter("Probability").Value = 0.063;

            Assert.AreEqual(126, main.GetProperty("Mean").Value);
            Assert.AreEqual(118.06, main.GetProperty("Variance").Value.Value, 0.005);

            AnalysisViewModel target = main.Analysis;

            Assert.IsNotNull(target);

            compute(target, 0, AnalysisViewModel.LessThan, 135);
            Assert.AreEqual(0.8099233, target.Probability, 1e-6);
        }

        [TestMethod]
        public void ConstructorTest_Example10a()
        {
            var main = new MainViewModel();

            main.SetDistribution("Normal");
            main.GetParameter("Mean").Value = 36;
            main.GetParameter("Std. Dev.").Value = 0.1;

            AnalysisViewModel target = main.Analysis;

            Assert.IsNotNull(target);

            compute(target, 0, AnalysisViewModel.LessThan, 35.8);
            Assert.AreEqual(0.0228, target.Probability, 5e-5);

            compute(target, 0, AnalysisViewModel.GreaterThan, 36.2);
            Assert.AreEqual(1.0 - 0.9772, target.Probability, 5e-5);

            compute(target, 35.8, AnalysisViewModel.Outside, 36.2);
            Assert.AreEqual(0.0455002, target.Probability, 5e-6);
        }

        [TestMethod]
        public void ConstructorTest_Example10d()
        {
            var main = new MainViewModel();

            main.SetDistribution("Normal");
            main.GetParameter("Mean").Value = 37;
            main.GetParameter("Std. Dev.").Value = 0.4;

            var target = main.Analysis;

            compute(target, 0, AnalysisViewModel.LessThan, 35.8);
            Assert.AreEqual(0.001349, target.Probability, 5e-5);

            compute(target, 0, AnalysisViewModel.GreaterThan, 36.2);
            Assert.AreEqual(0.977249, target.Probability, 5e-5);

            compute(target, 35.8, AnalysisViewModel.Outside, 36.2);
            Assert.AreEqual(0.978599, target.Probability, 5e-6);
        }

        public static void compute(AnalysisViewModel target, double left, string comparison, double right)
        {
            target.LeftValue = Double.NaN;
            target.RightValue = Double.NaN;

            target.LeftValue = left;
            target.ComparisonIndex = target.Comparisons.IndexOf(comparison);
            target.RightValue = right;
        }

        public static void probability(AnalysisViewModel target, string comparison, double probability)
        {
            target.Probability = Double.NaN;
            target.ComparisonIndex = target.Comparisons.IndexOf(comparison);
            target.Probability = probability;
        }



        [TestMethod]
        public void ConstructorTest_DiscreteColors()
        {
            var main = new MainViewModel();

            main.SetDistribution("Binomial");
            main.GetParameter("Trials").Value = 9;
            main.GetParameter("Probability").Value = 0.5;

            var target = main.Analysis;
            Assert.AreEqual(main.SelectedDistribution, target.SelectedDistribution);

            var red = OxyColor.FromRgb(250, 0, 0);
            PlotModel plot; 
            ColumnSeries series;

            compute(target, 0, AnalysisViewModel.LessThan, 3);
            plot = target.DensityFunction;
            series = plot.Series[0] as ColumnSeries;
            Assert.AreEqual(1, plot.Series.Count);
            Assert.AreEqual(7, series.Items.Count);

            Assert.AreEqual(red, series.Items[0].Color);
            Assert.AreEqual(red, series.Items[1].Color);
            Assert.AreEqual(red, series.Items[2].Color);
            Assert.AreNotEqual(red, series.Items[3].Color);
            Assert.AreNotEqual(red, series.Items[4].Color);
            Assert.AreNotEqual(red, series.Items[5].Color);
            Assert.AreNotEqual(red, series.Items[6].Color);

            compute(target, 0, AnalysisViewModel.LessThan, 4);
            plot = target.DensityFunction;
            series = plot.Series[0] as ColumnSeries;
            Assert.AreEqual(1, plot.Series.Count);
            Assert.AreEqual(7, series.Items.Count);

            Assert.AreEqual(red, series.Items[0].Color);
            Assert.AreEqual(red, series.Items[1].Color);
            Assert.AreEqual(red, series.Items[2].Color);
            Assert.AreEqual(red, series.Items[3].Color);
            Assert.AreNotEqual(red, series.Items[4].Color);
            Assert.AreNotEqual(red, series.Items[5].Color);
            Assert.AreNotEqual(red, series.Items[6].Color);

            compute(target, 0, AnalysisViewModel.LessThan, 5);
            plot = target.DensityFunction;
            series = plot.Series[0] as ColumnSeries;
            Assert.AreEqual(1, plot.Series.Count);
            Assert.AreEqual(8, series.Items.Count);

            Assert.AreEqual(red, series.Items[0].Color);
            Assert.AreEqual(red, series.Items[1].Color);
            Assert.AreEqual(red, series.Items[2].Color);
            Assert.AreEqual(red, series.Items[3].Color);
            Assert.AreEqual(red, series.Items[4].Color);
            Assert.AreNotEqual(red, series.Items[5].Color);
            Assert.AreNotEqual(red, series.Items[6].Color);
        }

        [TestMethod]
        public void ConstructorTest_NoncentralT()
        {
            var main = new MainViewModel();

            main.SetDistribution("Noncentral T");
            Assert.AreEqual(1, main.GetParameter("Degrees Of Freedom").Value);
            Assert.AreEqual(0, main.GetParameter("Noncentrality").Value);

            var target = main.Analysis;
            var red = OxyColor.FromRgb(250, 0, 0);
            var plot = target.DensityFunction;
            var line = plot.Series[0] as LineSeries;
            var area = plot.Series[1] as AreaSeries;

            Assert.IsNotNull(line);
            Assert.IsNotNull(area);
        }

        [TestMethod]
        public void ConstructorTest_Rebind()
        {
            var main = new MainViewModel();

            main.SetDistribution("Binomial");
            main.GetParameter("Trials").Value = 2000;
            main.GetParameter("Probability").Value = 0.063;

            Assert.IsTrue(main.SelectedDistribution.Instance is BinomialDistribution);

            var target = main.Analysis;
            compute(target, 0, AnalysisViewModel.LessThan, 2);
            compute(target, 0, AnalysisViewModel.LessThan, 53);

            main.SetDistribution("Normal");
            target = main.Analysis;
            Assert.AreEqual(target.SelectedDistribution, main.SelectedDistribution);

            Assert.IsTrue(main.SelectedDistribution.Instance is NormalDistribution);
        }

    }
}
