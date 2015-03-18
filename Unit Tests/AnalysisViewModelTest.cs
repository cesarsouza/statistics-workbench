using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.ViewModels;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Threading;

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

    }
}
