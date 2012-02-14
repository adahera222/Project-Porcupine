using UnityEngine;
using System.Collections.Generic;

public class VisionSensor : MonoBehaviour {
	
	List<Transform> visibleTargetables = null;
	List<Transform> visibleEnemies = null;
	List<Transform> visibleFriends = null;
	
	FactionControl factionControl = null;
	
	// Use this for initialization
	void Start () {
		factionControl = this.GetComponent<FactionControl>();
	}
	
	// Update is called once per frame
	void Update () {
		/**
		 * TODO: Benchmarking to see if this function is slow.
		 */
		UpdateVisibleTargets();
	}
	
	public Transform getNearestVisibleEnemy() {
		if(visibleEnemies != null && visibleEnemies.Count > 0) {
				return visibleEnemies[0];
		}
		
		return null;
	}
	
	void UpdateVisibleTargets() {
		Debug.Log("UpdateVisibleTargets");
		visibleTargetables = new List<Transform>();
		visibleEnemies = new List<Transform>();
		visibleFriends = new List<Transform>();
		
		GameObject[] targettables = GameObject.FindGameObjectsWithTag("Targettable");
		
		CharacterController myCC = this.transform.GetComponent<CharacterController>();
		Vector3 origin = this.transform.position + myCC.center;
		
		for(int i=0; i < targettables.Length; i++) {
			// Skip ourselves!
			if(targettables[i] == this.gameObject)
				continue;
			
			Vector3 pos = targettables[i].transform.position;
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(pos);

			if( Camera.main.pixelRect.Contains(screenPoint) ) {
		
				Vector3 direction = targettables[i].collider.ClosestPointOnBounds(origin) - origin;
				
				RaycastHit hitInfo;
				
				if(Physics.Raycast(origin, direction, out hitInfo)) {
					if(hitInfo.collider.transform == targettables[i].transform) {
						visibleTargetables.Add(targettables[i].transform);
						FactionControl fc = targettables[i].GetComponent<FactionControl>();
						
						// Now do friend-or-for detection
						if(factionControl == null || fc == null || factionControl.isEnemyOf(fc) ) {
							visibleEnemies.Add(targettables[i].transform);
						}
						else {
							visibleFriends.Add(targettables[i].transform);
						}
					}
				}
			}
		}
		
		// TODO: Sort targets by distance
		TargetComparer comparer = new TargetComparer(transform.position);
		visibleTargetables.Sort(comparer);
		visibleEnemies.Sort(comparer);
		visibleFriends.Sort(comparer);
	}

	public class TargetComparer: IComparer<Transform> {
		Vector3 origin;
		
		public TargetComparer(Vector3 origin) {
			this.origin = origin;
		}
		
	    public int Compare(Transform x, Transform y) {
			float distanceX = Vector3.Distance(origin, x.position);
			float distanceY = Vector3.Distance(origin, y.position);

			return distanceX.CompareTo(distanceY);
	    }
	}

}
