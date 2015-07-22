using UnityEngine;
using System.Collections;

public class ObjectChecker : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		this.GetComponent<CameraSeeTriggerObject> ().OnCameraEnter += DoEnter; //This is for debugging! 
		this.GetComponent<CameraSeeTriggerObject> ().OnCameraStay += DoStay; //This is for debugging! 
		this.GetComponent<CameraSeeTriggerObject> ().OnCameraExit += DoExit; //This is for debugging! 
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<CameraSeeTriggerObject> ().CheckSeenByCamera (GameObject.Find ("CameraManCamera").GetComponent<Camera>()); //This is for debugging! 


	}

	void DoEnter(Camera cam){
		Debug.Log ("Enter " + cam);
	}
	void DoStay(Camera cam){
		Debug.Log ("Stay " + cam);
	}

	void DoExit(Camera cam){
		Debug.Log ("Exit " + cam);
	}
}
