using UnityEngine;
using System.Collections;

public class GuiGroundHud : MonoBehaviour {
	
	GameObject player = null;
	TargettingControl playerTargettingControl = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(player == null) {
			player = GameObject.Find("Player");
			playerTargettingControl = player.GetComponent<TargettingControl>();
		}
		
		
		if( Input.GetKeyDown( KeyCode.Alpha1 ) ) {
			DoToolbar(1);
		}
	}
	
	void OnGUI() {
		
		GUILayout.BeginArea( new Rect(0,0,Screen.width,Screen.height) );
		
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		
		for(int i=1; i <= 10; i++) {
			if(GUILayout.Button("Toolbar " + i)) {
				DoToolbar(i);
			}
		}
		
		
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		
		GUILayout.EndArea();
		
	}
	
	void DoToolbar(int toolbar_id) {
		switch(toolbar_id) {
		case 1:
			// Hard coded to player "shoot" for testing
			DoShoot();
			break;
		default:
			Debug.LogError("Unimplemented toolbar action!");
			break;
		}
	}
	
	void DoShoot() {
		Debug.Log("BANG!");
		
		if(playerTargettingControl == null) {
			Debug.LogError("Trying to call DoShoot() without a targetting control.");
			return;
		}
		
		TargettingControl.TargetAcquiredCallback callback = new TargettingControl.TargetAcquiredCallback( DoShootTarget );
				
		playerTargettingControl.StartTargetting( callback );
	}
	
	void DoShootTarget(GameObject target) {
		Debug.Log("***DoShootTarget: " + target);
		DamageControl targetDamageControl = target.GetComponent<DamageControl>();
		DamageControl.DamageInfo di = new DamageControl.DamageInfo(100000);
		
		targetDamageControl.ReceiveDamage(di);
		
	}

}
