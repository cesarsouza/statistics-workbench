// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using Accord;
    using System;
    using System.Linq;
    using System.Reflection;
    using Workbench.Tools;

    /// <summary>
    ///   Describes a distribution's parameter value, such as the mean and
    ///   standard deviation values needed to create a Normal distribution.
    /// </summary>
    /// 
    public class ParameterViewModel
    {
        private double? value;



        /// <summary>
        ///   Occurs when the parameter's value changes.
        /// </summary>
        /// 
        public event EventHandler ValueChanged;


        /// <summary>
        ///   Gets the name of the parameter.
        /// </summary>
        /// 
        public string Name { get; private set; }

        /// <summary>
        ///   Gets or sets the current value for the parameter.
        /// </summary>
        /// 
        public double? Value
        {
            get { return value; }
            set { onValueChanged(value); }
        }

        /// <summary>
        ///   Gets a value indicating whether the parameter accepts only discrete (integer)
        ///   values. Otherwise, the value is allowed to take continuous (real) values.
        /// </summary>
        /// 
        public bool IsDiscrete { get; private set; }

        /// <summary>
        ///   Gets the maximum value this parameter can have.
        /// </summary>
        /// 
        public double Max { get; private set; }

        /// <summary>
        ///   Gets the minimum value this parameter can have.
        /// </summary>
        /// 
        public double Min { get; private set; }

        /// <summary>
        ///   Gets by how much this parameter should be changed
        ///   when configured through a user interface.
        /// </summary>
        /// 
        public double Step { get; private set; }

        /// <summary>
        ///   Gets the constructor's reflection parameter information.
        /// </summary>
        /// 
        public ParameterInfo Parameter { get; private set; }



        /// <summary>
        ///   Gets the parent constructor to whom this property belongs.
        /// </summary>
        /// 
        public ConstructorViewModel Owner { get; private set; }



        /// <summary>
        ///   Attempts to create a constructor's parameter. If the parameter doesn't
        ///   qualify as a valid parameter to be used in the automatic distribution
        ///   construction, the method fails and returns false.
        /// </summary>
        /// 
        /// <param name="info">The parameter's reflection information.</param>
        /// <param name="owner">The constructor that should own this parameter.</param>
        /// <param name="parameter">The created distribution parameter.</param>
        /// 
        /// <returns>True if the parameter could be created; false otherwise.</returns>
        /// 
        public static bool TryParse(ParameterInfo info, ConstructorViewModel owner, out ParameterViewModel parameter)
        {
            parameter = null;

            DoubleRange range;
            if (!DistributionManager.TryGetRange(info, out range))
                return false;

            double value;
            if (!DistributionManager.TryGetDefault(info, out value))
                return false;

            parameter = new ParameterViewModel(info, owner, range, value);

            return true;
        }

        /// <summary>
        ///   Attempts to synchronize the value of the parameter with the current property
        ///   values for the distribution. If there is a property with the same name as the
        ///   parameter, the parameter is updated to have the same value as the property.
        /// </summary>
        /// 
        public void Sync()
        {
            var match = Owner.Owner
                .Properties.Where(x => x.Name == this.Name).FirstOrDefault();

            if (match != null && match.Value != null)
                this.Value = match.Value;
        }




        private ParameterViewModel(ParameterInfo info, ConstructorViewModel owner, DoubleRange range, double value)
        {
            bool isInteger = info.ParameterType == typeof(int);

            double min = range.Min;
            double max = range.Max;

            if (min < -1e+5)
                min = -1e+5;

            if (max > 1e+5)
                max = 1e+5;

            double step = 0.1;
            if (isInteger)
                step = 1;

            Min = min;
            Max = max;
            Step = step;
            Value = value;
            Name = DistributionManager.Normalize(info.Name);
            Owner = owner;
            Parameter = info;
            IsDiscrete = isInteger;
        }

        private void onValueChanged(double? value)
        {
            this.value = value;

            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }


        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return Name;
        }
    }
}
