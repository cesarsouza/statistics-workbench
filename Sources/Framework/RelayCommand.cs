// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    ///   Relay command implementation.
    /// </summary>
    /// 
    public class RelayCommand : ICommand
    {

        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// 
        /// <param name="execute">The action to be taken when the command is executed.</param>
        /// 
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// 
        /// <param name="execute">The action to be taken when the command is executed.</param>
        /// <param name="canExecute">The boolean function that indicates whether the command can be executed.</param>
        /// 
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        ///   Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// 
        /// <param name="parameter">
        ///   Data used by the command.  If the command does not require 
        ///   data to be passed, this object can be set to null.
        /// </param>
        /// 
        /// <returns>
        ///   true if this command can be executed; otherwise, false.
        /// </returns>
        /// 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        /// <summary>
        ///   Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        /// 
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        ///   Defines the method to be called when the command is invoked.
        /// </summary>
        /// 
        /// <param name="parameter">
        ///   Data used by the command.  If the command does not require  
        ///   data to be passed, this object can be set to null.
        /// </param>
        /// 
        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
