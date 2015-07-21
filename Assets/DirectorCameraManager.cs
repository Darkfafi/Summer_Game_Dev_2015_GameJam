using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectorCameraManager : MonoBehaviour {
	public List<Camera> DirectorCameraList;
	private int m_iActiveCameraIndex = 0;

	// Use this for initialization
	void Start () 
	{
		//making sure only one camera is enabled
		foreach (Camera c in DirectorCameraList) 
		{
			c.gameObject.SetActive(false);
			c.enabled = true;
			Destroy(c.GetComponent<AudioListener>());
		}
		DirectorCameraList[0].gameObject.SetActive(true);
		m_iActiveCameraIndex = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButtonDown("Fire1")) {
			SwitchCamera();
		}
	}

	private void SwitchCamera ()
	{
		//disable old camera
		DirectorCameraList[m_iActiveCameraIndex].gameObject.SetActive(false);
		//get coorect index of new camera
		if (m_iActiveCameraIndex == DirectorCameraList.Count - 1) {
			m_iActiveCameraIndex = 0;
		}
		else {
			m_iActiveCameraIndex++;
		}
		//enable new camera
		DirectorCameraList[m_iActiveCameraIndex].gameObject.SetActive(true);
	}
}
