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
        /// <summary>
        ///   Initializes a new instance of the <see cref="XmlFileFormat"/> class.
        /// </summary>
        /// 
        public XmlFileFormat()
            : base(extension: "*.xml", description: "XML Serialization", canRead: true, canWrite: true)
        {
        }

        /// <summary>
        ///   Reads the specified file or stream into a table.
        /// </summary>
        /// 
        public DataTable Read(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            return (DataTable)serializer.Deserialize(stream);
        }

        /// <summary>
        ///   Writes the specified table into a file or stream.
        /// </summary>
        /// 
        public void Write(DataTable table, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(stream, table);
        }
    }
}
