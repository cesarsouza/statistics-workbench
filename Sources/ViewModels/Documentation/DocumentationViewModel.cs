// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.ViewModels
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    ///   Documentation view model for displaying documentation regarding one particular 
    ///   statistical distribution. Provides methods to launch code examples in a code
    ///   editor such as Visual Studio.
    /// </summary>
    /// 
    public class DocumentationViewModel
    {
        private Process visualStudio;


        /// <summary>
        ///   Gets or sets the this distribution's name.
        /// </summary>
        /// 
        public string Name { get; set; }

        /// <summary>
        ///   Gets or sets this distribution's summary text.
        /// </summary>
        /// 
        public string Summary { get; set; }

        /// <summary>
        ///   Gets or sets this distribution's remarks section.
        /// </summary>
        /// 
        public string Remarks { get; set; }

        /// <summary>
        ///   Gets or sets this distribution's example section text.
        /// </summary>
        /// 
        public string Example { get; set; }

        /// <summary>
        ///   Gets or sets this distribution's example code sections.
        /// </summary>
        /// 
        public BindingList<string> CodeBlocks { get; set; }

        /// <summary>
        ///   Gets or sets this distribution's see also links.
        /// </summary>
        /// 
        public BindingList<HyperlinkViewModel> SeeAlso { get; set; }

        /// <summary>
        ///   Open example command for opening a code sample in Visual Studio.
        /// </summary>
        /// 
        public RelayCommand OpenExample { get; private set; }



        /// <summary>
        ///   Initializes a new instance of the <see cref="DocumentationViewModel"/> class.
        /// </summary>
        /// 
        public DocumentationViewModel()
        {
            CodeBlocks = new BindingList<string>();
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
    
}
