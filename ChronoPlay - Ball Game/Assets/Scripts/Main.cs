using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Main : MonoBehaviour {

    //Private variables
    private bool timelineRetrieved = false;

    //__PRIVATE VARIABLES__
    public Timeline timeline = new Timeline();
    //__PUBLIC VARIABLES__
    public string superCollectionName = "chronozoom"; //chronozoom nobelprize
    public string collectionName = "cosmos"; //cosmos nobel
    public int wormholesPerPlatform = 3;
    public int platformsPerGames = 10;
    public bool limitContentToImages = true;
    public float plateDistance = 20f;
    private List<GameStage> game;

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
    }

    private void setupGame(List<GameStage> game)
    {
        //Loops through and creates a level per loop
        for (int i = 0; i < platformsPerGames; i++) {
            //Creates platforms
            GameObject platform = (GameObject)Instantiate(Resources.Load("Plate"));

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
        int row = UnityEngine.Random.Range(0, 8);
        int col = UnityEngine.Random.Range(0, 8);

        GameObject holePosition = GameObject.Find(platformName+"HoleCover-"+row+"-"+col);

        while (holePosition == null)
        {
            row = UnityEngine.Random.Range(0, 8);
            col = UnityEngine.Random.Range(0, 8);
            holePosition = GameObject.Find(platformName + "HoleCover-" + row + "-" + col);
        }



        //Create image which will be shown above the hole
        StartCoroutine(createItemImage(isCorrect, stage, holeNumber, holePosition));
    }

    IEnumerator createItemImage(bool isCorrect, GameStage stage, int holeNumber, GameObject holePosition)
    {
        Texture2D texture = new Texture2D(1, 1);
        Vector3 holeCoordinate = holePosition.transform.position;

        //Get uri of image
        string uri, title;
        if (isCorrect)
        {
            uri = stage.correctWormhole.uri;
            title = stage.correctWormhole.title;
        }
        else
        {
            uri = stage.incorrectWormholes[holeNumber].uri;
            title = stage.incorrectWormholes[holeNumber].title;
        }

        // Start a download of the given URL
        WWW www = new WWW(Uri.EscapeUriString(uri));

        // Wait for download to complete
        yield return www;

        // assign texture
        www.LoadImageIntoTexture(texture);


        //Creates item image object
        GameObject itemImage = (GameObject)Instantiate(Resources.Load("ItemImage"));

        //Create sensor
        GameObject itemImageSensor = (GameObject)Instantiate(Resources.Load("ItemImageSensor"));

        //Create hole on platform
        Destroy(holePosition);

        //Add correct/incorrect tags
        if (isCorrect)
            itemImageSensor.tag = "Correct";
        else
            itemImageSensor.tag = "Incorrect";

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
        ////this if check for the mouse left click
        //if (Input.GetButtonDown("0"))
        //{
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    //this if checks, a detection of hit in an GameObject with the mouse on screen
        //    if (Physics.Raycast(ray, hit))
        //    {
        //        //GameObject.Find("Nameofyourobject") search your gameobject on the hierarchy with the desired name and allows you to use it
        //        Destroy(GameObject.Find(hit.name));
        //    }
        //}


    }

    public List<ContentItem> getStageEventContent(int level)
    {
        return game[level].stageEvent.contentItems;
    }
}
