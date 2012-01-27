using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenTiled : MonoBehaviour {
	
	public GameObject[] walls;
	public GameObject fogOfWar;
	public GameObject player;
	public GameObject floor;
	public GameObject monster;
	
	// length and width in tile count
	// y is technically in the z axis in the world
	public int tilesX = 60;
	public int tilesY = 20;
	
	public float tileSize = 2.0f;

	// Actual physical length and width of our map area
	protected float sizeX;
	protected float sizeZ;
	
	protected float offsetX;
	protected float offsetZ;
	
	private GameObject[,] wallArray;
	

	// Use this for initialization
	public virtual void Start () {
		sizeX = tilesX * tileSize;
		sizeZ = tilesY * tileSize;
		
		offsetX = -sizeX/2+tileSize/2;
		offsetZ = -sizeZ/2+tileSize/2;
		
		wallArray = new GameObject[tilesX,tilesY];
		
		CreateFloor();
		
		//CreateFogOfWar();
	}
	
	void CreateFloor() {
		// Get our floor object
		GameObject floor_instance = (GameObject)GameObject.Instantiate(floor, Vector3.zero, Quaternion.identity);
		
		// Get it's mesh
		Mesh floor_mesh = ((MeshFilter)floor_instance.GetComponent(typeof(MeshFilter))).mesh;
		
		// And figure out it's size (by default, planes are 10x10)
		Vector3 floor_size = floor_mesh.bounds.size;

		// Now scale it for our desired floor size
		floor_instance.transform.localScale = new Vector3( sizeX/floor_size.x, 1.0f, sizeZ/floor_size.z );
		
		// TODO: Texture tiling
	}
	

	void CreateFogOfWar() {
		for(int x=0; x < tilesX; x++) {
			for(int y=0; y < tilesY; y++) {
				GameObject.Instantiate(fogOfWar, new Vector3(x*tileSize+offsetX, 1, y*tileSize+offsetZ), Quaternion.identity);
			}
		}
	}
	
	protected GameObject CreatePlayer(Vector3 pos, Quaternion rot) {
		GameObject p = (GameObject)GameObject.Instantiate(player, pos, rot);
		p.name = "Player";
		return p;
	}
	
	protected GameObject CreateMonster(Vector3 pos, Quaternion rot) {
		GameObject m = (GameObject)GameObject.Instantiate(monster, pos, rot);
		m.name = "Monster";
		return m;
	}
	
	protected GameObject CreateWall(int x, int y) {
		wallArray[x,y] = (GameObject)GameObject.Instantiate(walls[0], new Vector3(x*tileSize+offsetX, 0, y*tileSize+offsetZ), Quaternion.identity);
		return wallArray[x,y];
	}
	
	protected void DestroyWall(int x, int y) {
		if(wallArray[x,y] != null) {
			Destroy(wallArray[x,y]);
			wallArray[x,y] = null;
		}
	}
}
