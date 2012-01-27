using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenDungeon : MapGenTiled {
	
	public int minRoomX = 3;
	public int minRoomY = 3;
	public int maxRoomX = 8;
	public int maxRoomY = 8;
	
	public int numberOfRoomsMin = 5;
	public int numberOfRoomsMax = 10;
	private int numberOfRooms;
	
	// number_of_rooms * the results of this
	// NOTE: hallways will start from random rooms and then wander randomly
	// unconnected rooms will then get a hallway from them to another room
	public float hallwaysPerRoomMin = 1.0f;
	public float hallwaysPerRoomMax = 1.0f;
	
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
	public override void Start () {
		base.Start();
		CreateWalls();
		MakeRooms();
		
		// Make random, interesting hallways
		MakeHallways();
		
		// Make sure all the rooms are actually connected
		ConnectRooms();
		
	}
	
	void CreateWalls() {
		// This function fills the level with walls.
		
		for(int x=0; x < tilesX; x++) {
			for(int y=0; y < tilesY; y++) {
				//wallArray[x,y] = (GameObject)GameObject.Instantiate(walls[0], new Vector3(x*tileSize+offsetX, 0, y*tileSize+offsetZ), Quaternion.identity);
				CreateWall(x,y);
			}
		}
	}
	
	void MakeRooms() {
		numberOfRooms = Random.Range(numberOfRoomsMin, numberOfRoomsMax+1);
		room_array = new List<Room>();
		
		int i=0;
		while(i < numberOfRooms) {
			
			if( MakeRoom() ) {
				if(i==0) {
					// Place the player in the first room.
					CreatePlayer(new Vector3((room_array[i].origin_x+room_array[i].size_x/2)*tileSize+offsetX, 0, (room_array[i].origin_y+room_array[i].size_y/2)*tileSize+offsetZ), Quaternion.identity);
				}
				else {
					Room r = room_array[i];
					CreateMonster(
						new Vector3( (r.origin_x+r.size_x/2)*tileSize+offsetX, 0, (r.origin_y+r.size_y/2)*tileSize+offsetZ ),
						Quaternion.identity);
				}
				
				i++;
			}
		}
	}
	
	bool MakeRoom() {
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
			origin_x = Random.Range(1, tilesX-1);
			origin_y = Random.Range(1, tilesY-1);
			size_x = Random.Range(minRoomX, maxRoomX+1);
			size_y = Random.Range(minRoomY, maxRoomY+1);
			
			if(origin_x + size_x >= tilesX) {
				origin_x -= tilesX - size_x - 1;
			}
			if(origin_y + size_y >= tilesY) {
				origin_y -= tilesY - size_y - 1;
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
		
		return found_spot;
	}
	
	void RemoveWallsForRoom(Room r) {
		for(int x=r.origin_x; x < r.origin_x+r.size_x; x++) {
			for(int y=r.origin_y; y < r.origin_y+r.size_y; y++) {
				DestroyWall(x,y);
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
			
			DestroyWall(x,y);
			
			// TODO: Right now, all hallways are guaranteed to connect to a connected room
			// but 
			for(int i=0; i < room_array.Count; i++) {
			//	if( room_array[i].ContainsTile(x,y) ) {
			//		room_array[i].is_connected = true;
			//	}
			}
		}
		
	}
}
