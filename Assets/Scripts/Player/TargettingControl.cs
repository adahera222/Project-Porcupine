using UnityEngine;
using System.Collections;

/**
 * Another class (like when a player is using ability "Fire Gun") would call StartTargetting
 * and provide a callback function (delegate) to be run when a target is selected.
 */
public class TargettingControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Input.GetAxis ("NextTarget");
		Input.GetAxis ("ShiftModifier");

	}
	
	/**
	 * A helpful page on delegates and Unity:
	 * 	http://www.41post.com/3146/programming/using-c-delegates-in-unity3d-scripts
	 */
    public delegate void TargetAcquiredCallback(GameObject gameObject);  

	void StartTargetting(TargetAcquiredCallback callback) {
		callback(gameObject);
	}
}
