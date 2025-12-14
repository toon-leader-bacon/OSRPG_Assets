using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Unity MonoBehaviour that renders the MapData model to a Unity Tilemap.
/// This is the View layer - it only handles rendering, no business logic.
/// </summary>
public class MinimapView : MonoBehaviour
{
  [SerializeField] private Tilemap tilemap;
  [SerializeField] private TileBase cityTile;
  [SerializeField] private TileBase roadTile;

  /// <summary>
  /// Renders the entire map data to the tilemap.
  /// </summary>
  public void RenderMap(MapData mapData)
  {
    if (tilemap == null)
    {
      Debug.LogWarning("MinimapView: Tilemap is not assigned!");
      return;
    }

    // Clear the tilemap first
    tilemap.ClearAllTiles();

    // Render all roads
    if (roadTile != null)
    {
      foreach (Road road in mapData.Roads)
      {
        foreach (Vector2Int position in road.tilesInOrder)
        {
          // Only draw road if not a city (cities take priority)
          if (mapData.Grid.GetTileAt(position) != MapTileType.City)
          {
            tilemap.SetTile(new Vector3Int(position.x, position.y, 0), roadTile);
          }
        }
      }
    }

    // Render all cities (cities are drawn on top of roads)
    if (cityTile != null)
    {
      foreach (City city in mapData.Cities)
      {
        tilemap.SetTile(new Vector3Int(city.position.x, city.position.y, 0), cityTile);
      }
    }
  }

  /// <summary>
  /// Updates a single tile at the specified position.
  /// </summary>
  public void UpdateTileAt(Vector2Int position, MapTileType tileType)
  {
    if (tilemap == null)
    {
      return;
    }

    TileBase tile = GetTileForType(tileType);
    tilemap.SetTile(new Vector3Int(position.x, position.y, 0), tile);
  }

  /// <summary>
  /// Gets the appropriate tile sprite for a given tile type.
  /// </summary>
  private TileBase GetTileForType(MapTileType tileType)
  {
    return tileType switch
    {
      MapTileType.City => cityTile,
      MapTileType.Road => roadTile,
      _ => null,
    };
  }
}

