
using Html_Serializer;
using HtmlSerializer;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

var html = await Load("https://learn.malkabruk.co.il/");

var clearHtml = new Regex(@"[\r\n]+").Replace(html, "");
Regex r = new Regex("<(.*?)>");
var htmlLines = r.Split(clearHtml).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();


var root = new HtmlElement();
var current = root;
//for checkings:)
//var htmlLines = new List<string>()
//{
//    { "!DOCTYPE html" },
//    { "html" },
//    { "div id=\"my-id\" class=\"my-class-1 my-class-2\" width=\"100%\" "},
//    { "div" },
//    { "hello" },
//    { "div class=\"my-class-2\""},
//    { "/div" },
//    { "div class=\"my-class-2\"" },
//    { "/div" },
//    { "h1 class=\"my-class-1 my-class-2\"" },
//    { "banana"},
//    { "/h1" },
//    { "/div"},
//    { "/div"},
//    { "br class=\"my-class-1 my-class-2\"" },
//    { "/html"}
//};

foreach (var line in htmlLines)
{
    Regex regex = new Regex(@"(?:^|\s)(\S+)\b");
    var first = regex.Match(line);
    if (first.Value == "!DOCTYPE")
        continue;

    if (first.Value.StartsWith('/'))
    {
        if (current != null)
            current = current.Parent;
    }

    else
    {
        bool flag2 = false;
        foreach (var message in HtmlHelper.Instance.AllHtmlTags)
            if (message == first.Value)
            {
                flag2 = true;
                var child = new HtmlElement();
                child.Name = first.Value;
                var allWordsInLine = Regex.Split(line, @"\s+");
                var allAttributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line).Select(match => Tuple.Create(match.Groups[1].Value, match.Groups[2].Value))
                     .ToList();
                var classes = new List<string>();

                foreach (var attribute in allAttributes)
                {
                    if (attribute.Item1 == "class")
                    {
                        var allClasses = Regex.Split(attribute.Item2, @"\s+");
                        foreach (var cl in allClasses)
                            classes.Add(cl);
                    }
                    else
                        if (attribute.Item1 == "id")
                        child.Id = attribute.Item2;
                }
                    allAttributes = allAttributes.Where(attribute => !(attribute.Item1 == "class" || attribute.Item1 == "id")).ToList();
                    child.Classes = classes;
                    child.Attributes = allAttributes;
                if (current != null)
                    current.AddChild(child);
                bool flag = false;
                if (message.EndsWith('/'))              
                    flag = true;             
                foreach (var m in HtmlHelper.Instance.SelfClosingTags)
                    if (m == first.Value)
                    {
                        flag = true;
                        break;
                    }
                if (flag == false)
                {
                    current = child;
                }
                break;
            }
        if (flag2 == false)
        {
            current.InnerHtml = first.Value;
        }
    }
    if (first.Value == "/html")
        break;
}


static void PrintTree(HtmlElement element, int depth)
{
    Console.WriteLine(new string(' ', depth * 2) + element.ToString());

    foreach (var child in element.Children)
    {
        PrintTree(child, depth + 1);
    }
}
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var hrml = await response.Content.ReadAsStringAsync();
    return hrml;
}
IEnumerable<HtmlElement> elements = root.Descendants();
string selector = "div#my-id.my-class-1.my-class-2";
Selector n = Selector.RootSelector(selector);
Console.WriteLine("id: "+n.Id +" name: "+n.TagName+" numclasses:"+n.Classes.Count);
List<HtmlElement> nodes = HtmlElement.MatchElement(root, n, new List<HtmlElement>());
foreach(var node in nodes)
{
    Console.WriteLine(node.Name);
}
Console.ReadLine();



