using System;
namespace Algorithms
{
	public class RecursiveVisitAlgorithm : Algorithm
	{
		public RecursiveVisitAlgorithm (int mapWidth, int mapHeight)
			: base("Recursive visit", "High visit count", "Low visit count", mapWidth, mapHeight)
		{
		}
		
		public override void Analyze(ref int [,] map, Tile startTile)
		{
			MaxHeatMap = 0;
			
			TryVisit(ref map, startTile.x, startTile.y, 0);
			
			this.HeatMapRedValue = "High visit count (" + MaxHeatMap + ")";
		}
		
		void TryVisit(ref int [,] map, int x, int y, int distance)
		{
			if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
				return;
			
			if (map [x, y] == 0 && (MapDistances [x, y] == -1 || MapDistances[x, y] > distance)) {
				VisitCell(ref map, x, y, distance);
			}
		}
		
		void VisitCell(ref int [,] map,  int x, int y, int distance)
		{
			HeatMap[x,y] ++;
			
			// Set tile's current distance
			MapDistances[x, y] = distance;
			
			// Visit neighbor cells
			TryVisit(ref map, x - 1, y, distance + 1);
			TryVisit(ref map, x + 1, y, distance + 1);
			TryVisit(ref map, x, y - 1, distance + 1);
			TryVisit(ref map, x, y + 1, distance + 1);
			
			if (MaxHeatMap < HeatMap[x,y])
				MaxHeatMap = HeatMap[x,y];
		}
	}
}

