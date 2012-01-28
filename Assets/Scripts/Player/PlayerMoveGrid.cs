using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (CharacterController))]

public class PlayerMoveGrid : MonoBehaviour {

	private Transform my_transform;
	
	// This should not be here -- it should be a characterstic of the map object
	private float tile_size = 2.0f;
	
	private float movement_cost = 0.10f;				// 1 time unit per tile
	private float movement_cost_diagonal = 0.1414f;		// sqrt(2)
	
	private float cooldown_remaining = 0.0f;
	
	// Use this for initialization
	void Start () {
		my_transform = (Transform)this.GetComponent(typeof(Transform));
	}
	
	// Update is called once per frame
	void Update () {
		cooldown_remaining -= Time.deltaTime;
		
		if(cooldown_remaining < 0.0f)
			cooldown_remaining = 0.0f;
		
		//Debug.Log(Time.timeScale);
		
		Pause(cooldown_remaining <= 0.0);
		
		if(cooldown_remaining <= 0.0f) {
			if(Input.GetKeyDown(KeyCode.Keypad8)) {
				// North is positive Z
				my_transform.Translate( new Vector3(0, 0, tile_size) );
				cooldown_remaining += movement_cost;
			}
			else if(Input.GetKeyDown(KeyCode.Keypad9)) {
				// NE
				my_transform.Translate( new Vector3(tile_size, 0, tile_size) );
				cooldown_remaining += movement_cost_diagonal;
			}
			else if(Input.GetKeyDown(KeyCode.Keypad6)) {
				// East is positive X
				my_transform.Translate( new Vector3(tile_size, 0, 0) );
				cooldown_remaining += movement_cost;
			}
			else if(Input.GetKeyDown(KeyCode.Keypad3)) {
				// SE
				my_transform.Translate( new Vector3(tile_size, 0, -tile_size) );
				cooldown_remaining += movement_cost_diagonal;
			}
			else if(Input.GetKeyDown(KeyCode.Keypad2)) {
				// S
				my_transform.Translate( new Vector3(0, 0, -tile_size) );
				cooldown_remaining += movement_cost;
			}
			else if(Input.GetKeyDown(KeyCode.Keypad1)) {
				// SW
				my_transform.Translate( new Vector3(-tile_size, 0, -tile_size) );
				cooldown_remaining += movement_cost_diagonal;
			}
			else if(Input.GetKeyDown(KeyCode.Keypad4)) {
				// W
				my_transform.Translate( new Vector3(-tile_size, 0, 0) );
				cooldown_remaining += movement_cost;
			}
			else if(Input.GetKeyDown(KeyCode.Keypad7)) {
				// NW
				my_transform.Translate( new Vector3(-tile_size, 0, tile_size) );
				cooldown_remaining += movement_cost_diagonal;
			}
		}	// if(cooldown_remaining <= 0.0f)
	}
	
	void Pause(bool p) {
		if(p)
			Time.timeScale = 0;
		else
			Time.timeScale = 1.0f;
	}
}
