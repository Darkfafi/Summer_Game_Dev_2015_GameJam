using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DualStartGameScript : MonoBehaviour {

    private int playersConfirmed = 0;

    //Expects players to be player 1 or player 2
    public void PressStartToContinue(Button button)
    {
        
        button.GetComponentInChildren<Text>().text = "Waiting";
        button.interactable = false;
        playersConfirmed++;

        if (playersConfirmed >= 2)
        {
            //Do stuff when both players start the game
            //Application.LoadLevel(1);
            Debug.Log("Starting Game");
        }
    }
}
