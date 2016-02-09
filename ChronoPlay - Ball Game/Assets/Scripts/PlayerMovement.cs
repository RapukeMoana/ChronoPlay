using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {
    public float speed;
    public float plateDistance = 20f;
    public bool isKeyBoard;
    public int zoom;
    public CanvasGroup gameCanvas;
    public GameObject loadingImage;
    public Canvas feedbackMenu;
    public InputField loggedBy;
    public InputField comments;
    public Image progressBar;

    private Rigidbody rb;
    private float speedsmooth = 0.8f;
    private float myAlpha = 1.0f;
    private bool resultFade = true, sideDescriptionVisible = false;
    public static int numCorrect = 0, numIncorrect = 0, level = 0;
    public static float timeSince = 0;



    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();

        myAlpha = 1.0f; // maybe you need other value
        //setupStageEvent();
        numCorrect = 0;
        numIncorrect = 0;
        level = 0;
        timeSince = 0;
    }

	// Update is called once per frame
	void FixedUpdate () {

        //accelerometer option - but cant test 
        if (isKeyBoard)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
            rb.AddForce(movement * speed);
        }
        else
        {
            float moveHorizontal = Input.acceleration.x;
            float moveVertical = Input.acceleration.z;
            Vector3 movement = new Vector3(moveHorizontal, 0, -moveVertical);
            if (movement.sqrMagnitude > 1)
                movement.Normalize();
            rb.AddForce(movement * speed);
        }


        //x axis boundary
        if (transform.position.x > 10.0f)
        {
            transform.position = new Vector3(10.0f, transform.position.y, transform.position.z);
        } else if (transform.position.x < -10.0f)
        {
            transform.position = new Vector3(-10.0f, transform.position.y, transform.position.z);
        }

        //z axis boundary
        if (transform.position.z > 10.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 10.0f);
        } else if (transform.position.z < -10.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        }

        //Temporary Zoom 
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView += zoom;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView -= zoom;
        }
    }

    void OnCollisionEnter(Collision other) {
        GameObject result;
        //Checks to see which wormhole ball enters 
        switch (other.transform.tag)
        {
            //If correct hole, update score and show result
            case "Correct-Hole":
                //Close Side Description
                if (sideDescriptionVisible)
                    gameCanvas.alpha = 0;

                Destroy(other.gameObject);
                numCorrect++;
                result = (GameObject)Instantiate(Resources.Load("Result"));
                GameObject.Find("Result").GetComponent<Text>().text = "CORRECT";
                myAlpha = 1.0f;
                resultFade = true;
                Logger.LogPlayEvent("Correct Hole, Time Taken: "+(Time.timeSinceLevelLoad- timeSince).ToString("n1"), "Ball Game", level.ToString(), Main.superCollectionName, Main.collectionName, other.transform.name);
                timeSince = Time.timeSinceLevelLoad;
                level++;
                //setupStageEvent();

                //Increase fill on statusbar
                float progressFill = (level * 1f)/ Main.platformsPerGames;
                progressBar.fillAmount = progressFill;

                break;
            case "Incorrect-Hole":
                GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
                numIncorrect++;
                result = (GameObject)Instantiate(Resources.Load("Result"));
                GameObject.Find("Result").GetComponent<Text>().text = "INCORRECT";
                myAlpha = 1.0f;
                resultFade = true;
                Logger.LogPlayEvent("Incorrect Hole, Time Taken: " + (Time.timeSinceLevelLoad - timeSince).ToString("n1"), "Ball Game", level.ToString(), Main.superCollectionName, Main.collectionName, other.transform.name);
                timeSince = Time.timeSinceLevelLoad;
                
                break;
            case "Restart-Hole":
                Logger.LogPlayEvent("Total Time:"+ Time.timeSinceLevelLoad.ToString("n1")+", Correct:"+ numCorrect+" Incorrect:"+ numIncorrect, "Ball Game", level.ToString(), Main.superCollectionName, Main.collectionName, other.transform.name);
                numCorrect++;
                saveProgress();
                loadingImage.SetActive(true);
                SceneManager.LoadScene(0);
                break;
            default:
                break;
        }      
    }



    void Update()
    {
        //If result is being shown, continue to fade
        if (resultFade)
        {
            Text result = GameObject.Find("Result").GetComponent<Text>();
            myAlpha = myAlpha - speedsmooth * Time.deltaTime;
            if (myAlpha > 0)
            { 
                result.color = new Color(1.0f, 1.0f, 1.0f, myAlpha);
            }
            else {
                resultFade = false;
                result.color = new Color(1.0f, 1.0f, 1.0f, 0);
            }
        }

        //Finds the object clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 800f))
            {
                showDescription(GameObject.Find(hit.collider.gameObject.name));
            }
        }
        Text timerText = GameObject.Find("TimerText").GetComponent<Text>();
        timerText.text = ("TIME: "+(Time.timeSinceLevelLoad).ToString("n1")) + " seconds";

    }

    //Show description of selected item
    private void showDescription(GameObject selectedItem)
    {
        ContentItem selected = GameObject.Find("Main Camera").GetComponent<Main>().getContentItemById(level, selectedItem.name, selectedItem.tag);

        Text descriptionText = GameObject.Find("Description").GetComponent<Text>();
        Text descriptionTitleText = GameObject.Find("DescriptionTitle").GetComponent<Text>();
        RawImage descriptionImage = GameObject.Find("DescriptionImage").GetComponent<RawImage>();
        ScrollRect scrollRect = GameObject.Find("ScrollRect").GetComponent<ScrollRect>();

        //If the clicked object is a content item
        if (selected != null)
        {
            string id;
            if (selectedItem.tag == "ItemImageLarge")
                id = selected.id + "-large";
            else
                id = selected.id;
            Texture texture = GameObject.Find(id).gameObject.GetComponent<Renderer>().material.mainTexture;
            descriptionText.text = selected.description;
            descriptionTitleText.text = selected.title;
            descriptionImage.texture = texture;

            //Scroll to the top 
            scrollRect.verticalNormalizedPosition = 100f;
            sideDescriptionVisible = true;
            gameCanvas.alpha = 1;
        }

        
    }

    private void setupStageEvent()
    {
        //Logger.LogException("CZBALL", "PlayerMovement", "setupStageEvent", "Start");
        if (level > 0)
        {
            GameObject[] previousImage = GameObject.FindGameObjectsWithTag("ItemImageLarge");
            GameObject[] previousText = GameObject.FindGameObjectsWithTag("ItemImageDescription"); 
            GameObject[] previousTitle = GameObject.FindGameObjectsWithTag("ExhibitTitle");
            for (int i = 0; i < previousImage.Length; i++)
            {
                Destroy(previousImage[i]);
                Destroy(previousText[i]);
            }
            Destroy(previousTitle[0]);
        }

        int localLevel = level;

// DW       Exhibit exhibit = Main.game[level].stageEvent;
        Exhibit exhibit = GameObject.Find("Main Camera").GetComponent<Main>().getStageEventContent(localLevel);
        List<ContentItem> contentItems= exhibit.contentItems;
        //Create exhibit Title and year 3d text
        GameObject exhibitTitle = (GameObject)Instantiate(Resources.Load("ExhibitTitle"));
        exhibitTitle.tag = "ExhibitTitle";
        long year = exhibit.time;
        if (year < 0) {
            exhibitTitle.GetComponent<TextMesh>().text =  exhibit.title + (((year*-1).ToString().Length != 4) ? " (" + (year * (-1)).ToString("n0") + " BC)": " (" + year * (-1) + " BC)");
        } else
            exhibitTitle.GetComponent<TextMesh>().text = exhibit.title +  ((year.ToString().Length != 4) ? " (" + year.ToString("n0") + ")" : " (" + year + ")");
        exhibitTitle.transform.position = new Vector3(0f, -20f * (level)-0f, 100f);
//        exhibitTitle.transform.position = new Vector3(0f, -20f * (localLevel) - 0f, 100f);

        for (int z = 0; z < contentItems.Count; z++)
        {
            StartCoroutine(createStageEvent(contentItems[z], z, contentItems.Count, level));
//           StartCoroutine(createStageEvent(contentItems[z], z, contentItems.Count, localLevel));

        }
    }


    //Creates background stageevent images
    IEnumerator createStageEvent(ContentItem contentItem, int z, int count, int localLevel)
    {


        Texture2D texture = new Texture2D(1, 1);
        string stageEventid = contentItem.id+"-large";
        string stageEventUri = contentItem.uri;
        string stageEventDescription = contentItem.title;

        //Creates item image object
        GameObject itemImageLarge = (GameObject)Instantiate(Resources.Load("ItemImageLarge"));
        itemImageLarge.tag = "ItemImageLarge";
        itemImageLarge.name = stageEventid;

        //Makes the image object a child of the camera
        //GameObject mainCamera = GameObject.Find("Main Camera");
        //itemImageLarge.transform.parent = mainCamera.transform;


        //Place image behind the scene and centres
        itemImageLarge.transform.position = new Vector3(z * 100f - (45f * (count - 1)), -20f * (localLevel) - (10f * count) - 15f, 100f + ((count - 1) * 30f));
        

        //Create description 3d text
        GameObject itemImageLargeDescription = (GameObject)Instantiate(Resources.Load("ExhibitItemDescription"));
        itemImageLargeDescription.tag = "ItemImageDescription";

        Vector3 tempPosition = itemImageLarge.transform.position;
        itemImageLargeDescription.GetComponent<TextMesh>().text = stageEventDescription;
        itemImageLargeDescription.transform.position = new Vector3(tempPosition.x, tempPosition.y - 15f, tempPosition.z - 20f);

        // Start a download of the given URL
        WWW www = new WWW(Uri.EscapeUriString(stageEventUri));

        

        // Wait for download to complete
        yield return www;

        //Checks to see if the image is returned but the user is in a different level
        if (localLevel == level)
        {
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


        }
    }

    //Save score to storage
    private void saveProgress()
    {
        PlayerPrefs.SetString("Platforms", Main.platformsPerGames+"");
        PlayerPrefs.SetString("Holes", Main.wormholesPerPlatform + "");
        PlayerPrefs.SetInt("Total Correct", numCorrect);
        PlayerPrefs.SetInt("Total Incorrect", numIncorrect);
        PlayerPrefs.SetFloat("Total Time", timeSince);
        PlayerPrefs.SetFloat("Average Time", timeSince / Main.platformsPerGames);
        PlayerPrefs.SetString("Last Played Date", System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
        PlayerPrefs.Save();
    }

    //Clicking on feedback button shows the menu and pauses the game
    public void ShowFeedbackMenu()
    {
        feedbackMenu.enabled = true;
        Time.timeScale = 0;
    }

    //Submit feedback
    public void SubmitFeedback()
    {
        if (!string.IsNullOrEmpty(comments.text) || !string.IsNullOrEmpty(loggedBy.text))
        {
            Logger.LogFeedback(comments.text, 0, "", loggedBy.text);
            HideFeedbackMenu();
        }
        else
        {
            print("Insufficent details to submit feedback");
            HideFeedbackMenu();
        }
    }

    public void HideFeedbackMenu()
    {
        feedbackMenu.enabled = false;
        Time.timeScale = 1;
    }


}

