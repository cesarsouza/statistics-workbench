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
    public class MainViewModelTest
    {

        [TestMethod]
        public void ConstructorTest1()
        {
            MainViewModel target = new MainViewModel();

            Assert.AreEqual(1, target.Estimate.Values.Count);
            Assert.IsTrue(target.Distributions.Count > 0);
        }

        [TestMethod]
        public void Paste_SuccessTest1()
        {
            var main = new MainViewModel();
            var target = main.Estimate;

            string text = ""
             + "0.111\t2.222\n"
             + "0.333\t4.111\n"
             + "2.421\t3.141";

            Clipboard.SetText(text);
            target.Paste_Executed(null);

            Assert.AreEqual(4, target.Values.Count);
            Assert.AreEqual(0, target.Values[0].Value);
            Assert.AreEqual(1, target.Values[0].Weight);
            Assert.AreEqual(0.111, target.Values[1].Value);
            Assert.AreEqual(2.222, target.Values[1].Weight);
            Assert.AreEqual(0.333, target.Values[2].Value);
            Assert.AreEqual(4.111, target.Values[2].Weight);
            Assert.AreEqual(2.421, target.Values[3].Value);
            Assert.AreEqual(3.141, target.Values[3].Weight);
        }

        [TestMethod]
        public void Paste_SuccessTest2()
        {
            var main = new MainViewModel();
            var target = main.Estimate;


            string text = ""
             + "0.111\t2.222\n"
             + "A\tB\n"
             + "2.421\t3.141";

            Clipboard.SetText(text);
            target.Paste_Executed(null);

            Assert.AreEqual(3, target.Values.Count);
            Assert.AreEqual(0, target.Values[0].Value);
            Assert.AreEqual(1, target.Values[0].Weight);
            Assert.AreEqual(0.111, target.Values[1].Value);
            Assert.AreEqual(2.222, target.Values[1].Weight);
            Assert.AreEqual(2.421, target.Values[2].Value);
            Assert.AreEqual(3.141, target.Values[2].Weight);
        }

        [TestMethod]
        public void Generate_NotSupportedTest()
        {
            var main = new MainViewModel();

            // Select a Folded Normal distribution
            int index = main.Distributions.IndexOf(
                main.Distributions.Where(x => x.Name.Contains("Folded Normal")).First());

            main.SelectedDistributionIndex = index;

            SpinWait.SpinUntil(() => main.SelectedDistribution.IsInitialized);

            Assert.AreEqual(1, main.Estimate.Values.Count);
            Assert.IsTrue(String.IsNullOrEmpty(main.Estimate.Message));

            // Generate samples
            Assert.AreEqual(100, main.Estimate.NumberOfSamplesToBeGenerated);
            main.Estimate.GenerateCommand.Execute(null);

            Assert.AreEqual(100, main.Estimate.Values.Count);
            Assert.IsFalse(main.Estimate.Owner.SelectedDistribution.IsFittable);
            Assert.IsFalse(String.IsNullOrEmpty(main.Estimate.Message));
        }

        [TestMethod]
        public void GompertzTest()
        {
            var main = new MainViewModel();

            main.SetDistribution("Gompertz");

            Assert.AreEqual(1, main.Estimate.Values.Count);
            Assert.IsTrue(String.IsNullOrEmpty(main.Estimate.Message));

            // Generate samples
            Assert.AreEqual(100, main.Estimate.NumberOfSamplesToBeGenerated);
            main.Estimate.GenerateCommand.Execute(null);

            Assert.AreEqual(100, main.Estimate.Values.Count);
            Assert.IsFalse(main.Estimate.Owner.SelectedDistribution.IsFittable);
            Assert.IsFalse(String.IsNullOrEmpty(main.Estimate.Message));
        }

        [TestMethod]
        public void TrapezoidalTest()
        {
            var main = new MainViewModel();

            main.SetDistribution("Trapezoidal");

            Assert.AreEqual(1, main.Estimate.Values.Count);
            Assert.IsTrue(String.IsNullOrEmpty(main.Estimate.Message));

            // Generate samples
            Assert.AreEqual(100, main.Estimate.NumberOfSamplesToBeGenerated);
            main.Estimate.GenerateCommand.Execute(null);

            Assert.AreEqual(100, main.Estimate.Values.Count);
            Assert.IsFalse(main.Estimate.Owner.SelectedDistribution.IsFittable);
            Assert.IsFalse(String.IsNullOrEmpty(main.Estimate.Message));
        }


        [TestMethod]
        public void Estimate_UpdateOnEditTest()
        {
            var main = new MainViewModel();

            SpinWait.SpinUntil(() => main.SelectedDistribution.IsInitialized);

            main.Estimate.IsUpdatedOnEdit = false;

            main.Estimate.Values.Add(new SampleViewModel() { Value = 2 });
            main.Estimate.Values.Add(new SampleViewModel() { Value = 3 });
            main.Estimate.Values.Add(new SampleViewModel() { Value = 4 });
            main.Estimate.Values.Add(new SampleViewModel() { Value = 5 });

            Assert.AreEqual(0, main.SelectedDistribution.Instance.Mean);
            Assert.AreEqual(1, main.SelectedDistribution.Instance.Variance);

            main.Estimate.IsUpdatedOnEdit = true;

            main.Estimate.NewCommand.Execute(null);
            main.Estimate.Values.Add(new SampleViewModel() { Value = 1 });
            main.Estimate.Values.Add(new SampleViewModel() { Value = 2 });
            main.Estimate.Values.Add(new SampleViewModel() { Value = 3 });
            main.Estimate.Values.Add(new SampleViewModel() { Value = 4 });
            main.Estimate.Values.Add(new SampleViewModel() { Value = 5 });

            Assert.AreEqual(2.5, main.SelectedDistribution.Instance.Mean);
            Assert.AreEqual(3.5, main.SelectedDistribution.Instance.Variance);
        }

        [TestMethod]
        public void Documentation_Test()
        {
            var main = new MainViewModel();
            SpinWait.SpinUntil(() => main.SelectedDistribution.IsInitialized);
            Assert.AreEqual("Normal", main.SelectedDistribution.Name);

            main.SetDistribution("Gamma");
            SpinWait.SpinUntil(() => main.SelectedDistribution.IsInitialized);
            Assert.AreEqual("Gamma", main.SelectedDistribution.Name);

            var doc = main.SelectedDistribution.Documentation;

            Assert.AreEqual("Gamma", doc.Name);

            Assert.AreEqual(3459, doc.Remarks.Length);
            Assert.AreEqual(2, doc.SeeAlso.Count);

            string expectedSummary = @"<StackPanel>
<TextBlock>
Gamma distribution.</TextBlock>
</StackPanel>
";

            Assert.AreEqual(expectedSummary, doc.Summary);
            Assert.AreEqual(1, doc.Example.Length);
        }
    }
}
