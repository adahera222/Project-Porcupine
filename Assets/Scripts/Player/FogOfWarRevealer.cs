using UnityEngine;
using System.Collections;

public class FogOfWarRevealer : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		//Debug.Log( other );
		if( other.tag == "FogOfWar" ) {
			Destroy(other.gameObject);
		}
	}
}
