using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfoManager : MonoBehaviour
{
  public Tilemap tilemap;

  public TileSet activeTileSet;

  private Dictionary<TileBase, NocabTile> tileToNocabTileMap;

  void Start()
  {
    InitializeTileToNocabTileMap();
  }

  void InitializeTileToNocabTileMap()
  {
    tileToNocabTileMap = new Dictionary<TileBase, NocabTile>();

    if (activeTileSet != null)
    {
      foreach (TileAssociation association in activeTileSet.tileAssociations)
      {
        foreach (TileBase tile in association.tiles)
        {
          // Set the collider collision based on the data stored in the Nocab Tile prefab
          if (tile is Tile concreteTile)
          {
            concreteTile.colliderType = association.nocabTile.isWalkable ?
                                        Tile.ColliderType.None :
                                        Tile.ColliderType.Grid;
          }
          tileToNocabTileMap[tile] = association.nocabTile;
        }
      }
    }
  }

  public NocabTile GetTileAtWorldPosition(Vector3 worldPosition)
  {
    Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
    return GetTileAtCellPosition(cellPosition);
  }

  public NocabTile GetTileAtCellPosition(Vector3Int cellPosition)
  {
    TileBase tileBase = tilemap.GetTile(cellPosition);
    return GetNocabTileFromTileBase(tileBase);
  }

  public NocabTile GetNocabTileFromTileBase(TileBase tileBase)
  {
    if (tileToNocabTileMap.TryGetValue(tileBase, out NocabTile nocabTile))
    {
      return nocabTile;
    }
    else
    {
      Debug.LogWarning("No NocabTile found for TileBase: " + tileBase);
      return null;
    }
  }

}
