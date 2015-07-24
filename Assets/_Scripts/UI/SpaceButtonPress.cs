using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpaceButtonPress : MonoBehaviour {
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
			if (Input.GetButtonDown("SwitchCamera"))
			{
				dualStartGameScript.PressStartToContinue(this.gameObject.GetComponent<Button>());
			}
		}
		else
		{
			if (Input.GetButtonDown("SwitchCamera"))
			{
				endSceneManager.PressStartToContinue(this.gameObject.GetComponent<Button>());
			}
		}
	}
}
