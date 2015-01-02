using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Accord.IO.Csv;

namespace Statistics_Workbench.Formats
{
    public class XmlFileFormat : FileFormatBase, IFileFormat
    {

        public override string Extension { get { return "*.xml"; } }

        public override string Description { get { return "XML Serialization"; } }

        public override bool CanRead { get { return true; } }

        public override bool CanWrite { get { return true; } }

        public override DataTable Read(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            return (DataTable)serializer.Deserialize(stream);
        }

        public override void Write(DataTable table, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(stream, table);
        }
    }

}
