using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Assets.JNMTouchControls.Scripts;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    //Private variables
    private static bool timelineRetrieved = false;
    public static List<GameStage> game;
    private static bool[] holeRow;

    public static Timeline timeline = new Timeline();
    //__PUBLIC VARIABLES__
    public static string superCollectionName = "chronozoom"; //chronozoom nobelprize
    public static string collectionName = "cosmos"; //cosmos nobel
    public static int wormholesPerPlatform = 3;
    public static int platformsPerGames = 10;
    public static bool limitContentToImages = true;
    public static float plateDistance = 12f;
    public Canvas mainMenu;
    public Canvas settingsMenu;
    public Canvas timelineSelectMenu;
    public CanvasGroup loadingImage;

    public object contentItem { get; private set; }


    // Use this for initialization
    void Start()
    {
        loadingImage.alpha = 1;
        Time.timeScale = 0;

        timeline = ChronozoomHandler.RetrieveTimeline(superCollectionName, collectionName);
//        Logger.LogPlayEvent("Got Timelines: "+(Time.timeSinceLevelLoad).ToString("n1"), "Ball Game", "Main", Main.superCollectionName, Main.collectionName, "");
        if (timeline != null && !String.IsNullOrEmpty(timeline.__type))
        {
            timelineRetrieved = true;
            ChronozoomHandler.GenerateLists(timeline, limitContentToImages);
            game = ChronozoomHandler.SetUpGame(wormholesPerPlatform, platformsPerGames+1);

            if (timelineRetrieved)
            {
                if (game != null)
                {
                    setupGame(game);
                    Logger.LogPlayEvent("GameSetup Done", "Ball Game", "Main"+"Start", Main.superCollectionName, Main.collectionName, game.ToString());
                }
                else
                {
                    SceneManager.LoadScene("MainScene");
                }
            }

            long startYear = game[0].stageEvent.time;
            long endYear = game[game.Count - 1].stageEvent.time;

            RenderTimeline(startYear.ToString(), endYear.ToString());
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    private void setupGame(List<GameStage> game)
    {
        //Loops through and creates a level per loop
        for (int i = 0; i < platformsPerGames; i++) {
            //Creates platforms
            GameObject platform = (GameObject)Instantiate(Resources.Load("Plate"));

            //Reset one hole per row rule
            holeRow = new bool[7];

            //Reserve row 5 for title and year on platform
            holeRow[5] = true;
            holeRow[1] = true;
            holeRow[4] = true;

            //Add name to platform (e.g. platform-0 is first platform)
            platform.name = "Platform-" + i;

            //Position plate below the previous 
            Vector3 platformPosition = new Vector3(0f, -(i*plateDistance), 0f);
            platform.transform.position = platformPosition;

            //Create Exhibit content items "walls"
            setupExhibitContentItem(i);

            //Create holes
            //Last Platform
            if(i == platformsPerGames-1)
            {
                //setupLastPlatform(platform.name);
                //Correct wormhole
                setupHole(platform.name, game[i], true, 0, true);

                //Incorrect wormhole(s)
                for (int j = 0; j < wormholesPerPlatform - 1; j++)
                {
                    //Create ItemImages from URL 
                    setupHole(platform.name, game[i], false, j, true);
                }
            }
            //Normal Platform
            else
            {
                //Correct wormhole
                setupHole(platform.name, game[i], true, 0, false);

                //Incorrect wormhole(s)
                for (int j = 0; j < wormholesPerPlatform - 1; j++)
                {
                    //Create ItemImages from URL 
                    setupHole(platform.name, game[i], false, j, false);
                }
            } 
        }
        
    }

    private void setupExhibitContentItem(int plateNumber)
    {
        //Create exhibit year
        GameObject exhibitYear = (GameObject)Instantiate(Resources.Load("ExhibitYear"));

        Vector3 yearPosition = new Vector3(exhibitYear.transform.position.x, -(plateNumber * plateDistance) + 0.1f, exhibitYear.transform.position.z);
        exhibitYear.transform.position = yearPosition;

        //Calculate exhibit year
        long year = game[plateNumber].stageEvent.time;
        if (year < 0)
        {
            exhibitYear.GetComponent<TextMesh>().text = (((year * -1).ToString().Length != 4) ?  (year * (-1)).ToString("n0") + " BC" :  year * (-1) + " BC");
        }
        else
            exhibitYear.GetComponent<TextMesh>().text =  ((year.ToString().Length != 4) ?  year.ToString("n0")  : "" + year );

        //Create exhibit title
        GameObject exhibitTitle = (GameObject)Instantiate(Resources.Load("ExhibitTitle"));

        Vector3 titlePosition = new Vector3(exhibitTitle.transform.position.x, -(plateNumber * plateDistance) + 0.1f, exhibitTitle.transform.position.z);
        exhibitTitle.transform.position = titlePosition;

        string titleTemp = game[plateNumber].stageEvent.title;
        if (titleTemp.Length >= 40)
            titleTemp = titleTemp.Substring(0, 35) + "...";
            
        exhibitTitle.GetComponent<TextMesh>().text = titleTemp;

        //Second exhibit year and title
        GameObject exhibitYear2 = (GameObject)Instantiate(Resources.Load("ExhibitYear"));
        GameObject exhibitTitle2 = (GameObject)Instantiate(Resources.Load("ExhibitTitle"));

        exhibitYear2.GetComponent<TextMesh>().text = exhibitYear.GetComponent<TextMesh>().text;
        exhibitTitle2.GetComponent<TextMesh>().text = exhibitTitle.GetComponent<TextMesh>().text;

        exhibitYear2.transform.position = new Vector3(exhibitYear.transform.position.x, exhibitYear.transform.position.y, exhibitYear.transform.position.z+11.9f);
        exhibitTitle2.transform.position = new Vector3(exhibitTitle.transform.position.x, exhibitTitle.transform.position.y, exhibitTitle.transform.position.z + 11.9f);

        //Create exhibit  items
        int numberOfItems = game[plateNumber].stageEvent.contentItems.Count;

        //Only allow max 6 items
        if (numberOfItems > 6)
            numberOfItems = 6;

        for (var i = 0; i < numberOfItems; i++)
        {
            
            StartCoroutine(createExhibitItemImage(game[plateNumber].stageEvent.contentItems[i],i,plateNumber, numberOfItems));
        }
        
    }

    IEnumerator createExhibitItemImage(ContentItem contentItem, int itemNumber,int plateNumber, int numberOfItems)
    {
        

        Texture2D texture = new Texture2D(1, 1);
        string stageEventid = contentItem.id + "-large";
        string stageEventUri = contentItem.uri;
        string stageEventDescription = contentItem.title;

        //Creates item image object
        GameObject itemImageLarge = (GameObject)Instantiate(Resources.Load("Exhibit_Content_Items_"+(itemNumber+1)));
        itemImageLarge.tag = "ItemImageLarge";
        itemImageLarge.name = stageEventid;

        Vector3 itemPosition = new Vector3(itemImageLarge.transform.position.x, -(plateNumber * plateDistance)+2f, itemImageLarge.transform.position.z);
        itemImageLarge.transform.position = itemPosition;

        //Create description 3d text
        GameObject itemImageLargeDescription = (GameObject)Instantiate(Resources.Load("ExhibitItemDescription"));
        itemImageLargeDescription.tag = "ItemImageDescription";

        Vector3 tempPosition = itemImageLarge.transform.position;
        itemImageLargeDescription.GetComponent<TextMesh>().text = stageEventDescription;
        if(itemNumber == 5 || itemNumber== 6)
            itemImageLargeDescription.transform.position = new Vector3(tempPosition.x, tempPosition.y+3f , tempPosition.z+3f);
        else
            itemImageLargeDescription.transform.position = new Vector3(tempPosition.x, tempPosition.y + 4f, tempPosition.z);

        if(itemNumber == 0 || itemNumber == 2)
            itemImageLargeDescription.transform.eulerAngles = new Vector3(itemImageLarge.transform.eulerAngles.x, itemImageLarge.transform.eulerAngles.y+180, 0);
        else
            itemImageLargeDescription.transform.eulerAngles = new Vector3(itemImageLarge.transform.eulerAngles.x, itemImageLarge.transform.eulerAngles.y, 0);

        //Create small image object on platform
        GameObject itemImageSmall = (GameObject)Instantiate(Resources.Load("Exhibit_Content_Items_Small"));
        itemImageSmall.tag = "ItemImageLarge";
        itemImageSmall.name = stageEventid;
        Vector3 itemSmallPosition = new Vector3(itemNumber * 3.5f-((numberOfItems-1)*1.75f), itemPosition.y-2f, itemImageSmall.transform.position.z);
        itemImageSmall.transform.position = itemSmallPosition;

        // Start a download of the given URL
        WWW www = new WWW(Uri.EscapeUriString(stageEventUri));

        // Wait for download to complete
        yield return www;

        // assign texture
        //www.LoadImageIntoTexture(texture);
        try
        {
            // assign texture
            if (www.error == null)
            {
                www.LoadImageIntoTexture(texture);
            }
            else
            {
                Logger.LogException("CZBall", "Main", "createStageEvent", www.url + " not found");
            }
        }
        catch
        {
            Logger.LogException("CZBall", "PlayerMovemenr", "createStageEvent", "www.LoadImageIntoTexture(texture)");
        }
        itemImageLarge.gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        itemImageSmall.gameObject.GetComponent<Renderer>().material.mainTexture = texture;

    }

    private void setupHole(string platformName, GameStage stage, bool isCorrect, int holeNumber, bool isLast)
    {
        
        //Get random positions to choose hole location
        int row = UnityEngine.Random.Range(0, 7);
        int col = UnityEngine.Random.Range(0, 7);

        while (holeRow[row])
        {
            row = UnityEngine.Random.Range(0, 7);
        }

        GameObject holePosition = GameObject.Find(platformName+"HoleCover-"+row+"-"+col);

        //while (holePosition == null)
        //{
        //    row = UnityEngine.Random.Range(0, 8);
        //    col = UnityEngine.Random.Range(0, 8);
        //    holePosition = GameObject.Find(platformName + "HoleCover-" + row + "-" + col);
        //}

        holeRow[row] = true;

        //Create image which will be shown above the hole
        StartCoroutine(createItemImage(isCorrect, stage, holeNumber, holePosition, isLast));
    }

    IEnumerator createItemImage(bool isCorrect, GameStage stage, int holeNumber, GameObject holePosition, bool isLast)
    {
        Texture2D texture = new Texture2D(1, 1);
        Vector3 holeCoordinate = holePosition.transform.position;

        //Get uri of image
        string uri, title, id, yearFormatted;
        long year;
        if (isCorrect)
        {
            uri = stage.correctWormhole.uri;
            title = stage.correctWormhole.title;
            id = stage.correctWormhole.id;
            year = stage.correctWormhole.year;
        }
        else
        {
            uri = stage.incorrectWormholes[holeNumber].uri;
            title = stage.incorrectWormholes[holeNumber].title;
            id = stage.incorrectWormholes[holeNumber].id;
            year = stage.incorrectWormholes[holeNumber].year;
        }

        //Format year
        if (year < 0)
            yearFormatted = ((year * -1).ToString().Length != 4) ? (year * (-1)).ToString("n0") + " BC" : (year * -1).ToString() + " BC";
        else
            yearFormatted = (year.ToString().Length != 4) ? year.ToString("n0") : year.ToString();


        // Start a download of the given URL
        WWW www;
        www = new WWW(Uri.EscapeUriString(uri));

        // Wait for download to complete
        yield return www;

        try
        {
        // assign texture
        if (www.error == null)
            {
            www.LoadImageIntoTexture(texture);
            }
        else
            {
                Logger.LogException("CZBall", "Main", "createItemImage", www.url + " not found");
            }

        }
        catch
        {
            Logger.LogException("CZBall", "Main", "createItemImage", "www.LoadImageIntoTexture(texture)");
        }



        //Creates item image object
        GameObject itemImage = (GameObject)Instantiate(Resources.Load("ItemImage"));
        itemImage.name = id;

        //Create sensor
        GameObject itemImageSensor = (GameObject)Instantiate(Resources.Load("ItemImageSensor"));
        itemImageSensor.name = yearFormatted;

        //Create hole on platform
        Destroy(holePosition);

        //Add correct/incorrect tags
        if (isCorrect)
        {
            if (isLast)
            {
                itemImage.tag = "Correct";
                itemImageSensor.tag = "Restart-Hole";
            }
            else {
                itemImage.tag = "Correct";
                itemImageSensor.tag = "Correct-Hole";
            }
        }
        else
        {
            itemImageSensor.tag = "Incorrect-Hole";
            itemImage.tag = "Incorrect";
        }
            

        //Place image on top of the current wormhole
        itemImage.transform.position = new Vector3(holeCoordinate.x, holeCoordinate.y+3, holeCoordinate.z+2);
        itemImage.GetComponent<Renderer>().material.mainTexture = texture;

        //Place description on item image
        GameObject itemImageDescription = (GameObject)Instantiate(Resources.Load("ItemDescription"));

        Vector3 tempPosition = itemImage.transform.position;
        itemImageDescription.GetComponent<TextMesh>().text = title;
        itemImageDescription.GetComponent<TextMesh>().transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        itemImageDescription.transform.position = new Vector3(tempPosition.x, tempPosition.y- 1.5f, tempPosition.z-1.2f);


        //Place image on sensor (hole)
        itemImageSensor.transform.position = new Vector3(holeCoordinate.x+0.1f, holeCoordinate.y-0.5f, holeCoordinate.z);

        try
        {
            // assign texture
            // Was a crash from here but can't reproduce it. Was the audio on I think
            itemImageSensor.GetComponent<Renderer>().material.mainTexture = texture;            
        }
        catch
        {
            Logger.LogException("CZBall", "Main", "createItemImage", "itemImageSensor.GetComponent<Renderer>().material.mainTexture");
        }

    }

    //Last platform has one hole that lets user restart game
    private void setupLastPlatform(string platformName)
    {
        GameObject holePosition = GameObject.Find(platformName + "HoleCover-" + "1" + "-" + "5");
        Vector3 holeCoordinate = holePosition.transform.position;
        Texture2D texture = new Texture2D(1, 1);
        texture = (Texture2D)Resources.Load("NewGameImage");

        //GameObject itemImageDescription = (GameObject)Instantiate(Resources.Load("ItemDescription"));

        //Creates item image object
        GameObject itemImage = (GameObject)Instantiate(Resources.Load("ItemImage"));
        itemImage.name = "RestartGameImage";

        //Create sensor
        GameObject itemImageSensor = (GameObject)Instantiate(Resources.Load("ItemImageSensor"));
        itemImageSensor.name = "RestartGameImage-sensor";

        //Create hole on platform
        Destroy(holePosition);


        //Place image on top of the current wormhole
        itemImage.transform.position = new Vector3(holeCoordinate.x, holeCoordinate.y + 3, holeCoordinate.z + 2);
        itemImage.GetComponent<Renderer>().material.mainTexture = texture;

        //Place description on item image
        GameObject itemImageDescription = (GameObject)Instantiate(Resources.Load("ItemDescription"));

        Vector3 tempPosition = itemImage.transform.position;
        itemImageDescription.GetComponent<TextMesh>().text = "NEW GAME";
        itemImageDescription.GetComponent<TextMesh>().transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        itemImageDescription.transform.position = new Vector3(tempPosition.x, tempPosition.y - 1.35f, tempPosition.z - 1.2f);


        //Place image on sensor (hole)
        itemImageSensor.transform.position = new Vector3(holeCoordinate.x, holeCoordinate.y - 0.2f, holeCoordinate.z);
        itemImageSensor.tag = "Restart-Hole";
    }

    // Update is called once per frame
    void Update () {


    }

    public Exhibit getStageEventContent(int level)
    {
        Logger.LogException("CZBALL", "Main", "getStageEventContent", "A1");
        UpdateSlider(Convert.ToString(game[level].stageEvent.time));
        Logger.LogException("CZBALL", "Main", "getStageEventContent", "A2");
        return game[level].stageEvent;

    }


    public ContentItem getContentItemById(int level,string Id, string tag)
    {
        switch (tag)
        {
            case "Correct":
                return game[level].correctWormhole;
            case "Incorrect":
                for (int i = 0; i < game[level].incorrectWormholes.Count; i++)
                {
                    if (game[level].incorrectWormholes[i].id == Id)
                    {
                        return game[level].incorrectWormholes[i];
                    }
                }
                return null;
            case "ItemImageLarge":
                for (int i = 0; i < game[level].stageEvent.contentItems.Count; i++)
                {
                    if (game[level].stageEvent.contentItems[i].id+"-large" == Id)
                    {
                        return game[level].stageEvent.contentItems[i];
                    }
                }
                return null;
            default:
                return null;
        }
    }

    private void RenderTimeline(string startDate, string endDate)
    {
        GameObject temp = GameObject.Find("TimelineSlider");
        Slider timelineSlider = temp.GetComponent<Slider>();
        timelineSlider.minValue = Convert.ToInt64(startDate);
        timelineSlider.maxValue = Convert.ToInt64(endDate);
        SetSliderValue(startDate);

        SetStartTimeLabel(startDate);
        SetSliderLabel(startDate);
        SetEndTimeLabel(endDate);
    }

    private void SetStartTimeLabel(string startDate)
    {
        GameObject temp = GameObject.Find("StartTime");
        Text startTime = temp.GetComponent<Text>();
        long startYear = Convert.ToInt64(startDate);
        if (startYear < 0)
            startTime.text = ((startYear * -1).ToString().Length != 4) ? (startYear * (-1)).ToString("n0") + " BC" : (startYear * -1).ToString() + " BC";
        else
            startTime.text = (startYear.ToString().Length != 4) ? startYear.ToString("n0") : startYear.ToString();

    }

    private void SetEndTimeLabel(string endDate)
    {
        GameObject temp = GameObject.Find("EndTime");
        Text endTime = temp.GetComponent<Text>();

        long endYear = Convert.ToInt64(endDate);
        if (endYear < 0)
            endTime.text = ((endYear * -1).ToString().Length != 4) ? (endYear * (-1)).ToString("n0") + " BC" : (endYear * -1).ToString() + " BC";
        else
            endTime.text = (endYear.ToString().Length != 4) ? endYear.ToString("n0") : endYear.ToString();
    }

    private void SetSliderLabel(string time)
    {
        GameObject temp = GameObject.Find("CurrentTime");
        Text currentTime = temp.GetComponent<Text>();
        long year = Convert.ToInt64(time);
        if (year < 0)
            currentTime.text = ((year * -1).ToString().Length != 4) ? (year * (-1)).ToString("n0") + " BC" : (year * -1).ToString() + " BC";
        else
            currentTime.text = (year.ToString().Length != 4) ? year.ToString("n0") : year.ToString();
    }

    private void SetSliderValue(string value)
    {
        GameObject temp = GameObject.Find("TimelineSlider");
        Slider timelineSlider = temp.GetComponent<Slider>();
        timelineSlider.value = Convert.ToInt64(value);
    }

    private void UpdateSlider(string value)
    {
        SetSliderValue(value);
        SetSliderLabel(value);
    }

    public void playGame()
    {
        loadingImage.alpha = 0;
        Time.timeScale = 1;
    }
}
