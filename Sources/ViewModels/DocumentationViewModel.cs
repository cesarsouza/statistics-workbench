using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        public RelayCommand OpenExample { get; private set; }

        private Process visualStudio;


        public DocumentationViewModel()
        {
            ExampleCodes = new BindingList<string>();
            SeeAlso = new BindingList<HyperlinkViewModel>();

            OpenExample = new RelayCommand(OpenExample_Execute, OpenExample_CanExecute);
        }

        private void OpenExample_Execute(object obj)
        {
            string source = obj as string;

            var code = new StringBuilder();
            foreach (var line in source.Split('\n'))
                code.AppendLine("            " + line);

            var template = File.ReadAllText(@"Template\Program.default.cs");
            var program = template.Replace("#pragma CODE", code.ToString());
            File.WriteAllText(@"Template\Program.cs", program);

            if (visualStudio == null || !visualStudio.HasExited)
                visualStudio = Process.Start(@"Template\Template.sln");
        }

        private bool OpenExample_CanExecute(object obj)
        {
            return obj is string;
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
