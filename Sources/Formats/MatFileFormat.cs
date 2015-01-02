using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.IO;
using Accord.IO.Mat;
using Accord.Math;

namespace Statistics_Workbench.Formats
{
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
