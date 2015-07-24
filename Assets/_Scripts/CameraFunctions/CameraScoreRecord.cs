using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScoreRecord : MonoBehaviour {

	private bool _settingScore = false;
	private bool _recording = true;

	private ObjectiveList objList;

	void Start(){
		objList = GameObject.Find ("GameController").GetComponent<ObjectiveList>();
		objList.ObjectiveFinished += ObjectiveFinished;
	}

	void ToggleRecording(){
		_recording = !_recording;
	}

	public bool recording{
		get{return _recording;}
	}

	public void Record(List<ObjectViewInfo> objInfo){
		if (_recording) {
			//GameObject.Find("Score").GetComponent<Score>().AddScore(objInfo.coverData);
			objList.FilmingObject(objInfo);
		}
	}

	void ObjectiveFinished(float score){
		//if(!_settingScore){
		//	_settingScore = true;
		SetScore(score);
		//}
	}

	public void StopSeeingObject(ObjectViewInfo objInfo){
		objList.StoppedFilmingObject (objInfo);
	}

	void SetScore(float score){
		//yield return new WaitForSeconds (_waitForScoreInSeconds);

		Score.Instance.PushTotalScore(score);
		//_settingScore = false;
	}


}
