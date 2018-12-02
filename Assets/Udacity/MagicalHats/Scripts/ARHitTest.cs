using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ARHitTest : MonoBehaviour {
	public Camera ARCamera; //the Virtual Camera used for AR
	public GameObject hitPrefab; //prefab we place on a hit test
    public GameObject bunny; //bunny prefab
    bool raiseHat = false;
    GameObject selectedHat;

    private List<GameObject> spawnedObjects = new List<GameObject>(); //array used to keep track of spawned objects

	/// <summary>
	/// Function that is called on 
	/// NOTE: HIT TESTS DON'T WORK IN ARKIT REMOTE
	/// </summary>
	public void SpawnHitObject() {
		ARPoint point = new ARPoint { 
			x = 0.5f, //do a hit test at the center of the screen
			y = 0.5f
		};

		// prioritize result types
		ARHitTestResultType[] resultTypes = {
			//ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, // if you want to use bounded planes
			//ARHitTestResultType.ARHitTestResultTypeExistingPlane, // if you want to use infinite planes 
			ARHitTestResultType.ARHitTestResultTypeFeaturePoint // if you want to hit test on feature points
		}; 

		foreach (ARHitTestResultType resultType in resultTypes) {
			if (HitTestWithResultType (point, resultType)) {
				return;
			}
		}
	}

	bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes) {
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
		if (hitResults.Count > 0) {
			foreach (var hitResult in hitResults) {
                //TODO: get the position and rotations to spawn the hat
                Vector3 position = UnityARMatrixOps.GetPosition(hitResult.worldTransform); //returns a Vector3 in Unity Coordinates
                Quaternion rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform); //returns a Quaternion in Unity Coordinates
                spawnedObjects.Add(Instantiate(hitPrefab, position, rotation));
                return true;
			}
		}
		return false;
	}

	// Fixed Update is called once per frame
	void FixedUpdate () {
		if (Input.GetMouseButtonDown(0)) { //this works with touch as well as with a mouse
            //RemoveObject (Input.mousePosition);
            AddBunny(Input.mousePosition);
		}
        if (raiseHat == true)
        {
            RaiseHat();
        }
    }

    public void AddBunny(Vector2 point)
    {
        RaycastHit hit;
        if (Physics.Raycast (ARCamera.ScreenPointToRay(point), out hit))
        {
            selectedHat = hit.collider.transform.parent.gameObject;
            Vector3 relativePos = selectedHat.transform.position - ARCamera.transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            rotation = Quaternion.Euler(0, rotation.y, 0);
            Instantiate(bunny, selectedHat.transform.position, rotation);
            raiseHat = true;
        }
    }

    public void RaiseHat()
    {
        float bunnyHeight = 0.05f;
        if (selectedHat.transform.position.y < (bunny.transform.position.y + bunnyHeight))
        {
            selectedHat.transform.Translate(0, Time.deltaTime / 2, 0);
        }
        else
        {
            raiseHat = false;
        }
    }

	public void RemoveObject(Vector2 point) {
		RaycastHit hit;
		if (Physics.Raycast (ARCamera.ScreenPointToRay (point), out hit)) {
			GameObject item = hit.collider.transform.parent.gameObject;
			if (spawnedObjects.Remove (item)) {
				Destroy(item);
			}
		}
	}
		
	/// <summary>
	/// NOTE: A Function To Be Called When the Shuffle Button is pressed
	/// </summary>
	public void Shuffle(){
		StartCoroutine( ShuffleTime ( Random.Range(5, 10)) );
	}
		
	/// <summary>
	/// NOTE: A Co-routine that shuffles 
	/// </summary>
	IEnumerator ShuffleTime(int numSuffles) {
		//TODO:
		//iterate numShuffles times
		//pick two hats randomly from spawnedObject and call the Co-routine Swap with their Transforms
		yield return null; //placeholder to make sure this compiles
	}

	IEnumerator Swap(Transform item1, Transform item2, float duration){
		//Lerp the position of item1 and item2 so that they switch places
		//the transition should take "duration" amount of time
		//Optional: try making sure the hats do not collide with each other
		yield return null; //placeholder to make sure this compiles
	}
}