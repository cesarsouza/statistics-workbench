using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Statistics_Workbench
{
    public class AttachedProperties
    {
        // From https://codingcontext.wordpress.com/2008/12/10/commandbindings-in-mvvm/

        public static DependencyProperty RegisterCommandBindingsProperty =
            DependencyProperty.RegisterAttached("CommandBindings", 
            typeof(CommandBindingCollection), typeof(AttachedProperties),
            new PropertyMetadata(null, OnCommandBindingChanged));

        public static void SetCommandBindings(UIElement element, CommandBindingCollection value)
        {
            if (element != null)
                element.SetValue(RegisterCommandBindingsProperty, value);
        }

        public static CommandBindingCollection GetCommandBindings(UIElement element)
        {
            if (element == null)
                return null;

            return (CommandBindingCollection)element.GetValue(RegisterCommandBindingsProperty);
        }

        private static void OnCommandBindingChanged(DependencyObject sender, 
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
