// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord.Statistics.Distributions;
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;

    public class ConstructorViewModel
    {
        public ConstructorInfo Constructor { get; private set; }

        public ObservableCollection<ParameterViewModel> Parameters { get; private set; }

        public int Length { get { return Parameters.Count; } }

        public DistributionViewModel Owner { get; private set; }

        public static bool TryParse(ConstructorInfo ctor, DistributionViewModel owner,
            out ConstructorViewModel constructor)
        {
            constructor = new ConstructorViewModel();

            ParameterInfo[] info = ctor.GetParameters();
            var parameters = new ParameterViewModel[info.Length];

            for (int i = 0; i < info.Length; i++)
            {
                if (!ParameterViewModel.TryParse(info[i], constructor, out parameters[i]))
                    return false;
            }

            constructor.Owner = owner;
            constructor.Constructor = ctor;
            constructor.Parameters = new ObservableCollection<ParameterViewModel>(parameters);

            return true;
        }

        public bool TryCreate(out IUnivariateDistribution distribution)
        {
            distribution = null;

            var parameters = new object[Parameters.Count];

            try
            {
                foreach (var p in Parameters)
                    parameters[p.Parameter.Position] = Convert.ChangeType(p.Value, p.Parameter.ParameterType);

                distribution = (IUnivariateDistribution)Activator.CreateInstance(Owner.Type, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }
    }
}
