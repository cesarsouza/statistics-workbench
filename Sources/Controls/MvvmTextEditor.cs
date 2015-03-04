// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Workbench.Controls
{

    /// <summary>
    ///   Text editor that is compatible with the Model-View-ViewModel architecture.
    /// </summary>
    /// 
    /// <remarks> 
    ///   This class encompasses ICSharpCode's TextEditor control with MVVM enabled
    ///   capabilities, so its properties can be data-bound to view models in the
    ///   project's views.
    /// </remarks>
    /// 
    public class MvvmTextEditor : TextEditor, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   Gets or sets the document text.
        /// </summary>
        public string DocumentText
        {
            get { return base.Text; }
            set { base.Text = value; }
        }



        /// <summary>
        ///   The document text property that makes it possible to 
        ///   data-bind <see cref="DocumentText"/> in a MVVM context.
        /// </summary>
        /// 
        public static DependencyProperty DocumentTextProperty =
            DependencyProperty.Register("DocumentText", typeof(string), typeof(MvvmTextEditor),
            new PropertyMetadata((obj, args) =>
            {
                MvvmTextEditor target = (MvvmTextEditor)obj;
                target.DocumentText = (string)args.NewValue;
            }));


        /// <summary>
        ///   Raises the <see cref="E:ICSharpCode.AvalonEdit.TextEditor.TextChanged" /> event.
        /// </summary>
        /// 
        protected override void OnTextChanged(EventArgs e)
        {
            SetCurrentValue(DocumentTextProperty, base.Text);
            base.OnTextChanged(e);
        }

        /// <summary>
        ///   Raises the property changed.
        /// </summary>
        /// 
        public void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
