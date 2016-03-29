using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Dashboard : MonoBehaviour {

    public Text holesAndPlatforms;
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

    public Canvas feedbackMenu;
    public InputField loggedBy;
    public InputField comments;

    public Text loadingText;

    private SettingsConfig settings = new SettingsConfig();

    // Use this for initialization
    void Start () {
        if(PlayerPrefs.HasKey("Last Played Date"))
        {
            holesAndPlatforms.text = PlayerPrefs.GetString("Platforms") + "/" + PlayerPrefs.GetString("Holes");
            totalCorrectText.text = PlayerPrefs.GetInt("Total Correct")+"";
            totalIncorrectText.text = PlayerPrefs.GetInt("Total Incorrect")+"";
            totalTimeText.text = PlayerPrefs.GetFloat("Total Time").ToString("n0")+"s";
            averageTimeText.text = PlayerPrefs.GetFloat("Average Time").ToString("n0")+"s";
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
        sliderNumberOfPlatforms.text = sliderPlatform.value+"";
        sliderNumberOfHoles.text = sliderHoles.value + "";



    }

    // Update is called once per frame
    void Update () {
        if (!menuEnabled)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }
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

        //loadingImage.SetActive(true);
        //SceneManager.LoadScene(1);
        //menuEnabled = false;


        LoadLevel();
    }

    public void StartEarth()
    {
        settings.superCollection = "bighistorylabs";
        settings.collection = "bighistorylabs";
        settings.SetPublicVariables();

        //loadingImage.SetActive(true);
        //SceneManager.LoadScene(1);
        //menuEnabled = false;

        
        LoadLevel();

    }

    //Load level with progress updates
    void LoadLevel()
    {
        menuEnabled = false;
        loadingImage.SetActive(true);
        StartCoroutine(LevelCoroutine());
    }

    IEnumerator LevelCoroutine()
    {
        //TODO: Remove temporary wait to show loading
        yield return new WaitForSeconds(2);
        AsyncOperation async = SceneManager.LoadSceneAsync(1);

        while (!async.isDone)
        {
            yield return null;
        }

    }

    public void OpenChronozoomWebsite()
    {
        Application.OpenURL("http://www.chronozoom.com/");
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
