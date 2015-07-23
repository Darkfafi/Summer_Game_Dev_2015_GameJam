using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraSeeTriggerObject : MonoBehaviour { 

	public delegate void NonGivingDelegate();
	public delegate void CameraGivenDelegate(Camera curCam,GameObject objectSeen);

	public event CameraGivenDelegate OnCameraEnter;
	public event CameraGivenDelegate OnCameraStay;
	public event CameraGivenDelegate OnCameraExit;

	private List<Camera> _camerasThatSeeObject = new List<Camera>(){};

	public void CheckSeenByCamera(Camera cam) {
		if (RendererExtensions.IsVisibleFrom (GetComponent<Renderer> (), cam, GetComponent<Collider> () )) {
			if (!CheckSeenByCam (cam)) {
				StartSeen(cam);
			}
			Seen(cam);

		} else {
			if(CheckSeenByCam(cam)){
				StopSeen(cam);
			}
		}
	}

	void StartSeen(Camera cam){
		if(cam.GetComponent<CameraObjectScanner>() != null){
			cam.GetComponent<CameraObjectScanner>().StartSeeingObject(this.gameObject);
		}
		_camerasThatSeeObject.Add(cam);
		if(OnCameraEnter != null){
			OnCameraEnter (cam,this.gameObject);
		}
	}

	void Seen(Camera cam){
		if(cam.GetComponent<CameraObjectScanner>() != null){
			cam.GetComponent<CameraObjectScanner>().SeeingObject(this.gameObject);
		}
		if (OnCameraStay != null) {
			OnCameraStay (cam,this.gameObject);
		}
	}

	void StopSeen(Camera cam){
		if (cam.GetComponent<CameraObjectScanner> () != null) {
			cam.GetComponent<CameraObjectScanner>().StopSeeingObject(this.gameObject);
		}
		_camerasThatSeeObject.Remove(cam);
		if(OnCameraExit != null){
			OnCameraExit (cam,this.gameObject);
		}
	}

	bool CheckSeenByCam(Camera cam){
		bool result = false;
		for (int i = _camerasThatSeeObject.Count - 1; i >= 0; i--) {
			if(cam == _camerasThatSeeObject[i]){
				result = true;
				break;
			}
			//Debug.Log(i);
		}
		return result;
	}
}
