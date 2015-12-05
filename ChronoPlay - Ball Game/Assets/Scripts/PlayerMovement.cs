using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public float speed;
    public float plateDistance;

    private Rigidbody rb;
    private float speedsmooth = 0.8f;
    private float myAlpha = 1.0f;
    private bool resultFade = true;
    private int numCorrect = 0, numIncorrect = 0;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        plateDistance = 20f;

        myAlpha = 1.0f; // maybe you need other value
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision other) {
        Text scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        GameObject result;
        //Checks to see which wormhole ball enters 
        switch (other.transform.tag)
        {
            //If correct hole, update score and show result
            case "Correct":
                Destroy(other.gameObject);
                numCorrect++;
                scoreText.text = "SCORE: Correct = " + numCorrect+ "  Incorrect = "+numIncorrect;
                result = (GameObject)Instantiate(Resources.Load("Result"));
                GameObject.Find("Result").GetComponent<Text>().text = "CORRECT";
                myAlpha = 1.0f;
                resultFade = true;
                break;
            case "Incorrect":
                Destroy(other.gameObject);
                numIncorrect++;
                scoreText.text = "SCORE: Correct = " + numCorrect + "  Incorrect = " + numIncorrect;
                result = (GameObject)Instantiate(Resources.Load("Result"));
                GameObject.Find("Result").GetComponent<Text>().text = "INCORRECT";
                myAlpha = 1.0f;
                resultFade = true;
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
            print(myAlpha);
            if (myAlpha > 0)
            { 
                result.color = new Color(1.0f, 1.0f, 1.0f, myAlpha);
            }
            else {
                resultFade = false;
            }
        }
        
    }
}
