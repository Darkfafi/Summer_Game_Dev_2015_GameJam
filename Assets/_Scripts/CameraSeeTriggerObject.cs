using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraSeeTriggerObject : MonoBehaviour { 

	public delegate void NonGivingDelegate();
	public delegate void CameraGivenDelegate(Camera curCam);

	public event CameraGivenDelegate OnCameraEnter;
	public event CameraGivenDelegate OnCameraStay;
	public event CameraGivenDelegate OnCameraExit;

	private bool _checkObjectNow = false;

	private List<Camera> _camerasThatSeeObject = new List<Camera>(){};

	public void CheckSeenByCamera(Camera cam) {
		if (RendererExtensions.IsVisibleFrom (GetComponent<Renderer> (), cam)) {
			if (!CheckSeenByCam (cam)) {
				_camerasThatSeeObject.Add(cam);
				if(OnCameraEnter != null){
					OnCameraEnter (cam);
				}
			}
			if (OnCameraStay != null) {
				OnCameraStay (cam);
			}
		} else {
			if(CheckSeenByCam(cam)){
				_camerasThatSeeObject.Remove(cam);
				if(OnCameraExit != null){
					OnCameraExit (cam);
				}
			}
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
