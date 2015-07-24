using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveList : MonoBehaviour {

	private float _waitForScoreInSeconds = 0.5f; 
	private bool _filmDelaying = false;

	public delegate void ScoreContainingDelegate(float score);
	public event ScoreContainingDelegate ObjectiveFinished;
    public EndGameScript endGameScript;

	private List<Objective> _objectivesList = new List<Objective>(){};
	private GameObject[] _listOfFilmableObjects;
	private List<GameObject> _allNonObjectivesInScreen = new List<GameObject>(){};
	
	void Start(){

		_listOfFilmableObjects = GameObject.FindGameObjectsWithTag("FilmAble"); // if not an objective give a default score of 50 points.
		/*
		for (int i = _listOfFilmableObjects.Length - 1; i >= 0; i--) {
			CameraSeeTriggerObject objectToFilm = _listOfFilmableObjects[i].GetComponent<CameraSeeTriggerObject>();

			objectToFilm.OnCameraEnter += StartedFilmingObject;
			objectToFilm.OnCameraStay += FilmingObject;
			objectToFilm.OnCameraExit += StoppedFilmingObject;


		}*/
		//CreateObjective ("DomTower",100,5);
        CreateObjective("NoordBrug", 100, 10);
        CreateObjective("HoofdBrug", 100, 10);
        CreateObjective("ZuidBrug", 100, 10);
        CreateObjective("StadHuis", 100, 10);
        CreateObjective("LoempiaKraam", 100, 10);
        CreateObjective("DomToren", 100, 10);
        CreateObjective("WinkelVanSinkel", 150, 10);
	}

	public void FilmingObject(List<ObjectViewInfo> objectInfo){
		if (!_filmDelaying) {
			for(int i = objectInfo.Count - 1; i >= 0; i--){
				//Debug.Log(objectInfo[i].gObject);
				if(objectInfo[i].gObject.tag == "FilmAble"){
					if(!_filmDelaying){
						_filmDelaying = true;
					}
					StartCoroutine ("FilmingObjectDelayed", objectInfo[i]);
				}
			}
		}

	}

	IEnumerator FilmingObjectDelayed(ObjectViewInfo objectInfo){
		yield return new WaitForSeconds (_waitForScoreInSeconds);
		_filmDelaying = false;

		if (GetObjectiveByName (objectInfo.gObject.name) != null && !GetObjectiveByName(objectInfo.gObject.name).completed) { //Als het een objective is en niet gecomplete
			Objective curObjective = GetObjectiveByName (objectInfo.gObject.name);

			curObjective.AddFilmObjectTime (_waitForScoreInSeconds);
			if (!curObjective.completed) {
				float scoreObject = (curObjective.baseScore * objectInfo.coverData + (_allNonObjectivesInScreen.Count * 20));
				Score.Instance.AddScore(scoreObject);
				curObjective.AddScoreObject (Score.Instance.ConvertScore(scoreObject));
			} else if (curObjective.currentScore != 0) { 
				if(ObjectiveFinished != null){
					ObjectiveFinished(curObjective.currentScore);
					if(AllObjectivesComplete()){
						Invoke("ShowEndScreen",3);
					}
				}
				curObjective.ResetCurrentScore ();
			}
		} else { //else if not objective or objective is complete
			if(!_allNonObjectivesInScreen.Contains(objectInfo.gObject)){
				_allNonObjectivesInScreen.Add(objectInfo.gObject);
			}
		}
	}

	bool AllObjectivesComplete(){
		bool result = false;
		int counter = 0;
		for (int i = _objectivesList.Count - 1; i >= 0; i--) {
			if(_objectivesList[i].completed){
				counter ++;
			}
		}
		if (counter == _objectivesList.Count) {
			result = true;
		}

		return result;
	}

	void ShowEndScreen(){
        int finalScore = Mathf.RoundToInt(Score.Instance.GetScore);
        endGameScript.GameOver(finalScore);
	}

	public void StoppedFilmingObject(ObjectViewInfo objectInfo){
		if (GetObjectiveByName (objectInfo.gObject.name) != null && !GetObjectiveByName(objectInfo.gObject.name).completed) {
			Objective curObjective = GetObjectiveByName (objectInfo.gObject.name);
			//if (!curObjective.completed) {
			curObjective.ResetFilmObjective ();
			StopCoroutine("FilmingObjectDelayed");
			_filmDelaying = false;
			//}
		} else {
			if(_allNonObjectivesInScreen.Contains(objectInfo.gObject)){
				_allNonObjectivesInScreen.Remove(objectInfo.gObject);
			}
		}
	}

	Objective GetObjectiveByName(string name){
		Objective result = null;
		for (int i = _objectivesList.Count - 1; i >= 0; i--) {
			if(_objectivesList[i].name == name){
				result = _objectivesList[i];
			}
		}
		return result;
	}

	public void ResetAllObjectives(bool alsoCompletedObjectives = false){
		StopCoroutine("FilmingObjectDelayed");
		_filmDelaying = false;
		for (int i = _listOfFilmableObjects.Length - 1; i >= 0; i--) {
			Debug.Log(_listOfFilmableObjects[i]);
			if(GetObjectiveByName(_listOfFilmableObjects[i].name) != null){
				if(GetObjectiveByName(_listOfFilmableObjects[i].name).completed == false || alsoCompletedObjectives){
					GetObjectiveByName(_listOfFilmableObjects[i].name).ResetFilmObjective();
				}
			}
		}
	}

	void CreateObjective(string name, float baseScore, float timeToFilmInSeconds){
		//GameObject objectiveUI = Resources.Load("ObjectiveTabs/Prefabs/Tab") as GameObject;
		Objective objective = new Objective (name, baseScore, timeToFilmInSeconds, this);
		//objectiveUI.transform.SetParent(GameObject.Find ("Objectives").transform);
	}
	
	// list modifications 

	public void AddedObjective(Objective objective){
		_objectivesList.Add (objective);
	}

	void RemoveObjective(string nameGameobject){
		for (int i = _objectivesList.Count - 1; i >= 0; i--) {
			if(_objectivesList[i].name == nameGameobject){
				_objectivesList.Remove(_objectivesList[i]);
				break;
			}
		}
	}
}
