using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraObjectScanner : MonoBehaviour {

	public static int NON_CONTAINTING = 1236664321;

	Camera thisCamera;
	CameraScoreRecord recorder;


	//List<GameObject> _allVisableObjects = new List<GameObject>(){};
	List<ObjectViewInfo> _objectsViewInfoList = new List<ObjectViewInfo>(){};

	void Start(){
		thisCamera = GetComponent<Camera> ();
		
		if (GetComponent<CameraScoreRecord> () != null) {
			recorder = GetComponent<CameraScoreRecord>();
		}
	}

	void Update(){
		if(recorder != null && recorder.recording){
			for(int i = _objectsViewInfoList.Count - 1; i >= 0; i--){
				recorder.Record(_objectsViewInfoList[i]);
			}
		}
	}

	public void StartSeeingObject(GameObject obj){
		//_allVisableObjects.Add (obj);


	}

	public void SeeingObject(GameObject obj){
		if (recorder == null || recorder.recording) {

			float distance = Vector3.Distance (obj.transform.position, transform.position); 
			Vector3 rightestPoint;
			Vector3 highestPoint;
			float width = RendererExtensions.GetPerspectiveWidth (obj.GetComponent<Collider> (), thisCamera, out rightestPoint);
			float height = RendererExtensions.GetPerspectiveHeigth (obj.GetComponent<Collider> (), thisCamera, out highestPoint);
			Vector2 center = RendererExtensions.GetPerspectiveCenterOfObject (highestPoint, rightestPoint, width, height);
		
			ObjectViewInfo objInfo = new ObjectViewInfo (obj, distance, GetObjectBoundInViewPercentage (obj), width, height, center); // TODO Give screen perspective width and height! 


			int indexObject = getIndexInfoListObj (obj);
			if (indexObject != NON_CONTAINTING) {
				_objectsViewInfoList [indexObject] = objInfo;
			} else {
				_objectsViewInfoList.Add (objInfo);
			}
			objInfo.coverData = GetAllVisibleObjectsSurfacesByOverlap (_objectsViewInfoList) [obj];
			//Debug.Log (objInfo.pivot + " w, " + objInfo.widthObject + " h, " + objInfo.heightObject + " %, " + objInfo.percentageInViewObject + " ||, " + GetAllVisibleObjectsSurfacesByOverlap (_objectsViewInfoList) [obj]);
		}
	}

	public void StopSeeingObject(GameObject obj){
		//_allVisableObjects.Remove (obj);
		if (recorder == null || recorder.recording) {
			recorder.StopSeeingObject(_objectsViewInfoList[getIndexInfoListObj(obj)]);
		}
		if (getIndexInfoListObj (obj) != NON_CONTAINTING) {
			_objectsViewInfoList.RemoveAt (getIndexInfoListObj (obj));
		}
	}

	int getIndexInfoListObj(GameObject obj){
		int result = NON_CONTAINTING;
		for (int i =  _objectsViewInfoList.Count - 1; i >= 0; i--) {
			if(_objectsViewInfoList[i].gObject == obj){
				result = i;
				break;
			}
		}
		return result;
	}

	float GetObjectBoundInViewPercentage(GameObject otherObj){

		float distance = Vector3.Distance(otherObj.transform.position, transform.position);  // afstand tussen jou en het object

		float frustumHeight = 2 * distance * Mathf.Tan(thisCamera.fieldOfView * 0.5f * Mathf.Deg2Rad); // hoogte van je view

		float bottomFrustumHeight =  transform.position.y - (frustumHeight / 2) + (Mathf.Tan(Mathf.Deg2Rad * (360 - transform.eulerAngles.x)) * distance); // bodem van je view tot het object

		float percentageInView = (otherObj.transform.position.y + 0.5f) + (otherObj.GetComponent<Renderer> ().bounds.size.y + 0.5f) / (Mathf.Abs(bottomFrustumHeight) + (frustumHeight)); 

	//	Debug.Log (otherObj.GetComponent<Renderer>().bounds.size.y + " " +bottomFrustumHeight + " ,% =  " + percentageInView);
		return percentageInView;
	}

	Dictionary<GameObject,float> GetAllVisibleObjectsSurfacesByOverlap(List<ObjectViewInfo> objectViewList){
		Dictionary<GameObject,float> allSurfaces = new Dictionary<GameObject, float> (); // gameobject == the item, float == surface float.
		List<ObjectViewInfo> sortedOnDistanceList = new List<ObjectViewInfo> (){};


		Vector2 overLapSurface = new Vector2 ();

		//sorts the list on closest object first then going to farest
		for(int i = 0; i < objectViewList.Count;i++){
			if(sortedOnDistanceList.Count == 0){
				sortedOnDistanceList.Add(objectViewList[i]);
			}else{
				bool placed = false;
				for(int j = 0; j < sortedOnDistanceList.Count; j++){
					if(sortedOnDistanceList[j].distanceObject > objectViewList[i].distanceObject){
						sortedOnDistanceList.Insert(j + 1,objectViewList[i]);
						placed = true;
						break;
					}
				}

				if(!placed){
					sortedOnDistanceList.Add(objectViewList[i]);
				}
			}
		}

		for (int i = 0; i < sortedOnDistanceList.Count; i++) {

			overLapSurface = new Vector2();

			if(i == 0){
				allSurfaces.Add(sortedOnDistanceList[i].gObject,sortedOnDistanceList[i].widthObject * sortedOnDistanceList[i].heightObject);
			}else{
				ObjectViewInfo obj1;
				ObjectViewInfo obj2;

				obj1 = sortedOnDistanceList[i];

				for(int j = 0; j < i; j++){
					obj2 = sortedOnDistanceList[j];
					if((obj1.pivot.x + obj1.widthObject / 2 > obj2.pivot.x - obj2.widthObject / 2 || obj1.pivot.x - obj1.widthObject / 2 < obj2.pivot.x + obj2.widthObject / 2)
					   && (obj1.pivot.y + obj1.heightObject / 2 > obj2.pivot.y - obj2.heightObject / 2 || obj2.pivot.y + obj2.heightObject / 2 > obj1.pivot.y - obj1.heightObject / 2)){

						if(obj1.pivot.x + obj1.widthObject / 2 > obj2.pivot.x - obj2.widthObject / 2 && 
						   obj1.pivot.x + obj1.widthObject / 2 < obj2.pivot.x + obj2.widthObject / 2){

							overLapSurface.x += Mathf.Abs((obj2.pivot.x - obj2.widthObject / 2) - (obj1.pivot.x + obj1.widthObject / 2));
					
						}else if(obj1.pivot.x - obj1.widthObject / 2 < obj2.pivot.x + obj2.widthObject / 2 && 
						         obj1.pivot.x - obj1.widthObject / 2 > obj2.pivot.x - obj2.widthObject / 2){
							overLapSurface.x += Mathf.Abs((obj1.pivot.x - obj1.widthObject / 2) - (obj2.pivot.x + obj2.widthObject / 2));

						}

						if(overLapSurface.x > obj1.widthObject){
							overLapSurface.x = obj1.widthObject;
						}

						//Debug.Log((obj1.pivot.y + obj1.heightObject / 2) +" "+ (obj2.pivot.y - obj2.heightObject / 2));
						if(obj1.pivot.y + obj1.heightObject / 2 > obj2.pivot.y - obj2.heightObject / 2 &&
						   obj1.pivot.y + obj1.heightObject / 2 < obj2.pivot.y + obj2.heightObject / 2){
							overLapSurface.y += Mathf.Abs((obj2.pivot.y - obj2.heightObject / 2) - (obj1.pivot.y + obj1.heightObject / 2));

						}else if(obj1.pivot.y - obj1.heightObject / 2 < obj2.pivot.y + obj2.heightObject / 2 && 
						         obj1.pivot.y - obj1.heightObject / 2 > obj2.pivot.y - obj2.heightObject / 2){
							overLapSurface.y += Mathf.Abs((obj1.pivot.y - obj1.heightObject / 2) - (obj2.pivot.y + obj2.heightObject / 2));
						}

						if(overLapSurface.y > obj1.heightObject){
							overLapSurface.y = obj1.heightObject;
						}
					}
				}

				float surfaceBlockedObj = overLapSurface.x * overLapSurface.y;
				float surfaceObj = obj1.widthObject * obj1.heightObject;
				float surfaceObjVisable = surfaceObj - surfaceBlockedObj;
				allSurfaces.Add(obj1.gObject, surfaceObjVisable);
			}
		} 
		return allSurfaces;
	}
}
