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
        public virtual string Extension { get; private set; }

        /// <summary>
        ///   Gets the format's description.
        /// </summary>
        /// 
        public virtual string Description { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether this format can be read by this application.
        /// </summary>
        /// 
        public virtual bool CanRead { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether this format can be written by this application.
        /// </summary>
        /// 
        public virtual bool CanWrite { get; private set; }

        /// <summary>
        ///   Gets the filter expression (e.g. *.txt) that
        ///   should be used to filter filenames that belong
        ///   to this format.
        /// </summary>
        /// 
        public virtual string Filter { get { return String.Format("{0} ({1})|{1}", Description, Extension); } }


        /// <summary>
        ///   Initializes a new instance of the <see cref="FileFormatBase"/> class.
        /// </summary>
        /// 
        /// <param name="canRead">Whether the application can read files stored in this format.</param>
        /// <param name="canWrite">Whether the application can write to files stored in this format.</param>
        /// <param name="extension">The format's standard file extension.</param>
        /// <param name="description">The format's description.</param>
        /// 
        public FileFormatBase(bool canRead, bool canWrite, string extension, string description)
        {
            this.CanRead = canRead;
            this.CanWrite = canWrite;
            this.Extension = extension;
            this.Description = description;
        }
    }
}
