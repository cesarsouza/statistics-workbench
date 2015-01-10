// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using PropertyChanged;

    [ImplementPropertyChanged]
    public class SampleViewModel
    {
        public double Value { get; set; }

        public double Weight { get; set; }

        public SampleViewModel()
        {
            Weight = 1;
        }
    }
}
