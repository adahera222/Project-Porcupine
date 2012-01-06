using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGen_Dungeon : MonoBehaviour {
	
	public GameObject[] walls;
	public GameObject fog_of_war;
	public GameObject player;
	public GameObject floor;
	
	// length and width in tile count
	// y is technically in the z axis in the world
	public int tiles_x = 40;
	public int tiles_y = 20;
	
	public float tile_size = 2.0f;

	// Actual physical length and width of our map area
	private float size_x;
	private float size_z;
	
	private float offset_x;
	private float offset_z;
	
	private GameObject[,] wall_array;
	
	public int min_room_x = 3;
	public int min_room_y = 3;
	public int max_room_x = 8;
	public int max_room_y = 8;
	
	private class Room {
		public int origin_x, origin_y, size_x, size_y;
		
		public Room(int origin_x, int origin_y, int size_x, int size_y) {
			this.origin_x = origin_x;
			this.origin_y = origin_y;
			this.size_x = size_x;
			this.size_y = size_y;
		}
		
		public override string ToString() {
			return "Room (" + origin_x + "," + origin_y + "), (" + size_x + "," + size_y + ")";
		}
		
		public bool Collides(Room other_room) {
			if(origin_x > other_room.origin_x+other_room.size_x) {
				return false;
			}
			if(origin_x + size_x < other_room.origin_x) {
				return false;
			}
			if(origin_y > other_room.origin_y+other_room.size_y) {
				return false;
			}
			if(origin_y + size_y < other_room.origin_y) {
				return false;
			}
			
			return true;
		}
	}
	
	private List<Room> room_array;

	// Use this for initialization
	void Start () {
		size_x = tiles_x * tile_size;
		size_z = tiles_y * tile_size;
		
		offset_x = -size_x/2+tile_size/2;
		offset_z = -size_z/2+tile_size/2;
		
		CreateFloor();
		CreateWalls();
		MakeRooms();
		MakeHallways();
		ConnectRooms();
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
		floor_instance.transform.localScale = new Vector3( size_x/floor_size.x, 1.0f, size_z/floor_size.z );
		
		// TODO: Texture tiling
	}
	
	void CreateWalls() {
		// This function fills the level with walls.
		wall_array = new GameObject[tiles_x,tiles_y];
		
		for(int x=0; x < tiles_x; x++) {
			for(int y=0; y < tiles_y; y++) {
				wall_array[x,y] = (GameObject)GameObject.Instantiate(walls[0], new Vector3(x*tile_size+offset_x, 0, y*tile_size+offset_z), Quaternion.identity);
			}
		}
	}
	
	void MakeRooms() {
		int number_of_rooms = Random.Range(5, 5);
		room_array = new List<Room>();
		
		for(int i=0; i < number_of_rooms; i++) {
			MakeRoom();
			
			if(i==0) {
				// Place the player in the first room.
				GameObject.Instantiate(player, new Vector3((room_array[0].origin_x+room_array[0].size_x/2)*tile_size+offset_x, 0, (room_array[0].origin_y+room_array[0].size_y/2)*tile_size+offset_z), Quaternion.identity);
			}
		}
	}
	
	void MakeRoom() {
		Debug.Log("MakeRoom()");
		int origin_x=0, origin_y=0, size_x=0, size_y=0;
		bool found_spot = false;
		int search_limit = 10;
		Room r = null;
		
		Debug.Log("pre-loop");
		while(!found_spot) {
			Debug.Log("loop");
			if(search_limit-- == 0) {
				Debug.Log("MakeRoom() can't seem to place a room. Breaking!");
				break;
			}
			
			// Generate a new spot
			origin_x = Random.Range(1, tiles_x-1);
			origin_y = Random.Range(1, tiles_y-1);
			size_x = Random.Range(min_room_x, max_room_x+1);
			size_y = Random.Range(min_room_y, max_room_y+1);
			
			if(origin_x + size_x >= tiles_x) {
				origin_x -= tiles_x - size_x - 1;
			}
			if(origin_y + size_y >= tiles_y) {
				origin_y -= tiles_y - size_y - 1;
			}
			
			r = new Room(origin_x, origin_y, size_x, size_y);
				
			// Check for conflicts
			found_spot = true;
			Debug.Log(room_array.Count);
			for(int i=0; i < room_array.Count; i++) {
				Debug.Log("room_array[i] type :" + room_array[i] );

				if(	r.Collides(room_array[i]) ) {
					Debug.Log("!!!!!!!!!!!!!!Collision between "+r+" and "+room_array[i]);
					found_spot = false;
					break;
				}
				Debug.Log("No collision between "+r+" and "+room_array[i]);
			}
			Debug.Log("Done searching, did we find a match? " + found_spot);
		}
		
		if(found_spot) {
			room_array.Add(r);
			
			RemoveWallsForRoom(r);
		}
	}
	
	void RemoveWallsForRoom(Room r) {
		for(int x=r.origin_x; x < r.origin_x+r.size_x; x++) {
			for(int y=r.origin_y; y < r.origin_y+r.size_y; y++) {
				if(wall_array[x,y]) {
					Destroy(wall_array[x,y]);
					wall_array[x,y] = null;
				}
			}
		}
	}
	
	void MakeHallways() {
	}
	
	void ConnectRooms() {
	}
	
	void CreateFogOfWar() {
		for(int x=0; x < tiles_x; x++) {
			for(int y=0; y < tiles_y; y++) {
				GameObject.Instantiate(fog_of_war, new Vector3(x*tile_size+offset_x, 1, y*tile_size+offset_z), Quaternion.identity);
			}
		}
	}
}
