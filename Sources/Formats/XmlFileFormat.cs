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
        /// Gets the file's default extension.
        /// </summary>
        public override string Extension { get { return "*.xml"; } }

        /// <summary>
        /// Gets the format's description.
        /// </summary>
        public override string Description { get { return "XML Serialization"; } }

        /// <summary>
        /// Gets a value indicating whether this format can be read by this application.
        /// </summary>
        public bool CanRead { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether this format can be written by this application.
        /// </summary>
        public bool CanWrite { get { return true; } }

        /// <summary>
        /// Reads the specified file or stream into a table.
        /// </summary>
        public DataTable Read(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            return (DataTable)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Writes the specified table into a file or stream.
        /// </summary>
        public void Write(DataTable table, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(stream, table);
        }
    }

}
