using UnityEngine;
using System.Collections;

public class VisionRadius : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		//Debug.Log( "VisionRadius::OnTriggerEnter" );
		//Debug.Log( other );
		if( other.tag == "Creature" ) {
			Debug.Log( this + " can see " + other );
		}
	}
}
