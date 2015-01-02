using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using AForge;
using PropertyChanged;
using Statistics_Workbench.ViewModels;

namespace Statistics_Workbench.Models
{

    [ImplementPropertyChanged]
    public class DistributionParameterInfo : INotifyPropertyChanged
    {
        public string Name { get; private set; }

        public double? Value { get; set; }

        public bool IsInteger { get; private set; }

        public double Max { get; private set; }
        public double Min { get; private set; }

        public double Step { get; private set; }

        public DistributionViewModel Distribution { get; private set; }

        public ParameterInfo Parameter { get; private set; }
        public DistributionConstructorInfo Owner { get; private set; }



        public static bool TryParse(ParameterInfo parameterInfo, DistributionConstructorInfo owner, 
            out DistributionParameterInfo distributionParameter)
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

            distributionParameter = new DistributionParameterInfo()
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
