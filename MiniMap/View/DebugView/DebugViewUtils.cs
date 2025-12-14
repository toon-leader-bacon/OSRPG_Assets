using System;
using UnityEngine;

public static class DebugViewUtils
{
  public static Tuple<int, int> unityCoordsToGridCoords(
    int x,
    int y,
    int xOffset,
    int yOffset,
    int width,
    int height
  )
  {
    // Unity coordinates are (x, y) with (0,0) in the middle, positive X is up, positive Y is right
    // Grid coordinates are (x, y) with (0,0) in the top left, positive X is right, positive Y is down
    int gridX = x + xOffset;
    int gridY = y + yOffset;
    // Validate the grid coordinates are within the grid
    if ((gridX < 0) || (gridX >= width) || (gridY < 0) || (gridY >= height))
    {
      Debug.LogError($"Invalid grid coordinates: {x}, {y}");
      Debug.LogError($"Width: {width}, Height: {height}");
      Debug.LogError($"X Offset: {xOffset}, Y Offset: {yOffset}");
      Debug.LogError($"Grid X: {gridX}, Grid Y: {gridY}");
      throw new Exception("Invalid grid coordinates");
    }
    return new Tuple<int, int>(gridX, gridY);
  }
}
