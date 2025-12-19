using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using UnityEngine;

public enum NocabCorner
{
  TopLeft = 1,
  TL = TopLeft,

  TopRight = 2,
  TR = TopRight,

  BottomRight = 3,
  BR = BottomRight,

  BottomLeft = 4,
  BL = BottomLeft,
}

public static class Corner_Util
{
  public static readonly List<NocabCorner> ALL_CORNERS = new()
  {
    NocabCorner.TopLeft,
    NocabCorner.TopRight,
    NocabCorner.BottomRight,
    NocabCorner.BottomLeft,
  };

  public static NocabCorner opposite(NocabCorner corner)
  {
    switch (corner)
    {
      case NocabCorner.TopLeft:
        return NocabCorner.BottomRight;
      case NocabCorner.TopRight:
        return NocabCorner.BottomLeft;
      case NocabCorner.BottomRight:
        return NocabCorner.TopLeft;
      case NocabCorner.BottomLeft:
        return NocabCorner.TopRight;
      default:
        return NocabCorner.TopLeft;
    }
  }

  public static string ToString(NocabCorner corner)
  {
    switch (corner)
    {
      case NocabCorner.TopLeft:
        return "TopLeft";
      case NocabCorner.TopRight:
        return "TopRight";
      case NocabCorner.BottomRight:
        return "BottomRight";
      case NocabCorner.BottomLeft:
        return "BottomLeft";
      default:
        return "Unknown_Corner";
    }
  }

  public static NocabCorner FromString(string corner)
  {
    switch (corner)
    {
      case "TopLeft":
        return NocabCorner.TopLeft;
      case "TopRight":
        return NocabCorner.TopRight;
      case "BottomRight":
        return NocabCorner.BottomRight;
      case "BottomLeft":
        return NocabCorner.BottomLeft;
      default:
        throw new ArgumentException($"Invalid corner: {corner}");
    }
  }

  public static Vector2Int GetVector(NocabCorner corner)
  {
    // Positive Y is up, positive X is right (Unity coordinate system)
    switch (corner)
    {
      case NocabCorner.TopLeft:
        return new Vector2Int(-1, 1);
      case NocabCorner.TopRight:
        return new Vector2Int(1, 1);
      case NocabCorner.BottomRight:
        return new Vector2Int(1, -1);
      case NocabCorner.BottomLeft:
        return new Vector2Int(-1, -1);
      default:
        return new Vector2Int(0, 0);
    }
  }

  public static Vector2Int GetVector(NocabCorner corner, int distance)
  {
    return GetVector(corner) * distance;
  }

  public static NocabCorner GetCorner(Vector2Int vector)
  {
    if (vector.x < 0 && vector.y > 0)
      return NocabCorner.TopLeft; // negative X, positive Y
    if (vector.x < 0 && vector.y < 0)
      return NocabCorner.BottomLeft; // negative X, negative Y
    if (vector.x > 0 && vector.y > 0)
      return NocabCorner.TopRight; // positive X, positive Y
    if (vector.x > 0 && vector.y < 0)
      return NocabCorner.BottomRight; // positive X, negative Y

    throw new ArgumentException($"Invalid vector: {vector}");
  }
}
