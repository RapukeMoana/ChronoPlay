using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

class Connection
{
    // EXAMPLE USE:

    //string HtmlText = Connection.CheckForResponse("http://google.com");
    //if (HtmlText == "")
    //{
    //    //No internet connection
    //}
    //else if (!HtmlText.Contains("schema.org/WebPage"))
    //{
    //    //The connection has been redirected - i.e. to a login page
    //}
    //else
    //{
    //    //connection is present
    //}

    public static string CheckForResponse(string targetURL)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(targetURL);

        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return string.Empty;
        }

        return html;
    }
}
