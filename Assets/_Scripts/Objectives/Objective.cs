using UnityEngine;
using System.Collections;

public class Objective {


	private string _nameObjectToFilm;
	private float _baseScoreObject;
	private float _timeToFilmInSeconds;

	private bool _completed = false;
	private float _currentFilmTime = 0;
	private float _currentScore = 0;

	public Objective(string nameGameobject, float baseScore, float timeToFilmInSeconds, ObjectiveList list){
		_nameObjectToFilm = nameGameobject;
		_baseScoreObject = baseScore;
		_timeToFilmInSeconds = timeToFilmInSeconds;
		list.AddedObjective (this);
	}

	public float currentScore{
		get{return _currentScore;}
	}

	public string name{
		get{return _nameObjectToFilm;}
	}

	public bool completed{
		get{return _completed;}
	}

	public float baseScore{
		get{return _baseScoreObject;}
	}

	public void AddScoreObject(float score){
		_currentScore += score;
	}
	public void AddFilmObjectTime(float time){
		_currentFilmTime += time;
		ObjectiveManager.Instance.ObjectiveUpdate (_nameObjectToFilm, _currentFilmTime / _timeToFilmInSeconds);
		if (_currentFilmTime >= _timeToFilmInSeconds) {
			_currentFilmTime = _timeToFilmInSeconds;
			_completed = true;
		}
	}
	public void ResetCurrentScore(){
		Score.Instance.RemoveScore (_currentScore);
		_currentScore = 0;
	}
	public void ResetFilmObjective(){
		_currentFilmTime = 0;
		ResetCurrentScore ();
		_completed = false;
	}
}
