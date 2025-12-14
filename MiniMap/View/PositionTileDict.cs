using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PositionTileDict : Dictionary<Vector2Int, TileBase>
{
  public PositionTileDict(int capacity)
    : base(capacity) { }

  public PositionTileDict()
    : base() { }

  public void AddRoad(Road r, TileBase tb)
  {
    foreach (Vector2Int pos in r.tilesInOrder)
    {
      this[pos] = tb;
    }
  }

  public void AddCity(City c, TileBase tb)
  {
    this[c.position] = tb;
  }

  public void drawSelf(Tilemap tilemap)
  {
    foreach (var kvp in this)
    {
      Vector2Int pos = kvp.Key;
      TileBase tb = kvp.Value;

      tilemap.SetTile(new Vector3Int(pos.x, pos.y), tb);
    }
  }
}

public class TileLaydownLinter
{
  public TileLaydownLinter() { }

  public PositionTileDict SmartDrawTiles(PositionTileDict tiles)
  {
    SmartPathTileLookup tileLookup = new("Art/TilePalette/Paths/Tiles_Schwarnhild");
    PositionTileDict newDict = new(tiles.Count);

    foreach (KeyValuePair<Vector2Int, TileBase> kvp in tiles)
    {
      Vector2Int pos = kvp.Key;
      TileBase tb = kvp.Value;

      HashSet<CardinalDirection> neighbors = new(4);
      if (tiles.ContainsKey(pos + Vector2Int.up))
      {
        neighbors.Add(CardinalDirection.North);
      }
      if (tiles.ContainsKey(pos + Vector2Int.down))
      {
        neighbors.Add(CardinalDirection.South);
      }
      if (tiles.ContainsKey(pos + Vector2Int.right))
      {
        neighbors.Add(CardinalDirection.East);
      }
      if (tiles.ContainsKey(pos + Vector2Int.left))
      {
        neighbors.Add(CardinalDirection.West);
      }

      newDict[pos] = tileLookup.getTileBasedOnNeighbors(neighbors);
    }
    return newDict;
  }

  public void NameRoads(PositionTileDict tiles)
  {
    // Start at an arbitrary tile
    // Flood find all it's neighbors horizontally or vertically
    // This straight line will be a path
    // Then, along the path find any tile that has an out of line neighbor
    // This out of line neighbor...?

    foreach (KeyValuePair<Vector2Int, TileBase> kvp in tiles)
    {
      Vector2Int pos = kvp.Key;
      TileBase tb = kvp.Value;

      bool upNeighbor = tiles.ContainsKey(pos + Vector2Int.up);
      bool downNeighbor = tiles.ContainsKey(pos + Vector2Int.down);
      bool leftNeighbor = tiles.ContainsKey(pos + Vector2Int.left);
      bool rightNeighbor = tiles.ContainsKey(pos + Vector2Int.right);
    }
  }

  private HashSet<Vector2Int> FindLineFromTile(
    PositionTileDict tiles,
    Vector2Int position,
    Vector2Int direction
  )
  {
    HashSet<Vector2Int> linePositions = new();
    Vector2Int currentPosition = position;

    while (tiles.ContainsKey(currentPosition))
    {
      linePositions.Add(currentPosition);
      currentPosition += direction;
    }

    currentPosition = position - direction;
    while (tiles.ContainsKey(currentPosition))
    {
      linePositions.Add(currentPosition);
      currentPosition -= direction;
    }

    return linePositions;
  }

  private HashSet<Vector2Int> FindHorizontalLineFromTile(
    PositionTileDict tiles,
    Vector2Int position
  )
  {
    return FindLineFromTile(tiles, position, Vector2Int.right);
  }

  private HashSet<Vector2Int> FindVerticalLineFromTile(PositionTileDict tiles, Vector2Int position)
  {
    return FindLineFromTile(tiles, position, Vector2Int.up);
  }
}
