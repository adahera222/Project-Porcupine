using UnityEngine;
using System.Collections;

[RequireComponent (typeof (VisionSensor))]
[RequireComponent (typeof (CharacterController))]

public class ChasesEnemies : MonoBehaviour {
	
	Vector3 targetLocation;
	VisionSensor visionSensor;
	CharacterController characterController;
	
	float movementSpeed = 4f;

	// Use this for initialization
	void Start () {
		targetLocation = this.transform.position;
		visionSensor = this.GetComponent<VisionSensor>();
		characterController = this.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		Transform enemy = visionSensor.getNearestVisibleEnemy();
		if(enemy != null)
			targetLocation = enemy.position;
		
		if(Vector3.Distance(this.transform.position, targetLocation) >= 1f)
			Debug.Log(this.gameObject.name + " is heading to location: " + targetLocation);
		
		Vector3 dir = targetLocation - transform.position;
		characterController.Move(dir.normalized * movementSpeed *  Time.deltaTime);
	}
}
