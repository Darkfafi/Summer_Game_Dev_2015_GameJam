using UnityEngine;
using System.Collections;

public class EndGameScript : MonoBehaviour {

    public void GameOver(int score)
    {
        StaticStatScript.finalScore = score;
        if (StaticStatScript.highscore < score)
        {
            StaticStatScript.highscore = score;
        }
        Application.LoadLevel(2);
    }

}
