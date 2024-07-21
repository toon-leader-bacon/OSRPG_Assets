using System.Collections.Generic;
using UnityEngine;

public static class MiniMapUtilities
{
  public static List<Vector2> splitLine(Vector2 start, Vector2 end, int n)
  {
    List<Vector2> result = new List<Vector2>();
    if (n <= 0)
    {
      return result;
    }

    n += 1; // Skip the 0 point, instead return the next one and go from there.
    for (int i = 1; i < n; i++)
    {
      float t = i / (float)n;
      result.Add(Vector2.Lerp(start, end, t));
    }

    return result;
  }
}