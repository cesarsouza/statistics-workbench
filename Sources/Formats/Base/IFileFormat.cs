// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Formats
{
    using System.Data;
    using System.IO;

    /// <summary>
    ///   Common interface for file formats supported by this application.
    /// </summary>
    /// 
    public interface IFileFormat
    {
        /// <summary>
        ///   Gets the file's default extension.
        /// </summary>
        /// 
        string Extension { get; }

        /// <summary>
        ///   Gets the format's description.
        /// </summary>
        /// 
        string Description { get; }

        /// <summary>
        ///   Gets the filter expression (e.g. *.txt) that
        ///   should be used to filter filenames that belong
        ///   to this format.
        /// </summary>
        /// 
        string Filter { get; }

        /// <summary>
        ///   Gets a value indicating whether this format can be read by this application.
        /// </summary>
        /// 
        bool CanRead { get; }
        
        /// <summary>
        ///   Gets a value indicating whether this format can be written by this application.
        /// </summary>
        /// 
        bool CanWrite { get; }

        /// <summary>
        ///   Reads the specified file or stream into a table.
        /// </summary>
        /// 
        DataTable Read(Stream stream);

        /// <summary>
        ///   Writes the specified table into a file or stream.
        /// </summary>
        /// 
        void Write(DataTable table, Stream stream);

    }
}
