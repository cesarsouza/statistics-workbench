using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Accord.IO.Csv;

namespace Statistics_Workbench.Formats
{
    public class BinFileFormat : FileFormatBase, IFileFormat
    {

        public override string Extension { get { return "*.bin"; } }

        public override string Description { get { return "Binary serialized DataTable"; } }

        public override bool CanRead { get { return true; } }

        public override bool CanWrite { get { return true; } }

        public override DataTable Read(Stream stream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            return (DataTable)serializer.Deserialize(stream);
        }

        public override void Write(DataTable table, Stream stream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(stream, table);
        }
    }

}
