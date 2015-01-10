// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using AForge;
    using PropertyChanged;
    using System.ComponentModel;
    using System.Reflection;
    using Workbench.Tools;

    [ImplementPropertyChanged]
    public class ParameterViewModel : INotifyPropertyChanged
    {
        public string Name { get; private set; }

        public double? Value { get; set; }

        public bool IsInteger { get; private set; }

        public double Max { get; private set; }
        public double Min { get; private set; }

        public double Step { get; private set; }

        public DistributionViewModel Distribution { get; private set; }

        public ParameterInfo Parameter { get; private set; }
        public ConstructorViewModel Owner { get; private set; }



        public static bool TryParse(ParameterInfo parameterInfo, ConstructorViewModel owner, 
            out ParameterViewModel distributionParameter)
        {
            distributionParameter = null;

            DoubleRange range;
            if (!DistributionManager.TryGetRange(parameterInfo, out range))
                return false;

            double value;
            if (!DistributionManager.TryGetDefault(parameterInfo, out value))
                return false;

            bool isInteger = DistributionManager.IsInteger(parameterInfo);


            double min = range.Min;
            double max = range.Max;

            if (min < 1e-5)
                min = 1e-5;

            if (max > 1e+5)
                max = 1e+5;

            double step = 0.1;
            if (isInteger)
                step = 1;

            distributionParameter = new ParameterViewModel()
            {
                Min = min,
                Max = max,
                Step = step,
                Value = value,
                Name = DistributionManager.GetParameterName(parameterInfo),
                Owner = owner,
                Parameter = parameterInfo,
                IsInteger = isInteger
            };

            return true;
        }

       
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
