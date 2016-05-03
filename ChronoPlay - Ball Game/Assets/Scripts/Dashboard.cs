using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Dashboard : MonoBehaviour
{

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
    public Dropdown dropdown;

    public Canvas feedbackMenu;
    public InputField loggedBy;
    public InputField comments;

    private SettingsConfig settings = new SettingsConfig();
    private const string _ApiKey = "2c917dc4aaa343a0817688db82ef275d";
    private const string _PublishedCollectionsRequestURL = "http://chronoplayapi.azurewebsites.net:80/api/Counts?APIKey=";
    private const string _ProjectName = "ChronoPlay - Dashboard Scene";
    private const string _ScriptFileName = "Dashboard.cs";

    // Use this for initialization
    void Start()
    {
        List<PlayableCollection> playableCollections = RetrievePublishedCollections();

        if (PlayerPrefs.HasKey("Last Played Date"))
        {
            holesAndPlatforms.text = PlayerPrefs.GetString("Platforms") + "/" + PlayerPrefs.GetString("Holes");
            totalCorrectText.text = PlayerPrefs.GetInt("Total Correct") + "";
            totalIncorrectText.text = PlayerPrefs.GetInt("Total Incorrect") + "";
            totalTimeText.text = PlayerPrefs.GetFloat("Total Time").ToString("n0") + "s";
            averageTimeText.text = PlayerPrefs.GetFloat("Average Time").ToString("n0") + "s";
        }

        //TODO: Presist user preference
        if (PlayerPrefs.HasKey("Slider Platforms"))
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
        sliderNumberOfPlatforms.text = sliderPlatform.value + "";
        sliderNumberOfHoles.text = sliderHoles.value + "";

        //Initialise 
        Main.restartSameCollection = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<PlayableCollection> RetrievePublishedCollections()
    {
        string resultStr = String.Empty;
        List<PlayableCollection> logResponse = new List<PlayableCollection>();
        string requestUrl = _PublishedCollectionsRequestURL + _ApiKey;

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            IAsyncResult asyncResult = request.BeginGetResponse(null, null);
            while (!asyncResult.AsyncWaitHandle.WaitOne(100)) { }

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    resultStr = reader.ReadToEnd();
                }
            }

            logResponse = JsonConvert.DeserializeObject<List<PlayableCollection>>(resultStr);

            return logResponse;
        }
        catch (Exception)
        {
            Logger.LogException(_ProjectName, _ScriptFileName, "RetrievePublishedCollections", "Error connecting to API");
            return logResponse;
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

    //public void StartCosmos()
    //{
    //    settings.superCollection = "chronozoom";
    //    settings.collection = "cosmos";
    //    settings.SetPublicVariables();

    //    loadingImage.SetActive(true);
    //    SceneManager.LoadScene(1);
    //    menuEnabled = false;

    //}

    //public void StartEarth()
    //{
    //    settings.superCollection = "bighistorylabs";
    //    settings.collection = "bighistorylabs";
    //    settings.SetPublicVariables();

    //    loadingImage.SetActive(true);
    //    SceneManager.LoadScene(1);
    //    menuEnabled = false;

    //}

    public void StartGame(string collectionIdentifier)
    {
        Dictionary<string, string> gameDetailsDictionary = new Dictionary<string, string>();
        string collection = string.Empty;

        //How it works:
        //gameDetailsDictionary.Add("superCollectionName", "collectionName"]);
        gameDetailsDictionary.Add("mrmccrudden", "MrMcCrudden");
        gameDetailsDictionary.Add("chronozoom", "Cosmos");
        gameDetailsDictionary.Add("bmwooddell", "bmwooddell");
        gameDetailsDictionary.Add("nobelprize", "Nobel");
        gameDetailsDictionary.Add("loriguidos", "loriguidos");
        gameDetailsDictionary.Add("rachelkim", "rachelkim");
        gameDetailsDictionary.Add("sbontrag", "sbontrag");
        gameDetailsDictionary.Add("teresarosasilva", "TeresaRosaSilva");
        gameDetailsDictionary.Add("mrbond", "MrBond");
        gameDetailsDictionary.Add("cornish", "Cornish");
        gameDetailsDictionary.Add("missmartindale", "missmartindale");
        gameDetailsDictionary.Add("j30033", "j30033");
        gameDetailsDictionary.Add("kharazinuniversity", "KharazinUniversity-2-3");
        gameDetailsDictionary.Add("bighistorylabs", "bighistorylabs");
        gameDetailsDictionary.Add("geraledesma", "geraledesma");

        gameDetailsDictionary.TryGetValue(collectionIdentifier, out collection);
        Debug.Log("superCollection: " + collectionIdentifier + "    Collection: " + collection);

        if (!String.IsNullOrEmpty(collection))
        {
            settings.superCollection = collectionIdentifier;
            settings.collection = collection;
            settings.SetPublicVariables();

            loadingImage.SetActive(true);
            SceneManager.LoadScene(1);
            menuEnabled = false;
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

public class PlayableCollection
{
    public int idx { get; set; }
    public string SuperCollection { get; set; }
    public string Collection { get; set; }
    public double? Timeline_Count { get; set; }
    public double? Exhibit_Count { get; set; }
    public bool Publish { get; set; }
    public string CZClone { get; set; }
    public string Language { get; set; }
    public string Comment { get; set; }
    public double? Content_Item_Count { get; set; }
}
