using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class ExampleBehaviourScript : MonoBehaviour {
    //Private variables
    private bool timelineRetrieved = false;
    private float xDist = 1;
    private float yDist = 0.5f;
    private bool cPressed = false;

    //__PRIVATE VARIABLES__
    private Timeline timeline = new Timeline();
    //__PUBLIC VARIABLES__
    public string superCollectionName = "chronozoom"; //chronozoom nobelprize
    public string collectionName = "cosmos"; //cosmos nobel
    public int wormholesPerPlatform = 3;
    public int platformsPerGames = 10;
    public bool limitContentToImages = true;

    // Use this for initialization
    void Start () {
        timeline = ChronozoomHandler.RetrieveTimeline(superCollectionName, collectionName);
        if (!String.IsNullOrEmpty(timeline.__type))
        {
            timelineRetrieved = true;
            ChronozoomHandler.GenerateLists(timeline, limitContentToImages);
            List<GameStage> game = ChronozoomHandler.SetUpGame(wormholesPerPlatform, platformsPerGames);
        }
        //AzureMobileService.init();
        //AzureMobileService.DoLogin();
        //AzureMobileService.ReadItems();

        //Logger.GetLog();
        //Logger.Log("{\"EVENT\":\"JSON\"}", "TestingAPI", "GAMEID", "COLLECTIONID", "TIMELINEID", "CONTENTITEMID");
    }


    //  gameObject.GetComponent<Renderer>().material.color = Color.black;
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.C) && !cPressed && timelineRetrieved)
        {
            //if (!String.IsNullOrEmpty(timeline.title))
            //{
            //    Material material = (Material)Resources.Load("Material/Light Yellow");

            //    foreach (Exhibit exhibititem in timeline.exhibits)
            //    {
            //        GameObject exhibit = createGameObject(exhibititem.title, material);
            //        SetPrefabText(exhibit, exhibititem.title);    
            //        GenerateContentItems(exhibititem.contentItems);
            //        yDist ++;
            //    }
            //}
            //cPressed = true;

        }
    }

    private void SetPrefabText(GameObject gameObject, string desiredText)
    {
        Canvas[] canvas = gameObject.GetComponentsInChildren<Canvas>();
        Text[] text = canvas[0].GetComponentsInChildren<Text>();
        text[0].text = desiredText;
    }

    private GameObject createGameObject (string objectTitle, Material material)
    {
        GameObject gameObject = (GameObject)Instantiate(Resources.Load("ExhibitCube"));
        gameObject.transform.localScale = new Vector3(1, 1, 0.1f);
        gameObject.transform.Translate(new Vector3(-xDist, yDist, 0));
        gameObject.name = objectTitle;
        gameObject.GetComponent<Renderer>().material = material;
        xDist ++;
        return gameObject;
    }

    private void GenerateContentItems(List<ContentItem> contentItems)
    {
        Material material = (Material)Resources.Load("Material/White");
        foreach (ContentItem contentItem in contentItems)
        {
            GameObject content = createGameObject(contentItem.title, material);
            SetPrefabText(content, contentItem.title + "\n\n" + contentItem.description);
        }

        xDist = 1;
    }
}
