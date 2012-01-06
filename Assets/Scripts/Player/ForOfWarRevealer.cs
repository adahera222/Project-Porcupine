using UnityEngine;
using System.Collections;

public class ForOfWarRevealer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("ForOfWarRevealer");
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log( other );
		if( other.tag == "FogOfWar" ) {
			Destroy(other.gameObject);
		}
	}
}
