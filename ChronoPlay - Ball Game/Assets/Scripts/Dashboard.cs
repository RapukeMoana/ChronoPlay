using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Dashboard : MonoBehaviour {

    public Text totalCorrectText;
    public Text totalIncorrectText;
    public Text totalTimeText;
    public Text averageTimeText;
    public GameObject loadingImage;
    public static bool menuEnabled = true;

    public Slider sliderPlatform;
    public Slider sliderHoles;
    public Text sliderNumberOfPlatforms;
    public Text sliderNumberOfHoles;

    private SettingsConfig settings = new SettingsConfig();

    // Use this for initialization
    void Start () {
        if(PlayerPrefs.HasKey("Last Played Date"))
        {
            totalCorrectText.text = PlayerPrefs.GetInt("Total Correct")+"";
            totalIncorrectText.text = PlayerPrefs.GetInt("Total Incorrect")+"";
            totalTimeText.text = PlayerPrefs.GetFloat("Total Time").ToString("n1")+"s";
            averageTimeText.text = PlayerPrefs.GetFloat("Average Time").ToString("n1")+"s";
        }

        //TODO: Presist user preference
        if(PlayerPrefs.HasKey("Slider Platforms"))
        {
            //sliderPlatform.value = PlayerPrefs.GetInt("Slider Platforms");
            //sliderHoles.value = PlayerPrefs.GetInt("Slider Holes");
            settings.numPlatforms = Convert.ToInt32(sliderPlatform.value);
            settings.numHoles = Convert.ToInt32(sliderHoles.value);
        }
        else
        {
            settings.numPlatforms = Convert.ToInt32(sliderPlatform.value);
            settings.numHoles = Convert.ToInt32(sliderHoles.value);
        }


    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnEnable()
    {
        sliderPlatform.onValueChanged.AddListener(ChangeValuePlatform);
        sliderHoles.onValueChanged.AddListener(ChangeValueHoles);
        ChangeValuePlatform(sliderPlatform.value);
        ChangeValueHoles(sliderPlatform.value);
    }

    void OnDisable()
    {
        sliderPlatform.onValueChanged.RemoveAllListeners();
    }

    void ChangeValuePlatform(float platform)
    {
        sliderNumberOfPlatforms.text = platform + "";
        settings.numPlatforms = Convert.ToInt32(platform);

        PlayerPrefs.SetInt("Slider Platforms", Convert.ToInt32(platform));
        PlayerPrefs.Save();

    }

    void ChangeValueHoles(float holes)
    {
        sliderNumberOfHoles.text = holes + "";
        settings.numHoles = Convert.ToInt32(holes);

        PlayerPrefs.SetInt("Slider Holes", Convert.ToInt32(holes));
        PlayerPrefs.Save();
    }


    public void StartCosmos()
    {
        settings.superCollection = "chronozoom";
        settings.collection = "cosmos";
        settings.SetPublicVariables();

        loadingImage.SetActive(true);
        SceneManager.LoadScene(1);
        menuEnabled = false;
        
    }
}
