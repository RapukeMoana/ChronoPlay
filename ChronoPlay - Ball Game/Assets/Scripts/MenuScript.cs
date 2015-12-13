using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MenuScript : MonoBehaviour {
    public Canvas mainMenu;
    public Canvas settingsMenu;
    public Canvas timelineSelectMenu;
    public InputField superCollection;
    public InputField collection;
    public InputField holesPerPlatform;
    public InputField platformsPerGame;
    public Text errorMessage;


    // Use this for initialization
    void Start () {
        mainMenu = mainMenu.GetComponent<Canvas>();
        settingsMenu = settingsMenu.GetComponent<Canvas>();
        timelineSelectMenu = timelineSelectMenu.GetComponent<Canvas>();
        settingsMenu.enabled = false;
        timelineSelectMenu.enabled = false;

    }

    // Update is called once per frame
    void Update () {
	    
	}

    public void ShowSettingsMenu()
    {
        settingsMenu.enabled = true;
    }

    public void ShowTimelineSelectMenu()
    {
        timelineSelectMenu.enabled = true;
    }

    public void HideSettingsMenu()
    {
        settingsMenu.enabled = false;
    }

    public void HideTimelineSelectMenu()
    {
        timelineSelectMenu.enabled = false;
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

    public void QuickStart()
    {
        Application.LoadLevel(1);
        mainMenu.enabled = false;
    }

    public void Quit()
    {
        Application.Quit();
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
        Main.superCollectionName = superCollection;
        Main.collectionName = collection;
        Main.wormholesPerPlatform = numHoles;
        Main.platformsPerGames = numPlatforms;
    }
}
