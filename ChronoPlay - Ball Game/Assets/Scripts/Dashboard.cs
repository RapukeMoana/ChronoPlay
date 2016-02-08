using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dashboard : MonoBehaviour {

    public Text totalCorrectText;
    public Text totalIncorrectText;
    public Text totalTimeText;
    public Text averageTimeText;
    public GameObject loadingImage;
    public static bool menuEnabled = true;

    // Use this for initialization
    void Start () {
        if(PlayerPrefs.HasKey("Last Played Date"))
        {
            totalCorrectText.text = PlayerPrefs.GetInt("Total Correct")+"";
            totalIncorrectText.text = PlayerPrefs.GetInt("Total Incorrect")+"";
            totalTimeText.text = PlayerPrefs.GetFloat("Total Time").ToString("n1")+"s";
            averageTimeText.text = PlayerPrefs.GetFloat("Average Time").ToString("n1")+"s";
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void StartCosmos()
    {
        //loadingImage.SetActive(true);
        Debug.Log("YES");
        //SceneManager.LoadScene(1);
        //menuEnabled = false;
    }
}
