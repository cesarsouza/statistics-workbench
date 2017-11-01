// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    /// <summary>
    ///   View Model for a single row in the table of samples that should 
    ///   be used to fit a distribution. Those are displayed in a DataGrid 
    ///   in the Estimation tab of the application.
    /// </summary>
    /// 
    public class SampleViewModel
    {
        /// <summary>
        ///   Gets or sets the sample's value. Default is 0.
        /// </summary>
        /// 
        public double Value { get; set; }

        /// <summary>
        ///   Gets or sets the sample's weight. Default is 1.
        /// </summary>
        /// 
        public double Weight { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SampleViewModel"/>
        ///   class, giving 0 to its <c>Value</c> property and 1 to its 
        ///   <c>Weight</c>.
        /// </summary>
        /// 
        public SampleViewModel()
        {
            Weight = 1;
        }

    }
}
