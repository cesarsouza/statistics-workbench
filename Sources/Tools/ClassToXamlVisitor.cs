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
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using Workbench.ViewModels;

    public class ClassToXamlVisitor : Visitor
    {
        public Dictionary<string, DocumentationViewModel> Texts { get; set; }

        private string currentClassName;
        private DocumentationViewModel current;

        private MemberIdMap map;

        private StringBuilder builder;

        private bool insideTextBlock = false;

        public ClassToXamlVisitor(MemberIdMap map)
        {
            this.map = map;
            Texts = new Dictionary<string, DocumentationViewModel>();
        }


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

        public override void VisitSummary(Summary summary)
        {
            builder.Clear();
            insideTextBlock = false;

            builder.AppendLine("<StackPanel>");
            base.VisitSummary(summary);
            builder.AppendLine("</StackPanel>");

            current.Summary = builder.ToString();
        }

        public override void VisitRemarks(Remarks remarks)
        {
            builder.Clear();
            insideTextBlock = false;

            builder.AppendLine("<StackPanel>");
            base.VisitRemarks(remarks);
            builder.AppendLine("</StackPanel>");

            current.Remarks = builder.ToString();
        }

        public override void VisitExample(Example example)
        {
            builder.Clear();
            insideTextBlock = false;

            builder.AppendLine("<StackPanel>");
            base.VisitExample(example);
            builder.AppendLine("</StackPanel>");

            current.Example = builder.ToString();
        }


        public override void VisitPara(Para para)
        {
            builder.AppendLine("<TextBlock>");
            insideTextBlock = true;

            base.VisitPara(para);

            builder.AppendLine("</TextBlock>");
            insideTextBlock = false;
        }

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

        public override void VisitItem(Item item)
        {
            if (this.insideTextBlock)
                builder.AppendLine("<LineBreak /> - ");
            base.VisitItem(item);
        }

        public override void VisitListHeader(ListHeader header)
        {
            base.VisitListHeader(header);
        }



        public override void VisitCode(Code code)
        {
            current.ExampleCodes.Add(code.Content);

            string[] lines = code.Content
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);


            builder.Append("<ContentControl xml:space=\"preserve\" Tag=\"Code\">");

            foreach (var line in lines)
                builder.AppendLine(encode(line));

            builder.AppendLine("</ContentControl>");
        }



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

        public override void VisitSee(See see)
        {
            string url = DistributionManager.GetDocumentationUrl(see.Cref);
            string text = parse(see.Cref, false);

            if (!String.IsNullOrEmpty(see.Content))
                text = see.Content;

            builder.Append(hyperlink(url, text));
        }

        public override void VisitAnchor(Anchor anchor)
        {
            string url = DistributionManager.GetDocumentationUrl(anchor.Href);
            string text = parse(anchor.Href, false);

            if (!String.IsNullOrEmpty(anchor.Content))
                text = anchor.Content;

            builder.Append(hyperlink(url, text));
        }

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
            return " <Hyperlink NavigateUri=\"" + url + "\">" + text + "</Hyperlink> ";
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

        public static string encode(string text)
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
