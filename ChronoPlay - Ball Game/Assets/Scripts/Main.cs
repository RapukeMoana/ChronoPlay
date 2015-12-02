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

    // Use this for initialization
    void Start()
    {
        timeline = ChronozoomHandler.RetrieveTimeline(superCollectionName, collectionName);
        if (!String.IsNullOrEmpty(timeline.__type))
        {
            timelineRetrieved = true;
            ChronozoomHandler.GenerateLists(timeline, limitContentToImages);
            List<GameStage> game = ChronozoomHandler.SetUpGame(wormholesPerPlatform, platformsPerGames);
            print(game[0].correctWormhole.year);
            print(game[0].correctWormhole.id);
            print(game[0].correctWormhole.description);
            print(game[0].correctWormhole.title);
            print(game[0].correctWormhole.uri);
            print(game[0].incorrectWormholes.Count);
            GameObject correctWormHoleImage = GameObject.Find("BlueItem");

        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
