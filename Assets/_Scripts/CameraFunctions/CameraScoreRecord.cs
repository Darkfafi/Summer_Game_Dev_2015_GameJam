using UnityEngine;
using System.Collections;

public class CameraScoreRecord : MonoBehaviour {

	private bool _settingScore = false;
	private bool _recording = false;
	private float _waitForScoreInSeconds = 0.5f; 

	void ToggleRecording(){
		_recording = !_recording;
	}

	public bool recording{
		get{return _recording;}
	}

	public void Record(ObjectViewInfo objInfo){
		if (_recording) {
			//GameObject.Find("Score").GetComponent<Score>().AddScore(objInfo.coverData);
			if(!_settingScore){
				_settingScore = true;
				StartCoroutine("SetScore",objInfo);
			}
				
		}
	}

	IEnumerator SetScore(ObjectViewInfo objInfo){
		yield return new WaitForSeconds (_waitForScoreInSeconds);

		Score.Instance.AddScore(objInfo.coverData);
		_settingScore = false;
	}


}
