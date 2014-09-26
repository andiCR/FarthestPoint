using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Algorithms;

public class Map : MonoBehaviour {
	
	#region Properties

	public int Width;
	public int Height;
	public TextMesh distancePrefab;
	
	#endregion
	
	#region Variables
	int[,] map;
	
	FloodFillAlgorithm		floodFillAlgorithm;
	RecursiveVisitAlgorithm	recursiveAlgorithm;
	Algorithm 				visualizedAlgorithm;
	
	Tile startPoint;
	Tile endPoint;
	
	System.TimeSpan recursiveVisitTime;
	System.TimeSpan floodFillTime;
	#endregion
	
	#region Monobehaviour overrides
	void Start()
	{
		CreateAlgorithms();
		GenerateNewMap();
	}
	
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Space)) {
			GenerateNewMap ();
		}
	}
	
	void OnGUI()
	{
		Algorithm otherAlgorithm = (visualizedAlgorithm == floodFillAlgorithm)? (Algorithm)recursiveAlgorithm :(Algorithm) floodFillAlgorithm;
		
		// Draw 
		GUI.Label(new Rect(Screen.width * 0.85f,60,400, 20), "Recursive Visit: " + recursiveVisitTime.Ticks + " ticks"); 
		GUI.Label(new Rect(Screen.width * 0.85f,80,400, 20), "Flood Fill: " + floodFillTime.Ticks + " ticks"); 
		
		GUI.Label(new Rect(Screen.width * 0.85f - 22,120,200, 20), visualizedAlgorithm.Name);
		
		GUI.Label(new Rect(Screen.width * 0.85f,140,400, 20), visualizedAlgorithm.HeatMapRedValue);
		GUI.Label(new Rect(Screen.width * 0.85f,170,400, 20), visualizedAlgorithm.HeatMapGreenValue);
		GUIUtil.GUIDrawRect(new Rect(Screen.width * 0.85f - 22, 140, 20, 20), Color.red);
		GUIUtil.GUIDrawRect(new Rect(Screen.width * 0.85f - 22, 170, 20, 20), Color.green);
		
		if (GUI.Button(new Rect(Screen.width * 0.85f - 50, 200, 200, 50), "Swap to " + otherAlgorithm.Name))
		{
			visualizedAlgorithm = otherAlgorithm;
			DrawMap ();
		}
		
		if (GUI.Button(new Rect(Screen.width * 0.85f - 50, 260, 200, 50), "Regenerate map (space)"))
		{
			GenerateNewMap ();
		}
	}
	#endregion
	
	#region Private methods
	
	void GenerateNewMap()
	{
		CreateMap();
		CreateStartAndEndPoint();
		DrawMap();
	}
	
	void CreateMap()
	{
		float complexity = Random.Range (0.4f, 0.8f); // 0.6f
		map = new int[Width,Height];
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				map[i,j] = Random.value < complexity? 0: 1;
			}
		}
	}
	
	void CreateAlgorithms()
	{
		floodFillAlgorithm = new FloodFillAlgorithm(Width, Height);
		recursiveAlgorithm = new RecursiveVisitAlgorithm(Width, Height);
		
		visualizedAlgorithm = floodFillAlgorithm;
	}
	
	Tile GetEndPoint(Tile startPoint, Algorithm algorithm)
	{
		algorithm.Clear();
		
		algorithm.Analyze(ref map, startPoint);
		
		// Find max distance
		int maxDistance = 0;
		Tile maxDistancePoint = new Tile();
		for (int i = 0; i < Width; i++) {
			for (int j = 0; j < Height; j++) {
				if (algorithm.MapDistances[i,j] > maxDistance) {
					maxDistance = algorithm.MapDistances[i,j];
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
		endPoint = GetEndPoint(startPoint, recursiveAlgorithm);
		stopWatch.Stop();
		recursiveVisitTime = stopWatch.Elapsed;
		
		// Diagnose flood fill
		stopWatch = System.Diagnostics.Stopwatch.StartNew();
		endPoint = GetEndPoint(startPoint, floodFillAlgorithm);
		stopWatch.Stop();
		floodFillTime = stopWatch.Elapsed;
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
					text.text = visualizedAlgorithm.MapDistances[i, j].ToString();
					
					if (startPoint.x == i && startPoint.y == j)
						text.color = Color.red;
					
					if (endPoint.x == i && endPoint.y == j)
						text.color = Color.blue;
					
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.parent = text.transform;
					cube.transform.localPosition = Vector3.zero;
					float v = ((float)visualizedAlgorithm.HeatMap[i,j]) / visualizedAlgorithm.MaxHeatMap; ///10;;
					cube.renderer.material.color = new Color( 2.0f * v, 2.0f * (1 - v), 0);
					
					if (visualizedAlgorithm.HeatMap[i,j] == 0)
						cube.renderer.material.color = Color.blue;
					
					text.transform.position = new Vector3(i, 0, j);
					text.transform.parent = transform;
					
				}
			}
		}
	}
	#endregion
}
