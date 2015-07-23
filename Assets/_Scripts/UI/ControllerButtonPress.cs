using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerButtonPress : MonoBehaviour {

    public DualStartGameScript dualStartGameScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            dualStartGameScript.PressStartToContinue(this.gameObject.GetComponent<Button>());
        }
    }
}
