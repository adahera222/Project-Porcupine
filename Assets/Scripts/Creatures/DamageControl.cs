using UnityEngine;
using System.Collections;


/**
 * This behaviour deals with all objects (creatures and things like crates and such)
 * that can receive damage in the game.  This behaviour is also used by the
 * player's targetting system (i.e. the player cycles between things that
 * have a DamageControl script)
 * 
 */
public class DamageControl : MonoBehaviour {
	
	/**
	 * DamageInfo could eventually contain information about hit direction,
	 * damage types and flags, etc...
	 */
	public class DamageInfo {
		public float amount;

		public DamageInfo(float amount) {
			this.amount = amount;
		}
	}
	
	public float maxHealth = 100.0f;
	private float health;
	
	/**
	 * When dying=true, we should no longer take AI actions and/or accept player input.
	 * In the future, we may set this while playing a death animation.
	 */
	private bool dying = false;

	// Use this for initialization
	void Start () {
		health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if(isDying()) {
			Destroy(gameObject);
		}	
	}
	
	public void ReceiveDamage ( DamageInfo damageInfo ) {
		health -= damageInfo.amount;
		
		if(health <= 0) {
			setDying(true);
		}
	}
	
	void setDying(bool dying) {
		this.dying = dying;
	}
	
	bool isDying() {
		return dying;
	}
	
	float getHealth() {
		return health;
	}
}
