// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Navigation;

    /// <summary>
    ///   Xaml Frame control. This control enables consumers to
    ///   display and render XAML code on-the-fly in a WPF/XAML
    ///   application.
    /// </summary>
    /// 
    public partial class XamlFrame : UserControl
    {

        /// <summary>
        ///  Initializes a new instance of the <see cref="XamlFrame"/> class.
        /// </summary>
        /// 
        public XamlFrame()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Gets or sets a string containing the XAML 
        ///   code to be displayed inside this frame.
        /// </summary>
        /// 
        /// <value>
        ///   A string containing the XAML code being displayed.
        /// </value>
        /// 
        public string Source
        {
            get
            {
                return base.GetValue(SourceProperty) as string;
            }
            set
            {
                base.SetValue(SourceProperty, value);
                display(value);
            }
        }



        /// <summary>
        ///   Displays the specified xaml in the frame.
        /// </summary>
        /// 
        /// <param name="xaml">The xaml code to be displayed.</param>
        /// 
        private void display(string xaml)
        {
            if (string.IsNullOrEmpty(xaml))
            {
                this.Content = String.Empty;
                return;
            }

            var context = new ParserContext();
            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            var rootObject = XamlReader.Parse(xaml, context);

            this.Content = rootObject;

            foreach (var c in children<TextBlock>(rootObject as UIElement))
            {
                foreach (var t in c.Inlines)
                {
                    Hyperlink h = t as Hyperlink;

                    if (h != null) // make hyperlinks navigble
                        h.RequestNavigate += navigate;
                }
            }
        }

        /// <summary>
        ///   Iterates over all children in the visual tree of a dependency object.
        /// </summary>
        /// 
        private static IEnumerable<T> children<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in children<T>(child))
                        yield return childOfChild;
                }
            }
        }


        /// <summary>
        ///   Called when the user clicks on a link presented in the XAML code.
        /// </summary>
        /// 
        private void navigate(object sender, RequestNavigateEventArgs e)
        {
            // Launches the system default browser.
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }


        /// <summary>
        ///   Dependency property for the Source property.
        /// </summary>
        /// 
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(XamlFrame),
            new UIPropertyMetadata(onSourceChanged));

        private static void onSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((XamlFrame)sender).Source = e.NewValue as String;
        }

    }
}
