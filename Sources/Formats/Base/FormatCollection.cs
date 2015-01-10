// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Formats
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class FormatCollection : Collection<IFileFormat>
    {
        public FormatCollection()
        {

        }

        public string GetFilterString(bool includeAllFiles)
        {
            var filters = new List<string>();
            foreach (var format in this)
                filters.Add(format.Filter);

            if (includeAllFiles)
                filters.Add("All files (*.*)|(*.*)");

            return String.Join("|", filters);
        }
    }
}
