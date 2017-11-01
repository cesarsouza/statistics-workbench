// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Statistics.Analysis;
    using Workbench.Tools;

    /// <summary>
    ///   View Model for a good of fit test for the current selected distribution
    ///   in the application and another target distribution (specified on this
    ///   class' property Name. This represents a single row in the candidate
    ///   distribution list in the Estimate window.
    /// </summary>
    /// 
    public class GoodnessOfFitViewModel
    {
        /// <summary>
        ///   Gets the rank of the goodness of fit test against
        ///   the target distribution in this good-of-fit test.
        /// </summary>
        /// 
        public string Rank { get; private set; }

        /// <summary>
        ///   Gets the name of the target distribution 
        ///   assessed by the goodness-of-fit test.
        /// </summary>
        /// 
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the value of the Chi-Square goodness-of-fit test.
        /// </summary>
        /// 
        public double ChiSquare { get; private set; }

        /// <summary>
        ///   Gets the value of the Kolmogorov-Smirnov goodness-of-fit test.
        /// </summary>
        /// 
        public double KolmogorovSmirnov { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GoodnessOfFitViewModel"/> class.
        /// </summary>
        /// 
        /// <param name="gof">The goodness-of-fits results against a particular distribution.</param>
        /// 
        public GoodnessOfFitViewModel(GoodnessOfFit gof)
        {
            this.Name = DistributionManager.Normalize(gof.Distribution.GetType().Name);

            // Transform the rank to ordinal positions
            // i.e. 0 to "1st", 1 to "2nd", 2 to "3rd"
            this.Rank = suffix(gof.ChiSquareRank + 1); 
           
            this.ChiSquare = gof.ChiSquare;
            this.KolmogorovSmirnov = gof.KolmogorovSmirnov;
        }


        private static string suffix(int num)
        {
            if (num.ToString().EndsWith("11")) return num.ToString() + "th";
            if (num.ToString().EndsWith("12")) return num.ToString() + "th";
            if (num.ToString().EndsWith("13")) return num.ToString() + "th";
            if (num.ToString().EndsWith("1")) return num.ToString() + "st";
            if (num.ToString().EndsWith("2")) return num.ToString() + "nd";
            if (num.ToString().EndsWith("3")) return num.ToString() + "rd";
            return num.ToString() + "th";
        }
    }
}
