﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Assets.JNMTouchControls.Scripts;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Main : MonoBehaviour {

    //Private variables
    private static bool timelineRetrieved = false;
    public static List<GameStage> game;
    private static bool[] holeRow;
    
    private int resHeight;

    public static bool restartSameCollection = false;
    public static Timeline timeline = new Timeline();
    //__PUBLIC VARIABLES__
    public static string superCollectionName = "chronozoom"; //chronozoom nobelprize
    public static string collectionName = "cosmos"; //cosmos nobel
    public static int wormholesPerPlatform = 3;
    public static int platformsPerGames = 10;
    public static bool limitContentToImages = true;
    public static float plateDistance = 12f;
    public static GameObject[] progressBarYearGameObjects;
    public Canvas mainMenu;
    public Canvas settingsMenu;
    public Canvas timelineSelectMenu;
    public CanvasGroup loadingImage;
    public Text loadingText;
    public Button playButton;

    public object contentItem { get; private set; }

    public static List<ContentItem> contentItemList;


    // Use this for initialization
    void Start()
    {
        //Bypasses the usual loading screen if restarting same collection
        if (restartSameCollection)
        {
            playGame();
            Time.timeScale = 1;
        }
        else
        {
            loadingImage.alpha = 1;
            GameObject player = GameObject.Find("Player");
            player.GetComponent<Rigidbody>().useGravity = false;
            StartCoroutine(playInstructionAnimation());
            
        }

        //For checking if screensize changed
        resHeight = Screen.height;
        progressBarYearGameObjects = new GameObject[platformsPerGames+1];

        StartCoroutine(getTimeLine());
        contentItemList = new List<ContentItem>();
    }

    //Fade in animation for instruction elements
    IEnumerator playInstructionAnimation()
    {
        Image instruction1 = GameObject.Find("Instruction-1").GetComponent<Image>();
        instruction1.canvasRenderer.SetAlpha(0.0f);
        Image instruction2 = GameObject.Find("Instruction-2").GetComponent<Image>();
        instruction2.canvasRenderer.SetAlpha(0.0f);
        Image instruction3 = GameObject.Find("Instruction-3").GetComponent<Image>();
        instruction3.canvasRenderer.SetAlpha(0.0f);
        Image instruction4 = GameObject.Find("Instruction-4").GetComponent<Image>();
        instruction4.canvasRenderer.SetAlpha(0.0f);
        Image instruction5 = GameObject.Find("Instruction-5").GetComponent<Image>();
        instruction5.canvasRenderer.SetAlpha(0.0f);
        Image instruction6 = GameObject.Find("Instruction-6").GetComponent<Image>();
        instruction6.canvasRenderer.SetAlpha(0.0f);
        Image instruction7 = GameObject.Find("Instruction-7").GetComponent<Image>();
        instruction7.canvasRenderer.SetAlpha(0.0f);

        yield return new WaitForSeconds(1);
        instruction1.CrossFadeAlpha(1.0f, 1, false);

        yield return new WaitForSeconds(1);
        instruction2.CrossFadeAlpha(1.0f, 1, false);

        yield return new WaitForSeconds(1);
        instruction3.CrossFadeAlpha(1.0f, 1, false);

        yield return new WaitForSeconds(1);
        instruction4.CrossFadeAlpha(1.0f, 1, false);

        yield return new WaitForSeconds(1.5f);
        instruction5.CrossFadeAlpha(1.0f, 1, false);

        yield return new WaitForSeconds(1);
        instruction6.CrossFadeAlpha(1.0f, 1, false);

        yield return new WaitForSeconds(1);
        instruction7.CrossFadeAlpha(1.0f, 1, false);
    }

    IEnumerator getTimeLine()
    {
        if (!restartSameCollection)
        {
            yield return StartCoroutine(checkForResponse("http://google.com"));
        }       
        constructTimeline();
    }

    public IEnumerator checkForResponse(string targetURL)
    {
        string html = string.Empty;
        WWW www = new WWW(targetURL);

        yield return www;

        if (www.error != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            html = www.text.Substring(0, 80);
            if (html.IndexOf("schema.org/WebPage") > -1)
            {
                yield return StartCoroutine(retrieveTimelineAsync(superCollectionName, collectionName));
                Debug.Log("Getting Timeline");
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

    }

    IEnumerator retrieveTimelineAsync(string superCollectionName, string collectionName)
    {

        string requestTemplate = "http://www.chronozoom.com/api/gettimelines?supercollection={0}&collection={1}";
        string requestUrl = String.Format(requestTemplate, superCollectionName, collectionName);
        WWW www = new WWW(requestUrl);
        yield return www;
        if (www.error == null)
        {
            timeline = JsonConvert.DeserializeObject<Timeline>(www.text);
            timelineRetrieved = true;
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
        }
    }

    private void constructTimeline()
    {
        if (timeline != null && !String.IsNullOrEmpty(timeline.__type))
        {
            timelineRetrieved = true;

            if (!restartSameCollection)
            {
                ChronozoomHandler.GenerateLists(timeline, limitContentToImages);
                game = ChronozoomHandler.SetUpGame(wormholesPerPlatform, platformsPerGames + 1);
            }

            if (timelineRetrieved)
            {
                if (game != null)
                {
                    setupGame(game);
                    Logger.LogPlayEvent("GameSetup Done", "Ball Game", "Main" + "Start", Main.superCollectionName, Main.collectionName, game.ToString());
                }
                else
                {
                    SceneManager.LoadScene("MainScene");
                }
            }
            playButton.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }


    private void setupGame(List<GameStage> game)
    {
        //Loops through and creates a level per loop
        for (int i = 0; i < platformsPerGames+1; i++) {
            //Creates platforms
            GameObject platform = (GameObject)Instantiate(Resources.Load("Plate"));

            //Final platform, activate review trigger
            if (i == platformsPerGames)
            {
                platform.transform.Find("MainPlate").tag = "Review-Hole";
            }
                

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

            //Create holes
            //Last Platform
            if(i == platformsPerGames)
            {
                setupLastPlatform(platform.name);

                //Create Exhibit content items "walls"
                setupExhibitContentItem(i);
            }
            //Normal Platform
            else
            {
                //Create Exhibit content items "walls"
                setupExhibitContentItem(i);

                //Correct wormhole
                setupHole(platform.name, game[i], true, 0, false);

                //Incorrect wormhole(s)
                for (int j = 0; j < wormholesPerPlatform - 1; j++)
                {
                    //Create ItemImages from URL 
                    setupHole(platform.name, game[i], false, j, false);
                }
            }

            //Create progress bar year texts
            GameObject yearUI = (GameObject)Instantiate(Resources.Load("YearUI-Orange"));
            yearUI.transform.SetParent(GameObject.Find("Game_Canvas").transform);
            yearUI.GetComponent<Text>().text = formatYear(game[i].stageEvent.time);
            yearUI.GetComponent<RectTransform>().position = new Vector3(15f,i!=platformsPerGames?i==0?Screen.height: 
                Screen.height-((Screen.height/platformsPerGames)* i-4f):15f, 0f);
            if(i!=0)
                yearUI.SetActive(false);
            progressBarYearGameObjects[i] = yearUI;
        }

        //Set collection name ui text
        GameObject.Find("Collection Name").GetComponent<Text>().text = PlayerPrefs.GetString("Collection Name")
            + " ("+formatYear(game[0].stageEvent.time)+ " to "+ formatYear(game[platformsPerGames].stageEvent.time)+")";

    }
    
    private void setupExhibitContentItem(int plateNumber)
    {
        //Create exhibit year
        GameObject exhibitYear = (GameObject)Instantiate(Resources.Load("ExhibitYear"));

        Vector3 yearPosition = new Vector3(exhibitYear.transform.position.x, -(plateNumber * plateDistance) + 0.1f, exhibitYear.transform.position.z);
        exhibitYear.transform.position = yearPosition;

        //Calculate exhibit year
        long year = game[plateNumber].stageEvent.time;
        exhibitYear.GetComponent<TextMesh>().text = formatYear(year);

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
            year = stage.correctWormhole.ParentExhibitTime;
        }
        else
        {
            uri = stage.incorrectWormholes[holeNumber].uri;
            title = stage.incorrectWormholes[holeNumber].title;
            id = stage.incorrectWormholes[holeNumber].id;
            year = stage.incorrectWormholes[holeNumber].ParentExhibitTime;
            contentItemList.Add(stage.incorrectWormholes[holeNumber]);
        }

        yearFormatted = formatYear(year);



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

        //Create circles to put next to image object (assist movement)
        //GameObject movementAssistLeft = (GameObject)Instantiate(Resources.Load("MovementAssistButton"));
        //GameObject movementAssistRight = (GameObject)Instantiate(Resources.Load("MovementAssistButton"));


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
        
    }

    // Update is called once per frame
    void Update () {

        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));

        if (resHeight != Screen.height)
        {
            resHeight = Screen.height;
            if(progressBarYearGameObjects[platformsPerGames] != null)
                repositionYear();
        }
    }

    private void repositionYear()
    {
        for(int i = 0; i < progressBarYearGameObjects.Length; i++)
        {
            progressBarYearGameObjects[i].GetComponent<RectTransform>().position = new Vector3(15f, i != platformsPerGames ? i == 0 ? Screen.height :
                Screen.height - ((Screen.height / platformsPerGames) * i - 4f) : 15f, 0f);
        }
    }

    public Exhibit getStageEventContent(int level)
    {
        Logger.LogException("CZBALL", "Main", "getStageEventContent", "A1");
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

    private String formatYear(long year)
    {
        if (year < 0)
            return ((year * -1).ToString().Length != 4) ? (year * (-1)).ToString("n0") + " BCE" : (year * -1).ToString() + " BCE";
        else
            return (year.ToString().Length != 4) ? year.ToString("n0") : year.ToString();
    }

    public void playGame()
    {
        loadingImage.alpha = 0;
        loadingImage.blocksRaycasts = false;
        GameObject player = GameObject.Find("Player");
        player.GetComponent<Rigidbody>().useGravity = true;

    }


}
