using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NuDoq;

namespace Statistics_Workbench.Models
{
    public class ClassToXamlVisitor : Visitor
    {
        public Dictionary<string, string> Texts { get; set; }

        private MemberIdMap map;

        private MemoryStream stream;
        private IndentedTextWriter writer;

        private bool insideTextBlock = false;

        public ClassToXamlVisitor(MemberIdMap map)
        {
            this.map = map;
            Texts = new Dictionary<string, string>();
        }


        public override void VisitMember(Member member)
        {
            if (member.Kind.HasFlag(MemberKinds.Class))
            {
                stream = new MemoryStream();
                writer = new IndentedTextWriter(new StreamWriter(stream));
                insideTextBlock = false;
                firstSeeAlso = true;

                writer.WriteLine(@"
<StackPanel>
  <StackPanel.Resources>
    
    <Style TargetType=""{x:Type Label}"" x:Key=""Header"">
      <Setter Property=""FontFamily"" Value=""Segoe UI Semibold"" />
      <Setter Property=""FontSize"" Value=""20"" />
      <Setter Property=""FontWeight"" Value=""SemiBold""/>
      <Setter Property=""Margin"" Value=""0,25,0,0"" />
      <Setter Property=""Template"">
        <Setter.Value>
          <ControlTemplate TargetType=""Label"">
            <Grid>
              <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""16""/>
                <ColumnDefinition Width=""1*""/>
              </Grid.ColumnDefinitions>
              <Image Grid.Column=""0"" 
                Source=""/Statistics Workbench;component/Resources/SectionExpanded.png""/>
              <TextBlock Grid.Column=""1"" Text=""{TemplateBinding Content}"" />
            </Grid>
          </ControlTemplate>
        </Setter.Value>
      </Setter>
    </Style>

    <Style TargetType=""{x:Type TextBlock}"" x:Key=""Paragraph"">
      <Setter Property=""FontFamily"" Value=""Segoe UI"" />
      <Setter Property=""FontSize"" Value=""12"" />
      <Setter Property=""FontWeight"" Value=""Normal""/>
      <Setter Property=""TextWrapping"" Value=""Wrap""/>
      <Setter Property=""Margin"" Value=""10,10,10,10"" />
    </Style>

<Style TargetType=""{x:Type TextBlock}"" x:Key=""Code"">
      <Setter Property=""FontFamily"" Value=""Consolas"" />
      <Setter Property=""FontSize"" Value=""12"" />
      <Setter Property=""FontWeight"" Value=""Normal""/>
      <Setter Property=""TextWrapping"" Value=""Wrap""/>
      <Setter Property=""Margin"" Value=""25,10,10,10"" />
    </Style>

    <Style TargetType=""{x:Type TextBlock}"" x:Key=""Summary"" BasedOn=""{StaticResource Paragraph}"" />

  </StackPanel.Resources>");


                writer.Indent += 2;
                base.VisitMember(member);
                writer.Indent -= 2;
                writer.WriteLine("</StackPanel>");

                writer.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                TextReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                Texts[member.Info.Name] = text;
            }
        }

        public override void VisitSummary(Summary summary)
        {
            //writer.WriteLine("<Label Content=\"Summary\" Style=\"{StaticResource Header}\" />");
            writer.WriteLine("<TextBlock Style=\"{StaticResource Summary}\">");
            writer.Indent += 2;
            insideTextBlock = true;
            base.VisitSummary(summary);
            writer.Indent -= 2;
            writer.WriteLine("</TextBlock>");
            insideTextBlock = false;
        }

        public override void VisitRemarks(Remarks remarks)
        {
            writer.WriteLine("<Label Content=\"Description\" Style=\"{StaticResource Header}\" />");
            base.VisitRemarks(remarks);
        }

        public override void VisitPara(Para para)
        {
            writer.WriteLine("<TextBlock Style=\"{StaticResource Paragraph}\">");
            writer.Indent += 2;
            insideTextBlock = true;

            base.VisitPara(para);

            writer.Indent -= 2;
            writer.WriteLine("</TextBlock>");
            insideTextBlock = false;
        }

        public override void VisitList(List list)
        {
            bool addedBlock = false;
            if (!insideTextBlock)
            {
                addedBlock = true;
                writer.WriteLine("<TextBlock>");
                this.insideTextBlock = true;
            }

            base.VisitList(list);

            if (addedBlock)
            {
                writer.WriteLine("</TextBlock>");
                this.insideTextBlock = false;
            }
        }

        public override void VisitItem(Item item)
        {
            if (this.insideTextBlock)
                writer.WriteLine("<LineBreak /> - ");
            base.VisitItem(item);
        }

        public override void VisitListHeader(ListHeader header)
        {
            base.VisitListHeader(header);
        }

        public override void VisitExample(Example example)
        {
            writer.WriteLine("<Label Content=\"Example\" Style=\"{StaticResource Header}\" />");
            base.VisitExample(example);
        }

        public override void VisitCode(Code code)
        {
            writer.WriteLine("<TextBlock Style=\"{StaticResource Code}\">");
            string[] lines = code.Content
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                writer.WriteLine(encode(line) + "<LineBreak/>");
            }

            writer.WriteLine("</TextBlock>");
        }

        public override void VisitSee(See see)
        {
            string baseURL = "http://accord-framework.net/docs/html/";
            string seeURL = see.Cref.Replace(".", "_").Replace(":", "_");
            string docPage = baseURL + seeURL + ".htm";

            string url = docPage;
            string text = parse(see.Cref, false);

            if (!String.IsNullOrEmpty(see.Content))
                writer.Write("<Hyperlink NavigateUri=\"" + url + "\">" + see.Content + "</Hyperlink>");
            else
                writer.Write("<Hyperlink NavigateUri=\"" + url + "\">" + text + "</Hyperlink>");
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

        public override void VisitText(Text text)
        {
            if (!insideTextBlock)
            {
                writer.WriteLine("<TextBlock Style=\"{StaticResource Paragraph}\">");
                writer.Write(encode(text.Content));
                writer.WriteLine("</TextBlock>");
            }
            else
            {
                writer.Write(encode(text.Content));
            }
        }

        public string encode(string text)
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

        bool firstSeeAlso = false;

        public override void VisitSeeAlso(SeeAlso seeAlso)
        {
            if (firstSeeAlso)
            {
                firstSeeAlso = false;
                writer.WriteLine("<Label Content=\"See also (online)\" Style=\"{StaticResource Header}\" />");
            }

            string baseURL = "http://accord-framework.net/docs/html/";
            string seeURL = seeAlso.Cref.Replace(".", "_").Replace(":", "_");
            string docPage = baseURL + seeURL + ".htm";

            string url = docPage;
            string text = parse(seeAlso.Cref, false);

            writer.WriteLine("<TextBlock Style=\"{StaticResource Paragraph}\"> - ");
            writer.Indent += 2;

            if (!String.IsNullOrEmpty(seeAlso.Content))
                writer.Write("<Hyperlink NavigateUri=\"" + docPage + "\">" + seeAlso.Content + "</Hyperlink>");
            else
                writer.Write("<Hyperlink NavigateUri=\"" + docPage + "\">" + text + "</Hyperlink>");

            writer.Indent -= 2;
            writer.WriteLine("</TextBlock>");
        }
    }
}
