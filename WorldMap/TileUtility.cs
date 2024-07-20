using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileUtility
{

  public static Vector3 SnapToGrid(Vector3 position)
  {
    float x = Mathf.Round(position.x);
    float y = Mathf.Round(position.y);
    return new Vector3(x, y, position.z);
  }
}
