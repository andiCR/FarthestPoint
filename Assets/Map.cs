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
	
	Tile GetEndPoint(Tile startPoint)
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
		
		
		// Visit cell
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
		endPoint = GetEndPoint(startPoint);
		
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
					
					Debug.Log (mapVisitCount[i,j]);
					text.transform.position = new Vector3(i, 0, j);
					text.transform.parent = transform;
					
				}
			}
		}
	}
}
