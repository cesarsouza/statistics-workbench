// Statistics Workbench
// http://accord-framework.net
//
// The MIT License (MIT)
// Copyright © 2014-2015, César Souza
//

namespace Workbench.Tools
{
    using NuDoq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using Workbench.ViewModels;

    /// <summary>
    ///   NuDoq's visitor implementation used to parse through classes XML documentation
    ///   files and generate XAML code containing such documentation. An object of this
    ///   class should be given to a NuDoq.AssemblyMember's Visit method. Afterwards,
    ///   the generated documentation will be available as XAML codes in this object's
    ///   Text property.
    /// </summary>
    /// 
    public class ClassToXamlVisitor : Visitor
    {

        private string currentClassName;
        private DocumentationViewModel current;

        private MemberIdMap map;
        private StringBuilder builder;
        private bool insideTextBlock = false;


        /// <summary>
        ///   Initializes a new instance of the <see cref="ClassToXamlVisitor"/> class.
        /// </summary>
        /// 
        /// <param name="map">The member-id map given by the NuDoq parser.</param>
        /// 
        public ClassToXamlVisitor(MemberIdMap map)
        {
            this.map = map;
            this.Texts = new Dictionary<string, DocumentationViewModel>();
        }


        /// <summary>
        ///   Gets the collection of XAML code generated for documented members.
        /// </summary>
        /// 
        public Dictionary<string, DocumentationViewModel> Texts { get; private set; }



        /// <summary>
        ///   Visit the generic base class <see cref="T:NuDoq.Member" />.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method is called for all <see cref="T:NuDoq.Member" />-derived types.
        /// </remarks>
        /// 
        public override void VisitMember(Member member)
        {
            if (member.Kind.HasFlag(MemberKinds.Class))
            {
                this.currentClassName = member.Info.Name;
                current = new DocumentationViewModel();
                Texts[currentClassName] = current;

                builder = new StringBuilder();
                insideTextBlock = false;

                base.VisitMember(member);
            }
        }

        /// <summary>
        ///   Visits the <c>summary</c> documentation element.
        /// </summary>
        /// 
        public override void VisitSummary(Summary summary)
        {
            builder.Clear();
            insideTextBlock = false;

            builder.AppendLine("<StackPanel>");
            base.VisitSummary(summary);
            builder.AppendLine("</StackPanel>");

            current.Summary = builder.ToString();
        }

        /// <summary>
        ///   Visits the <c>remarks</c> documentation element.
        /// </summary>
        /// 
        public override void VisitRemarks(Remarks remarks)
        {
            builder.Clear();
            insideTextBlock = false;

            builder.AppendLine("<StackPanel>");
            base.VisitRemarks(remarks);
            builder.AppendLine("</StackPanel>");

            current.Remarks = builder.ToString();
        }

        /// <summary>
        ///   Visits the <c>example</c> documentation element.
        /// </summary>
        /// 
        public override void VisitExample(Example example)
        {
            builder.Clear();
            insideTextBlock = false;

            builder.AppendLine("<StackPanel>");
            base.VisitExample(example);
            builder.AppendLine("</StackPanel>");

            current.Example = builder.ToString();
        }


        /// <summary>
        ///   Visits the <c>para</c> documentation element.
        /// </summary>
        /// 
        public override void VisitPara(Para para)
        {
            builder.AppendLine("<TextBlock>");
            insideTextBlock = true;

            base.VisitPara(para);

            builder.AppendLine("</TextBlock>");
            insideTextBlock = false;
        }

        /// <summary>
        ///   Visits the <c>list</c> documentation element.
        /// </summary>
        /// 
        public override void VisitList(List list)
        {
            bool addedBlock = false;
            if (!insideTextBlock)
            {
                addedBlock = true;
                builder.AppendLine("<TextBlock>");
                this.insideTextBlock = true;
            }

            base.VisitList(list);

            if (addedBlock)
            {
                builder.AppendLine("</TextBlock>");
                this.insideTextBlock = false;
            }
        }

        /// <summary>
        ///   Visits the <c>item</c> documentation element.
        /// </summary>
        /// 
        public override void VisitItem(Item item)
        {
            if (this.insideTextBlock)
                builder.AppendLine("<LineBreak /> - ");
            base.VisitItem(item);
        }

        /// <summary>
        ///   Visits the <c>listheader</c> documentation element.
        /// </summary>
        /// 
        public override void VisitListHeader(ListHeader header)
        {
            base.VisitListHeader(header);
        }

        /// <summary>
        ///   Visits the <c>code</c> documentation element.
        /// </summary>
        /// 
        public override void VisitCode(Code code)
        {
            current.CodeBlocks.Add(code.Content);


            if (code.Source == null)
            {
                string[] lines = code.Content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                builder.Append("<ContentControl xml:space=\"preserve\" Tag=\"Code\">");
                foreach (var line in lines)
                    builder.AppendLine(encode(line));
                builder.AppendLine("</ContentControl>");
                return;
            }

            string source = code.Source;
            string region = code.Region;
            string baseAddress = "https://raw.githubusercontent.com/accord-net/framework/development/";
            string sourceCode;

            try
            {
                // Download the source file from the Accord.NET GitHub website
                using (var client = new WebClient())
                {
                    string sourceAddress = baseAddress + source;
                    Stream stream = client.OpenRead(sourceAddress);
                    TextReader reader = new StreamReader(stream);
                    sourceCode = reader.ReadToEnd();
                }

                string startMark = "#region " + region;
                string endMark = "#endregion";

                int startIndex = sourceCode.IndexOf(startMark) + startMark.Length;
                int endIndex = sourceCode.IndexOf(endMark, startIndex) - endMark.Length;
                string exampleCode = sourceCode.Substring(startIndex, endIndex - startIndex);

                // Remove indentation
                string trimmedCode = exampleCode.TrimStart();
                int whitespace = exampleCode.Length - trimmedCode.Length - 2; // 2 because assuming \r\n
                string indentMarker = "\r\n" + new String(' ', whitespace);
                exampleCode = exampleCode.Replace(indentMarker, "\r\n").Trim();

                builder.Append("<ContentControl xml:space=\"preserve\" Tag=\"Code\">");
                builder.Append(encode(exampleCode));
                builder.AppendLine("</ContentControl>");
            }
            catch 
            {
                builder.Append("<ContentControl xml:space=\"preserve\" Tag=\"Code\">");
                builder.Append(encode("The documentation could not be found online."));
                builder.AppendLine("</ContentControl>");
            }
        }

        /// <summary>
        ///   Visits the literal text inside other documentation elements.
        /// </summary>
        /// 
        public override void VisitText(Text text)
        {
            if (!insideTextBlock)
            {
                builder.AppendLine("<TextBlock>");
                builder.Append(encode(text.Content));
                builder.AppendLine("</TextBlock>");
            }
            else
            {
                builder.Append(encode(text.Content));
            }
        }

        /// <summary>
        ///   Visits the <c>see</c> documentation element.
        /// </summary>
        /// 
        public override void VisitSee(See see)
        {
            string url = DistributionManager.GetDocumentationUrl(see.Cref);
            string text = parse(see.Cref, false);

            if (!String.IsNullOrEmpty(see.Content))
                text = see.Content;

            builder.Append(hyperlink(url, text));
        }

        /// <summary>
        ///   Visits the <c>a</c> extended documentation element.
        /// </summary>
        /// 
        public override void VisitAnchor(Anchor anchor)
        {
            string url = DistributionManager.GetDocumentationUrl(anchor.Href);
            string text = parse(anchor.Href, false);

            if (!String.IsNullOrEmpty(anchor.Content))
                text = anchor.Content;

            builder.Append(hyperlink(url, text));
        }

        /// <summary>
        ///   Visits the <c>seealso</c> documentation element.
        /// </summary>
        /// 
        public override void VisitSeeAlso(SeeAlso seeAlso)
        {
            string url = DistributionManager.GetDocumentationUrl(seeAlso.Cref);
            string text = parse(seeAlso.Cref, false);

            var hyperlink = new HyperlinkViewModel() { Url = url, Text = text };

            if (!String.IsNullOrEmpty(seeAlso.Content))
                hyperlink.Text = seeAlso.Content;

            current.SeeAlso.Add(hyperlink);
        }



        private static string hyperlink(string url, string text)
        {
            return " <Hyperlink NavigateUri=\"" + url + "\">" + encode(text) + "</Hyperlink> ";
        }

        private string parse(string cref, bool full)
        {
            string toAppend = String.Empty;
            MemberInfo m = map.FindMember(cref);

            if (m != null)
            {
                toAppend = m.Name;

                if (!full)
                    toAppend = cref.Substring(cref.LastIndexOf(".") + 1);
                else
                {
                    TypeInfo t = m as TypeInfo;

                    if (t != null)
                        toAppend = t.FullName;
                }
            }

            return toAppend;
        }

        private static string encode(string text)
        {
            char[] chars = HttpUtility.HtmlEncode(text).ToCharArray();
            StringBuilder result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);

                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}
