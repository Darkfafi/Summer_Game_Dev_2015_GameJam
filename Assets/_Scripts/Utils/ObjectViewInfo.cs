using UnityEngine;
using System.Collections;

public class ObjectViewInfo {

	private GameObject _gObject;
	private float _widthObject;
	private float _heightObject;
	private float _distanceObject;
	private Vector2 _pivot;
	private float _percentageInViewObject;
	private float _coverData = 0;


	public ObjectViewInfo(GameObject gameObject, float distance, float percentageInView,float widthObj, float heightObj,Vector2 pivotPosition){
		_gObject = gameObject;
		_widthObject = widthObj;
		_heightObject = heightObj;

		_pivot = pivotPosition; //new Vector2(widthObject / 2, heightObject / 2); // TODO Pivot moet ook positie op scherm krijgen.

		_distanceObject = distance;
		_percentageInViewObject = percentageInView;
	}

	public float coverData{
		get{return _coverData;}
		set{_coverData = value;}
	}

	public Vector2 pivot{
		get{return _pivot;}
	}

	public GameObject gObject{
		get{return _gObject;}
		//set{_gObject = value;}
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
	//	set{_distanceObject = value;}
	}
	public float percentageInViewObject{
		get{return _percentageInViewObject;}
	//	set{_percentageInViewObject = value;}
	}
}
