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

        [Conditional("Debug")]
        [DebuggerStepThrough()]
        protected void VerifyCalledOnUIThread()
        {
            if (dispatcher != Dispatcher.CurrentDispatcher)
                Debug.Fail("Must be called from the UI thread.");
        }

        [Conditional("Debug")]
        [DebuggerStepThrough()]
        protected void CheckPropertyName(string propertyName)
        {
            VerifyCalledOnUIThread();
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                Debug.Fail(string.Format("Property name does not exist: {0}", propertyName));
        }

        public CommandBindingCollection CommandBindings
        {
            get { return commandBindings; }
        }

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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            VerifyCalledOnUIThread();

            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
