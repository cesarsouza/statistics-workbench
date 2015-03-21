using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Workbench.ViewModels;

namespace Unit_Tests
{
    public static class Tools
    {
        public static void SetDistribution(this MainViewModel main, string distName)
        {
            // Select a Folded Normal distribution
            int index = main.Distributions.IndexOf(main.Distributions.
                Where(x => x.Name.Contains(distName))
                .OrderBy(x=>x.Name.Length)
                .First());

            main.SelectedDistributionIndex = index;

            SpinWait.SpinUntil(() => main.SelectedDistribution.IsInitialized);
        }

        public static ParameterViewModel GetParameter(this MainViewModel main, string paramName)
        {
            var param = main.SelectedDistribution.Parameters.Where(x => x.Name == paramName).First();
            return param;
        }

        public static PropertyViewModel GetProperty(this MainViewModel main, string propName)
        {
            var prop = main.SelectedDistribution.Properties.Where(x => x.Name == propName).First();
            return prop;
        }
    }
}
