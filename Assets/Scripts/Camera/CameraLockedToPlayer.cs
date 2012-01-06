using UnityEngine;
using System.Collections;

public class CameraLockedToPlayer : MonoBehaviour {

	private Transform player_transform = null;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if( !player_transform ) {
			// We can't grab the player in Start() because the map might not be generated yet.
			GameObject player = GameObject.Find("Player");
			Debug.Log(player);
			if(player)
				player_transform = (Transform)player.GetComponent(typeof(Transform));
		}
		
		
		if( player_transform ) {
			transform.position = new Vector3(
					player_transform.position.x, 
					transform.position.y, 
					player_transform.position.z
				);
		}
	}
}
