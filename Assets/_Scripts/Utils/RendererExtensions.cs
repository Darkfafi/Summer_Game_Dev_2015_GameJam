using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RendererExtensions : MonoBehaviour {

	public static bool IsVisibleFrom(Renderer renderer, Camera camera, Collider collider)
	{
        //Code for checking whether the object is rendered within the "Bounding box" of the camera

		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

		//Debug.Log (planes);
		//return 
        bool canBeVisible = GeometryUtility.TestPlanesAABB(planes, collider.bounds);
        
        if (!canBeVisible) return false;
        //Code for checking whether the objects corners are visible from the camera perspective. So not blocked by any other objects
        Bounds objBounds = collider.bounds;
        float adjustValue = 1;
        float xNeg = objBounds.center.x - (objBounds.extents.x * adjustValue);
        float xPos = objBounds.center.x + (objBounds.extents.x * adjustValue);
        float yNeg = objBounds.center.y - (objBounds.extents.y * adjustValue);
        float yPos = objBounds.center.y + (objBounds.extents.y * adjustValue); 
        float zNeg = objBounds.center.z - (objBounds.extents.z * adjustValue);
        float zPos = objBounds.center.z + (objBounds.extents.z * adjustValue);

        //List all different coordinates of the cube vertexes
        List<Vector3> cornerCoordinates = new List<Vector3>();
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 coor = new Vector3();
                    coor.x = (x == 0) ? xNeg : xPos;
                    coor.y = (y == 0) ? yNeg : yPos;
                    coor.z = (z == 0) ? zNeg : zPos;
                    cornerCoordinates.Add(coor);
                }
            }
        }

        //Cast a raycast from the camera to the vertex and see if it hits the actual object
        int hits = 0;


        foreach (Vector3 coor in cornerCoordinates)
        {
            Vector3 newCoor = coor;
            float frustatedModifier = 0.9f;
            //Adapt the coor to the field of view? approximately
            float distance = Vector3.Distance(objBounds.center, camera.transform.position);  // afstand tussen jou en het object
            float frustumHeight = 2 * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * frustatedModifier; // hoogte van je view
            float bottomFrustumHeight = camera.transform.position.y - (frustumHeight / 2) + (Mathf.Tan(Mathf.Deg2Rad * (360 - camera.transform.eulerAngles.x)) * distance); // bodem van je view tot het object
            float topFrustumHeight = frustumHeight + bottomFrustumHeight;

            float frustumWidth = frustumHeight * camera.aspect;
            //float leftFrustumWidth = camera.transform.position.y - (frustumWidth / 2) + (Mathf.Tan(Mathf.Deg2Rad * (360 - camera.transform.eulerAngles.x)) * distance);
            //float rightFrustumWidth = frustumWidth + leftFrustumWidth;

            //Debug.Log(topFrustumHeight);
            //Debug.Log(bottomFrustumHeight);
            
            if (coor.y == yPos)
            {
                if (coor.y >= topFrustumHeight)
                {
                    newCoor.y = topFrustumHeight;
                }
            }
            else if(coor.y == yNeg)
            {
                if (coor.y <= bottomFrustumHeight)
                {
                    newCoor.y = bottomFrustumHeight;
                }
            }

            //int layerMask = (int)(1 << 8);
            RaycastHit hitInfo = new RaycastHit();
            

            if (Physics.Raycast(camera.transform.position, newCoor - camera.transform.position, out hitInfo, 1000))
            {
                
                //Debug.Log(collider.gameObject);
                if (hitInfo.collider.gameObject.Equals(collider.gameObject))
                {
                    Debug.DrawRay(camera.transform.position, newCoor - camera.transform.position, Color.green, 0.5f);
                    //Debug.Log(hitInfo.collider.gameObject + " Coor: " + newCoor);
                    hits++;
                    //TODO remove debug this
                    Vector3 rightestPoint;
                    Vector3 highestPoint;
                    float width = GetPerspectiveWidth(collider, camera, out rightestPoint);
                    float height = GetPerspectiveHeigth(collider, camera, out highestPoint);
                    Vector2 center = GetPerspectiveCenterOfObject(highestPoint, rightestPoint, width, height);
                }
            }
        }
        //Debug.Log(hits);
        //For now, 1+ hits means it is visible
        return (hits > 0);
	}

    public static float GetPerspectiveWidth(Collider objectCollider, Camera camera, out Vector3 rightestViewPoint)
    {
        //First get the center middle points
        Bounds objBounds = objectCollider.bounds;
        float adjustValue = 1f;
        float xNeg = objBounds.center.x - (objBounds.extents.x * adjustValue);
        float xPos = objBounds.center.x + (objBounds.extents.x * adjustValue);

        float zNeg = objBounds.center.z - (objBounds.extents.z * adjustValue);
        float zPos = objBounds.center.z + (objBounds.extents.z * adjustValue);

        List<Vector3> cornerCoordinates = new List<Vector3>();
        for (int x = 0; x < 2; x++)
        {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 coor = new Vector3();
                    coor.x = (x == 0) ? xNeg : xPos;
                    coor.y = objBounds.center.y;
                    coor.z = (z == 0) ? zNeg : zPos;
                    cornerCoordinates.Add(coor);
                }
        }

        Vector3 mostLeft = camera.WorldToViewportPoint(cornerCoordinates[0]);
        Vector3 mostRight = camera.WorldToViewportPoint(cornerCoordinates[0]);

        Vector3 originalLeft = cornerCoordinates[0];
        Vector3 originalRight = cornerCoordinates[0];

        //Debug.Log("Center: " + mostLeft);

        foreach (Vector3 coor in cornerCoordinates)
        {

            Vector3 screenCoor = camera.WorldToViewportPoint(coor);
            if (screenCoor.x <= mostLeft.x)
            {
                mostLeft = camera.WorldToViewportPoint(coor);
                originalLeft = coor;
            }
            else if (screenCoor.x >= mostRight.x)
            {
                mostRight = camera.WorldToViewportPoint(coor);
                originalRight = coor;
            }
            //Debug.Log(screenCoor);
        }
        
        //Fix for weird thing with only seeing one plane (ask me, or not. Rather dont actually)
        Vector3 firstClosest = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 secondClosest = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 thirdClosest = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        if (originalLeft.x == originalRight.x || originalRight.z == originalLeft.z)
        {
            
            foreach (Vector3 coor in cornerCoordinates)
            {
                if (Vector3.Distance(camera.transform.position, thirdClosest) >= Vector3.Distance(camera.transform.position, coor))
                {
                    thirdClosest = coor;
                    if (Vector3.Distance(camera.transform.position, secondClosest) >= Vector3.Distance(camera.transform.position, thirdClosest))
                    {
                        Vector3 placeholder = secondClosest;
                        secondClosest = thirdClosest;
                        thirdClosest = placeholder;
                        if (Vector3.Distance(camera.transform.position, firstClosest) >= Vector3.Distance(camera.transform.position, secondClosest))
                        {
                            Vector3 placeholder2 = firstClosest;
                            firstClosest = secondClosest;
                            secondClosest = placeholder2;
                        }
                    }
                }
            }

            //Vector3

            if (Mathf.Abs(objBounds.center.x - camera.transform.position.x) <= Mathf.Abs(objBounds.center.x - firstClosest.x))
            {
                if (firstClosest.z == thirdClosest.z)
                {
                    secondClosest = thirdClosest;
                }
            }
            else
            {
                //dan is ie in de z categorie
                if (firstClosest.x == thirdClosest.x)
                {
                    secondClosest = thirdClosest;
                }
            }


            if (camera.WorldToViewportPoint(firstClosest).x >= camera.WorldToViewportPoint(secondClosest).x)
            {
                originalLeft = secondClosest;
                originalRight = firstClosest;
            }
            else
            {
                originalLeft = firstClosest;
                originalRight = secondClosest;
            }

            mostLeft = camera.WorldToViewportPoint(originalLeft);
            mostRight = camera.WorldToViewportPoint(originalRight);

//            Debug.Log("weird");
        }
        //TODO fix bug with the viewpoint switching and the code below is not correct anymore somehow
        

        //Debug.Log(camera.ViewportToScreenPoint(mostRight));
        //Debug.Log("left" + camera.ViewportToScreenPoint(mostLeft));

        if (mostLeft.x <= 0)
        {
            mostLeft.x = 0;
        }
        else if (mostLeft.x >= 1)
        {
            mostLeft.x = 1;
        }

        if (mostRight.x <= 0)
        {
            mostRight.x = 0;
        }
        else if (mostRight.x >= 1)
        {
            mostRight.x = 1;
        }


        Debug.DrawLine(originalLeft, objBounds.center, Color.blue, 0.5f);
        Debug.DrawLine(originalRight, objBounds.center, Color.red, 0.5f);
        
        //Debug.Log(camera.pixelRect);
        
        float width = camera.ViewportToScreenPoint(mostRight).x - camera.ViewportToScreenPoint(mostLeft).x;
//        Debug.Log(width);
        rightestViewPoint = camera.ViewportToScreenPoint(mostRight);
        return width;  //Total width can be gotten with pixelwidth of camera.

        // also return x and y from center point of visible points!
        //maybe also inform of pixel rect
    }

    public static float GetPerspectiveHeigth(Collider objectCollider, Camera camera, out Vector3 highestViewPoint)
    {
        Bounds objBounds = objectCollider.bounds;

        float adjustValue = 1;
        float xNeg = objBounds.center.x - (objBounds.extents.x * adjustValue);
        float xPos = objBounds.center.x + (objBounds.extents.x * adjustValue);
        float yNeg = objBounds.center.y - (objBounds.extents.y * adjustValue);
        float yPos = objBounds.center.y + (objBounds.extents.y * adjustValue);
        float zNeg = objBounds.center.z - (objBounds.extents.z * adjustValue);
        float zPos = objBounds.center.z + (objBounds.extents.z * adjustValue);

        //List all different coordinates of the cube vertexes
        List<Vector3> cornerCoordinates = new List<Vector3>();
        List<Vector3> viewPortCornerCoordinates = new List<Vector3>();
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 coor = new Vector3();
                    coor.x = (x == 0) ? xNeg : xPos;
                    coor.y = (y == 0) ? yNeg : yPos;
                    coor.z = (z == 0) ? zNeg : zPos;
                    cornerCoordinates.Add(coor);
                    viewPortCornerCoordinates.Add(camera.WorldToViewportPoint(coor));
                }
            }
        }

        Vector3 highestPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 lowestPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        foreach (Vector3 vpCoor in viewPortCornerCoordinates)
        {
            if (vpCoor.y >= highestPoint.y)
            {
                highestPoint = vpCoor;
            }
            else if(vpCoor.y <= lowestPoint.y)
            {
                lowestPoint = vpCoor;
            }
        }

        if (highestPoint.y >= 1)
        {
            highestPoint.y = 1;
        }
        
        if (lowestPoint.y <= 0)
        {
            lowestPoint.y = 0;
        }
  //      Debug.Log("lowest" + lowestPoint);
        float height = camera.ViewportToScreenPoint(highestPoint).y - camera.ViewportToScreenPoint(lowestPoint).y;
 //       Debug.Log("heigth" + height);

        highestViewPoint = camera.ViewportToScreenPoint(highestPoint);
        return height;
    }

    public static Vector2 GetPerspectiveCenterOfObject(Vector3 highestPoint, Vector3 rightestPoint, float width, float height)
    {
        Vector2 result = new Vector2(highestPoint.y - (height/2), rightestPoint.x - (width/2)); 
        //Debug.Log(result);
        return result;
    }
}
