using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MenuScript : MonoBehaviour {
    public static bool menuEnabled = true;
    public Canvas mainMenu;
    public Canvas settingsMenu;
    public Canvas timelineSelectMenu;
    public Canvas feedbackMenu;
    public InputField superCollection;
    public InputField timelineSelectSuperCollection;
    public InputField timelineSelectCollection;
    public InputField collection;
    public InputField holesPerPlatform;
    public InputField platformsPerGame;
    public InputField loggedBy;
    public InputField comments;

    public Text errorMessage;

    public GameObject loadingImage;

    // Use this for initialization
    void Start () {
        mainMenu = mainMenu.GetComponent<Canvas>();
        settingsMenu = settingsMenu.GetComponent<Canvas>();
        timelineSelectMenu = timelineSelectMenu.GetComponent<Canvas>();
        settingsMenu.enabled = false;
        timelineSelectMenu.enabled = false;
        feedbackMenu.enabled = false;
    }

    // Update is called once per frame
    void Update () {
	    
	}

    public void ShowSettingsMenu()
    {
        settingsMenu.enabled = true;
    }

    public void ShowFeedbackMenu()
    {
        feedbackMenu.enabled = true;
    }

    public void ShowTimelineSelectMenu()
    {
        timelineSelectMenu.enabled = true;
    }

    public void HideFeedbackMenu()
    {
        feedbackMenu.enabled = false;
    }

    public void HideSettingsMenu()
    {
        settingsMenu.enabled = false;
        timelineSelectMenu.enabled = false;
    }

    public void HideTimelineSelectMenu()
    {
        timelineSelectMenu.enabled = false;
        settingsMenu.enabled = false;
    }

    public void SaveSettingsMenu()
    {
        SettingsConfig settings = new SettingsConfig();
        try
        {
            settings.superCollection = superCollection.text;
            settings.collection = collection.text;
            settings.numHoles = Convert.ToInt32(holesPerPlatform.text);
            settings.numPlatforms = Convert.ToInt32(platformsPerGame.text);

            bool isValid = settings.IsInputValid();
            if (isValid)
            {
                settings.SetPublicVariables();
                HideSettingsMenu();
            }
            else
            {
                SetErrorMessage("Please ensure you have filled out the required fields correctly.");
            }
        }
        catch(Exception)
        {
            SetErrorMessage("Please ensure you have filled out the required fields correctly.");
        }
    }

    public void TimelineSelectPlay()
    {
        SettingsConfig settings = new SettingsConfig();
        try
        {
            settings.superCollection = timelineSelectSuperCollection.text;
            settings.collection = timelineSelectCollection.text;

            if (!string.IsNullOrEmpty(settings.superCollection) || !string.IsNullOrEmpty(settings.collection))
            {
                settings.SetPublicVariables();
                HideSettingsMenu();
                QuickStart();
            }
            else
            {
               Debug.Log("Please ensure you have filled out the required fields correctly.");
            }
        }
        catch (Exception)
        {
            Debug.Log("An exception occured. You should probably log this later on..");
        }
    }

    public void QuickStart()
    {
        loadingImage.SetActive(true);
        Application.LoadLevel(1);
        menuEnabled = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SubmitFeedback()
    {
        if (!string.IsNullOrEmpty(comments.text) || !string.IsNullOrEmpty(loggedBy.text))
        {
            Logger.LogFeedback(comments.text, 0, "", loggedBy.text);
            HideFeedbackMenu();
        }
        else
        {
            Debug.Log("Insufficent details to submit feedback");
            HideFeedbackMenu();
        }
    }

    public void SetErrorMessage(string message)
    {
        errorMessage.text = message;
    }
}

public class SettingsConfig
{
    public string superCollection { get; set; }
    public string collection { get; set; }
    public int numHoles { get; set; }
    public int numPlatforms { get; set; }

    public bool IsInputValid()
    {
        if (string.IsNullOrEmpty(superCollection))
        {
            return false;
        }
        if (string.IsNullOrEmpty(collection))
        {
            return false;
        }
        if (numHoles < 1)
        {
            return false;
        }
        if (numPlatforms < 1)
        {
            return false;
        }
        return true;
    } 

    public void SetPublicVariables()
    {
        if (superCollection != null)
        {
            Main.superCollectionName = superCollection;
        }
        if (collection != null)
        {
            Main.collectionName = collection;
        }
        if (numHoles > 0)
        {
            Main.wormholesPerPlatform = numHoles;
        }
        if (numPlatforms > 0)
        {
            Main.platformsPerGames = numPlatforms;
        }
    }

    
}
