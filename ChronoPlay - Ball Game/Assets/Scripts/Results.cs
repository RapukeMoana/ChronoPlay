using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Results : MonoBehaviour {

    public Text totalCorrectText;
    public Text totalIncorrectText;
    public Text totalTimeText;
    public Text averageTimeText;

    // Use this for initialization
    void Start () {
        totalCorrectText.text = "Total Correct: "+ PlayerMovement.numCorrect;
        totalIncorrectText.text = "Total Incorrect: " + PlayerMovement.numIncorrect;
        totalTimeText.text = "Total Time: " + (PlayerMovement.timeSince).ToString("n1");
        averageTimeText.text = "Average Time Per Platform: " + (PlayerMovement.timeSince/Main.platformsPerGames).ToString("n1");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void returnToMainScreen()
    {
        SceneManager.LoadScene(0);
    }
}
