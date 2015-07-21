using UnityEngine;
using System.Collections;

public class ObjectViewInfo {

	private GameObject _gObject;
	private float _widthObject;
	private float _heightObject;
	private float _distanceObject;
	private float _percentageInViewObject;

	public ObjectViewInfo(GameObject gameObject, float distance, float percentageInView){
		_gObject = gameObject;
		_widthObject = _gObject.GetComponent<Renderer> ().bounds.size.x;
		_heightObject = _gObject.GetComponent<Renderer> ().bounds.size.y;

		_distanceObject = distance;
		_percentageInViewObject = percentageInView;
	}

	public GameObject gObject{
		get{return _gObject;}
		set{_gObject = value;}
	}

	public float widthObject{
		get{return _widthObject;}
		set{_widthObject = value;}
	}
	public float heightObject{
		get{return _heightObject;}
		set{_heightObject = value;}
	}
	public float distanceObject{
		get{return _distanceObject;}
		set{_distanceObject = value;}
	}
	public float percentageInViewObject{
		get{return _percentageInViewObject;}
		set{_percentageInViewObject = value;}
	}
}
