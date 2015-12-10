using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PlayerMovement : MonoBehaviour {
    public float speed;
    public float plateDistance = 20f;
    public bool isKeyBoard;
    public int zoom;

    private Rigidbody rb;
    private float speedsmooth = 0.8f;
    private float myAlpha = 1.0f;
    private bool resultFade = true, sideDescriptionVisible = false;
    private int numCorrect = 0, numIncorrect = 0, level = 0;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();

        myAlpha = 1.0f; // maybe you need other value
        StartCoroutine(setupStageEvent());
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
        Text scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        GameObject result;
        //Checks to see which wormhole ball enters 
        switch (other.transform.tag)
        {
            //If correct hole, update score and show result
            case "Correct-Hole":
                Destroy(other.gameObject);
                numCorrect++;
                scoreText.text = "SCORE: Correct = " + numCorrect+ "  Incorrect = "+numIncorrect;
                result = (GameObject)Instantiate(Resources.Load("Result"));
                GameObject.Find("Result").GetComponent<Text>().text = "CORRECT";
                myAlpha = 1.0f;
                resultFade = true;
                level++;
                StartCoroutine(setupStageEvent());
                break;
            case "Incorrect-Hole":
                Destroy(other.gameObject);
                numIncorrect++;
                scoreText.text = "SCORE: Correct = " + numCorrect + "  Incorrect = " + numIncorrect;
                result = (GameObject)Instantiate(Resources.Load("Result"));
                GameObject.Find("Result").GetComponent<Text>().text = "INCORRECT";
                myAlpha = 1.0f;
                resultFade = true;
                level++;
                StartCoroutine(setupStageEvent());
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

            if (Physics.Raycast(ray, out hit, 200f))
                showDescription(GameObject.Find(hit.collider.gameObject.name));


        }

    }

    //Show description of selected item
    private void showDescription(GameObject selectedItem)
    {
        ContentItem selected = GameObject.Find("Main Camera").GetComponent<Main>().getContentItemById(level, selectedItem.name, selectedItem.tag);
        Text descriptionText = GameObject.Find("Description").GetComponent<Text>();
        Text descriptionTitleText = GameObject.Find("DescriptionTitle").GetComponent<Text>();
        RawImage descriptionImage = GameObject.Find("DescriptionImage").GetComponent<RawImage>();
        Image blackBackground =  GameObject.Find("SideDescription").GetComponent<Image>();
        Image handle = GameObject.Find("Handle").GetComponent<Image>();

        print(selectedItem.name);


        if (selected != null)
        {
            string id = selected.id;
            Texture texture = GameObject.Find(id).gameObject.GetComponent<Renderer>().material.mainTexture;
            descriptionText.text = selected.description;
            descriptionTitleText.text = selected.title;
            descriptionImage.enabled = true;
            blackBackground.enabled = true;
            handle.enabled = true;
            descriptionImage.texture = texture;
            sideDescriptionVisible = true;
        }
        else if(sideDescriptionVisible)
        {
            //TODO: Disabled until fix: panel dissappears when click
            //descriptionText.text = "";
            //descriptionTitleText.text = "";
            //descriptionImage.enabled = false;
            //blackBackground.enabled = false;
            //handle.enabled = false;
            //sideDescriptionVisible = false;
        }
        
    }

    //Creates background stageevent images
    IEnumerator setupStageEvent()
    {
        

        if (level > 0)
        {
            GameObject[] previousImage = GameObject.FindGameObjectsWithTag("ItemImageLarge");
            GameObject[] previousText = GameObject.FindGameObjectsWithTag("ItemImageDescription"); 
            for (int i = 0; i < previousImage.Length; i++)
            {
                Destroy(previousImage[i]);
                Destroy(previousText[i]);
            }
        }
        print("Level:"+level);
        List<ContentItem> contentItems = GameObject.Find("Main Camera").GetComponent<Main>().getStageEventContent(level);
        print(contentItems.Count);
        int localLevel = level;
        for (int z = 0; z < contentItems.Count; z++)
        {
            Texture2D texture = new Texture2D(1, 1);
            string stageEventid = contentItems[z].id;
            string stageEventUri = contentItems[z].uri;
            //print(stageEventUri);
            //print(contentItems[z].mediaType);

            // Start a download of the given URL
            WWW www = new WWW(Uri.EscapeUriString(stageEventUri));



            // Wait for download to complete
            yield return www;
            
            //Checks to see if the image is returned but the user is in a different level
            if(localLevel == level)
            {
                // assign texture
                www.LoadImageIntoTexture(texture);

                //Creates item image object
                GameObject itemImageLarge = (GameObject)Instantiate(Resources.Load("ItemImageLarge"));
                itemImageLarge.tag = "ItemImageLarge";
                itemImageLarge.name = stageEventid;
                print("Local Level:" + localLevel + "," + level);

                //Makes the image object a child of the camera
                GameObject mainCamera = GameObject.Find("Main Camera");
                itemImageLarge.transform.parent = mainCamera.transform;

                //Get position of current platform
                Vector3 cameraPosition = mainCamera.transform.position;

                //Place image behind the scene and centres
                itemImageLarge.transform.position = new Vector3(z * 100f - (30f * (contentItems.Count - 1f)), cameraPosition.y - 20f, 100f);
                itemImageLarge.gameObject.GetComponent<Renderer>().material.mainTexture = texture;

                //Create description 3d text
                GameObject itemImageLargeDescription = (GameObject)Instantiate(Resources.Load("ItemDescription"));
                itemImageLargeDescription.tag = "ItemImageDescription";

                Vector3 tempPosition = itemImageLarge.transform.position;
                itemImageLargeDescription.GetComponent<TextMesh>().text = contentItems[z].title;
                itemImageLargeDescription.transform.position = new Vector3(tempPosition.x - 39f, tempPosition.y - 15f, tempPosition.z - 20f);
                itemImageLargeDescription.transform.parent = mainCamera.transform;
            }

            

        }
    }
}
