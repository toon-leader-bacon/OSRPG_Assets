using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for the entire map state. This is the core model that represents
/// the minimap data structure.
/// </summary>
public class MapData
{
  public MapGrid Grid { get; private set; }
  public List<City> Cities { get; private set; }
  public List<Road> Roads { get; private set; }

  public int Width => Grid.Width;
  public int Height => Grid.Height;

  public MapData(int width, int height)
  {
    Grid = new MapGrid(width, height);
    Cities = new List<City>();
    Roads = new List<Road>();
  }

  /// <summary>
  /// Adds a city to the map and updates the grid.
  /// </summary>
  public void AddCity(City city)
  {
    if (Cities.Contains(city))
    {
      return;
    }
    Cities.Add(city);
    Grid.SetTileAt(city.position, MapTileType.City);
  }

  /// <summary>
  /// Adds a road to the map and updates the grid.
  /// </summary>
  public void AddRoad(Road road)
  {
    if (Roads.Contains(road))
    {
      return;
    }
    Roads.Add(road);
    foreach (Vector2Int position in road.tilesInOrder)
    {
      // Only set as road if not already a city
      if (Grid.GetTileAt(position) != MapTileType.City)
      {
        Grid.SetTileAt(position, MapTileType.Road);
      }
    }
  }

  /// <summary>
  /// Gets the city at the specified position, if any.
  /// </summary>
  public City GetCityAt(Vector2Int position)
  {
    foreach (City city in Cities)
    {
      if (city.position == position)
      {
        return city;
      }
    }
    return null;
  }

  /// <summary>
  /// Clears all data from the map.
  /// </summary>
  public void Clear()
  {
    Grid.Clear();
    Cities.Clear();
    Roads.Clear();
  }
}

