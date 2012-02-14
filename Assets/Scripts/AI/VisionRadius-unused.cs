using UnityEngine;
using System.Collections;

public class VisionRadius : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		//Debug.Log( "VisionRadius::OnTriggerEnter" );
		//Debug.Log( other );
		if( other.tag == "Creature" ) {
			
			CharacterController myCC = this.transform.parent.GetComponent<CharacterController>();
			CharacterController otherCC = other.collider.GetComponent<CharacterController>();
			
			//Debug.Log( myCC + " is trying to see " + otherCC );
			
			RaycastHit hit;
			if (Physics.Raycast (
					transform.position+myCC.center, 
					otherCC.transform.position-transform.position, 
					out hit)
				) {
				CharacterController hitCC = hit.collider.GetComponent<CharacterController>();
				//Debug.Log(hit.collider);
				if(otherCC==hitCC) {
					Debug.Log( this + " can see " + otherCC );
				}
			}
		}
	}
}
