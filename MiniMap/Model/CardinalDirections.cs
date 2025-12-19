using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using UnityEngine;

public enum CardinalDirection
{
  North = 1,
  East = 2,
  South = 3,
  West = 4,
}

public static class CardinalDirection_Util
{
  public static readonly List<CardinalDirection> ALL_DIRECTIONS = new()
  {
    CardinalDirection.North,
    CardinalDirection.South,
    CardinalDirection.East,
    CardinalDirection.West,
  };

  public static CardinalDirection Opposite(CardinalDirection direction)
  {
    switch (direction)
    {
      case CardinalDirection.North:
        return CardinalDirection.South;
      case CardinalDirection.South:
        return CardinalDirection.North;
      case CardinalDirection.East:
        return CardinalDirection.West;
      case CardinalDirection.West:
        return CardinalDirection.East;
      default:
        return CardinalDirection.North;
    }
  }

  public static List<CardinalDirection> OrthogonalTo(CardinalDirection direction)
  {
    switch (direction)
    {
      case CardinalDirection.North:
      case CardinalDirection.South:
        return new() { CardinalDirection.East, CardinalDirection.West };
      case CardinalDirection.East:
      case CardinalDirection.West:
        return new() { CardinalDirection.North, CardinalDirection.South };
      default:
        throw new ArgumentException($"Invalid direction: {direction}");
    }
  }

  public static string ToString(CardinalDirection direction)
  {
    switch (direction)
    {
      case CardinalDirection.North:
        return "North";
      case CardinalDirection.South:
        return "South";
      case CardinalDirection.East:
        return "East";
      case CardinalDirection.West:
        return "West";
      default:
        return "Unknown_Direction";
    }
  }

  public static CardinalDirection FromString(string direction)
  {
    switch (direction)
    {
      case "North":
        return CardinalDirection.North;
      case "South":
        return CardinalDirection.South;
      case "East":
        return CardinalDirection.East;
      case "West":
        return CardinalDirection.West;
      default:
        throw new ArgumentException($"Invalid direction: {direction}");
    }
  }

  public static Vector2Int GetVector(CardinalDirection direction)
  {
    // Assume positive Y is upwards.
    switch (direction)
    {
      case CardinalDirection.North:
        return new Vector2Int(0, 1);
      case CardinalDirection.East:
        return new Vector2Int(1, 0);
      case CardinalDirection.South:
        return new Vector2Int(0, -1);
      case CardinalDirection.West:
        return new Vector2Int(-1, 0);
      default:
        return new Vector2Int(0, 0);
    }
  }

  public static Vector2Int GetVector(CardinalDirection direction, int distance)
  {
    return GetVector(direction) * distance;
  }

  public static CardinalDirection GetDirection(Vector2Int vector)
  {
    if (vector.x == 0 && vector.y > 0)
      return CardinalDirection.North;
    if (vector.x == 0 && vector.y < 0)
      return CardinalDirection.South;
    if (vector.x > 0 && vector.y == 0)
      return CardinalDirection.East;
    if (vector.x < 0 && vector.y == 0)
      return CardinalDirection.West;

    throw new ArgumentException($"Invalid vector: {vector}");
  }
}
