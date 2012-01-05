using UnityEngine;
using System.Collections;

public class CameraLockedToPlayer : MonoBehaviour {

	private Transform player_transform = null;
	
	// Use this for initialization
	void Start () {
		player_transform = (Transform)GameObject.Find("Player").GetComponent("Transform");
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = new Vector3(
				player_transform.position.x, 
				transform.position.y, 
				player_transform.position.z
			);
	}
}
