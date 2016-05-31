using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour {

    public CanvasGroup gameCanvas;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void closePanel()
    {
        gameCanvas.alpha = 0;
        Time.timeScale = 1;
    }
}
