using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour {
    public float speed;
    public float plateDistance;
    private Rigidbody rb;
    private int level;
    

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        plateDistance = 20f;
        level = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision other) {
        //Checks to see which wormhole ball enters 
        switch (other.transform.tag)
        {
            case "Correct":
                print("Correct");
                Destroy(other.gameObject);
                break;
            case "Incorrect":
                print("Incorrect");
                Destroy(other.gameObject);
                break;
            default:
                break;
        }      
    }
}
