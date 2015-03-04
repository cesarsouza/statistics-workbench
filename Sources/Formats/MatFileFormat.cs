// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Formats
{
    using Accord.IO;
    using Accord.Math;
    using System;
    using System.Data;
    using System.IO;

    /// <summary>
    ///   Format handler for data tables stored as MATLAB/Octave MAT4 files.
    /// </summary>
    /// 
    public class MatFileFormat : FileFormatBase, IFileFormat
    {
        /// <summary>
        /// Gets the file's default extension.
        /// </summary>
        /// 
        public override string Extension { get { return "*.mat"; } }

        /// <summary>
        /// Gets the format's description.
        /// </summary>
        /// 
        public override string Description { get { return "MAT/Octave files"; } }

        /// <summary>
        /// Gets a value indicating whether this format can be read by this application.
        /// </summary>
        /// 
        public bool CanRead { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether this format can be written by this application.
        /// </summary>
        /// 
        public bool CanWrite { get { return false; } }

        /// <summary>
        /// Reads the specified file or stream into a table.
        /// </summary>
        /// 
        public DataTable Read(Stream stream)
        {
            var reader = new MatReader(new BinaryReader(stream), true);
            var matrix = reader[0].GetValue<double[,]>();
            return matrix.ToTable();
        }

        /// <summary>
        /// Writes the specified table into a file or stream.
        /// </summary>
        /// 
        public void Write(DataTable table, Stream stream)
        {
            throw new NotSupportedException();
        }
    }
}
