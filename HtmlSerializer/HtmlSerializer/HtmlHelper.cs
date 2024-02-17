using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Html_Serializer
{
    internal class HtmlHelper
    {
        private readonly static HtmlHelper _instance= new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] AllHtmlTags { get; set; }
        public string[] SelfClosingTags { get; set; }

        private HtmlHelper()
        {
            AllHtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("AllTags.json"));
            SelfClosingTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("SelfClosingTags.json"));
        }
    }
}
