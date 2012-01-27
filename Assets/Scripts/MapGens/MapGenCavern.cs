using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class generates a cavern-like map using cellular automata rules.
 * 
 * Many of the ideas for this came from:
 * 	http://roguebasin.roguelikedevelopment.org/index.php/Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels
 * 
 * FIXME: There is a chance that this creates small areas that are isolated from each other.  
 * We much do a pathfinding search to detect this and either:
 *  a) Brute-force a connection
 *  b) Fill in the smallest unconnected areas
 *  c) Simply ensure that the player spawn and relevant exits only spawn in same connected area (ideally the largest)
 * 
 * Since unconnected areas should be quite small, it's probably fine to just fill them in.
 * 
 */
public class MapGenCavern : MapGenTiled {
	
	/**
	 * The chance that a tile will have a wall during the initial generation.
	 */
	public float density = 0.4f;
	
	/*
	 * Cutoff determines the threshold for a wall is to "stick around".
	 */
	public int cutoff = 5;
	
	/*
	 * Cutoff2 determines the threshold for repopulating very empty areas.
	 */
	public int cutoff2 = 3;
	
	public int smoothingIterations = 2;
	public int superSmoothIterations = 1;
	
	private int[,] cavernArray;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		cavernArray = new int[tilesX,tilesY];
		
		InitialWalls();
		
		for(int i=0; i < smoothingIterations; i++) {
			IterateWalls(1, 2);
		}
		for(int i=0; i < superSmoothIterations; i++) {
			IterateWalls(1);
		}
		
		FinalizeWalls();
		
		
		CreatePlayer(new Vector3(sizeX/2 + offsetX, 0, sizeZ/2 + offsetZ), Quaternion.identity);
	}
	
	void InitialWalls() {
		for(int x=0; x < tilesX; x++) {
			for(int y=0; y < tilesY; y++) {
				if(x==0 || x == tilesX-1 || y==0 || y == tilesY-1 || Random.Range(0f,1f) < density) {
					cavernArray[x,y] = 1;
				}
				else {
					cavernArray[x,y] = 0;
				}
			}
		}
	}
	
	void FinalizeWalls() {
		for(int x=0; x < tilesX; x++) {
			for(int y=0; y < tilesY; y++) {
				if(cavernArray[x,y] == 1) {
					CreateWall(x,y);
				}
			}
		}
	}
	
	void IterateWalls(int radius) {
		int[,] cavernArrayNew = (int[,])cavernArray.Clone();
		
		for(int x=1; x < tilesX-1; x++) {
			for(int y=1; y < tilesY-1; y++) {
				if( GetWallCountAt(x,y, radius) >= cutoff) {
					cavernArrayNew[x,y] = 1;
				}
				else {
					cavernArrayNew[x,y] = 0;
				}
			}
		}
		
		cavernArray = cavernArrayNew;
	}
	
	void IterateWalls(int radius1, int radius2) {
		int[,] cavernArrayNew = (int[,])cavernArray.Clone();
		
		for(int x=1; x < tilesX-1; x++) {
			for(int y=1; y < tilesY-1; y++) {
				if( GetWallCountAt(x,y, radius1) >= cutoff || GetWallCountAt(x,y,radius2) <= cutoff2) {
					cavernArrayNew[x,y] = 1;
				}
				else {
					cavernArrayNew[x,y] = 0;
				}
			}
		}
		
		cavernArray = cavernArrayNew;
	}
	
	
	int GetWallCountAt(int centerX, int centerY, int radius) {
		int wallCount = 0;
		for(int x=centerX-radius; x <= centerX+radius; x++) {
			for(int y=centerY-radius; y <= centerY+radius; y++) {
				if(x >= 0 && x < tilesX && y >= 0 && y < tilesY && cavernArray[x,y] != 0) {
					wallCount++;
				}
			}
		}
		return wallCount;
	}
}
