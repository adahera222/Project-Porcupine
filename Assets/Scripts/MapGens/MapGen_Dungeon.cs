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
	
	public int number_of_rooms_min = 5;
	public int number_of_rooms_max = 10;
	private int number_of_rooms;
	
	// number_of_rooms * the results of this
	// NOTE: hallways will start from random rooms and then wander randomly
	// unconnected rooms will then get a hallway from them to another room
	public float hallways_per_room_min = 1.0f;
	public float hallways_per_room_max = 1.0f;
	
	private class Room {
		public int origin_x, origin_y, size_x, size_y;
		public bool is_connected;
		
		public Room(int origin_x, int origin_y, int size_x, int size_y) {
			this.origin_x = origin_x;
			this.origin_y = origin_y;
			this.size_x = size_x;
			this.size_y = size_y;
			this.is_connected = false;
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
		
		public bool ContainsTile(int x, int y) {
			if( x >= origin_x && x < origin_x + size_x &&
				y >= origin_y && y < origin_y + size_y )
					return true;
			
			return false;
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
		
		// Make random, interesting hallways
		MakeHallways();
		
		// Make sure all the rooms are actually connected
		ConnectRooms();
		
		CreateFogOfWar();
		
		
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
		number_of_rooms = Random.Range(number_of_rooms_min, number_of_rooms_max+1);
		room_array = new List<Room>();
		
		for(int i=0; i < number_of_rooms; i++) {
			MakeRoom();
			
			if(i==0) {
				// Place the player in the first room.
				GameObject p = (GameObject)GameObject.Instantiate(player, new Vector3((room_array[0].origin_x+room_array[0].size_x/2)*tile_size+offset_x, 0, (room_array[0].origin_y+room_array[0].size_y/2)*tile_size+offset_z), Quaternion.identity);
				p.name = "Player";
			}
		}
	}
	
	void MakeRoom() {
		//Debug.Log("MakeRoom()");
		int origin_x=0, origin_y=0, size_x=0, size_y=0;
		bool found_spot = false;
		int search_limit = 10;
		Room r = null;
		
		while(!found_spot) {
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
			//Debug.Log(room_array.Count);
			for(int i=0; i < room_array.Count; i++) {
				//Debug.Log("room_array[i] type :" + room_array[i] );

				if(	r.Collides(room_array[i]) ) {
					//Debug.Log("MakeRoom() -- Collision between "+r+" and "+room_array[i]);
					found_spot = false;
					break;
				}
				//Debug.Log("No collision between "+r+" and "+room_array[i]);
			}
			//Debug.Log("Done searching, did we find a match? " + found_spot);
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
		//int number_of_hallways = number_of_rooms * Random.Range(hallways_per_room_min, hallways_per_room_max);
	}
	
	void ConnectRooms() {
		// TODO: Use pathfinding checks to detect if each room is connected
		// to the starting room.
		
		// For now, assume all rooms but the starting on are unconnected
		room_array[0].is_connected = true;
		
		
		List<Room> unconnected_rooms = new List<Room>();
		List<Room> connected_rooms = new List<Room>();
		for(int i=0; i < room_array.Count; i++) {
			if( room_array[i].is_connected == false ) {
				unconnected_rooms.Add(room_array[i]);
			}
			else {
				connected_rooms.Add(room_array[i]);
			}
		}
		
		while(unconnected_rooms.Count > 0) {
			Debug.Log("Unconnected rooms: " + unconnected_rooms.Count);
			Room r = unconnected_rooms[0];
			r.is_connected = true;
			
			Room other_room = r;
			while(other_room == r)
				other_room = connected_rooms[Random.Range(0, connected_rooms.Count)];
			
			MakeConnection(r, other_room);
			
			// Making this connection may have passed through multiple
			// rooms, so just regenerate everything
			// TODO: This code will go away once we have proper hallway generation
			unconnected_rooms = new List<Room>();
			connected_rooms = new List<Room>();
			for(int i=0; i < room_array.Count; i++) {
				if( room_array[i].is_connected == false ) {
					unconnected_rooms.Add(room_array[i]);
				}
				else {
					connected_rooms.Add(room_array[i]);
				}
			}
			
			Debug.Log("Unconnected rooms: " + unconnected_rooms.Count);
		//	return;
		
		}
		
	}
	
	void MakeConnection(Room r1, Room r2) {
		MakeHallway(
				r1.origin_x+r1.size_x/2,
				r1.origin_y+r1.size_y/2,
				r2.origin_x+r2.size_x/2,
				r2.origin_y+r2.size_y/2
			);
	}
	
	void MakeHallway(int x1, int y1, int x2, int y2) {
		// Super basic method.  Need to make non-diagonal lines later
		int x = x1;
		int y = y1;
		
		while(x != x2 || y != y2) {
			int dx = x2 - x;
			int dy = y2 - y;
			
			//if(Mathf.Abs(dx) > Mathf.Abs(dy)) {
			if(dx != 0) {
				if(dx < 0)
					x--;
				else
					x++;
			}
			else {
				if(dy < 0)
					y--;
				else
					y++;
			}
			
			Destroy(wall_array[x,y]);
			
			// TODO: Right now, all hallways are guaranteed to connect to a connected room
			// but 
			for(int i=0; i < room_array.Count; i++) {
			//	if( room_array[i].ContainsTile(x,y) ) {
			//		room_array[i].is_connected = true;
			//	}
			}
		}
		
	}
	
	void CreateFogOfWar() {
		for(int x=0; x < tiles_x; x++) {
			for(int y=0; y < tiles_y; y++) {
				GameObject.Instantiate(fog_of_war, new Vector3(x*tile_size+offset_x, 1, y*tile_size+offset_z), Quaternion.identity);
			}
		}
	}
}
