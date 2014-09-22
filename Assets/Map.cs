using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {
	
	#region Structs
	public struct Tile {
		public int x;
		public int y;
		
		public Tile (int x = 0, int y = 0) 
		{
			this.x = x;
			this.y = y; 
		}
	}
	#endregion
	
	#region Properties

	public int Width;
	public int Height;
	public TextMesh distancePrefab;
	
	#endregion
	
	#region Variables
	int[,] map;
	int[,] mapDistances;
	int[,] mapVisitCount;
	int maxDistance;
	Tile maxDistancePoint;
	Tile startPoint;
	Tile endPoint;
	
	System.TimeSpan recursiveVisitTime;
	System.TimeSpan floodFillTime;
	#endregion
	
	void Start()
	{
		CreateMap();
		CreateStartAndEndPoint();
		DrawMap();
	}
	
	void Update()
	{
		if (Input.GetMouseButtonUp(0)) {
			Start ();
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width * 0.85f,40,400, 20), "Distance algorithm performance"); 
		GUI.Label(new Rect(Screen.width * 0.85f,60,400, 20), "Recursive Visit: " + recursiveVisitTime.Ticks); 
		GUI.Label(new Rect(Screen.width * 0.85f,80,400, 20), "Flood Fill: " + floodFillTime.Ticks); 
	}
	
	
	void CreateMap()
	{
		map = new int[Width,Height];
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				map[i,j] = Random.value < 0.6? 0: 1;
			}
		}
	}
	
	Tile GetEndPoint(Tile startPoint, bool floodfill)
	{
		// Clear map distances
		mapDistances = new int[Width,Height];
		mapVisitCount = new int[Width, Height];
		for (int i = 0; i < Width; i++) {
			for (int j = 0; j < Height; j++) {
				this.mapDistances[i, j] = -1;
				mapVisitCount[i,j] = 0;
			}
		}
		
		if (floodfill)
			FloodFill(startPoint.x, startPoint.y);
		else
			VisitCell (startPoint.x, startPoint.y, 0);
		
		// Find max distance
		int maxDistance = 0;
		Tile maxDistancePoint = new Tile();
		for (int i = 0; i < Width; i++) {
			for (int j = 0; j < Height; j++) {
				if (mapDistances[i,j] > maxDistance) {
					maxDistance = mapDistances[i,j];
					maxDistancePoint.x = i;
					maxDistancePoint.y = j;
				}
			}
		}
		return maxDistancePoint;
	}
	
	void CreateStartAndEndPoint()
	{
		// Create start point
		startPoint = new Tile();
		do
		{
			startPoint.x = Random.Range(0, Width);
			startPoint.y = Random.Range(0, Height);
		} while (map[startPoint.x, startPoint.y] == 1);
		
		// Get the end point
		
		// Diagnose recursive visit
		var stopWatch = System.Diagnostics.Stopwatch.StartNew();
		endPoint = GetEndPoint(startPoint, false);
		stopWatch.Stop();
		recursiveVisitTime = stopWatch.Elapsed;
		
		// Diagnose flood fill
		stopWatch = System.Diagnostics.Stopwatch.StartNew();
		endPoint = GetEndPoint(startPoint, true);
		stopWatch.Stop();
		floodFillTime = stopWatch.Elapsed;
	}
	
	
	void FloodFill(int x, int y) 
	{
		// We have two buffers. The tiles being processed, and the tiles that are
		// to be processed on the next iteration. This is way faster than having
		// a generic list and adding tiles on the fly.
		Tile[,] tileBuffer = new Tile[2,100];
		int currentBuffer = 0;
		int nextBuffer = 1;
		
		int tileCount = 1;
		int newTileCount = 0;
		int distance = 0;

		// Add the starting tile to the buffer so that it's processed
		tileBuffer[currentBuffer, 0].x = x;
		tileBuffer[currentBuffer, 0].y = y;
		
		while (tileCount > 0)
		{
			newTileCount = 0;
			for (int i = 0; i < tileCount; i++)
			{
				// Get x and y of current tile
				x = tileBuffer[currentBuffer, i].x;
				y = tileBuffer[currentBuffer, i].y;
				
				// "visit"
				mapDistances[x, y] = distance;
				mapVisitCount[x, y] = 1;
				
				// Check if we should add contiguous tiles to the next buffer
				// Left tile
				if (x > 0 && map[x-1, y] == 0 && mapDistances[x-1, y] == -1)
				{
					tileBuffer[nextBuffer, newTileCount].x = x-1;
					tileBuffer[nextBuffer, newTileCount].y = y;
					mapDistances[x-1, y] = distance + 1;
					newTileCount++;
				}
				// Right tile
				if (x < map.GetLength(0)-1 && map[x+1, y] == 0 && mapDistances[x+1, y] == -1)
				{
					tileBuffer[nextBuffer, newTileCount].x = x+1;
					tileBuffer[nextBuffer, newTileCount].y = y;
					mapDistances[x+1, y] = distance + 1;
					newTileCount++;
				}
				// Top tile
				if (y > 0 && map[x, y-1] == 0 && mapDistances[x, y-1] == -1) 
				{
					tileBuffer[nextBuffer, newTileCount].x = x;
					tileBuffer[nextBuffer, newTileCount].y = y-1;
					mapDistances[x, y-1] = distance + 1;
					newTileCount++;
				}
				// Bottom tile
				if (y < map.GetLength(1)-1 && map[x, y+1] == 0 && mapDistances[x, y+1] == -1)
				{
					tileBuffer[nextBuffer, newTileCount].x = x;
					tileBuffer[nextBuffer, newTileCount].y = y + 1;
					mapDistances[x, y+1] = distance + 1;
					newTileCount++;
				}
			}
			// Swap buffers
			nextBuffer = (nextBuffer + 1 ) % 2;
			currentBuffer = (currentBuffer + 1 ) % 2;
			tileCount = newTileCount;
			distance++;
		}
	}
	
	void TryVisit(int x, int y, int distance)
	{
		if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
			return;
		
		if (map [x, y] == 0 && (mapDistances [x, y] == -1 || mapDistances[x, y] > distance)) {
			VisitCell(x, y, distance);
		}
	}
	
	void VisitCell(int x, int y, int distance)
	{
		mapVisitCount[x,y] ++;
		
		// Set tile's current distance
		mapDistances[x, y] = distance;
		
		// Visit neighbor cells
		TryVisit(x - 1, y, distance + 1);
		TryVisit(x + 1, y, distance + 1);
		TryVisit(x, y - 1, distance + 1);
		TryVisit(x, y + 1, distance + 1);
	}
	
	void DrawMap()
	{
		// Destroy current map
		foreach (Transform child in transform)
		{
			Destroy (child.gameObject);
		}

		// Recreate
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				if (map[i, j] == 1) {
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.position = new Vector3(i, 0, j);
					cube.transform.parent = transform;
					cube.renderer.material.color = Color.gray;
				}
				else {
					TextMesh text = (TextMesh)Instantiate(distancePrefab);
					text.text = mapDistances[i, j].ToString();
					
					if (startPoint.x == i && startPoint.y == j)
						text.color = Color.red;
					
					if (endPoint.x == i && endPoint.y == j)
						text.color = Color.blue;
					
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.parent = text.transform;
					cube.transform.localPosition = Vector3.zero;
					float v = ((float)mapVisitCount[i,j]) / 10;
					cube.renderer.material.color = new Color( 2.0f * v, 2.0f * (1 - v), 0);
					
					if (mapVisitCount[i,j] == 0)
						cube.renderer.material.color = Color.blue;
					
					text.transform.position = new Vector3(i, 0, j);
					text.transform.parent = transform;
					
				}
			}
		}
	}
}
