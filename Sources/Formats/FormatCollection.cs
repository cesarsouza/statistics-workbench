using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics_Workbench.Formats
{
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
