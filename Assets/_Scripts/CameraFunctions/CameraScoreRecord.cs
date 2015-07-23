using UnityEngine;
using System.Collections;

public class CameraScoreRecord : MonoBehaviour {

	private bool _recording = false;

	void ToggleRecording(){
		_recording = !_recording;
	}

	public bool recording{
		get{return _recording;}
	}
}
