// -----------------------------------------------------------------------
// <copyright file="Command.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Wrapper class for bound commands to simplify WPF command binding 
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// Action to execute on command
        /// </summary>
        private Action<object> action;

        /// <summary>
        /// Initializes a new instance of the Command class
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="canExecute">Default flag for command being enabled</param>
        public Command(Action<object> action, bool canExecute = true)
        {
            this.action = action;
            this.CanCommandExecute = canExecute;
        }

        /// <summary>
        /// CanExecuteChanged event that we don't really care about at the moment
        /// </summary>
        public event EventHandler CanExecuteChanged = new EventHandler((o, e) => { });

        /// <summary>
        /// Gets or sets a value indicating whether the command can execute
        /// </summary>
        public bool CanCommandExecute { get; set; }

        /// <summary>
        /// Returns true when command can be executed
        /// </summary>
        /// <param name="parameter">unused command parameter</param>
        /// <returns>True if the command can execute</returns>
        public bool CanExecute(object parameter)
        {
            return this.CanCommandExecute;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="parameter">handler parameter</param>
        public void Execute(object parameter)
        {
            if (this.action != null)
            {
                this.action(parameter);
            }
        }
    }
}
