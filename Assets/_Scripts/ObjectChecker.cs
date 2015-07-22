using UnityEngine;
using System.Collections;

public class ObjectChecker : MonoBehaviour {

	CameraSeeTriggerObject[] _allSeeableGameobjects;

	// Use this for initialization
	void Start () {
		//GameObject.Find ("Cube").GetComponent<CameraSeeTriggerObject> ().OnCameraEnter += DoEnter; //This is for debugging! 
		//GameObject.Find ("Cube").GetComponent<CameraSeeTriggerObject> ().OnCameraStay += DoStay; //This is for debugging! 
		//GameObject.Find ("Cube").GetComponent<CameraSeeTriggerObject> ().OnCameraExit += DoExit; //This is for debugging!

		_allSeeableGameobjects = GameObject.FindObjectsOfType<CameraSeeTriggerObject> ();



	}
	
	// Update is called once per frame
	void Update () {
		//GameObject.Find ("Cube").GetComponent<CameraSeeTriggerObject> () //This is for debugging! 
		for (int i = 0; i < _allSeeableGameobjects.Length; i++) {
			_allSeeableGameobjects[i].CheckSeenByCamera(GameObject.Find("CameraManCamera").GetComponent<Camera>());
		}

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
