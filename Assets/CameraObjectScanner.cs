using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraObjectScanner : MonoBehaviour {

	Camera thisCamera;

	List<GameObject> _allVisableObjects = new List<GameObject>(){};
	List<ObjectViewInfo> _objectsViewInfo = new List<ObjectViewInfo>(){};

	void Start(){
		thisCamera = GetComponent<Camera> ();
	}

	void Update(){
		GetObjectBoundInViewPercentage(GameObject.Find("Cube")); // For Debugging! Will later be called through object

	}

	public void StartSeeingObject(GameObject obj){
		_allVisableObjects.Add (obj);

		//ObjectViewInfo objInfo = new ObjectViewInfo(obj,)
		_objectsViewInfo.Add(objInfo)
	}

	public void SeeingObject(GameObject obj){
		
	}

	public void StopSeeingObject(GameObject obj){
		_allVisableObjects.Remove (obj);
	}

	void GetObjectBoundInViewPercentage(GameObject otherObj){

		float distance = Vector3.Distance(otherObj.transform.position, transform.position);  // afstand tussen jou en het object

		float frustumHeight = 2 * distance * Mathf.Tan(thisCamera.fieldOfView * 0.5f * Mathf.Deg2Rad); // hoogte van je view

		float bottomFrustumHeight =  transform.position.y - (frustumHeight / 2) + (Mathf.Tan(Mathf.Deg2Rad * (360 - transform.eulerAngles.x)) * distance); // bodem van je view tot het object

		float percentageInView = (otherObj.transform.position.y + 0.5f) + (otherObj.GetComponent<Renderer> ().bounds.size.y + 0.5f) / (Mathf.Abs(bottomFrustumHeight) + (frustumHeight)); 

		Debug.Log (otherObj.GetComponent<Renderer>().bounds.size.y + " " +frustumHeight + " ,% =  " + percentageInView);
	}
}
