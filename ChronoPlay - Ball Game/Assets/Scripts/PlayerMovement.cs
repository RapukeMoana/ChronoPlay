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

    private Rigidbody rb;
    private float speedsmooth = 0.8f;
    private float myAlpha = 1.0f;
    private bool resultFade = true;
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
            print(moveVertical +" "+ moveVertical);
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

            if (Physics.Raycast(ray, out hit, 100f))
                showDescription(GameObject.Find(hit.collider.gameObject.name));


        }

    }

    //Show description of selected item
    private void showDescription(GameObject selectedItem)
    {
        ContentItem selected = GameObject.Find("Main Camera").GetComponent<Main>().getContentItemById(level, selectedItem.name, selectedItem.tag == "Correct");
        if(selected != null)
        {
            Text descriptionText = GameObject.Find("Description").GetComponent<Text>();
            Text descriptionTitleText = GameObject.Find("DescriptionTitle").GetComponent<Text>();
            descriptionText.text = selected.description;
            descriptionTitleText.text = selected.title;
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
        List<ContentItem> contentItems = GameObject.Find("Main Camera").GetComponent<Main>().getStageEventContent(level);

        for (int z = 0; z < contentItems.Count; z++)
        {
            Texture2D texture = new Texture2D(1, 1);

            string stageEventUri = contentItems[z].uri;
            print(stageEventUri);
            print(contentItems[z].mediaType);

            // Start a download of the given URL
            WWW www = new WWW(Uri.EscapeUriString(stageEventUri));



            // Wait for download to complete
            yield return www;

            // assign texture
            www.LoadImageIntoTexture(texture);

            //Creates item image object
            GameObject itemImageLarge = (GameObject)Instantiate(Resources.Load("ItemImageLargeWrapper"));
            itemImageLarge.tag = "ItemImageLarge";

            //Makes the image object a child of the camera
            GameObject mainCamera = GameObject.Find("Main Camera");
            itemImageLarge.transform.parent = mainCamera.transform;

            //Get position of current platform
            Vector3 cameraPosition = mainCamera.transform.position;

            //Place image behind the scene and centres
            itemImageLarge.transform.GetChild(0).position = new Vector3(z * 100f - (30f* (contentItems.Count-1f)), cameraPosition.y-20f, 100f);
            itemImageLarge.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            //Create description 3d text
            GameObject itemImageLargeDescription = (GameObject)Instantiate(Resources.Load("ItemDescription"));
            itemImageLargeDescription.tag = "ItemImageDescription";

            Vector3 tempPosition = itemImageLarge.transform.GetChild(0).position;
            itemImageLargeDescription.GetComponent<TextMesh>().text = contentItems[z].title;
            itemImageLargeDescription.transform.position = new Vector3(tempPosition.x - 40f, tempPosition.y-20f, tempPosition.z-20f);
            itemImageLargeDescription.transform.parent = mainCamera.transform;

        }
    }
}
