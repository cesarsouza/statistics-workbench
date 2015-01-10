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
        public override string Extension { get { return "*.mat"; } }

        public override string Description { get { return "MAT/Octave files"; } }

        public override bool CanRead { get { return true; } }

        public override bool CanWrite { get { return false; } }

        public override DataTable Read(Stream stream)
        {
            var reader = new MatReader(new BinaryReader(stream), true);
            var matrix = reader[0].GetValue<double[,]>();
            return matrix.ToTable();
        }

        public override void Write(DataTable table, Stream stream)
        {
            throw new NotSupportedException();
        }
    }
}
