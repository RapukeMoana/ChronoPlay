using Pathfinding.Serialization.JsonFx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

public class Logger
{
    private const string _ApiKey = "2c917dc4aaa343a0817688db82ef275d";
    private const string _RequestURL = "http://chronoplayapi.azurewebsites.net:80/api/PlayEvents?APIKey=";

    public static List<PlayEvent> GetLog()
    {
        string result;
        List<PlayEvent> logResponse = new List<PlayEvent>();
        string requestUrl = _RequestURL + _ApiKey;

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            logResponse = JsonReader.Deserialize<List<PlayEvent>>(result);

            return logResponse;
        }
        catch (Exception)
        {
            Debug.Log("Error retrieving data from chronozoom.");
            return logResponse;
        }
    }

    public static bool Log(string playEventJson, string playEventType, string gameID, string collectionID = null, string timelineID = null, string contentItemID = null)
    {
        PlayEvent playEvent = new PlayEvent();
        DateTime now = DateTime.Now;
        playEvent._createdAt = now;
        playEvent._updatedAt = now;
        playEvent._deleted = false;
        playEvent.collectionsID = collectionID;
        playEvent.timelinesID = timelineID;
        playEvent.contentItemsID = contentItemID;
        playEvent.gameID = gameID;
        playEvent.playEventJSON = playEventJson;
        playEvent.playEventType = playEventType;

        string url = _RequestURL + _ApiKey;
        string result = string.Empty;
        string JSONRequestData = JsonWriter.Serialize(playEvent);
        byte[] byteArray = Encoding.UTF8.GetBytes(JSONRequestData);
        PlayEvent RequestResponse = new PlayEvent();

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/json";
            request.ContentLength = byteArray.Length;
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JSONRequestData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            RequestResponse = JsonReader.Deserialize<PlayEvent>(result);
            if(RequestResponse.ID > 0)
            {
                return true;
            }
            else
            {
                return false; 
            }
        }
        catch (Exception)
        {
            Debug.Log("Error POSTing data to API.");
            return false;
        }
    }
}

public class PlayEvent
{
    public int ID { get; set; }
    [CLSCompliant(false)]
    public DateTime _createdAt { get; set; }
    [CLSCompliant(false)]
    public DateTime _updatedAt { get; set; }
    [CLSCompliant(false)]
    public bool _deleted { get; set; }
    public string collectionsID { get; set; }
    public string timelinesID { get; set; }
    public string contentItemsID {get; set;}
    public string gameID { get; set; }
    public string playEventJSON { get; set; }
    public string playEventType { get; set; }
}

