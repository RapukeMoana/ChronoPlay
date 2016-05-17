using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Dashboard : MonoBehaviour
{

    public Text holesAndPlatforms;
    public Text totalCorrectText;
    public Text totalIncorrectText;
    public Text totalTimeText;
    public Text averageTimeText;
    public static bool menuEnabled = true;

    public Slider sliderPlatform;
    public Slider sliderHoles;
    public Text sliderNumberOfPlatforms;
    public Text sliderNumberOfHoles;
    public Dropdown dropdown;

    public ScrollRect collectionScrollRect;
    public Scrollbar collectionScrollbar;
    public RawImage collectionTemplate;

    public Canvas feedbackMenu;
    public InputField loggedBy;
    public InputField comments;

    private SettingsConfig settings = new SettingsConfig();
    private const string _ApiKey = "2c917dc4aaa343a0817688db82ef275d";
    private const string _PublishedCollectionsRequestURL = "http://chronoplayapi.azurewebsites.net:80/api/Counts?APIKey=";
    private const string _ProjectName = "ChronoPlay - Dashboard Scene";
    private const string _ScriptFileName = "Dashboard.cs";
    private List<PlayableCollection> _playableCollections = new List<PlayableCollection>();

    // Use this for initialization
    void Start()
    {
        _playableCollections = RetrievePublishedCollections();
        StartCoroutine(populateCollectionScrollView());
        //populateCollectionScrollView();
        collectionScrollbar.value = 0;

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

    IEnumerator populateCollectionScrollView()
    {
        float xLocation = 0;
        GameObject template = GameObject.Find("Collection_playableItem");
        foreach (PlayableCollection collection in _playableCollections)
        {
            GameObject myTemplate = (GameObject)Instantiate(Resources.Load("CollectionItem"));
            Texture2D texture = new Texture2D(1, 1);

            Text[] textLabels = myTemplate. GetComponentsInChildren<Text>();
            //myTemplate.renderer = true;
            textLabels[0].text = collection.Title;
            textLabels[1].text = SetStartTimeLabel(collection.StartDate.ToString()) + "-" + SetEndTimeLabel(collection.EndDate.ToString());
            myTemplate.name = collection.Collection;
            myTemplate.transform.SetParent(GameObject.Find("Content").transform);

            Vector3 contentPosition = new Vector3(template.transform.position.x + xLocation, template.transform.position.y, template.transform.position.z);
            myTemplate.transform.position = contentPosition;
            myTemplate.transform.localScale = new Vector3(1, 1);
            xLocation = xLocation + 150;
            Button templateFunction = myTemplate.GetComponent<Button>();
            string title = collection.Title;
            templateFunction.onClick.AddListener(delegate { StartGame(); });

            if (collection.ImageURL != null)
            {
                RawImage image = myTemplate.GetComponent<RawImage>();
                WWW www = new WWW(Uri.EscapeUriString(collection.ImageURL));

                yield return www;
                // assign texture
                try
                {
                    // assign texture
                    if (www.error == null)
                    {
                        www.LoadImageIntoTexture(texture);
                    }
                    else
                    {
                        Logger.LogException("CZBall", "Dashboard", "populateCollectionScrollView", www.url + " not found");
                    }
                }
                catch
                {
                    Logger.LogException("CZBall", "Dashboard", "populateCollectionScrollView", "www.LoadImageIntoTexture(texture)");
                }
                image.texture = texture;
            }
        }

        template.SetActive(false);
    }

    private string SetStartTimeLabel(string startDate)
    {
        Int64 startYear = (Int64)Math.Round(Convert.ToDouble(startDate));
        long minus1 = (long)-1;
        if (startYear < 0)
            return ((startYear * minus1).ToString().Length != 4) ? (startYear * (minus1)).ToString("n0") + " BC" : (startYear * minus1).ToString() + " BC";
        else
            return (startYear.ToString().Length != 4) ? startYear.ToString("n0") : startYear.ToString();

    }

    private string SetEndTimeLabel(string endDate)
    {
        Int64 endYear = (Int64) Math.Round(Convert.ToDouble(endDate));
        long minus1 = (long)-1;
        if (endYear < 0)
            return((endYear * minus1).ToString().Length != 4) ? (endYear * (minus1)).ToString("n0") + " BC" : (endYear * minus1).ToString() + " BC";
        else
            return (endYear.ToString().Length != 4) ? endYear.ToString("n0") : endYear.ToString();
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

    public void StartGame()
    {
        string collectionName =  EventSystem.current.currentSelectedGameObject.name;
        PlayableCollection gameCol = null;

        foreach (PlayableCollection col in _playableCollections)
        {
            if(col.Collection == collectionName)
            {
                gameCol = col;
                break;
            }
        }

        Debug.Log("Starting: " + gameCol.Title);

        if (!String.IsNullOrEmpty(gameCol.Collection))
        {
            settings.superCollection = gameCol.SuperCollection;
            settings.collection = gameCol.Collection;
            settings.SetPublicVariables();
            PlayerPrefs.SetString("Collection Name", gameCol.Title);

            //loadingImage.SetActive(true);
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
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Title { get; set; }
    public string ImageURL { get; set; }
}
