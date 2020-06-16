using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
namespace HtmlAgilityPackProj
{
    class Version
    {
        public static readonly string KR = "kr/components";
        public static readonly string JP = "jp/components";
        public static readonly string EN = "en/components";
        public static readonly string[] versions = { KR};
    }
    class Program
    {

        static string path = "C:/IgniteUI/igniteui-docfx/";
        static string xplatPath = "C:/IgniteUI/xplat-docfx/doc/";
        static Regex TopicHeadRegex = new Regex("---");
        static Regex DivRegex = new Regex("<\\s*div\\s*class=\"sample-container[^>]*>\\s*(.*?)\\s*<\\s*\\/\\s*div>", RegexOptions.Singleline);
        static Regex StylingSectionRegex = new Regex("^([#]{1,3}\\s*Styling)\\s\n", RegexOptions.Multiline);

        static Regex IframeAndButtonRegex = new Regex(
            "<\\s*div\\s*class=\"sample-container[^>]*>" +
            "\\s*<\\s*iframe[^>]*>\\s*<\\s*\\/iframe\\s*>\\s*" +
            "<\\s*\\/\\s*div>\\s*" +
            "<\\s*div\\s*>\\s*" +
            "<\\s*button\\s*[^>]*class=\"stackblitz-btn\"[^>]*>" +
            "\\s*(.*?)\\s*" +
            "<\\s*\\/\\s*button>" +
            "\\s*<\\s*\\/\\s*div\\s*>", RegexOptions.Singleline);
        static Regex StackBlitzButtonRegex = new Regex("<\\s*button\\s*[^>]*class=\"stackblitz-btn\"[^>]*>\\s*(.*?)\\s*<\\s*\\/\\s*button>", RegexOptions.Singleline);
        static Match ContainerMatcher(FileInfo file)
        {
            return Regex
                .Match
                (File.ReadAllText(file.Directory.ToString() + "/" + file.ToString()), DivRegex.ToString(), DivRegex.Options);
        }

        public static Match SectionContainerMatcher(string section)
        {
            return Regex
                .Match
                (section, DivRegex.ToString(), DivRegex.Options);
        }

        static string StylingSection(FileInfo file, Regex regex)
        {
            string fileContent = File.ReadAllText(file.Directory.ToString() + "/" + file.ToString());
            int sectionIndex =
    Regex
    .Match
    (fileContent, regex.ToString(), regex.Options).Index;
            if (sectionIndex != 0)
            {
                return fileContent.Substring(sectionIndex);
            }

            return null;
        }

        static Match TopicHeadMatch(FileInfo file)
        {
            return Regex
                .Match
                (File.ReadAllText(file.Directory.ToString() + "/" + file.ToString()), TopicHeadRegex.ToString());
        }

        static MatchCollection IframeAndButtonMatcher(FileInfo file)
        {
            return Regex
                .Matches
                (File.ReadAllText(file.Directory.ToString() + "/" + file.ToString()), IframeAndButtonRegex.ToString(), IframeAndButtonRegex.Options);
        }

        static Match StackBlitzButtonMatcher(FileInfo file)
        {
            return Regex
                .Match
                (File.ReadAllText(file.Directory.ToString() + "/" + file.ToString()), StackBlitzButtonRegex.ToString(), StackBlitzButtonRegex.Options);
        }

        public static FileInfo[] GetFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetFiles();
        }
        public static DirectoryInfo[] GetDirectories(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            List<DirectoryInfo> DirList = di.GetDirectories().ToList();
            //DirList.RemoveAll((dir) => { return dir.Name.Equals("grids_templates"); });
            return DirList.ToArray();
        }

        public static List<FileInfo> GetAllFilesWithDirs()
        {
            List<FileInfo> result = new List<FileInfo>();
            foreach (string version in Version.versions)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine(path + version);
                //Console.ForegroundColor = ConsoleColor.White;
                if (GetDirectories(path + version).Length != 0)
                {
                    foreach (DirectoryInfo dir in GetDirectories(path + version))
                    {
                        if (!dir.Name.Contains("grids_templates"))
                        {
                            continue;
                        }
                        //Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine(dir);
                        //Console.ForegroundColor = ConsoleColor.White;
                        foreach (FileInfo file in dir.GetFiles())
                        {
                            result.Add(file);
                            //Console.WriteLine(file);
                            //Console.WriteLine();
                        }
                    }
                }
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine("Unpackaged");
                //Console.ForegroundColor = ConsoleColor.White;
                foreach (FileInfo file in GetFiles(path + version))
                {
                    result.Add(file);
                    //Console.WriteLine(file);
                    //Console.WriteLine();
                }
            }

            return result.FindAll(fi => !fi.Name.Contains("map_") && !fi.Name.Contains("chart") && !fi.Name.Contains("graph")
                && !fi.Name.Contains("gauge") && !fi.Name.Contains("spreadsheet") && !fi.Name.Contains("toc") && !fi.Name.Contains("excel_library"));
        }
        public static List<FileInfo> GetAllFiles()
        {
            List<FileInfo> result = new List<FileInfo>();
            foreach (string version in Version.versions)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unpackaged");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (FileInfo file in GetFiles(path + version))
                {
                    result.Add(file);
                    Console.WriteLine(file);
                    Console.WriteLine();
                }
            }
            //|| fi.Name.Contains("bulletgraph")
            //|| fi.Name.Contains("excel_library") || fi.Name.Contains("exporter")
            //|| fi.Name.Contains("gauge)"
            //.FindAll(fi => File.ReadAllText(fi.Directory.ToString() + "/" + fi.ToString()).Contains("sample-container"))
            return result.FindAll(fi => File.ReadAllText(fi.Directory.ToString() + "/" + fi.ToString()).Contains("sample-container") && !fi.Name.Contains("map_") && !fi.Name.Contains("chart") && !fi.Name.Contains("graph")
                && !fi.Name.Contains("gauge") && !fi.Name.Contains("spreadsheet") && !fi.Name.Contains("toc") && !fi.Name.Contains("excel_library") && !fi.Name.Contains("sparkline"));
        }

        public static List<FileInfo> GetAllXplatFiles()
        {
            List<FileInfo> result = new List<FileInfo>();

            foreach (string version in Version.versions)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unpackaged");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (FileInfo file in GetFiles(xplatPath + version))
                {
                    result.Add(file);
                    if (!file.Name.Contains("grid_table"))
                    {
                        Console.WriteLine(file);
                        Console.WriteLine();
                    }

                }
            }

            //|| fi.Name.Contains("bulletgraph")
            //|| fi.Name.Contains("excel_library") || fi.Name.Contains("exporter")
            //|| fi.Name.Contains("gauge)"
            //.FindAll(fi => File.ReadAllText(fi.Directory.ToString() + "/" + fi.ToString()).Contains("sample-container"))
            return result.FindAll(fi => File.ReadAllText(fi.Directory.ToString() + "/" + fi.ToString()).Contains("sample-container") && fi.Name.Contains("chart"));
        }

        private static readonly System.Threading.EventWaitHandle waitHandle = new System.Threading.AutoResetEvent(false);

        static bool ShouldBeReplaced;
        static bool ShouldAddLoadingClass;

        static HtmlNode GetIframe(Match MatchedTag)
        {
            var node = HtmlNode.CreateNode(DivRegex.Match(MatchedTag.Value).Value);

            return node.ChildNodes.FindFirst("iframe");
        }

        static HtmlNode GetButton(Match MatchedTag)
        {
            var node = HtmlNode.CreateNode(StackBlitzButtonRegex.Match(MatchedTag.Value).Value);
            return node;
        }

        static bool CheckIframeId(HtmlNode SampleContainer, HtmlNode StackBlitzButton)
        {

            var sampleIframeId = SampleContainer.Attributes.ToList().Find(attr => attr.Name.Contains("id"));
            var buttonDataIframeId = StackBlitzButton.Attributes.ToList().Find(attr => attr.Name.Contains("data-iframe-id"));
            if (sampleIframeId.Value.Equals(buttonDataIframeId.Value))
            {
                return false;
            }
            StackBlitzButton.Attributes.ToList().Find(attr => attr.Name.Contains("data-iframe-id")).Value = sampleIframeId.Value;
            return true;
        }

        static HtmlNode FirstSampleContainer(Match MatchedTag)
        {

            ShouldAddLoadingClass = true;
            var node = HtmlNode.CreateNode(MatchedTag.Value);
            node.OwnerDocument.OptionOutputOriginalCase = true;
            if (!node.Attributes.ToList().Find(attr => attr.Name.Equals("class")).Value.Contains("loading"))
            {
                node.AddClass("loading");
            }
            else
            {
                ShouldAddLoadingClass = false;
            }
            return node;
        }

        static HtmlNode FirstSampleContainerChildNode(HtmlNode ParentNode)
        {
            var childNode = ParentNode.ChildNodes.FindFirst("iframe");
            if (!childNode.Attributes.ToList().Exists(attr => attr.Name.Equals("onload")))
            {
                childNode.SetAttributeValue("onload", "onDvSampleIframeContentLoaded(this);");
                return childNode;
            }
            if (ShouldAddLoadingClass == true)
                return childNode;
            return null;
        }

        static HtmlNode SampleContainer(Match MatchedTag)
        {
            var node = HtmlNode.CreateNode(MatchedTag.Value);
            node.OwnerDocument.OptionOutputOriginalCase = true;
            if (!node.Attributes.ToList().Find(attr => attr.Name.Equals("class")).Value.Contains("loading"))
            {
                node.AddClass("loading");
                ShouldBeReplaced = true;
            }

            return node;
        }

        static HtmlNode SampleContainerChildNode(HtmlNode ParentNode)
        {
            var childNode = ParentNode.ChildNodes.FindFirst("iframe");
            childNode.OwnerDocument.OptionOutputOriginalCase = true;
            var flag = 0;
            childNode.Attributes.ToList().ForEach(attr =>
            {
                if (attr.Name.Equals("src"))
                {
                    attr.Name = "data-src";
                    flag = 1;
                }

                if (attr.Name.Equals("onload"))
                {
                    childNode.Attributes.Remove("onload");
                    flag = 1;
                }

                if (!childNode.HasClass("lazyload"))
                {
                    childNode.AddClass("lazyload");
                    flag = 1;
                }
            });
            if (flag == 1)
            {
                ShouldBeReplaced = true;
            }
            return childNode;
        }
        static void AddLazyLoading(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {

                ShouldBeReplaced = false;
                string filePath = file.Directory.ToString() + "/" + file.ToString();
                string fileContentManipulator = File.ReadAllText(filePath);
                int fileContentLength = File.ReadAllText(filePath).Length;
                Match TagString = ContainerMatcher(file);

                //   Console.WriteLine(file);

                HtmlNode Node = FirstSampleContainer(TagString);
                HtmlNode ChildNode = FirstSampleContainerChildNode(Node);
                int indexDiff = fileContentLength - fileContentManipulator.Length;
                if (ChildNode != null)
                {
                    fileContentManipulator = DivRegex.Replace(fileContentManipulator, Node.OuterHtml, 1, TagString.Index - indexDiff);
                    ShouldBeReplaced = true;
                }
                TagString = TagString.NextMatch();
                if (TagString.Success)
                {
                    Console.WriteLine(file);
                    while (TagString.Success)
                    {
                        Node = SampleContainer(TagString);
                        ChildNode = SampleContainerChildNode(Node);

                        indexDiff = fileContentLength - fileContentManipulator.Length;
                        fileContentManipulator = DivRegex.Replace(fileContentManipulator, Node.OuterHtml, 1, TagString.Index - indexDiff);

                        TagString = TagString.NextMatch();
                    }
                }

                if (ShouldBeReplaced)
                    File.WriteAllText(filePath, fileContentManipulator);
            }
        }

        static void ChangeXPlatHandler(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {

                string filePath = file.Directory.ToString() + "/" + file.ToString();
                string fileContentManipulator = File.ReadAllText(filePath);
                int fileContentLength = File.ReadAllText(filePath).Length;
                Match TagString = ContainerMatcher(file);

                Console.WriteLine(file);

                HtmlNode IframeDivContainer = HtmlNode.CreateNode(TagString.Value);
                IframeDivContainer.OwnerDocument.OptionOutputOriginalCase = true;
                IframeDivContainer
                    .ChildNodes
                    .FindFirst("iframe")
                    .Attributes
                    .ToList()
                    .Find(attr => attr.Name.Equals("onload"))
                    .Value = "onXPlatSampleIframeContentLoaded(this);";

                fileContentManipulator = DivRegex.Replace(fileContentManipulator, IframeDivContainer.OuterHtml, 1, TagString.Index);

                File.WriteAllText(filePath, fileContentManipulator.Replace("=\"\"", ""));
            }

        }

        static void FixIframeIds(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                ShouldBeReplaced = false;
                string filePath = file.Directory.ToString() + "/" + file.ToString();
                string fileContentManipulator = File.ReadAllText(filePath);
                int fileContentLength = File.ReadAllText(filePath).Length;
                int indexDiff = fileContentLength - fileContentManipulator.Length;
                List<Match> IframesWithButtons = IframeAndButtonMatcher(file).ToList();
                Console.WriteLine(file);
                IframesWithButtons.ForEach(match =>
                {
                    var iframe = GetIframe(match);
                    var button = GetButton(match);
                    if (CheckIframeId(iframe, button))
                    {
                        indexDiff = fileContentLength - fileContentManipulator.Length;
                        fileContentManipulator = StackBlitzButtonRegex.Replace(fileContentManipulator, button.OuterHtml, 1, match.Index - indexDiff);
                        ShouldBeReplaced = true;
                    }

                });

                if (ShouldBeReplaced)
                {
                    File.WriteAllText(filePath, fileContentManipulator.Replace("=\"\"", ""));

                }
            }
        }

        static void XPlatChangeDemosUrl(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file);

                string filePath = file.Directory.ToString() + "/" + file.ToString();
                string fileContentManipulator = File.ReadAllText(filePath);
                int fileContentLength = File.ReadAllText(filePath).Length;
                int indexDiff = fileContentLength - fileContentManipulator.Length;
                fileContentManipulator = fileContentManipulator.Replace("{environment:demosBaseUrl}", "{environment:dvDemosBaseUrl}");
                File.WriteAllText(filePath, fileContentManipulator.Replace("=\"\"", ""));
            }
        }

        static void AddVersion(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                string filePath = file.Directory.ToString() + "/" + file.ToString();
                string fileContentManipulator = File.ReadAllText(filePath);
                int fileContentLength = File.ReadAllText(filePath).Length;
                int indexDiff = fileContentLength - fileContentManipulator.Length;
                Match HeadMatch = TopicHeadMatch(file);
                Console.WriteLine(file);
                var open = HeadMatch.Index;
                var close = HeadMatch.NextMatch().Index;
                var head = fileContentManipulator.Substring(open, close);
                if (!head.Contains("_language"))
                {
                    var version = "ja";

                    var newHead = head + String.Format("_language: {0}\n", version);
                    fileContentManipulator = fileContentManipulator.Replace(head, newHead);
                    File.WriteAllText(filePath, fileContentManipulator);
                    List<Match> IframesWithButtons = IframeAndButtonMatcher(file).ToList();
                    Console.WriteLine(file);
                    IframesWithButtons.ForEach(match =>
                    {
                        var iframe = GetIframe(match);
                        var button = GetButton(match);
                        if (CheckIframeId(iframe, button))
                        {
                            indexDiff = fileContentLength - fileContentManipulator.Length;
                            fileContentManipulator = StackBlitzButtonRegex.Replace(fileContentManipulator, button.OuterHtml, 1, match.Index - indexDiff);
                            ShouldBeReplaced = true;
                        }

                    });

                    if (ShouldBeReplaced)
                    {
                        File.WriteAllText(filePath, fileContentManipulator.Replace("=\"\"", ""));

                    }
                }

            }
        }

        static HtmlNode AppendNoStyleClass(Match MatchString)
        {

            var node = HtmlNode.CreateNode(MatchString.Value);
            node.OwnerDocument.OptionOutputOriginalCase = true;

            var childNode = node.ChildNodes.FindFirst("iframe");

            if (!childNode.HasClass("no-theming"))
            {
                childNode.AddClass("no-theming");
                ShouldBeReplaced = true;
            }
            if (ShouldBeReplaced == true)
                return node;
            ShouldBeReplaced = false;
            return null;
        }

        static void Add_NoTheme_Class(List<FileInfo> files)
        {
            int i = 0;

            foreach (FileInfo file in files)
            {

                ShouldBeReplaced = false;
                string filePath = file.Directory.ToString() + "/" + file.ToString();
                string fileContent = File.ReadAllText(filePath);

                string stylingSection = StylingSection(file, StylingSectionRegex);
                if (stylingSection != null)
                {
                    int stylingSectionContentLength = stylingSection.Length;
                    string stylingSectionManipulator = stylingSection;
                    Match TagString = SectionContainerMatcher(stylingSection);
                    int indexDiff = stylingSectionContentLength - stylingSectionManipulator.Length;
                    while (TagString != null && TagString.Success)
                    {
                        Console.WriteLine(file);

                        HtmlNode Node = AppendNoStyleClass(TagString);
                        if (Node != null)
                        {
                            stylingSectionManipulator = DivRegex.Replace(stylingSectionManipulator, Node.OuterHtml, 1, TagString.Index - indexDiff);
                            indexDiff = stylingSectionContentLength - stylingSectionManipulator.Length;
                        }
                        Console.WriteLine(file);

                        TagString = TagString.NextMatch();
                    }

                    if (ShouldBeReplaced)
                        File.WriteAllText(filePath, fileContent.Replace(stylingSection, stylingSectionManipulator.Replace("=\"\"", "")));

                }


            }
            Console.WriteLine(i);

        }

        static void Main(string[] args)
        {
            List<FileInfo> files = GetAllXplatFiles();

            XPlatChangeDemosUrl(files);
        }
    }
}

