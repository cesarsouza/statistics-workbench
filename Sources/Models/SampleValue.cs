using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace Statistics_Workbench.Models
{

    [ImplementPropertyChanged]
    public class SampleValue
    {
        public double Value { get; set; }
        public double Weight { get; set; }

        public SampleValue()
        {
            Weight = 1;
        }
    }
}
