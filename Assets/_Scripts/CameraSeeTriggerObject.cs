using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraSeeTriggerObject : MonoBehaviour { 

	public delegate void NonGivingDelegate();
	public delegate void CameraGivenDelegate(Camera curCam);

	public event CameraGivenDelegate OnCameraEnter;
	public event CameraGivenDelegate OnCameraStay;
	public event CameraGivenDelegate OnCameraExit;

	private List<Camera> _camerasThatSeeObject = new List<Camera>(){};

	public void CheckSeenByCamera(Camera cam) {
		if (RendererExtensions.IsVisibleFrom (GetComponent<Renderer> (), cam)) {
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
		_camerasThatSeeObject.Add(cam);
		if(OnCameraEnter != null){
			OnCameraEnter (cam);
		}
	}

	void Seen(Camera cam){
		if (OnCameraStay != null) {
			OnCameraStay (cam);
		}
	}

	void StopSeen(Camera cam){
		_camerasThatSeeObject.Remove(cam);
		if(OnCameraExit != null){
			OnCameraExit (cam);
		}
	}

	bool CheckSeenByCam(Camera cam){
		bool result = false;
		for (int i = _camerasThatSeeObject.Count - 1; i >= 0; i--) {
			if(cam == _camerasThatSeeObject[i]){
				result = true;
				break;
			}
		}
		return result;
	}
}
