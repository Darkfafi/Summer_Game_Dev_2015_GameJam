using UnityEngine;
using System.Collections;

public class DualScreenResolutionMod : MonoBehaviour {
	// Use this for initialization

	public Vector2 screenOneResolution = new Vector2();
	public Vector2 screenTwoResolution = new Vector2();

	public Camera cameraScreenOne;
	public Camera cameraScreenTwo;


	void Start () {
		Screen.SetResolution((int)(screenOneResolution.x +  screenTwoResolution.x), 1080,false);

		float fullScreenWidth = screenOneResolution.x + screenTwoResolution.x;

		float cameraOneScale;
		float cameraTwoScale;

		cameraOneScale = screenOneResolution.x / fullScreenWidth;
		cameraTwoScale = screenTwoResolution.x / fullScreenWidth;

		cameraScreenTwo.rect = new Rect (0,0,cameraOneScale,1);
		cameraScreenOne.rect = new Rect (cameraOneScale,0,cameraTwoScale,1);
	}
}
