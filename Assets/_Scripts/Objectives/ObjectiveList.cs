﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveList : MonoBehaviour {

	private float _waitForScoreInSeconds = 0.5f; 
	private bool _filmDelaying = false;

	public delegate void ScoreContainingDelegate(float score);
	public event ScoreContainingDelegate ObjectiveFinished;

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
		ScoreMultiplier.Instance.AddMultiplier (Multiplier.x2);
		CreateObjective ("DomTower",100,5);
		CreateObjective ("Tree", 50, 10);
	}


	void StartedFilmingObject(Camera cam, GameObject gObject){

	}

	public void FilmingObject(ObjectViewInfo objectInfo){
		if (!_filmDelaying) {
			StartCoroutine ("FilmingObjectDelayed", objectInfo);
			_filmDelaying = true;
		}
	}

	IEnumerator FilmingObjectDelayed(ObjectViewInfo objectInfo){
		yield return new WaitForSeconds (_waitForScoreInSeconds);
		_filmDelaying = false;

		if (GetObjectiveByName (objectInfo.gObject.name) != null) {
			Objective curObjective = GetObjectiveByName (objectInfo.gObject.name);

			curObjective.AddFilmObjectTime (_waitForScoreInSeconds); // --> 1 second added per second <--- old

			if (!curObjective.completed) {
				float scoreObject = curObjective.baseScore + (_allNonObjectivesInScreen.Count * 50);
				curObjective.AddScoreObject (Score.Instance.ConvertScore(scoreObject)); // TODO goede score in doen dat berekend is.
				Score.Instance.AddScore(scoreObject);

			} else if (curObjective.currentScore != 0) {
				//end objective event
				//Score.Instance.AddScore (curObjective.currentScore);
				if(ObjectiveFinished != null){
					ObjectiveFinished(curObjective.currentScore); // in currentScore moet de score staan die berekend is aan de hand van alle multi's en all.
				}
				curObjective.ResetCurrentScore ();
			}
		} else {
			if(!_allNonObjectivesInScreen.Contains(objectInfo.gObject)){
				_allNonObjectivesInScreen.Add(objectInfo.gObject);
			}
		}
	}

	public void StoppedFilmingObject(ObjectViewInfo objectInfo){
		if (GetObjectiveByName (objectInfo.gObject.name) != null) {
			Objective curObjective = GetObjectiveByName (objectInfo.gObject.name);
			if (!curObjective.completed) {
				curObjective.ResetFilmObjective ();
				StopCoroutine("FilmingObjectDelayed");
				_filmDelaying = false;
			}
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

	void CreateObjective(string name, float baseScore, float timeToFilmInSeconds){
		Objective objective = new Objective (name, baseScore, timeToFilmInSeconds, this);
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
