using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions;

namespace Statistics_Workbench.Models
{
    public class DistributionConstructorInfo
    {
        public ConstructorInfo Constructor { get; private set; }

        public ObservableCollection<DistributionParameterInfo> Parameters { get; private set; }

        public int Length { get { return Parameters.Count; } }

        public DistributionInfo Owner { get; private set; }

        public static bool TryParse(ConstructorInfo ctor, DistributionInfo owner,
            out DistributionConstructorInfo constructor)
        {
            constructor = new DistributionConstructorInfo();

            ParameterInfo[] info = ctor.GetParameters();
            var parameters = new DistributionParameterInfo[info.Length];

            for (int i = 0; i < info.Length; i++)
            {
                if (!DistributionParameterInfo.TryParse(info[i], constructor, out parameters[i]))
                    return false;
            }

            constructor.Owner = owner;
            constructor.Constructor = ctor;
            constructor.Parameters = new ObservableCollection<DistributionParameterInfo>(parameters);

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
