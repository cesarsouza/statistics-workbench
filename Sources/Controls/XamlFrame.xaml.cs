using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Statistics_Workbench.Controls
{

    public partial class XamlFrame : UserControl
    {
        string xaml;

        public XamlFrame()
        {
            InitializeComponent();
        }

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

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(XamlFrame), 
            new UIPropertyMetadata(MyPropertyChangedHandler));


        public static void MyPropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((XamlFrame)sender).Source = e.NewValue as String;
        }


        private void display(string xaml)
        {
            var context = new ParserContext();
            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            var rootObject = XamlReader.Parse(xaml, context);
            this.Content = rootObject;

            foreach (var c in FindVisualChildren<TextBlock>(rootObject as UIElement))
            {
                foreach (var t in c.Inlines)
                {
                    Hyperlink h = t as Hyperlink;
                    if (h != null)
                        h.RequestNavigate += RequestNavigate;
                }
                
            }
        }

        void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }
}
