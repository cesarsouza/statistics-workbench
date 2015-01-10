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
    using System.Xml.Serialization;

    /// <summary>
    ///   Format handler for data tables stored as XML serialized DataTables.
    /// </summary>
    /// 
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
