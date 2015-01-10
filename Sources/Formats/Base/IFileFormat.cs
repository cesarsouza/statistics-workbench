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
