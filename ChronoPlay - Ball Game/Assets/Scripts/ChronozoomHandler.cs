// Old Code incase the changes break the game again 13/12/2015
#region Working Code
////Chronoplay UOA
//#region VERSION DETAILS
////VERSION 0.0.1
////--Added Logging
////--Bug fixes when displaying videos and gifs
////--Added GameID and GameStageID's for use in logging
//#endregion

//using Pathfinding.Serialization.JsonFx;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using UnityEngine;

//public class Timeline
//{
//    public string __type { get; set; }
//    public bool FromIsCirca { get; set; }
//    public string Height { get; set; }
//    public string Regime { get; set; }
//    public bool ToIsCirca { get; set; }
//    public int? aspectRatio { get; set; }
//    public string backgroundUrl { get; set; }
//    public string end { get; set; }
//    public Exhibit[] exhibits { get; set; }
//    public string id { get; set; }
//    public string offsetY { get; set; }
//    public string start { get; set; }
//    public Timeline[] timelines { get; set; }
//    public string title { get; set; }
//}

//public class Exhibit
//{
//    public string __type { get; set; }
//    public bool IsCirca { get; set; }
//    public string UpdatedBy { get; set; }
//    public string UpdatedTime { get; set; }
//    public List<ContentItem> contentItems { get; set; }
//    public string id { get; set; }
//    public string offsetY { get; set; }
//    public Int64 time { get; set; }
//    public string title { get; set; }
//    public string ParentTimeLineId { get; set; }
//}

//public class ContentItem
//{
//    public string __type { get; set; }
//    public int year { get; set; }
//    public string attribution { get; set; }
//    public string description { get; set; }
//    public string id { get; set; }
//    public string mediaSource { get; set; }
//    public string mediaType { get; set; }
//    public int order { get; set; }
//    public string title { get; set; }
//    public string uri { get; set; }
//    public string ParentExhibitId { get; set; }
//    public Int64 ParentExhibitTime { get; set; }
//}

//public class GameStage
//{
//    public string GameID { get; set; }
//    public string GameStageID { get; set; }
//    public Exhibit stageEvent { get; set; }
//    public ContentItem correctWormhole { get; set; }
//    public List<ContentItem> incorrectWormholes { get; set; }
//}

//public class ChronozoomHandler
//{
//    //CONST GLOBALS
//    private const string _ProjectName = "ChronoPlay - Ball Game";
//    private const string _ScriptFileName = "ChronozoomHandler.cs";

//    //__PRIVATE VARIABLES__

//    private static List<ContentItem> contentItemList = new List<ContentItem>();
//    private static List<Exhibit> exhibitList = new List<Exhibit>();
//    private static bool onlyPictures = false;

//    //__PUBLIC FUNCTIONS__

//    //Retrieves and serializes the Chronozoom data into an object structure using the included classes
//    public static Timeline RetrieveTimeline(string superCollectionName, string collectionName)
//    {
//        string result;
//        Timeline timeline = new Timeline();
//        string requestTemplate = "http://www.chronozoom.com/api/gettimelines?supercollection={0}&colection={1}";
//        string requestUrl = String.Format(requestTemplate, superCollectionName, collectionName);

//        try
//        {
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
//            request.Method = "GET";
//            request.ContentType = "application/x-www-form-urlencoded";

//            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
//            {
//                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
//                {
//                    result = reader.ReadToEnd();
//                }
//            }

//            timeline = JsonReader.Deserialize<Timeline>(result);

//            return timeline;
//        }
//        catch (Exception)
//        {
//            Logger.LogException(_ProjectName, _ScriptFileName, "RetrieveTimeline", "Error retrieving data from chronozoom");
//            return timeline;
//        }
//    }

//    //Sets up the contentItem and Exhibit Lists based on the timeline retrieved from Chronozoom
//    public static void GenerateLists(Timeline timeline, bool limitToImages)
//    {
//        try
//        {
//            onlyPictures = limitToImages;
//            GetSubTimelinesInTimeline(timeline);
//            SortContentItemList();
//            SortExhibitList();
//        }
//        catch(Exception)
//        {
//            Logger.LogException(_ProjectName, _ScriptFileName, "GenerateLists", "Error generating Lists");
//        }
//    }

//    //Sets up the game which is a list of game 'stages' described by the gameStage class 
//    public static List<GameStage> SetUpGame(int numWormholes, int numPlatforms)
//    {
//        try
//        {
//            return GenerateGame(numWormholes, numPlatforms);
//        }
//        catch(Exception)
//        {
//            Logger.LogException(_ProjectName, _ScriptFileName, "SetUpGame", "Error setting up game");
//            return null;
//        }
//    }

//    //__PRIVATE FUNCTIONS__

//    //Sort function for content Item
//    private static void SortContentItemList()
//    {
//        contentItemList.Sort((x, y) => x.ParentExhibitTime.CompareTo(y.ParentExhibitTime));
//    }

//    //Sort function for exhibits
//    private static void SortExhibitList()
//    {
//        exhibitList.Sort((x, y) => x.time.CompareTo(y.time));
//    }

//    //Function to drill down recursively on timelines and generate the exhibit item and content item lists
//    private static void GetSubTimelinesInTimeline(Timeline timeline)
//    {
//        //if there are sub timelines in the given timeline drill down and call getExhibits at each level
//        if (timeline.timelines != null)
//        {
//            foreach (Timeline subTimeline in timeline.timelines)
//            {
//                //recursive function call to drill down
//                GetSubTimelinesInTimeline(subTimeline);
//                GetExhibitsInTimeline(subTimeline);
//            }
//        }
//        else //if not just get Exhibits
//        {
//            GetExhibitsInTimeline(timeline);
//        }
//    }

//    //Function to drill down into exhibits to get content items
//    private static void GetExhibitsInTimeline(Timeline timeline)
//    {
//        foreach (Exhibit exhibit in timeline.exhibits)
//        {
//            //In the chronozoom suppercollection there are duplicate exhibits so this just checks for any duplicates before adding - won't not be needed for custom data sets
//            bool alreadyExists = exhibitList.Any(item => item.id == exhibit.id);
//            if (!alreadyExists)
//            {
//                exhibitList.Add(exhibit);
//            }

//            GetContentItemsInExhibit(exhibit);
//        }
//    }

//    //Function to extract content items from exhibits
//    private static void GetContentItemsInExhibit(Exhibit exhibit)
//    {
//        foreach (ContentItem contentItem in exhibit.contentItems)
//        {
//            //Once again due to duplicate exhibits there are duplicate content items so just checking for duplicates
//            bool alreadyExists = contentItemList.Any(item => item.id == contentItem.id);
//            if (!alreadyExists)
//            {
//                contentItem.ParentExhibitTime = exhibit.time;

//                //Types in Chronozoom include: picture, image, photosynth and video however there is no naming conventions in place when it comes to defining the media source
//                //Based on the setting 'onlyPictures' it either returns all content items or filters to only images
//                bool isValid = ValidateMediaSource(contentItem.mediaSource);
//                if (onlyPictures && (contentItem.mediaType.ToUpper() == "PICTURE" || contentItem.mediaType.ToUpper() == "IMAGE") && isValid)
//                {
//                    contentItemList.Add(contentItem);
//                }
//                else if (!onlyPictures)
//                {
//                    contentItemList.Add(contentItem);
//                }
//            }
//        }
//    }

//    private static ContentItem GenerateCorrectWormhole(List<ContentItem> eventContentItems)
//    {
//        ContentItem correctWormhole = new ContentItem();
//        List<ContentItem> potentialWormholes = new List<ContentItem>();
//        System.Random rand = new System.Random();

//        foreach (ContentItem ci in eventContentItems)
//        {
//            if (onlyPictures && (ci.mediaType.ToUpper() == "PICTURE" || ci.mediaType.ToUpper() == "IMAGE") && ValidateMediaSource(ci.mediaSource))
//            {
//                potentialWormholes.Add(ci);
//            }
//            else if (!onlyPictures)
//            {
//                potentialWormholes.Add(ci);
//            }
//        }
//        int countPotentialEventItems = eventContentItems.Count;
//        correctWormhole = eventContentItems.ElementAt(rand.Next(0, countPotentialEventItems));

//        return correctWormhole;
//    }

//    private static List<ContentItem> GenerateIncorrectWormholes(Int64 eventDate, bool getHistoric, int numItems)
//    {
//        List<ContentItem> incorrectWormholes = new List<ContentItem>();

//        if (getHistoric)
//        {
//            int upperLimitIndex = 0;
//            foreach (ContentItem contentItem in contentItemList)
//            {
//                if (contentItem.ParentExhibitTime > eventDate)
//                {
//                    break;
//                }
//                upperLimitIndex++;
//            }

//            List<int> indexList = GenerateIndexList(numItems, 0, upperLimitIndex);
//            foreach (int index in indexList)
//            {
//                incorrectWormholes.Add(contentItemList.ElementAt(index));
//            }
//        }

//        return incorrectWormholes;
//    }

//    private static List<GameStage> GenerateGame(int numWormholes, int numEvents)
//    {
//        List<GameStage> game = new List<GameStage>();
//        List<int> indexList = GenerateIndexList(numEvents, 1, exhibitList.Count);
//        Guid GameGuid = Guid.NewGuid();

//        for (int i = 0; i < indexList.Count; i++)
//        {
//            GameStage gameStage = new GameStage();
//            System.Random rand = new System.Random();
//            Guid GameStageGuid = Guid.NewGuid();

//            gameStage.stageEvent = exhibitList.ElementAt(indexList.ElementAt(i));
//            if (i + 1 < indexList.Count)
//            {
//                Exhibit nextEvent = exhibitList.ElementAt(indexList.ElementAt(i + 1));
//                if (nextEvent != null)
//                {
//                    gameStage.correctWormhole = GenerateCorrectWormhole(nextEvent.contentItems);
//                }
//                else
//                {
//                    gameStage.correctWormhole = null;
//                }
//            }

//            gameStage.incorrectWormholes = GenerateIncorrectWormholes(gameStage.stageEvent.time, true, numWormholes - 1);

//            gameStage.GameID = GameGuid.ToString();
//            gameStage.GameStageID = GameStageGuid.ToString();
//            game.Add(gameStage);
//        }

//        string gameJSON = JsonWriter.Serialize(game);
//        return game;
//    }

//    private static List<int> GenerateIndexList(int length, int minValue, int maxValue)
//    {
//        List<int> indexList = new List<int>();
//        System.Random rand = new System.Random();
//        int i = 0;
//        while (i < length)
//        {
//            //min value of 1 insures that there is always historic content items
//            int index = rand.Next(minValue, maxValue);
//            //checks that all index items are unique
//            bool alreadyExists = indexList.Any(item => item == index);

//            if (!alreadyExists)
//            {
//                indexList.Add(index);
//                i++;
//            }
//        }

//        indexList.Sort((x, y) => x.CompareTo(y));

//        return indexList;
//    }

//    private static bool ValidateMediaSource(string str)
//    {
//        if (String.IsNullOrEmpty(str))
//        {
//            return false;
//        }
//        else
//        {
//            if (4 >= str.Length)
//            {
//                return false;
//            }
//            else if (str.Substring(str.Length - 4).ToUpper() == ".GIF")
//            {
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }
//    }
//}
#endregion

//Chronoplay UOA
#region VERSION DETAILS
//VERSION 0.0.1
//--Added Logging
//--Bug fixes when displaying videos and gifs
//--Added GameID and GameStageID's for use in logging
#endregion

using Pathfinding.Serialization.JsonFx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class Timeline
{
    public string __type { get; set; }
    public bool FromIsCirca { get; set; }
    public string Height { get; set; }
    public string Regime { get; set; }
    public bool ToIsCirca { get; set; }
    public int? aspectRatio { get; set; }
    public string backgroundUrl { get; set; }
    public string end { get; set; }
    public Exhibit[] exhibits { get; set; }
    public string id { get; set; }
    public string offsetY { get; set; }
    public string start { get; set; }
    public Timeline[] timelines { get; set; }
    public string title { get; set; }
}

public class Exhibit
{
    public string __type { get; set; }
    public bool IsCirca { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedTime { get; set; }
    public List<ContentItem> contentItems { get; set; }
    public string id { get; set; }
    public string offsetY { get; set; }
    public Int64 time { get; set; }
    public string title { get; set; }
    public string ParentTimeLineId { get; set; }
}

public class ContentItem
{
    public string __type { get; set; }
    public int year { get; set; }
    public string attribution { get; set; }
    public string description { get; set; }
    public string id { get; set; }
    public string mediaSource { get; set; }
    public string mediaType { get; set; }
    public int order { get; set; }
    public string title { get; set; }
    public string uri { get; set; }
    public string ParentExhibitId { get; set; }
    public Int64 ParentExhibitTime { get; set; }
}

public class GameStage
{
    public string GameID { get; set; }
    public string GameStageID { get; set; }
    public Exhibit stageEvent { get; set; }
    public ContentItem correctWormhole { get; set; }
    public List<ContentItem> incorrectWormholes { get; set; }
}

public class ChronozoomHandler
{
    //CONST GLOBALS
    private const string _ProjectName = "ChronoPlay - Ball Game";
    private const string _ScriptFileName = "ChronozoomHandler.cs";

    //__PRIVATE VARIABLES__

    private static List<ContentItem> contentItemList = new List<ContentItem>();
    private static List<Exhibit> exhibitList = new List<Exhibit>();
    private static bool onlyPictures = false;

    //__PUBLIC FUNCTIONS__

    //Retrieves and serializes the Chronozoom data into an object structure using the included classes
    public static Timeline RetrieveTimeline(string superCollectionName, string collectionName)
    {
        string result;
        Timeline timeline = new Timeline();
        string requestTemplate = "http://www.chronozoom.com/api/gettimelines?supercollection={0}&colection={1}";
        string requestUrl = String.Format(requestTemplate, superCollectionName, collectionName);

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

            timeline = JsonReader.Deserialize<Timeline>(result);

            return timeline;
        }
        catch (Exception)
        {
            Logger.LogException(_ProjectName, _ScriptFileName, "RetrieveTimeline", "Error retrieving data from chronozoom");
            return timeline;
        }
    }

    //Sets up the contentItem and Exhibit Lists based on the timeline retrieved from Chronozoom
    public static void GenerateLists(Timeline timeline, bool limitToImages)
    {
        contentItemList.Clear();
        exhibitList.Clear();

        try
        {
            onlyPictures = limitToImages;
            GetSubTimelinesInTimeline(timeline);
            SortContentItemList();
            SortExhibitList();
        }
        catch (Exception)
        {
            Logger.LogException(_ProjectName, _ScriptFileName, "GenerateLists", "Error generating Lists");
        }
    }

    //Sets up the game which is a list of game 'stages' described by the gameStage class 
    public static List<GameStage> SetUpGame(int numWormholes, int numPlatforms)
    {
        try
        {
            return GenerateGame(numWormholes, numPlatforms);
        }
        catch (Exception)
        {
            Logger.LogException(_ProjectName, _ScriptFileName, "SetUpGame", "Error setting up game");
            return null;
        }
    }

    //__PRIVATE FUNCTIONS__

    private static Exhibit CleanStageEvent(Exhibit stageEvent)
    {
        var numContentItems = stageEvent.contentItems.Count;
        for (int i = 0; i < numContentItems; i++)
        {
            bool sourceIsValid = ValidateMediaSource(stageEvent.contentItems[i].uri);
            if (!sourceIsValid || !(stageEvent.contentItems[i].mediaType.ToUpper() == "PICTURE" || stageEvent.contentItems[i].mediaType.ToUpper() == "IMAGE"))
            {
                if (numContentItems > 1)
                {
                    stageEvent.contentItems.RemoveAt(i);
                    numContentItems = stageEvent.contentItems.Count;
                }
            }
        }
        return stageEvent;
    }
    //Sort function for content Item
    private static void SortContentItemList()
    {
        contentItemList.Sort((x, y) => x.ParentExhibitTime.CompareTo(y.ParentExhibitTime));
    }

    //Sort function for exhibits
    private static void SortExhibitList()
    {
        exhibitList.Sort((x, y) => x.time.CompareTo(y.time));
    }

    //Function to drill down recursively on timelines and generate the exhibit item and content item lists
    private static void GetSubTimelinesInTimeline(Timeline timeline)
    {
        //if there are sub timelines in the given timeline drill down and call getExhibits at each level
        if (timeline.timelines != null)
        {
            foreach (Timeline subTimeline in timeline.timelines)
            {
                //recursive function call to drill down
                GetSubTimelinesInTimeline(subTimeline);
                GetExhibitsInTimeline(subTimeline);
            }
        }
        else //if not just get Exhibits
        {
            GetExhibitsInTimeline(timeline);
        }
    }

    //Function to drill down into exhibits to get content items
    private static void GetExhibitsInTimeline(Timeline timeline)
    {
        foreach (Exhibit exhibit in timeline.exhibits)
        {
            //In the chronozoom suppercollection there are duplicate exhibits so this just checks for any duplicates before adding - won't not be needed for custom data sets
            bool alreadyExists = exhibitList.Any(item => item.id == exhibit.id);
            if (!alreadyExists)
            {
                exhibitList.Add(exhibit);
            }

            GetContentItemsInExhibit(exhibit);
        }
    }

    //Function to extract content items from exhibits
    private static void GetContentItemsInExhibit(Exhibit exhibit)
    {
        foreach (ContentItem contentItem in exhibit.contentItems)
        {
            //Once again due to duplicate exhibits there are duplicate content items so just checking for duplicates
            bool alreadyExists = contentItemList.Any(item => item.id == contentItem.id);
            if (!alreadyExists)
            {
                contentItem.ParentExhibitTime = exhibit.time;

                //Types in Chronozoom include: picture, image, photosynth and video however there is no naming conventions in place when it comes to defining the media source
                //Based on the setting 'onlyPictures' it either returns all content items or filters to only images
                bool isValid = ValidateMediaSource(contentItem.uri);
                if (onlyPictures && (contentItem.mediaType.ToUpper() == "PICTURE" || contentItem.mediaType.ToUpper() == "IMAGE") && isValid)
                {
                    contentItemList.Add(contentItem);
                }
                else if (!onlyPictures)
                {
                    contentItemList.Add(contentItem);
                }
            }
        }
    }

    private static ContentItem GenerateCorrectWormhole(List<ContentItem> eventContentItems)
    {
        ContentItem correctWormhole = new ContentItem();
        List<ContentItem> potentialWormholes = new List<ContentItem>();
        System.Random rand = new System.Random();
        int CICount = eventContentItems.Count;

        // Filter out undesirable content items  into a seperate list
        foreach (ContentItem ci in eventContentItems)
        {
            if (onlyPictures && (ci.mediaType.ToUpper() == "PICTURE" || ci.mediaType.ToUpper() == "IMAGE") && ValidateMediaSource(ci.uri))
            {
                potentialWormholes.Add(ci);
            }
            else if (!onlyPictures)
            {
                potentialWormholes.Add(ci);
            }
        }

        int countPotentialEventItems = potentialWormholes.Count;

        if (countPotentialEventItems > 0) // Try filter out bad content items
        {
            correctWormhole = potentialWormholes.ElementAt(rand.Next(0, countPotentialEventItems));
        }
        else if (CICount > 0) // Other wise just pick a content item disregarding whether or not the image works
        {
            correctWormhole = eventContentItems.ElementAt(rand.Next(0, CICount));
        }

        return correctWormhole;
    }

    private static List<ContentItem> GenerateIncorrectWormholes(Int64 eventDate, bool getHistoric, int numItems)
    {
        List<ContentItem> incorrectWormholes = new List<ContentItem>();

        if (getHistoric)
        {
            int upperLimitIndex = 0;

            // Find the last index that is below the stage events time
            foreach (ContentItem contentItem in contentItemList)
            {
                if (contentItem.ParentExhibitTime > eventDate)
                {
                    break;
                }

                upperLimitIndex++;
            }

            // Generate random incorrect wormholes from the list up until the given index
            List<int> indexList = GenerateIndexList(numItems, 0, upperLimitIndex - 1);

            foreach (int index in indexList)
            {
                // ContentItem List already filtered
                incorrectWormholes.Add(contentItemList.ElementAt(index));
            }
        }

        return incorrectWormholes;
    }

    private static List<GameStage> GenerateGame(int numWormholes, int numEvents)
    {
        List<GameStage> game = new List<GameStage>();
        List<int> indexList = GenerateIndexList(numEvents, 1, exhibitList.Count);
        Guid GameGuid = Guid.NewGuid();

        for (int i = 0; i < indexList.Count; i++)
        {
            GameStage gameStage = new GameStage();
            System.Random rand = new System.Random();
            Guid GameStageGuid = Guid.NewGuid();
            Exhibit cleanExhibit = CleanStageEvent(exhibitList.ElementAt(indexList.ElementAt(i)));

            gameStage.stageEvent = cleanExhibit;
            //gameStage.stageEvent = exhibitList.ElementAt(indexList.ElementAt(i));

            if (i + 1 < indexList.Count)
            {
                Exhibit nextEvent = exhibitList.ElementAt(indexList.ElementAt(i + 1));
                if (nextEvent != null)
                {
                    gameStage.correctWormhole = GenerateCorrectWormhole(nextEvent.contentItems);
                }
                else
                {
                    gameStage.correctWormhole = null;
                }
            }

            gameStage.incorrectWormholes = GenerateIncorrectWormholes(gameStage.stageEvent.time, true, numWormholes - 1);

            gameStage.GameID = GameGuid.ToString();
            gameStage.GameStageID = GameStageGuid.ToString();
            game.Add(gameStage);
        }

        string gameJSON = JsonWriter.Serialize(game);
        return game;
    }

    private static List<int> GenerateIndexList(int length, int minValue, int maxValue)
    {
        List<int> indexList = new List<int>();
        System.Random rand = new System.Random();
        int i = 0;
        while (i < length)
        {
            //min value of 1 insures that there is always historic content items
            int index = rand.Next(minValue, maxValue);
            //checks that all index items are unique
            bool alreadyExists = indexList.Any(item => item == index);

            if (!alreadyExists)
            {
                indexList.Add(index);
                i++;
            }
        }

        indexList.Sort((x, y) => x.CompareTo(y));

        return indexList;
    }

    private static bool ValidateMediaSource(string str)
    {
        if (String.IsNullOrEmpty(str))
        {
            return false;
        }
        else
        {
            if (4 >= str.Length)
            {
                return false;
            }
            else if (str.Substring(str.Length - 4).ToUpper() == ".GIF")
            {
                return false;
            }
            else if (str.ToUpper().IndexOf("PHOTOSYNTH") > -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}




