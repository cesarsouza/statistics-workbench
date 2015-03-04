// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    ///   Hyperlink view model for launching hyperlinks in the browser when clicked.
    /// </summary>
    /// 
    public class HyperlinkViewModel
    {
        /// <summary>
        ///   Gets or sets the hyperlink's URL.
        /// </summary>
        /// 
        public string Url { get; set; }

        /// <summary>
        ///   Gets or sets the hyperlink's text.
        /// </summary>
        /// 
        public string Text { get; set; }

        /// <summary>
        ///   Command for launching the hyperlink in a browser.
        /// </summary>
        /// 
        public RelayCommand Go { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HyperlinkViewModel"/> class.
        /// </summary>
        /// 
        public HyperlinkViewModel()
        {
            Go = new RelayCommand(execute, canExecute);
        }

        private bool canExecute(object obj)
        {
            return Uri.IsWellFormedUriString(Url, UriKind.Absolute);
        }

        private void execute(object obj)
        {
            System.Diagnostics.Process.Start(Url.ToString());
        }
    }
}
