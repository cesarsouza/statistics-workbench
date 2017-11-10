using Accord.Statistics.Distributions.Univariate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions.Fitting;
using Accord.MachineLearning;
using System.Globalization;

namespace Unit_Tests
{
    [TestClass]
    public class ArticleExamplesTest
    {
        [TestMethod]
        public void NormalGenerateTest()
        {
            // Create a Normal with mean 2 and sigma 5
            var normal = new NormalDistribution(2, 5);

            // Generate 1000000 samples from it
            double[] samples = normal.Generate(10000000);

            // Try to estimate a new Normal distribution from the 
            // generated samples to check if they indeed match
            var actual = NormalDistribution.Estimate(samples);

            string result = actual.ToString("N2"); // N(x; μ = 2.01, σ² = 25.03)

            Assert.AreEqual("N(x; μ = 2.00, σ² = 25.00)", result);
        }

        [TestMethod]
        public void BinomialGenerateTest()
        {
            // Create a Binomial with n = 4 and p = 0.2
            var binomial = new BinomialDistribution(4, 0.2);

            // Generate 1000000 samples from it
            double[] samples = binomial.Generate(1000000).ToDouble();

            // Try to estimate a new Binomial distribution from
            // generated samples to check if they indeed match
            var actual = new BinomialDistribution(4);
            actual.Fit(samples);

            string result = actual.ToString("N2"); // Binomial(x; n = 4.00, p = 0.20)

            Assert.AreEqual("Binomial(x; n = 4.00, p = 0.20)", result);
        }

        [TestMethod]
        public void MultivariateNormalGenerateTest()
        {
            // mean vector
            double[] mu = { 2.0, 6.0 };

            // covariance
            double[,] cov =
            {
                { 2, 1 },
                { 1, 5 }
            };

            // Create a multivariate Normal distribution
            var normal = new MultivariateNormalDistribution(mu, cov);

            // Generate 1000000 samples from it
            double[][] samples = normal.Generate(1000000);

            // Try to estimate a new Normal distribution from
            // generated samples to check if they indeed match
            var actual = MultivariateNormalDistribution.Estimate(samples);

            Assert.IsTrue(mu.IsEqual(actual.Mean, 0.1));
            Assert.IsTrue(cov.IsEqual(actual.Covariance, 0.1));
        }

        [TestMethod]
        public void RankDistribution()
        {
            // Create a new distribution, such as a Poisson
            var poisson = new PoissonDistribution(lambda: 0.42);

            // Draw enough samples from it
            double[] samples = poisson.Generate(1000000).ToDouble();

            // Let's pretend we don't know from which distribution
            // those sample come from, and create an analysis object
            // to check it for us:
            var analysis = new DistributionAnalysis();

            // Compute the analysis
            var gof = analysis.Learn(samples);

            // Get the most likely distribution
            var mostLikely = gof[0];

            // The result should be Poisson(x; λ = 0.420961)
            var result = (mostLikely.Distribution as IFormattable).ToString("N3", CultureInfo.InvariantCulture);

            Assert.AreEqual("Poisson(x; λ = 0.420)", result);
        }

        [TestMethod]
        public void MixtureDistributionExample()
        {
            var samples1 = new NormalDistribution(mean: -2, stdDev: 1).Generate(10000000);
            var samples2 = new NormalDistribution(mean: +4, stdDev: 1).Generate(10000000);

            // Mix the samples from both distributions
            var samples = samples1.Concatenate(samples2);

            // Create a new mixture distribution with two Normal components
            var mixture = new Mixture<NormalDistribution>(
                new NormalDistribution(-1),
                new NormalDistribution(+1));

            // Estimate the distribution
            mixture.Fit(samples);

            var a = mixture.Components[0].ToString("N2"); // N(x; μ = -2.00, σ² = 1.00)
            var b = mixture.Components[1].ToString("N2"); // N(x; μ =  4.00, σ² = 1.02)

            Assert.AreEqual("N(x; μ = -2.00, σ² = 0.99)", a);
            Assert.AreEqual("N(x; μ = 4.00, σ² = 1.01)", b);
        }

        [TestMethod]
        public void GaussianMixtureModelExample()
        {
            // Test Samples
            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 1, 1 },
                new double[] { 0, 7 },
                new double[] { 1, 1 },
                new double[] { 6, 2 },
                new double[] { 6, 5 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 5, 1 }
            };

            double[] sample = samples[0];


            // Create a new Gaussian mixture with 2 components
            var gmm = new GaussianMixtureModel(components: 2);

            // Compute the model (estimate)
            var clusters = gmm.Learn(samples);

            double error = gmm.LogLikelihood;

            // Classify a single sample
            int c0 = clusters.Decide(samples[0]);
            int c1 = clusters.Decide(samples[1]);

            int c7 = clusters.Decide(samples[7]);
            int c8 = clusters.Decide(samples[8]);

            Assert.AreEqual(c0, c1);
            Assert.AreEqual(c7, c8);
            Assert.AreNotEqual(c0, c8);


            // Extract the multivariate Normal distribution from it
            MultivariateMixture<MultivariateNormalDistribution> mixture =
                gmm.ToMixtureDistribution();

            Assert.AreEqual(2, mixture.Dimension);
            Assert.AreEqual(2, mixture.Components.Length);
            Assert.AreEqual(2, mixture.Coefficients.Length);
        }

        [TestMethod]
        public void IndependentModelExample()
        {
            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 1, 1 },
                new double[] { 0, 7 },
                new double[] { 1, 1 },
                new double[] { 6, 2 },
                new double[] { 6, 5 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 5, 1 }
            };

            // Create a new 2D independent Normal distribution
            var independent = new Independent<NormalDistribution>(
                new NormalDistribution(), new NormalDistribution());

            // Estimate it!
            independent.Fit(samples);

            var a = independent.Components[0].ToString("N2"); // N(x1; μ = 3.20, σ² = 7.96)
            var b = independent.Components[1].ToString("N2"); // N(x2; μ = 2.20, σ² = 4.40)

            Assert.AreEqual("N(x; μ = 3.20, σ² = 7.96)", a);
            Assert.AreEqual("N(x; μ = 2.20, σ² = 4.40)", b);
        }

    }
}
