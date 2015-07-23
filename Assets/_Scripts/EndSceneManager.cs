using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour {

    private int playersConfirmed = 0;
    public Text scoreText1;
    public Text scoreText2;
    public Text highscoreText1;
    public Text highscoreText2;

	// Use this for initialization
	void Start () {
        scoreText1.text = StaticStatScript.finalScore.ToString();
        scoreText2.text = StaticStatScript.finalScore.ToString();

        highscoreText1.text = StaticStatScript.highscore.ToString();
        highscoreText2.text = StaticStatScript.highscore.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PressStartToContinue(Button button)
    {
        button.GetComponentInChildren<Text>().text = "Waiting";
        button.interactable = false;
        playersConfirmed++;

        if (playersConfirmed >= 2)
        {
            //Do stuff when both players start the game
            //Application.LoadLevel(0);
            Debug.Log("Going back to start of Game");
        }
    }
}
