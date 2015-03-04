// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Framework
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    ///   Base class for specifying ViewModels.
    /// </summary>
    /// 
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly Dispatcher dispatcher;
        private readonly CommandBindingCollection commandBindings;
        private event PropertyChangedEventHandler propertyChanged;


        /// <summary>
        ///   Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// 
        public ViewModelBase()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            commandBindings = new CommandBindingCollection();
        }

        /// <summary>
        ///   Checks whether the caller is running on the UI thread.
        /// </summary>
        /// 
        [Conditional("Debug")]
        [DebuggerStepThrough()]
        protected void VerifyCalledOnUIThread()
        {
            if (dispatcher != Dispatcher.CurrentDispatcher)
                Debug.Fail("Must be called from the UI thread.");
        }

        /// <summary>
        ///   Checks if the current object contains a property with the given name.
        /// </summary>
        /// 
        [Conditional("Debug")]
        [DebuggerStepThrough()]
        protected void CheckPropertyName(string propertyName)
        {
            VerifyCalledOnUIThread();
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                Debug.Fail(string.Format("Property name does not exist: {0}", propertyName));
        }

        /// <summary>
        ///   Gets the command bindings for this view model.
        /// </summary>
        /// 
        public CommandBindingCollection CommandBindings
        {
            get { return commandBindings; }
        }

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        /// 
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                VerifyCalledOnUIThread();
                propertyChanged += value;
            }
            remove
            {
                VerifyCalledOnUIThread();
                propertyChanged -= value;
            }
        }

        /// <summary>
        ///   Called when a property value changes.
        /// </summary>
        /// 
        /// <param name="propertyName">Name of the property.</param>
        /// 
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            VerifyCalledOnUIThread();

            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
