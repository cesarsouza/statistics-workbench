using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics_Workbench.Formats
{
    public abstract class FileFormatBase : IFileFormat
    {
        public abstract string Extension { get; }
        public abstract string Description { get; }

        public string Filter { get { return String.Format("{0} ({1})|{1}", Description, Extension); } }

        public abstract bool CanRead { get; }
        public abstract bool CanWrite { get; }

        public abstract DataTable Read(Stream stream);

        public abstract void Write(DataTable table, Stream stream);


    }
}
