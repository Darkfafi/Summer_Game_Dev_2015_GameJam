using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveList : MonoBehaviour {

	private List<Objective> _objectivesList = new List<Objective>(){};
	private GameObject[] _listOfFilmableObjects;

	void Start(){

		_listOfFilmableObjects = GameObject.FindGameObjectsWithTag("FilmAble");

		for (int i = _listOfFilmableObjects.Length - 1; i >= 0; i--) {
			CameraSeeTriggerObject objectToFilm = _listOfFilmableObjects[i].GetComponent<CameraSeeTriggerObject>();

			objectToFilm.OnCameraEnter += StartedFilmingObject;
			objectToFilm.OnCameraStay += FilmingObject;
			objectToFilm.OnCameraExit += StoppedFilmingObject;


		}
		CreateObjective ("DomTower",100,5);
		CreateObjective ("Tree", 50, 10);
	}


	void StartedFilmingObject(Camera cam, GameObject gObject){

	}

	void FilmingObject(Camera cam, GameObject gObject){
		if (GetObjectiveByName (gObject.name) != null) {
			Objective curObjective = GetObjectiveByName (gObject.name);

			curObjective.AddFilmObjectTime (Time.deltaTime); // 1 second added per second

			if (!curObjective.completed) {
				curObjective.AddScoreObject(curObjective.baseScore); // TODO goede score in doen dat berekend is.
			}else{
				Score.Instance.AddScore (curObjective.currentScore);
			}
		}
	}

	void StoppedFilmingObject(Camera cam, GameObject gObject){
		if (GetObjectiveByName (gObject.name) != null) {
			Objective curObjective = GetObjectiveByName (gObject.name);
			if (!curObjective.completed) {
				curObjective.ResetFilmObjective();
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
