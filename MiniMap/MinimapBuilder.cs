using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public struct CardinalDirectionsBlocked
{
  public bool UpBlocked;
  public bool DownBlocked;
  public bool LeftBlocked;
  public bool RightBlocked;

  public CardinalDirectionsBlocked(bool upBlocked = false,
                                   bool downBlocked = false,
                                   bool leftBlocked = false,
                                   bool rightBlocked = false)
  {
    UpBlocked = upBlocked;
    DownBlocked = downBlocked;
    LeftBlocked = leftBlocked;
    RightBlocked = rightBlocked;
  }
}


public class MinimapBuilder : MonoBehaviour
{

  public Tilemap tilemap;
  public TileBase pathTile;
  public TileBase cityTile;

  public void drawRoad(List<Vector2Int> pts)
  {
    foreach (Vector2Int pt in pts)
    {
      tilemap.SetTile(new Vector3Int(pt.x, pt.y), pathTile);
    }
  }


  public List<Vector2Int> blab()
  {
    /**
     * First, create a collection of points that represent the main loop
     * these points will be the main cities
     */
    Box2D mainLoop = new(0, 0, 20, 10, positiveYDown: true);

    List<Vector2Int> cityPoints = new();

    // Left edge:
    int leftCityCount = 1;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.TL_v, mainLoop.BL_v, leftCityCount))
    {
      cityPoints.Add(new Vector2Int((int)cityPoint.x, (int)cityPoint.y));
    }

    // bottom edge:
    int bottomCityCount = 4;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.BL_v, mainLoop.BR_v, bottomCityCount))
    {
      cityPoints.Add(new Vector2Int((int)cityPoint.x, (int)cityPoint.y));
    }


    // right edge:
    int rightCityCount = 2;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.BR_v, mainLoop.TR_v, rightCityCount))
    {
      cityPoints.Add(new Vector2Int((int)cityPoint.x, (int)cityPoint.y));
    }

    // top edge:
    int topCityCount = 3;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.TR_v, mainLoop.TL_v, topCityCount))
    {
      cityPoints.Add(new Vector2Int((int)cityPoint.x, (int)cityPoint.y));
    }


    return cityPoints;

  }


}
