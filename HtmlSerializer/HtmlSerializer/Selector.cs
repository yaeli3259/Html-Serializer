using Html_Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class Selector
    {
        public string TagName{ get; set; }
        public string Id{ get; set; }
        public List<string> Classes{ get; set; }
        public Selector Parent{ get; set; }
        public Selector Child{ get; set; }

        public Selector(string name=null, string id=null, List<string> classes=null, Selector parent=null, Selector child=null)
        {
            TagName = name;
            Id = id;
            Classes = classes ??new List<string>() ;
            Parent = parent;
            Child = child;
        }

        public static bool IsElementTag(string tagName)
        {
            foreach (var tag in HtmlHelper.Instance.AllHtmlTags)
            {
                if (tag == tagName) return true;
            }
            return false;
        }
        public void AddChild(Selector child)
        {
            child.Parent = this;
            this.Child = child;
        }
        public static string[] DivideString(string input)
        {
            List<string> result = new List<string>();
            int startIndex = 0;
            string tmp="";
            for (int i = 0; i < input.Length; i++)
            {
                if ( input[i] == '.' || input[i] == '#'||i==0)
                {
                    if (tmp != "")
                    {
                        result.Add(tmp);
                    }
                    tmp = "";         
                }
                tmp += input[i];
            }
            if (tmp != "")
            {
                tmp += '\n';
                result.Add(tmp);
            }
            return result.ToArray();
        }
        public static Selector RootSelector(string query)
        {
            var allWords = Regex.Split(query, @"\s+");
            Selector root = new Selector();
            Selector current = root;
            Selector child;
            string[] parts;
            foreach (var word in allWords)
            {
                char[] separators = { '.', '#' };
                 parts = DivideString(word);
                foreach (var part in parts)
                {
                    if (part.StartsWith('.'))
                        current.Classes.Add(part.Trim('.'));
                    else
                        if (part.StartsWith('#'))
                        current.Id = part.Trim('#');
                    else if (IsElementTag(part))
                        current.TagName = part;
                  
                }
                child = new Selector();
                current.AddChild(child);
                current = child;
            }
            current.Parent.Child = null;
            return root;
        }
    }


}
