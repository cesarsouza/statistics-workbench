using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics_Workbench.Formats
{
    public interface IFileFormat
    {
        string Extension { get; }
        string Description { get; }

        string Filter { get; }

        bool CanRead { get; }
        bool CanWrite { get; }

        DataTable Read(Stream stream);

        void Write(DataTable table, Stream stream);

    }
}
