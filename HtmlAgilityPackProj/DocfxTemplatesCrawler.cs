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

    class Version2
    {
        // private static readonly string IgniteUI_Docfx = "C:/IgniteUI/igniteui-docfx/";
        public static readonly string KR = "kr/components/grids_templates";
        public static readonly string JP = "jp/components/grids_templates";
        public static readonly string EN = "en/components/grids_templates";
        public static readonly string[] versions = { EN, JP };
    }

    class GridsDirs
    {
        public static readonly string GRID = "/grid/";
        public static readonly string HIERARCHICAL = "/hierarchical-grid/";
        public static readonly string TREE = "/tree-grid/";
        public static readonly string[] GRIDS = { GRID, HIERARCHICAL, TREE };
    }
    class DocfxTemplatesCrawler
    {
        static string path = "C:/IgniteUI/igniteui-docfx/";

        static Regex DivRegex = new Regex("<\\s*div\\s*class=\"sample-container[^>]*>\\s*(.*?)\\s*<\\s*\\/\\s*div>", RegexOptions.Singleline);

        static MatchCollection ContainersMatcher(FileInfo file)
        {
            return Regex
                .Matches
                (File.ReadAllText(file.Directory.ToString() + "/" + file.ToString()), DivRegex.ToString(), DivRegex.Options);
        }

        public static FileInfo[] GetFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetFiles();
        }

        public static List<FileInfo> GetAllFiles()
        {
            List<FileInfo> result = new List<FileInfo>();
            foreach (string version in Version2.versions)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(path + version);
                Console.ForegroundColor = ConsoleColor.White;
                foreach (FileInfo file in GetFiles(path + version))
                {
                    result.Add(file);
                    Console.WriteLine(file);
                    Console.WriteLine();
                }
            }
            return result.FindAll(fi => File.ReadAllText(fi.Directory.ToString() + "/" + fi.ToString()).Contains("sample-container"));
        }

        static bool ShouldAddLoadingClass;
        static bool ShouldBeReplaced;
        static int GridTypesCount(MatchCollection Tags)
        {
            var grids = GridsDirs.GRIDS.ToList();
            for (int i = 0; i < Tags.Count; i++)
            {
                foreach (var grid in grids)
                {
                    if (Tags.ElementAt(i).Value.Contains(grid))
                    {
                        grids.Remove(grid);
                        break;
                    }
                }
            }

            return GridsDirs.GRIDS.Length - grids.Count;
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

            var childNode = ParentNode.ChildNodes.ToList().Find(child => child.HasAttributes == true);

            if (!childNode.Attributes.ToList().Exists(attr => attr.Name.Equals("onload")))
            {
                childNode.Attributes.Add("onload", "onSampleIframeContentLoaded(this);");
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
            }
            return node;
        }

        static HtmlNode SampleContainerChildNode(HtmlNode ParentNode)
        {
            var childNode = ParentNode.ChildNodes.ToList().Find(child => child.HasAttributes == true);
            childNode.Attributes.ToList().ForEach(attr =>
            {
                if (attr.Name.Equals("src"))
                    attr.Name = "data-src";
                if (attr.Name.Equals("onload"))
                    childNode.Attributes.Remove("onload");
            });
            childNode.AddClass("lazyload");
            return childNode;
        }

        static void FileChanger(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                ShouldBeReplaced = false;
                string filePath = file.Directory.ToString() + "/" + file.ToString();
                string fileContentManipulator = File.ReadAllText(filePath);
                int fileContentLength = File.ReadAllText(filePath).Length;
                MatchCollection Tags = ContainersMatcher(file);
                int indexDiff = fileContentLength - fileContentManipulator.Length;
                int gridCount = GridTypesCount(Tags);
                if (gridCount == 0)
                {
                    continue;
                }
                for (int i = 0; i < gridCount; i++)
                {
                    HtmlNode Node = FirstSampleContainer(Tags.ElementAt(i));
                    HtmlNode ChildNode = FirstSampleContainerChildNode(Node);
                    if (ChildNode != null)
                    {
                        fileContentManipulator = DivRegex.Replace(fileContentManipulator, Node.OuterHtml, 1, Tags.ElementAt(i).Index - indexDiff);
                        ShouldBeReplaced = true;
                    }
                }
                if (Tags.Skip(gridCount).Count() != 0 || ShouldBeReplaced)
                {
                    foreach (Match item in Tags.Skip(gridCount))
                    {
                        HtmlNode Node = SampleContainer(item);
                        HtmlNode ChildNode = SampleContainerChildNode(Node);

                        indexDiff = fileContentLength - fileContentManipulator.Length;
                        fileContentManipulator = DivRegex.Replace(fileContentManipulator, Node.OuterHtml, 1, item.Index - indexDiff);
                    }
                    File.WriteAllText(filePath, fileContentManipulator);
                }
            }
        }
    }
}