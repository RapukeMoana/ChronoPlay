//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using System.Reflection;
//using System.Threading.Tasks;

public class ChronozoomHandler
{
	public static Collection RetrieveTimeline()
	{
		string result;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.chronozoom.com/api/gettimelines?supercollection=chronoplaytesting&collection=Archean");
        request.Method = "GET";
		request.ContentType = "application/x-www-form-urlencoded";
		using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
		using (StreamReader reader = new StreamReader(response.GetResponseStream()))
		{
			result = reader.ReadToEnd();
		}

        Collection collection = JsonReader.Deserialize<Collection>(result);

        //List<Exhibit> exhibitList = new List<Exhibit>();

        //foreach (Exhibit exhibit in obj.exhibits)
        //{
        //    Exhibit exhibitInstance = new Exhibit();
        //    exhibitInstance.title = exhibit.title;
        //    List<ContentItem> contentItemsList = new List<ContentItem>();

        //    foreach (object item in exhibit.contentItems)
        //    {
        //        ContentItem contentItem = new ContentItem();

        //        contentItem.title = item.title;
        //        contentItem.description = item.description;
        //        contentItem.order = item.order;
        //        contentItem.year = item.Year;
        //        contentItem.imageURI = item.uri;

        //        contentItemsList.Add(contentItem);
        //    }

        //    exhibitInstance.contentItems = contentItemsList;
        //    exhibitList.Add(exhibitInstance);
        //}

        return collection;
	}
}

public class Collection
{
    //public string __type { get; set; }
    //public bool FromIsCirca { get; set; }
    //public string Height { get; set; }
    public string Regime { get; set; }
    //public bool ToIsCirca { get; set; }
    //public int? aspectRatio { get; set; }
    //public string backgroundUrl { get; set; }
    //public string end { get; set; }
    public Exhibit[] exhibits { get; set; }
    //public string id { get; set; }
    //public string offsetY { get; set; }
    //public string start { get; set; }
    //public object[] timelines { get; set; }
    public string title { get; set; }
} 

public class Exhibit
{
	public string title { get; set; }
	public List<ContentItem> contentItems { get; set; }
}

public class ContentItem
{
    public string description { get; set; }
    public string title { get; set; }
    public string uri { get; set; }
    public int order { get; set; }
    public int year { get; set; }
}

