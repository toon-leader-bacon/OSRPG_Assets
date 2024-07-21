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

  public void DrawRoad(List<Vector2Int> pts)
  {
    foreach (Vector2Int pt in pts)
    {
      tilemap.SetTile(new Vector3Int(pt.x, pt.y), pathTile);
    }
  }


  public void DrawCity(List<Vector2Int> pts)
  {
    foreach (Vector2Int pt in pts)
    {
      tilemap.SetTile(new Vector3Int(pt.x, pt.y), cityTile);
    }
  }

  public List<Vector2Int> CircleOfCities()
  {
    /**
     * First, create a collection of points that represent the main loop
     * these points will be the main cities
     */
    NocabRNG rng = NocabRNG.defaultRNG;
    Box2D mainLoop = new(0, 0, 20, 10, positiveYDown: true);

    List<Vector2Int> cityPoints = new();

    // Left edge:
    int leftCityCount = 1;
    int horizontalWiggleRange = 2;
    int verticalWiggleRange = 2;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.TL_v, mainLoop.BL_v, leftCityCount))
    {
      cityPoints.Add(new Vector2Int(
        (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
        (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
      ));
    }

    // bottom edge:
    int bottomCityCount = 4;
    horizontalWiggleRange = 0;
    verticalWiggleRange = 4;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.BL_v, mainLoop.BR_v, bottomCityCount))
    {
      cityPoints.Add(new Vector2Int(
        (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
        (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
      ));
    }


    // right edge:
    int rightCityCount = 2;
    horizontalWiggleRange = 1;
    verticalWiggleRange = 1;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.BR_v, mainLoop.TR_v, rightCityCount))
    {
      cityPoints.Add(new Vector2Int(
        (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
        (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
      ));
    }

    // top edge:
    int topCityCount = 3;
    horizontalWiggleRange = 1;
    verticalWiggleRange = 2;
    foreach (Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.TR_v, mainLoop.TL_v, topCityCount))
    {
      cityPoints.Add(new Vector2Int(
        (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
        (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
      ));
    }

    return cityPoints;
  }

  public List<Vector2Int> ConnectCitiesInOrder(List<Vector2Int> cityPoints)
  {
    List<Vector2Int> result = new();
    if (cityPoints.Count == 0)
    {
      return result;
    }
    ElbowLine el = new(NocabRNG.newRNG);

    Vector2Int pointA = cityPoints[0];
    for (int i = 1; i < cityPoints.Count; i++)
    {
      Vector2Int pointB = cityPoints[i];
      result.AddRange(el.ConnectPoints(pointA, pointB));
      pointA = pointB;
    }
    return result;
  }

}
