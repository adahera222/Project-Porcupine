using UnityEngine;
using System.Collections;

public class FactionControl : MonoBehaviour {
	
	public int faction = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public bool isEnemyOf(FactionControl other) {
		return faction != other.faction;
	}
}
