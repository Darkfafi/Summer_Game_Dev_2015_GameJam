using UnityEngine;
using System.Collections;

public class CameraObjectScanner : MonoBehaviour {

	Camera thisCamera;

	void Start(){
		thisCamera = GetComponent<Camera> ();
	}

	void Update(){
		GetObjectBoundInViewPercentage(GameObject.Find("Cube"));
	}

	void GetObjectBoundInViewPercentage(GameObject otherObj){

		float distance = Vector3.Distance(otherObj.transform.position, transform.position);  // afstand tussen jou en het object

		float frustumHeight = 2 * distance * Mathf.Tan(thisCamera.fieldOfView * 0.5f * Mathf.Deg2Rad); // hoogte van je view

		float bottomFrustumHeight =  transform.position.y - (frustumHeight / 2) + (Mathf.Tan(Mathf.Deg2Rad * (360 - transform.eulerAngles.x)) * distance); // bodem van je view tot het object

		float percentageInView = (otherObj.transform.position.y + 0.5f) + (otherObj.GetComponent<Renderer> ().bounds.size.y + 0.5f) / (Mathf.Abs(bottomFrustumHeight) + (frustumHeight)); 

		Debug.Log (otherObj.GetComponent<Renderer>().bounds.size.y + " " +frustumHeight + " ,% =  " + percentageInView);
	}
}
