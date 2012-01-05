using UnityEngine;
using System.Collections;

public class GUITimeLogger : MonoBehaviour {
	
	private GUIText gui_text;
	
	// Use this for initialization
	void Start () {
		gui_text = (GUIText)GetComponent(typeof(GUIText));
	}
	
	// Update is called once per frame
	void Update () {
		gui_text.text = "Time: " + Time.time + " Real Time: " + Time.realtimeSinceStartup;
	}
}
