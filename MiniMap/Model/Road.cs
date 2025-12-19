using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Represents a road as an ordered sequence of tile positions.
/// Roads are effectively immutable - all modification methods return new Road instances.
/// </summary>
public class Road : IEquatable<Road>
{
  public readonly List<Vector2Int> tilesInOrder;

  public Vector2Int StartPoint
  {
    get { return tilesInOrder[0]; }
  }
  public Vector2Int EndPoint
  {
    get { return tilesInOrder[tilesInOrder.Count - 1]; }
  }

  public int Length => tilesInOrder.Count;

  public Road(List<Vector2Int> tilesAlongRoad)
  {
    this.tilesInOrder = tilesAlongRoad;
  }

  /// <summary>Copy constructor</summary>
  public Road(Road other)
    : this(new List<Vector2Int>(other.tilesInOrder)) { }

  public Road Clone()
  {
    return new Road(this);
  }

  #region Equality & Hashing

  public bool Equals(Road other)
  {
    if (other is null)
      return false;
    if (ReferenceEquals(this, other))
      return true;
    return tilesInOrder.SequenceEqual(other.tilesInOrder);
  }

  public override bool Equals(object obj)
  {
    return Equals(obj as Road);
  }

  public override int GetHashCode()
  {
    return NocabHashUtility.generateHash(tilesInOrder);
  }

  public static bool operator ==(Road left, Road right)
  {
    if (left is null)
      return right is null;
    return left.Equals(right);
  }

  public static bool operator !=(Road left, Road right)
  {
    return !(left == right);
  }

  #endregion

  public override string ToString()
  {
    if (tilesInOrder.Count == 0)
      return "Road(empty)";
    return $"Road({StartPoint} -> {EndPoint}, {Length} tiles)";
  }

  public Road RemovePoint(Vector2Int point, bool quiet = true)
  {
    /**
     * WARNING: This method is fairly dumb. Removing a point from the middle of a road, while possible,
     * does not make much sense. It's probably better to break the road into 2 roads.
     *
     * This method will remove a point from the middle of the road without warning, so this one object
     * may have a disconnected list of tiles. You have been warned...
     *
     * Remove a point from the road.
     * If the point is not found, and quiet is false, an error will be thrown instead.
     * Otherwise, if quiet is true and the point is not found, the returned road will be the same as the original road.
     *
     * @param point - The point to remove.
     * @param quiet - If true, no error will be logged if the point is not found.
     * @return The new road.
     */
    if (!this.tilesInOrder.Contains(point))
    {
      if (!quiet)
      {
        throw new System.Exception($"Point {point} not found in road {this}");
      }
      // No target point found, but we are quiet. So just make a copy and return it.
      return this.Clone();
    }
    // Else the point is found, so make a copy and remove it
    List<Vector2Int> newTiles = new(this.tilesInOrder);
    newTiles.Remove(point);
    return new Road(newTiles);
  }
}
