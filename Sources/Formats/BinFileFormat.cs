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
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    ///   Format handler for data tables stored as binary serialized DataTables.
    /// </summary>
    /// 
    public class BinFileFormat : FileFormatBase, IFileFormat
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="BinFileFormat"/> class.
        /// </summary>
        /// 
        public BinFileFormat()
            : base(extension: "*.bin", description: "Binary serialized DataTable", canRead: true, canWrite: true)
        {
        }

        /// <summary>
        ///   Reads the specified file or stream into a table.
        /// </summary>
        /// 
        public DataTable Read(Stream stream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            return (DataTable)serializer.Deserialize(stream);
        }

        /// <summary>
        ///   Writes the specified table into a file or stream.
        /// </summary>
        /// 
        public void Write(DataTable table, Stream stream)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(stream, table);
        }
    }
}
