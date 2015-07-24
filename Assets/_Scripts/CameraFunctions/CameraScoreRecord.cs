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
		if (!_recording) {
			objList.ResetAllObjectives();

		}
	}

	public bool recording{
		get{return _recording;}
	}
	/*
	void Update(){
		if (Input.GetButtonDown ("Submit") || Input.GetKeyDown (KeyCode.Space))
		{
			ToggleRecording();
		}
	}
	*/
	public void Record(List<ObjectViewInfo> objInfo){
		if (_recording) {
			//GameObject.Find("Score").GetComponent<Score>().AddScore(objInfo.coverData);
			objList.FilmingObject(objInfo);
			if (GameObject.Find ("Player").GetComponent<CharacterController> ().velocity.magnitude > 0) {
				if(ScoreMultiplier.Instance.Multipliers.Count < 1){
					ScoreMultiplier.Instance.AddMultiplier(Multiplier.x2);
				}
			}else{
				while(ScoreMultiplier.Instance.Multipliers.Count > 0){
					ScoreMultiplier.Instance.RemoveMultiplier(Multiplier.x2);
				}
			}
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
