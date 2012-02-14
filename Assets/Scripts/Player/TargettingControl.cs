using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Another class (like when a player is using ability "Fire Gun") would call StartTargetting
 * and provide a callback function (delegate) to be run when a target is selected.
 */
public class TargettingControl : MonoBehaviour {
	
	public GameObject selectionWidget;
	Transform selectWidgetTransform = null;
	Renderer selectWidgetRenderer = null;
	
	List<Transform> visibleTargetables = null;
	int currentSelection = 0;
	
	// What we had selected last time
	Transform savedSelection = null;
	
	// Use this for initialization
	void Start () {
		selectWidgetTransform = ((GameObject)GameObject.Instantiate(selectionWidget)).transform;
		selectWidgetRenderer = selectWidgetTransform.GetChild(0).renderer;
		StopTargetting();
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown( KeyCode.Return ) || Input.GetKeyDown( KeyCode.KeypadEnter ) ) {
			TargettingComplete(savedSelection.gameObject);
		}
		
		if( Input.GetKeyDown( KeyCode.Tab ) ) {
			currentSelection++;
			if( currentSelection >= visibleTargetables.Count ) {
				currentSelection = 0;
			}
			savedSelection = visibleTargetables[currentSelection];
		}


		Input.GetAxis ("ShiftModifier");
		
		UpdateWidget();
	}
	
	/**
	 * A helpful page on delegates and Unity:
	 * 	http://www.41post.com/3146/programming/using-c-delegates-in-unity3d-scripts
	 */
    public delegate void TargetAcquiredCallback(GameObject gameObject);
	
	TargetAcquiredCallback callback;
	
	public void StartTargetting(TargetAcquiredCallback callback) {
		this.callback = callback;
		visibleTargetables = GetVisibleTargets();
		
		if(visibleTargetables.Count == 0) {
			TargettingComplete(null);
		}
		else {
			//currentSelection = 0;
			setDefaultSelection();
			selectWidgetRenderer.enabled = true;
			UpdateWidget();
		}
	}
	
	public void StopTargetting() {
		selectWidgetRenderer.enabled = false;
		visibleTargetables = null;
	}
	
	void setDefaultSelection() {
		currentSelection = 0;
		if(savedSelection && visibleTargetables.Contains(savedSelection)) {
			currentSelection = visibleTargetables.IndexOf(savedSelection);
		}
		savedSelection = visibleTargetables[currentSelection];
	}
				
	void TargettingComplete(GameObject selection) {
		StopTargetting();
		callback(selection);
	}
	
	List<Transform> GetVisibleTargets() {
		//Debug.Log("GetVisibleTargets()");
		List<Transform> targets = new List<Transform>();
		
		GameObject[] targettables = GameObject.FindGameObjectsWithTag("Targettable");
		
		//Debug.Log("Targettables: " + targettables.Length);

		CharacterController myCC = this.transform.GetComponent<CharacterController>();
		Vector3 origin = this.transform.position + myCC.center;
		
		for(int i=0; i < targettables.Length; i++) {
			// Skip ourselves!
			if(targettables[i] == this.gameObject)
				continue;
			
			Vector3 pos = targettables[i].transform.position;
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(pos);
			//Debug.Log(targettables[i].name + " screenPoint = " + screenPoint);
			if( Camera.main.pixelRect.Contains(screenPoint) ) {
		
				Vector3 direction = targettables[i].collider.ClosestPointOnBounds(origin) - origin;
				//Debug.Log("Origin: " + origin + " Direction: " + direction);
				
				RaycastHit hitInfo;
				
				if(Physics.Raycast(origin, direction, out hitInfo)) {
					//Debug.Log("We hit: " + hitInfo.collider.name);
					if(hitInfo.collider.transform == targettables[i].transform) {
						targets.Add(targettables[i].transform);
					}
				}
			}
		}
		
		//Debug.Log("Targets: " + targets.Count);
		
		return targets;
	}
	
	void UpdateWidget() {
		if(visibleTargetables == null || currentSelection >= visibleTargetables.Count) {
			return;
		}
		
		Debug.Log("Setting widget to: " + visibleTargetables[currentSelection].position);
		
		selectWidgetTransform.position = visibleTargetables[currentSelection].position;
	}
	
}
