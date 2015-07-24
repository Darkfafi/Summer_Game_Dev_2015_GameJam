using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DirectorCameraManager : MonoBehaviour {
	public List<Camera> DirectorCameraList;
	private int m_iActiveCameraIndex = 0;
    public List<Image> CamTabs;
    public List<Text> CamTabLabels;

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
		if (Input.GetButtonDown("SwitchCamera")) {
			SwitchCamera();
			//Score.Instance.AddScore(100);
		}
		/*
		if (Input.GetButtonDown("Fire1")) {
			//Score.Instance.PushTotalScore(100);
		}*/
	}

	private void SwitchCamera ()
	{
		//disable old camera
		DirectorCameraList[m_iActiveCameraIndex].gameObject.SetActive(false);
        //SetOldcolors back
        CamTabs[m_iActiveCameraIndex].color = new Color(51f / 256f, 72f / 256f, 51f / 256f);
        CamTabLabels[m_iActiveCameraIndex].color = new Color(153f / 256f, 153f / 256f, 153f / 256f);

		//get coorect index of new camera
		if (m_iActiveCameraIndex == DirectorCameraList.Count - 1) {
			m_iActiveCameraIndex = 0;
		}
		else {
			m_iActiveCameraIndex++;
		}
		//enable new camera
		DirectorCameraList[m_iActiveCameraIndex].gameObject.SetActive(true);
        //setnewColors
        CamTabs[m_iActiveCameraIndex].color = new Color(85f / 256f, 85f / 256f, 85f / 256f);
        CamTabLabels[m_iActiveCameraIndex].color = new Color(251f / 256f, 96f / 256f, 37f / 256f);

	}
}
