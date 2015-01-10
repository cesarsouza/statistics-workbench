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
