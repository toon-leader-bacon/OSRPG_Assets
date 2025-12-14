using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D grid representing the tile map. Pure data structure with no Unity dependencies.
/// </summary>
public class MapGrid
{
  private readonly int width;
  private readonly int height;
  private readonly MapTileType[,] grid;

  public MapGrid(int width, int height)
  {
    this.width = width;
    this.height = height;
    this.grid = new MapTileType[width, height];
  }

  public int Width => width;
  public int Height => height;

  /// <summary>
  /// Gets the tile type at the specified position.
  /// Returns Empty if position is out of bounds.
  /// </summary>
  public MapTileType GetTileAt(Vector2Int position)
  {
    if (!IsValidPosition(position))
    {
      return MapTileType.Empty;
    }
    return grid[position.x, position.y];
  }

  /// <summary>
  /// Sets the tile type at the specified position.
  /// Does nothing if position is out of bounds.
  /// </summary>
  public void SetTileAt(Vector2Int position, MapTileType tileType)
  {
    if (!IsValidPosition(position))
    {
      return;
    }
    grid[position.x, position.y] = tileType;
  }

  /// <summary>
  /// Checks if the position is within the grid bounds.
  /// </summary>
  public bool IsValidPosition(Vector2Int position)
  {
    return position.x >= 0 && position.x < width &&
           position.y >= 0 && position.y < height;
  }

  /// <summary>
  /// Clears all tiles in the grid (sets to Empty).
  /// </summary>
  public void Clear()
  {
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        grid[x, y] = MapTileType.Empty;
      }
    }
  }

  /// <summary>
  /// Gets all positions that have the specified tile type.
  /// </summary>
  public List<Vector2Int> GetPositionsOfType(MapTileType tileType)
  {
    List<Vector2Int> positions = new List<Vector2Int>();
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        if (grid[x, y] == tileType)
        {
          positions.Add(new Vector2Int(x, y));
        }
      }
    }
    return positions;
  }
}

