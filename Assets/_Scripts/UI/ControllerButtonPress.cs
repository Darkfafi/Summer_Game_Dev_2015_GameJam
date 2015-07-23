using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerButtonPress : MonoBehaviour {
    public bool isEnd = false;
    public DualStartGameScript dualStartGameScript;
    public EndSceneManager endSceneManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        if (!isEnd)
        {
            if (Input.GetButtonDown("Submit"))
            {
                dualStartGameScript.PressStartToContinue(this.gameObject.GetComponent<Button>());
            }
        }
        else
        {
            if (Input.GetButtonDown("Submit"))
            {
                endSceneManager.PressStartToContinue(this.gameObject.GetComponent<Button>());
            }
        }
    }
}
