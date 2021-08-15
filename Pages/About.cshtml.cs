using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SiteScrape2.Pages
{
    public class AboutModel : PageModel
    {

        public int WordCount { get; set; }
        public int ImageCount { get; set; }
        public int UniqueWordCount { get; set; }
        public List<string> SiteImages { get; set; }
        public List<string> AllWords { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            string _url =  Request.Query["url"];
            
            Message = "Resources from:" + _url;

            HtmlWeb _doc = new HtmlWeb();
            string _html = "";

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                WebClient _client = new WebClient();
                _client.Headers.Add("User-Agent: Only a test");
                _html = _client.DownloadString(_url);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var response = ex.Response;
                    var dataStream = response.GetResponseStream();
                    var reader = new StreamReader(dataStream);
                    var details = reader.ReadToEnd();
                }
            }

            WordCount = GetWordCount(_html, false);
            ImageCount = GetElementCount(_html);
            UniqueWordCount = GetWordCount(_html, true);
            SiteImages = GetImages(_html);
            AllWords = GetAllWords(_html);

            // Console.WriteLine("Image Tag Count   : " + GetElementCount(_html, TagType.IMG));
            // Console.WriteLine("Word Count        : " + GetWordCount(_html));
            // Console.WriteLine("Unique Word Count : " + GetUniqueWordCount(_html));

        }

        int GetElementCount(string _html){
        
            var doc = new HtmlDocument();
            doc.LoadHtml(_html);
            List<string> _images = new List<String>();
            int _length = 0;
            

            try{

                foreach (HtmlNode img in doc.DocumentNode.SelectNodes("//img")){
                    if(img.Attributes.Contains(new string("src"))){
                        System.Console.WriteLine(@"IMG Tag :{0}", img.Attributes[0].Value);
                        _images.Add(img.Attributes[0].Value);
                    };
                }

                _length = _images.Count;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Exception at:" + ex.ToString());  
            }

            return _length;
        }

        List<string> GetImages(string _html){
        
            var doc = new HtmlDocument();
            doc.LoadHtml(_html);
            var _images = new List<String>();
            
            List<string> _forLinq = new List<string>();

            try{
                
                foreach (HtmlNode img in doc.DocumentNode.SelectNodes("//img")){
                    if(img.Attributes.Contains(new string("src"))){
                        System.Console.WriteLine(@"IMG Tag :{0}", img.Attributes[0].Value);
                        _images.Add(img.Attributes[0].Value);
                    };
                }

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Exception at:" + ex.ToString());  
            }

            return _images;
        }

        List<string> GetAllWords(string content)
        {
            
            List<string> _allWordsList = new List<string>();

            string _allWords = "";

            HtmlDocument doc = CleanDocument(content);            

            foreach (HtmlTextNode node in doc.DocumentNode.SelectNodes("//*[not(self::script or self::style)]/text()[normalize-space()]"))
            {
                _allWords += " " + node.InnerText;
            }

            var g = _allWords.Split(" ").GroupBy(i => i).OrderByDescending( x => x.Count());
            foreach(var group in g)
            {
                _allWordsList.Add(group.Key +":"+ group.Count());

                // Console.WriteLine("{0} {1}", group.Key, group.Count());
            }
            return _allWordsList;
            
        }

        static HtmlDocument CleanDocument(string content)
        {
            
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            
            //*[not(self::script or self::style)]/text()
            foreach (HtmlNode style in doc.DocumentNode.Descendants("style").ToArray())
            {
                style.Remove();
            }
            foreach (HtmlNode script in doc.DocumentNode.Descendants("script").ToArray())
            {
                script.Remove();
            }

            return doc;
        }

         static int GetWordCount(string content, bool unique)
         {

            string _allWords = "";
            
            HtmlDocument doc = CleanDocument(content);

            foreach (HtmlTextNode node in doc.DocumentNode.SelectNodes("//*[not(self::script or self::style)]/text()[normalize-space()]"))
            {
                _allWords += " " + node.InnerText;
            }

            var distinct = _allWords.Split(" ")
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct();
            
            var words = _allWords.Split(" ").Where(x => !string.IsNullOrEmpty(x));

            if(unique) return distinct.Count();

            return words.Count();

        }
    }
}
