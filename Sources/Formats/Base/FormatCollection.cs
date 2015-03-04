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

    /// <summary>
    ///   File format collection.
    /// </summary>
    public class FormatCollection : Collection<IFileFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatCollection"/> class.
        /// </summary>
        public FormatCollection()
        {

        }

        /// <summary>
        /// Gets a filter string that can be given to a Win32 file save/open dialog
        /// to filter only filenames that match the formats contained in this collection.
        /// </summary>
        /// <param name="includeAllFiles">If set to <c>true</c>, an additional "All files (*.*)"
        /// entry will be included at the end of the filter string.</param>
        /// <returns></returns>
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
