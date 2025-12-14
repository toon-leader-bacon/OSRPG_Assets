using System;
using System.IO;
using UnityEngine;

public class PictureView
{
  /**
   * A view that renders map data to a picture.
   * Typically it takes in a collection of Roads and Cities, and returns a PNG image.
   * This picture is a 2d array of characters, where each pixel represents a tile.
   */
  int width;
  public int Width => width;
  int height;
  public int Height => height;

  int xOffset;
  int yOffset;
  Color[,] grid;

  public PictureView(int width, int height, Color defaultColor)
  {
    this.width = width;
    this.height = height;
    this.grid = new Color[width, height];
    initializeGrid(defaultColor);
  }

  public PictureView(int width, int height)
    : this(width, height, Color.white) { }

  public PictureView(int minX, int maxX, int minY, int maxY, Color defaultColor)
  {
    this.width = maxX - minX + 1;
    this.height = maxY - minY + 1;
    this.xOffset = this.width / 2;
    this.yOffset = this.height / 2;
    this.grid = new Color[width, height];
    initializeGrid(defaultColor);
  }

  public PictureView(int minX, int maxX, int minY, int maxY)
    : this(minX, maxX, minY, maxY, Color.white) { }

  private void initializeGrid(Color defaultColor)
  {
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        grid[x, y] = defaultColor;
      }
    }
  }

  public Tuple<int, int> unityCoordsToGridCoords(int x, int y)
  {
    return DebugViewUtils.unityCoordsToGridCoords(x, y, xOffset, yOffset, width, height);
  }

  public void AddRoad(Road road, Color color)
  {
    foreach (Vector2Int pos in road.tilesInOrder)
    {
      Tuple<int, int> gridCoords = unityCoordsToGridCoords(pos.x, pos.y);
      grid[gridCoords.Item1, gridCoords.Item2] = color;
    }
  }

  public void AddRoad(Road road)
  {
    AddRoad(road, Color.green);
  }

  public void AddCity(City city, Color color)
  {
    Tuple<int, int> gridCoords = unityCoordsToGridCoords(city.position.x, city.position.y);
    grid[gridCoords.Item1, gridCoords.Item2] = color;
  }

  public void AddCity(City city)
  {
    AddCity(city, Color.red);
  }

  public void Render(string filename, bool positiveYIsUp = true)
  {
    // Note: Unity Texture2D is a 2d array of colors, with (0,0) in the bottom left
    // If positiveYIsUp is false, we need to flip the grid vertically
    Texture2D texture = new Texture2D(width, height);
    if (!positiveYIsUp)
    {
      // Flip the grid vertically
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          texture.SetPixel(x, height - y - 1, grid[x, y]);
        }
      }
    }
    else
    {
      for (int y = height - 1; y >= 0; y--)
      {
        for (int x = 0; x < width; x++)
        {
          texture.SetPixel(x, y, grid[x, y]);
        }
      }
    }
    texture.Apply();
    byte[] bytes = texture.EncodeToPNG();
    File.WriteAllBytes(filename, bytes);
  }
}
