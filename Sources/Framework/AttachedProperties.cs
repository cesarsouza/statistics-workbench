// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    ///   Attached properties used in the project.
    /// </summary>
    /// 
    public class AttachedProperties
    {
        // From https://codingcontext.wordpress.com/2008/12/10/commandbindings-in-mvvm/

        /// <summary>
        ///   Command bindings dependency property to make
        ///   this attached property available in XAML.
        /// </summary>
        /// 
        public static DependencyProperty RegisterCommandBindingsProperty =
            DependencyProperty.RegisterAttached("CommandBindings", 
            typeof(CommandBindingCollection), typeof(AttachedProperties),
            new PropertyMetadata(null, onCommandBindingChanged));

        /// <summary>
        ///   Sets the command bindings.
        /// </summary>
        /// 
        public static void SetCommandBindings(UIElement element, CommandBindingCollection value)
        {
            if (element != null)
                element.SetValue(RegisterCommandBindingsProperty, value);
        }

        /// <summary>
        ///   Gets the command bindings.
        /// </summary>
        /// 
        public static CommandBindingCollection GetCommandBindings(UIElement element)
        {
            if (element == null)
                return null;

            return (CommandBindingCollection)element.GetValue(RegisterCommandBindingsProperty);
        }

        private static void onCommandBindingChanged(DependencyObject sender, 
            DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;

            if (element == null) 
                return;

            var bindings = (e.NewValue as CommandBindingCollection);

            if (bindings != null)
            {
                // clear the collection first
                element.CommandBindings.Clear();
                element.CommandBindings.AddRange(bindings);
            }
        }
    }
}
