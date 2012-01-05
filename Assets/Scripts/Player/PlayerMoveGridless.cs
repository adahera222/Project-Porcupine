using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class PlayerMoveGridless : MonoBehaviour {

	private CharacterController character_controller;
	
	// This should not be here -- it should be a characterstic of the map object
	private float movement_rate = 5.0f;
	
	private float last_delta = 0.0f;
	
	// Use this for initialization
	void Start () {
		character_controller = (CharacterController)this.GetComponent(typeof(CharacterController));
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Time.deltaTime > 0.0f)
				last_delta = Time.deltaTime;
		
		
		
		Vector3 movement = Vector3.zero;
		//Debug.Log(movement);
		
		if(Input.GetKey(KeyCode.Keypad8)) {
			// North is positive Z
			movement += new Vector3(0, 0, movement_rate);
		}
		else if(Input.GetKey(KeyCode.Keypad9)) {
			// NE
			movement += new Vector3(movement_rate, 0, movement_rate);
		}
		else if(Input.GetKey(KeyCode.Keypad6)) {
			// East is positive X
			movement += new Vector3(movement_rate, 0, 0);
		}
		else if(Input.GetKey(KeyCode.Keypad3)) {
			// SE
			movement += new Vector3(movement_rate, 0, -movement_rate);
		}
		else if(Input.GetKey(KeyCode.Keypad2)) {
			// S
			movement +=  new Vector3(0, 0, -movement_rate);
		}
		else if(Input.GetKey(KeyCode.Keypad1)) {
			// SW
			movement += new Vector3(-movement_rate, 0, -movement_rate);
		}
		else if(Input.GetKey(KeyCode.Keypad4)) {
			// W
			movement += new Vector3(-movement_rate, 0, 0);
		}
		else if(Input.GetKey(KeyCode.Keypad7)) {
			// NW
			movement += new Vector3(-movement_rate, 0, movement_rate);
		}
		
		
		if(movement.magnitude > 0) {
			Pause(false);
			movement = movement.normalized * movement_rate;
			character_controller.Move(movement * last_delta);
		}
		else if (Input.GetKey(KeyCode.Keypad5)) {
			Pause(false);
		}
		else {
			Pause();
		}
	}
	
	void Pause(bool p = true) {
		if(p)
			Time.timeScale = 0;
		else
			Time.timeScale = 1.0f;
	}
}