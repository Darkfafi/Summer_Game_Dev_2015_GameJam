using UnityEngine;
using System.Collections;

public class RendererExtensions : MonoBehaviour {

	public static bool IsVisibleFrom(Renderer renderer, Camera camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		Debug.Log (planes);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}
}
