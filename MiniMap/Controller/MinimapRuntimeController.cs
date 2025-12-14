using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Read-only controller for querying map data during gameplay.
/// This controller provides read-only access to the map data.
/// </summary>
public class MinimapRuntimeController
{
  private MapData mapData;

  public MinimapRuntimeController(MapData mapData)
  {
    this.mapData = mapData;
  }

  /// <summary>
  /// Gets the tile type at the specified position.
  /// </summary>
  public MapTileType GetTileAt(Vector2Int position)
  {
    return mapData.Grid.GetTileAt(position);
  }

  /// <summary>
  /// Gets the city at the specified position, if any.
  /// </summary>
  public City GetCityAt(Vector2Int position)
  {
    return mapData.GetCityAt(position);
  }

  /// <summary>
  /// Gets all roads connected to a city.
  /// </summary>
  public List<Road> GetRoadsFromCity(City city)
  {
    return city.GetAllRoads();
  }

  /// <summary>
  /// Checks if a position is valid (within map bounds).
  /// </summary>
  public bool IsValidPosition(Vector2Int position)
  {
    return mapData.Grid.IsValidPosition(position);
  }

  /// <summary>
  /// Gets all cities in the map.
  /// </summary>
  public List<City> GetAllCities()
  {
    return new List<City>(mapData.Cities);
  }

  /// <summary>
  /// Gets all roads in the map.
  /// </summary>
  public List<Road> GetAllRoads()
  {
    return new List<Road>(mapData.Roads);
  }

  /// <summary>
  /// Gets the map dimensions.
  /// </summary>
  public Vector2Int GetMapSize()
  {
    return new Vector2Int(mapData.Width, mapData.Height);
  }
}

