using System;
using System.Collections.Generic;
using UnityEngine;

public class City
{
  public readonly Vector2Int position;

  Dictionary<CardinalDirection, Road> roads = new();

  public City(Vector2Int position)
  {
    this.position = position;
  }

  public City(int position_X, int position_Y)
    : this(new Vector2Int(position_X, position_Y)) { }

  public void insertRoad(Road newRoad, CardinalDirection direction)
  {
    this.roads[direction] = newRoad;
  }

  public bool directionOccupied(CardinalDirection direction)
  {
    return this.roads.ContainsKey(direction);
  }

  public bool directionUnoccupied(CardinalDirection direction)
  {
    return !this.directionOccupied(direction);
  }

  public List<CardinalDirection> OccupiedDirections()
  {
    List<CardinalDirection> result = new(this.roads.Count);
    foreach (CardinalDirection direction in this.roads.Keys)
    {
      result.Add(direction);
    }
    return result;
  }

  public List<CardinalDirection> UnoccupiedDirections()
  {
    List<CardinalDirection> result = new()
    {
      CardinalDirection.North,
      CardinalDirection.East,
      CardinalDirection.South,
      CardinalDirection.West,
    };

    foreach (CardinalDirection direction in this.roads.Keys)
    {
      result.Remove(direction);
    }

    return result;
  }

  /// <summary>
  /// Gets the road connected in the specified direction, if any.
  /// </summary>
  public Road GetRoad(CardinalDirection direction)
  {
    return roads.ContainsKey(direction) ? roads[direction] : null;
  }

  /// <summary>
  /// Gets all roads connected to this city.
  /// </summary>
  public List<Road> GetAllRoads()
  {
    return new List<Road>(roads.Values);
  }
}
