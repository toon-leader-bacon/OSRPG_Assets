using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Road
{

  public readonly List<Vector2Int> tilesInOrder;

  public Road(List<Vector2Int> tilesAlongRoad)
  {
    this.tilesInOrder = tilesAlongRoad;
  }
}