using System;
namespace Algorithms
{
	public abstract class Algorithm
	{
		public int[,] MapDistances;
		public int[,] HeatMap;
		
		public string HeatMapRedValue;
		public string HeatMapGreenValue;
		public string Name;
		public int MaxHeatMap;
		
		protected int mapWidth;
		protected int mapHeight;
		
		protected Algorithm (string name, string heatMapRedValue, string heatMapGreenValue, int mapWidth, int mapHeight)
		{
			this.Name = name;
			this.HeatMapGreenValue = heatMapGreenValue;
			this.HeatMapRedValue = heatMapRedValue;
			this.MapDistances = new int[mapWidth, mapHeight];
			this.HeatMap = new int[mapWidth, mapHeight];
			
			this.mapWidth = mapWidth;
			this.mapHeight = mapHeight;
			
			Clear();
		}
		
		public void Clear()
		{
			// Clear map distances
			for (int i = 0; i < mapWidth; i++) {
				for (int j = 0; j < mapHeight; j++) {
					MapDistances[i, j] = -1;
					HeatMap[i,j] = 0;
				}
			}
		}
		
		public abstract void Analyze(ref int [,] map, Tile startTile);
	}
}