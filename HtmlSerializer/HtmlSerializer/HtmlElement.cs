using Html_Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
   internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Tuple<string, string>> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement(string id = null, string name = null, List<Tuple<string, string>> attributes = null,
                           List<string> classes = null, string innerHtml = null)
        {
            Id = id;
            Name = name;
            Attributes = attributes ?? new List<Tuple<string, string>>();
            Classes = classes ?? new List<string>();
            InnerHtml = innerHtml;
            Parent = null;
            Children = new List<HtmlElement>();
        }

        public void AddChild(HtmlElement childElement)
        {
            childElement.Parent = this;
            Children.Add(childElement);
        }

        public override string ToString()
        {
            string result = $"<{Name}";

            if (!string.IsNullOrEmpty(Id))
                result += $" id=\"{Id}\"";

            if (Attributes.Count > 0)
                result += $" {string.Join(" ", Attributes.Select(attr => $"{attr.Item1}=\"{attr.Item2}\""))}";

            if (Classes.Count > 0)
                result += $" class=\"{string.Join(" ", Classes)}\"";

            //if the tags is not close
            foreach (var message in HtmlHelper.Instance.SelfClosingTags)
                if (message == Name)
                    return (result += "/>");

            result += ">";

            if (!string.IsNullOrEmpty(InnerHtml))
                result += $"\n  {InnerHtml}\n";
            //check!!!!
            foreach (var child in Children)
                result += $"  {child.ToString()}\n";
            result += $"</{Name}>";
            return result;
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            if (this == null)
            {
                yield return this;
            }
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                HtmlElement element = queue.Dequeue();
                yield return element;
                foreach (HtmlElement child in element.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        public static IEnumerable<HtmlElement> Ancestors(HtmlElement node)
        {
            if (node == null)
            {
                yield return node;
            }
            while (node.Parent != null)
            {
                yield return node.Parent;
                node = node.Parent;
            }
        }
        public static bool EqualClasses(List<string> e, List<string> s)
        {
            for (int i = 0; i < e.Count; i++)
            {
                if (e[i] != s[i])
                    return false;
            }
            return true;
        }
        public static List<HtmlElement> MatchElement(HtmlElement element, Selector selector, List<HtmlElement> elements)
        {
            if (selector == null)
                return null;
            IEnumerable<HtmlElement> allDecendents = element.Descendants();
            foreach (HtmlElement el in allDecendents)
            {
                if (selector.Id != el.Id)
                { continue; }
                if (selector.TagName != el.Name)
                { continue; }
                if (el.Classes != null && selector.Classes != null 
                    && el.Classes.Count == selector.Classes.Count)
                {
                    if (!(el.Classes.Intersect(selector.Classes).Any()))
                        continue;
                }
                else
                { continue; }
                if (selector.Child == null)
                {
                    elements.Add(el);
                }
                else
                {
                    MatchElement(el, selector.Child, elements);
                }
            }
            var resault = new HashSet<HtmlElement>(elements);
            return resault.ToList();
        }
    }
}