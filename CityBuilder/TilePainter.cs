using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePainter : MonoBehaviour
{
  public Tilemap tilemap;
  public TileBase tile;

  void placeTile(Vector2Int position, TileBase tile)
  {
    tilemap.SetTile(StaticVectorTools.V3I(position), tile);
  }

  void removeTile(Vector2Int position)
  {
    tilemap.SetTile(StaticVectorTools.V3I(position), null);
  }
}
