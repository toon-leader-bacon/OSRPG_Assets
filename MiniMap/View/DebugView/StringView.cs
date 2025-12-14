using System;
using System.Text;
using UnityEngine;

public class StringView
{
  /**
   * A view that renders map data to a string.
   * Typically it takes in a collection of Roads and Cities, and returns a String
   * This string is a 2d array of characters, where each character represents a tile.
   */
  int width;
  public int Width => width;
  int height;
  public int Height => height;

  int xOffset;
  int yOffset;
  char[,] grid;

  public StringView(int width, int height, char defaultCharacter = '.')
  {
    this.width = width; // Width of the renderable area
    this.height = height; // Height of the renderable area

    // Assume the center of the renderable area is always the middle of the grind
    // IE: The Unity coordinate (0,0) will be the center of the grid.
    this.xOffset = width / 2;
    this.yOffset = height / 2;

    this.grid = new char[width, height]; // 2d array of characters
    initializeGrid(defaultCharacter);
  }

  public StringView(int minX, int maxX, int minY, int maxY, char defaultCharacter = '.')
  {
    this.width = maxX - minX + 1;
    this.height = maxY - minY + 1;
    this.xOffset = this.width / 2;
    this.yOffset = this.height / 2;
    this.grid = new char[width, height];
    initializeGrid(defaultCharacter);
  }

  private void initializeGrid(char defaultCharacter = '.')
  {
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        grid[x, y] = defaultCharacter;
      }
    }
  }

  public Tuple<int, int> unityCoordsToGridCoords(int x, int y)
  {
    return DebugViewUtils.unityCoordsToGridCoords(x, y, xOffset, yOffset, width, height);
  }

  public void AddRoad(Road road, char character = 'X')
  {
    foreach (Vector2Int pos in road.tilesInOrder)
    {
      Tuple<int, int> gridCoords = unityCoordsToGridCoords(pos.x, pos.y);
      grid[gridCoords.Item1, gridCoords.Item2] = character;
    }
  }

  public void AddCity(City city, char character = 'C')
  {
    Tuple<int, int> gridCoords = unityCoordsToGridCoords(city.position.x, city.position.y);
    grid[gridCoords.Item1, gridCoords.Item2] = character;
  }

  public string Render(bool positiveYIsUp = true)
  {
    StringBuilder sb = new();
    if (!positiveYIsUp)
    {
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          sb.Append(grid[x, y]);
        }
        sb.AppendLine();
      }
    }
    else
    {
      for (int y = height - 1; y >= 0; y--)
      {
        for (int x = 0; x < width; x++)
        {
          sb.Append(grid[x, y]);
        }
        sb.AppendLine();
      }
    }
    return sb.ToString();
  }
}
