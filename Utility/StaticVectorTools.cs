using UnityEngine;


public static class StaticVectorTools
{

  public static Vector2Int V2I(Vector3Int vec)
  {
    return new Vector2Int(vec.x, vec.y);
  }

  public static Vector3Int V3I(Vector2Int vec, int z = 0)
  {
    return new Vector3Int(vec.x, vec.y, 0);
  }
}