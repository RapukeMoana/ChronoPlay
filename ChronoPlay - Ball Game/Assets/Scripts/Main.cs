using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    //Private variables
    private bool timelineRetrieved = false;
    private List<GameStage> game;
    private bool[] holeRow;

    public Timeline timeline = new Timeline();
    //__PUBLIC VARIABLES__
    public string superCollectionName = "chronozoom"; //chronozoom nobelprize
    public string collectionName = "cosmos"; //cosmos nobel
    public int wormholesPerPlatform = 3;
    public int platformsPerGames = 10;
    public bool limitContentToImages = true;
    public float plateDistance = 20f;



    // Use this for initialization
    void Start()
    {
        timeline = ChronozoomHandler.RetrieveTimeline(superCollectionName, collectionName);
        if (!String.IsNullOrEmpty(timeline.__type))
        {
            timelineRetrieved = true;
            ChronozoomHandler.GenerateLists(timeline, limitContentToImages);
            game = ChronozoomHandler.SetUpGame(wormholesPerPlatform, platformsPerGames);

            if(timelineRetrieved)
                setupGame(game);
        }
        string startDate = Convert.ToString(game[0].stageEvent.time);
        string endDate = Convert.ToString(game[game.Count - 1].stageEvent.time);
        RenderTimeline(startDate, endDate);

    }

    private void setupGame(List<GameStage> game)
    {
        //Loops through and creates a level per loop
        for (int i = 0; i < platformsPerGames; i++) {
            //Creates platforms
            GameObject platform = (GameObject)Instantiate(Resources.Load("Plate"));

            //Reset one hole per row rule
            holeRow = new bool[7];

            //Add name to platform (e.g. platform-0 is first platform)
            platform.name = "Platform-" + i;

            //Position plate below the previous 
            Vector3 platformPosition = new Vector3(0f, -(i*plateDistance), 0f);
            platform.transform.position = platformPosition;

            //Correct wormhole
            setupHole(platform.name, game[i],true, 0);

            //Incorrect wormhole(s)
            for (int j = 0; j < wormholesPerPlatform-1; j++)
            {
                //Create ItemImages from URL 
                setupHole(platform.name, game[i],false,j);
            }
        }
        
    }

    private void setupHole(string platformName, GameStage stage, bool isCorrect, int holeNumber)
    {
        
        //Get random positions to choose hole location
        int row = UnityEngine.Random.Range(0, 6);
        int col = UnityEngine.Random.Range(0, 6);

        while (holeRow[row])
        {
            row = UnityEngine.Random.Range(0, 6);
        }

        GameObject holePosition = GameObject.Find(platformName+"HoleCover-"+row+"-"+col);

        while (holePosition == null)
        {
            row = UnityEngine.Random.Range(0, 8);
            col = UnityEngine.Random.Range(0, 8);
            holePosition = GameObject.Find(platformName + "HoleCover-" + row + "-" + col);
        }

        holeRow[row] = true;

        //Create image which will be shown above the hole
        StartCoroutine(createItemImage(isCorrect, stage, holeNumber, holePosition));
    }

    IEnumerator createItemImage(bool isCorrect, GameStage stage, int holeNumber, GameObject holePosition)
    {
        Texture2D texture = new Texture2D(1, 1);
        Vector3 holeCoordinate = holePosition.transform.position;

        //Get uri of image
        string uri, title, id;
        if (isCorrect)
        {
            uri = stage.correctWormhole.uri;
            title = stage.correctWormhole.title;
            id = stage.correctWormhole.id;
        }
        else
        {
            uri = stage.incorrectWormholes[holeNumber].uri;
            title = stage.incorrectWormholes[holeNumber].title;
            id = stage.incorrectWormholes[holeNumber].id;
        }

        // Start a download of the given URL
        WWW www = new WWW(Uri.EscapeUriString(uri));

        // Wait for download to complete
        yield return www;

        // assign texture
        www.LoadImageIntoTexture(texture);


        //Creates item image object
        GameObject itemImage = (GameObject)Instantiate(Resources.Load("ItemImage"));
        itemImage.name = id;

        //Create sensor
        GameObject itemImageSensor = (GameObject)Instantiate(Resources.Load("ItemImageSensor"));
        itemImageSensor.name = id+"-sensor";

        //Create hole on platform
        Destroy(holePosition);

        //Add correct/incorrect tags
        if (isCorrect)
        {
            itemImageSensor.tag = "Correct-Hole";
            itemImage.tag = "Correct";
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
        itemImageDescription.transform.position = new Vector3(tempPosition.x-3.5f, tempPosition.y- 1.35f, tempPosition.z-1.2f);


        //Place image on sensor (hole)
        itemImageSensor.transform.position = new Vector3(holeCoordinate.x, holeCoordinate.y-0.2f, holeCoordinate.z);
        itemImageSensor.GetComponent<Renderer>().material.mainTexture = texture;


    }
    // Update is called once per frame
    void Update () {


    }

    public List<ContentItem> getStageEventContent(int level)
    {
        UpdateSlider(Convert.ToString(game[level].stageEvent.time));
        return game[level].stageEvent.contentItems;
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
                    if (game[level].stageEvent.contentItems[i].id == Id)
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
        GameObject gameObject = (GameObject)Instantiate(Resources.Load("TimelineCanvas"));
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
        startTime.text = startDate;
    }

    private void SetEndTimeLabel(string endDate)
    {
        GameObject temp = GameObject.Find("EndTime");
        Text endTime = temp.GetComponent<Text>();
        endTime.text = endDate;
    }

    private void SetSliderLabel(string time)
    {
        GameObject temp = GameObject.Find("CurrentTime");
        Text currentTime = temp.GetComponent<Text>();
        currentTime.text = time;
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
}
