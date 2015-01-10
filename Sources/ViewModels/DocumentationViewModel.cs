using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbench.ViewModels
{

    public class DocumentationViewModel
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public string Summary { get; set; }

        public string Remarks { get; set; }

        public string Example { get; set; }

        public BindingList<string> ExampleCodes { get; set; }

        public BindingList<HyperlinkViewModel> SeeAlso { get; set; }

        public DocumentationViewModel()
        {
            ExampleCodes = new BindingList<string>();
            SeeAlso = new BindingList<HyperlinkViewModel>();
        }
    }

    public class HyperlinkViewModel
    {
        public string Url { get; set; }

        public string Text { get; set; }

        public RelayCommand Go { get; private set; }

        public HyperlinkViewModel()
        {
            Go = new RelayCommand(execute, canExecute);
        }

        private bool canExecute(object obj)
        {
            return Uri.IsWellFormedUriString(Url, UriKind.Absolute);
        }

        private void execute(object obj)
        {
            System.Diagnostics.Process.Start(Url.ToString());
        }
    }
}
