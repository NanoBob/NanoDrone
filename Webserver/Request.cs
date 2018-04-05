using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Webserver
{
    public class Request
    {
        private string source;
        private string[] lines;
        public List<string> path;
        public Dictionary<string, string> query;
        public string body;
        public Method method;
        

        public Request(string requestString)
        {
            this.source = requestString;
            this.lines = this.source.Split('\n');
            this.query = new Dictionary<string, string>();
            Parse();
        }

        public void Parse()
        {
            ParseMethod();
            ParsePath();
            ParseQuery();
        }

        private void ParseMethod()
        {
            string methodString = this.lines[0].Split(' ')[0];
            switch (methodString)
            {
                case "GET":
                    this.method = Method.GET;
                    break;
                case "POST":
                    this.method = Method.POST;
                    break;
                case "PUT":
                    this.method = Method.PUT;
                    break;
                case "DELETE":
                    this.method = Method.DELETE;
                    break;
            }
        }

        private void ParsePath()
        {
            try
            {
                this.path = new List<String>(this.lines[0].Split(' ')[1].Split('?')[0].Split('/'));
                this.path.Remove("");
            } catch(Exception)
            {
                this.path = new List<string>();
            }
        }

        private void ParseQuery()
        {
            var requestLines = this.lines[0].ToString().Split(' ');

            var url = requestLines.Length > 1 ? requestLines[1] : "";

            string[] splits = url.Split('?');
            string queryString = splits.Length > 1 ? splits[1] : "";

            string[] queryVariables = queryString.Split('&');
            foreach(string queryVariable in queryVariables)
            {
                string[] variableParts = queryVariable.Split('=');
                string name = variableParts[0];
                string value = variableParts.Length > 1 ? variableParts[1] : "";
                this.query.Add(name, value);
                Debug.WriteLine("Query string variable: {0}={1}", name, value);
            }
        }
    }
}
