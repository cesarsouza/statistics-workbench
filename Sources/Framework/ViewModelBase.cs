using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Statistics_Workbench.Framework
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly Dispatcher dispatcher;
        private readonly CommandBindingCollection commandBindings;
        private event PropertyChangedEventHandler propertyChanged;


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
