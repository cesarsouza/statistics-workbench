// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Formats
{
    using System;
    using System.Data;
    using System.IO;

    /// <summary>
    ///   Base class for base file formats supported by this application.
    /// </summary>
    public abstract class FileFormatBase
    {
        /// <summary>
        ///   Gets the file's default extension.
        /// </summary>
        /// 
        public abstract string Extension { get; }

        /// <summary>
        ///   Gets the format's description.
        /// </summary>
        /// 
        public abstract string Description { get; }

        /// <summary>
        ///   Gets the filter expression (e.g. *.txt) that
        ///   should be used to filter filenames that belong
        ///   to this format.
        /// </summary>
        /// 
        public string Filter { get { return String.Format("{0} ({1})|{1}", Description, Extension); } }

    }
}
