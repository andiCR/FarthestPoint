using System;
namespace Algorithms
{
	public class FloodFillAlgorithm: Algorithm
	{
		public FloodFillAlgorithm (int mapWidth, int mapHeight)
		: base("Flood fill", "Longer", "Closer", mapWidth, mapHeight)
		{
		}
	
		public override void Analyze(ref int [,] map, Tile startTile)
		{
			// We have two buffers. The tiles being processed, and the tiles that are
			// to be processed on the next iteration. This is way faster than having
			// a generic list and adding tiles on the fly.
			Tile[,] tileBuffer = new Tile[2,100];
			int currentBuffer = 0;
			int nextBuffer = 1;
			
			int currentBufferCount = 1;
			int nextBufferCount = 0;
			int distance = 0;
			int x = startTile.x;
			int y = startTile.y;
			
			// Add the starting tile to the buffer so that it's processed
			tileBuffer[currentBuffer, 0].x = x;
			tileBuffer[currentBuffer, 0].y = y;
			
			while (currentBufferCount > 0)
			{
				nextBufferCount = 0;
				for (int i = 0; i < currentBufferCount; i++)
				{
					// Get x and y of current tile
					x = tileBuffer[currentBuffer, i].x;
					y = tileBuffer[currentBuffer, i].y;
					
					// "visit"
					MapDistances[x, y] = distance;
					HeatMap[x, y] = distance + 1;
					
					// Check if we should add contiguous tiles to the next buffer
					// Left tile
					if (x > 0 && map[x-1, y] == 0 && MapDistances[x-1, y] == -1)
					{
						tileBuffer[nextBuffer, nextBufferCount].x = x-1;
						tileBuffer[nextBuffer, nextBufferCount].y = y;
						MapDistances[x-1, y] = distance + 1;
						nextBufferCount++;
					}
					// Right tile
					if (x < map.GetLength(0)-1 && map[x+1, y] == 0 && MapDistances[x+1, y] == -1)
					{
						tileBuffer[nextBuffer, nextBufferCount].x = x+1;
						tileBuffer[nextBuffer, nextBufferCount].y = y;
						MapDistances[x+1, y] = distance + 1;
						nextBufferCount++;
					}
					// Top tile
					if (y > 0 && map[x, y-1] == 0 && MapDistances[x, y-1] == -1) 
					{
						tileBuffer[nextBuffer, nextBufferCount].x = x;
						tileBuffer[nextBuffer, nextBufferCount].y = y-1;
						MapDistances[x, y-1] = distance + 1;
						nextBufferCount++;
					}
					// Bottom tile
					if (y < map.GetLength(1)-1 && map[x, y+1] == 0 && MapDistances[x, y+1] == -1)
					{
						tileBuffer[nextBuffer, nextBufferCount].x = x;
						tileBuffer[nextBuffer, nextBufferCount].y = y + 1;
						MapDistances[x, y+1] = distance + 1;
						nextBufferCount++;
					}
				}
				// Swap buffers
				nextBuffer = (nextBuffer + 1 ) % 2;
				currentBuffer = (currentBuffer + 1 ) % 2;
				currentBufferCount = nextBufferCount;
				distance++;
			}
			
			MaxHeatMap = distance - 1;
		}
	}
}

