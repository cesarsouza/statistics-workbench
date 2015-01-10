// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Behaviors
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    // http://stackoverflow.com/questions/1585462/bubbling-scroll-events-from-a-listview-to-its-parent

    /// <summary>
    ///   Behavior to make UI elements ignore mousewheel events.
    /// </summary>
    /// 
    public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
    {

        /// <summary>
        ///   Called when the behavior is attached to a UI element.
        /// </summary>
        /// 
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        /// <summary>
        ///   Called when the behavior is being detached from a UI element.
        /// </summary>
        /// 
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }


        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            AssociatedObject.RaiseEvent(e2);
        }
    }
}
