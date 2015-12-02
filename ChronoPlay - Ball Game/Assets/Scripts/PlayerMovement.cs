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
            case "BlueCheckPoint":
                print("Blue Chosen");

                //createNewPlate();
                //GameObject go = GameObject.FindGameObjectWithTag("BlueCheckPoint");
                //GameObject a = GameObject.FindGameObjectWithTag("Finish");

                //Destroy(go);

                //(a.GetComponent(typeof(Collider)) as Collider).isTrigger = true;
                break;
            case "RedCheckPoint":
                print("Red Chosen");
                break;
            case "GreenCheckPoint":
                print("Green Chosen");
                break;
            default:
                break;
        }      
    }

    //Dynamically creates new plates
    private void createNewPlate()
    {
        GameObject nextPlate = (GameObject)Instantiate(Resources.Load("Plate"));
        level++;
        Vector3 platePosition = GameObject.Find("Plate").transform.position;
        nextPlate.transform.position = new Vector3(platePosition.x, platePosition.y - level*plateDistance, platePosition.z);
    }
}
