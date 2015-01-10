// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Formats
{
    using System;
    using System.Data;
    using System.IO;

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
