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
    private const string _PlayEventsRequestURL = "http://chronoplayapi.azurewebsites.net:80/api/PlayEvents?APIKey=";
    private const string _ExceptionRequestURL = "http://chronoplayapi.azurewebsites.net:80/api/ExceptionLogs?APIKey=";
    private const string _FeedbackRequestURL = "http://chronoplayapi.azurewebsites.net:80/api/FeedbackLogs?APIKey=";

    private const string _ProjectName = "ChronoPlay - Ball Game";
    private const string _ScriptFileName = "Logger.cs";

    public static List<PlayEvent> GetPlayEventLog()
    {
        string result;
        List<PlayEvent> logResponse = new List<PlayEvent>();
        string requestUrl = _PlayEventsRequestURL + _ApiKey;

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
            Logger.LogException(_ProjectName, _ScriptFileName, "GetPlayEventLog", "Error connecting to API");
            return logResponse;
        }
    }

    public static List<ExceptionLog> GetExceptionLog()
    {
        string result;
        List<ExceptionLog> logResponse = new List<ExceptionLog>();
        string requestUrl = _ExceptionRequestURL + _ApiKey;

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

            logResponse = JsonReader.Deserialize<List<ExceptionLog>>(result);

            return logResponse;
        }
        catch (Exception)
        {
            Logger.LogException(_ProjectName, _ScriptFileName, "GetExceptionLog", "Error connecting to API");
            return logResponse;
        }
    }

    public static List<Feedback> GetFeedbackLog()
    {
        string result;
        List<Feedback> logResponse = new List<Feedback>();
        string requestUrl = _FeedbackRequestURL + _ApiKey;

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

            logResponse = JsonReader.Deserialize<List<Feedback>>(result);

            return logResponse;
        }
        catch (Exception)
        {
            Logger.LogException(_ProjectName, _ScriptFileName, "GetFeedbackLog", "Error connecting to API");
            return logResponse;
        }
    }

    public static bool LogPlayEvent(string playEventJson, string playEventType, string gameID, string collectionID = null, string timelineID = null, string contentItemID = null)
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

        string url = _PlayEventsRequestURL + _ApiKey;
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
            if (RequestResponse.ID > 0)
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
            Logger.LogException(_ProjectName, _ScriptFileName, "LogPlayEvent", "Error connecting to API");
            return false;
        }
    }

    public static bool LogException(string projectName, string scriptFile, string functionName, string message)
    {
        ExceptionLog exception = new ExceptionLog();
        DateTime now = DateTime.Now;
        exception._loggedAt = now;
        exception.projectname = projectName;
        exception.scriptfile = scriptFile;
        exception.functionname = functionName;
        exception.message = message;

        string url = _ExceptionRequestURL + _ApiKey;
        string result = string.Empty;
        string JSONRequestData = JsonWriter.Serialize(exception);
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
            if (RequestResponse.ID > 0)
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
            Logger.LogException(_ProjectName, _ScriptFileName, "LogException", "Error connecting to API");
            return false;
        }
    }

    public static bool LogFeedback(string feedbackComments, int userRating, string gameID = null, string loggedBy = null)
    {
        Feedback feedback = new Feedback();
        DateTime now = DateTime.Now;
        feedback._loggedAt = now;
        feedback.gameID = gameID;
        feedback.feedbackComments = feedbackComments;
        feedback.userRating = userRating;
        feedback.loggedBy = loggedBy;

        string url = _FeedbackRequestURL + _ApiKey;
        string result = string.Empty;
        string JSONRequestData = JsonWriter.Serialize(feedback);
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
            if (RequestResponse.ID > 0)
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
            Logger.LogException(_ProjectName, _ScriptFileName, "LogFeedback", "Error connecting to API");
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
    public string contentItemsID { get; set; }
    public string gameID { get; set; }
    public string playEventJSON { get; set; }
    public string playEventType { get; set; }
}

public class ExceptionLog
{
    public int ID { get; set; }
    public DateTime _loggedAt { get; set; }
    public string scriptfile { get; set; }
    public string functionname { get; set; }
    public string message { get; set; }
    public string projectname { get; set; }
}

public class Feedback
{
    public int ID { get; set; }
    public DateTime _loggedAt { get; set; }
    public string loggedBy { get; set; }
    public string gameID { get; set; }
    public int userRating { get; set; }
    public string feedbackComments { get; set; }
}

