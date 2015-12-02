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
        //print("Started");
        //timeline = ChronozoomHandler.RetrieveTimeline(superCollectionName, collectionName);
        //if (!String.IsNullOrEmpty(timeline.__type))
        //{
        //    timelineRetrieved = true;
        //    ChronozoomHandler.GenerateLists(timeline, limitContentToImages);
        //    List<GameStage> game = ChronozoomHandler.SetUpGame(wormholesPerPlatform, platformsPerGames);
        //    print(game[0].correctWormhole.year);
        //    print(game[0].correctWormhole.id);
        //    print(game[0].correctWormhole.description);
        //    print(game[0].correctWormhole.title);
        //    print(game[0].correctWormhole.uri);
        //    print(game[0].incorrectWormholes.Count);
        //    GameObject correctWormHoleImage = GameObject.Find("BlueItem");

        //}

        setupGame();
    }

    private void setupGame()
    {
        //Loops through and creates a level per loop
        for (int i = 0; i < platformsPerGames; i++) {
            //Creates platforms
            GameObject platform = (GameObject)Instantiate(Resources.Load("Plate"));

            //Add name to platform (e.g. platform-0 is first platform)
            platform.name = "platform-" + i;

            //Position plate below the previous 
            Vector3 platePosition = new Vector3(0f, -(i*plateDistance), 0f);
            platform.transform.position = platePosition;

            //Create wormholes 
            for (int j = 0; j < wormholesPerPlatform; j++) {
                //Create ItemImages from URL 
                StartCoroutine(createItemImage(platePosition, j));
            }
        }
        
    }

    IEnumerator createItemImage(Vector3 platePosition, int j)
    {
        print("Jello");
        Texture2D texture = new Texture2D(1, 1);

        // Start a download of the given URL
        WWW www = new WWW("http://cdni.condenast.co.uk/642x390/g_j/Interstellar-01-GQ-30Oct14_pr_b_642x390.jpg");

        // Wait for download to complete
        yield return www;

        www.LoadImageIntoTexture(texture);
        // assign texture

        //Creates item image object
        GameObject itemImage = (GameObject)Instantiate(Resources.Load("ItemImage"));
        itemImage.transform.position = new Vector3(platePosition.x + (j * 10), platePosition.y + 5, platePosition.z);
        itemImage.GetComponent<Renderer>().material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
