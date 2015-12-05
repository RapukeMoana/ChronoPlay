using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    //Private variables
    private bool timelineRetrieved = false;

    //__PRIVATE VARIABLES__
    private Timeline timeline = new Timeline();
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
            List<GameStage> game = ChronozoomHandler.SetUpGame(wormholesPerPlatform, platformsPerGames);
            //print(game[0].correctWormhole.year);
            //print(game[0].correctWormhole.id);
            //print(game[0].correctWormhole.description);
            //print(game[0].correctWormhole.title);
            //print(game[0].correctWormhole.uri);
            //print(game[0].incorrectWormholes.Count);

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
        print("HoleCover-" + row + "-" + col);
        GameObject holePosition = GameObject.Find(platformName+"HoleCover-"+row+"-"+col);
        while(holePosition == null)
        {
            print("null called");
            row = UnityEngine.Random.Range(0, 8);
            col = UnityEngine.Random.Range(0, 8);
            holePosition = GameObject.Find(platformName + "HoleCover-" + row + "-" + col);
        }

        Vector3 holeCoordinates = holePosition.transform.position; 
        Destroy(holePosition);
        //Create image which will be shown above the hole
        StartCoroutine(createItemImage(holeCoordinates, isCorrect, stage, holeNumber));
    }

    IEnumerator createItemImage(Vector3 holeCoordinate, bool isCorrect, GameStage stage, int holeNumber)
    {
        Texture2D texture = new Texture2D(1, 1);

        //Get uri of image
        string uri;
        if (isCorrect)
        {
            uri = stage.correctWormhole.uri;
        }
        else
        {
            uri = stage.incorrectWormholes[holeNumber].uri;
        }
        // Start a download of the given URL
        WWW www = new WWW(uri);

        // Wait for download to complete
        yield return www;

        // assign texture
        www.LoadImageIntoTexture(texture);


        //Creates item image object
        GameObject itemImage = (GameObject)Instantiate(Resources.Load("ItemImage"));

        //Create sensor
        GameObject itemImageSensor = (GameObject)Instantiate(Resources.Load("ItemImageSensor"));

        //Add correct/incorrect tags
        if(isCorrect)
            itemImageSensor.tag = "Correct";
        else
            itemImageSensor.tag = "Incorrect";

        //Place image on top of the current wormhole
        itemImage.transform.position = new Vector3(holeCoordinate.x, holeCoordinate.y+3, holeCoordinate.z+2);
        itemImage.GetComponent<Renderer>().material.mainTexture = texture;

        itemImageSensor.transform.position = new Vector3(holeCoordinate.x, holeCoordinate.y, holeCoordinate.z);
        itemImageSensor.GetComponent<Renderer>().material.mainTexture = texture;


    }
    // Update is called once per frame
    void Update () {
	
	}
}
