using UnityEngine;
using System.Collections;

public class ForOfWarRevealer : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		//Debug.Log( other );
		if( other.tag == "FogOfWar" ) {
			Destroy(other.gameObject);
		}
	}
}
