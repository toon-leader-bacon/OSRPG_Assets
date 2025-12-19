using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

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

  public void DrawRoad(List<Road> roads)
  {
    foreach (Road r in roads)
    {
      this.DrawRoad(r.tilesInOrder);
    }
  }

  public void DrawCity(Vector2Int pt)
  {
    tilemap.SetTile(new Vector3Int(pt.x, pt.y), cityTile);
  }

  public void DrawCity(List<Vector2Int> pts)
  {
    foreach (Vector2Int pt in pts)
    {
      DrawCity(pt);
    }
  }

  public void DrawCity(List<City> cities)
  {
    foreach (City city in cities)
    {
      DrawCity(city.position);
    }
  }

  public List<City> CircleOfCities()
  {
    /**
     * First, create a collection of points that represent the main loop
     * these points will be the main cities
     */
    NocabRNG rng = NocabRNG.defaultRNG;
    Box2D mainLoop = new(0, 0, 20, 10, positiveYDown: true);

    List<City> cityPoints = new();

    // Left edge:
    int leftCityCount = 1;
    int horizontalWiggleRange = 2;
    int verticalWiggleRange = 2;
    foreach (
      Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.TL_v, mainLoop.BL_v, leftCityCount)
    )
    {
      cityPoints.Add(
        new City(
          (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
          (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
        )
      );
    }

    // bottom edge:
    int bottomCityCount = 4;
    horizontalWiggleRange = 0;
    verticalWiggleRange = 4;
    foreach (
      Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.BL_v, mainLoop.BR_v, bottomCityCount)
    )
    {
      cityPoints.Add(
        new City(
          (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
          (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
        )
      );
    }

    // right edge:
    int rightCityCount = 2;
    horizontalWiggleRange = 1;
    verticalWiggleRange = 1;
    foreach (
      Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.BR_v, mainLoop.TR_v, rightCityCount)
    )
    {
      cityPoints.Add(
        new City(
          (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
          (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
        )
      );
    }

    // top edge:
    int topCityCount = 3;
    horizontalWiggleRange = 1;
    verticalWiggleRange = 2;
    foreach (
      Vector2 cityPoint in MiniMapUtilities.splitLine(mainLoop.TR_v, mainLoop.TL_v, topCityCount)
    )
    {
      cityPoints.Add(
        new City(
          (int)cityPoint.x + rng.generateInt(-horizontalWiggleRange, horizontalWiggleRange),
          (int)cityPoint.y + rng.generateInt(-verticalWiggleRange, verticalWiggleRange)
        )
      );
    }

    return cityPoints;
  }

  Road connectCitiesInLine(City cityA, City cityB)
  {
    Road r = new Road(
      ElbowLine.ConnectPoints(
        cityA.position,
        cityB.position,
        // Ignored b/c straight line
        false,
        ElbowLine.ElbowTypes.OneTurn
      )
    );
    // Is the road vertical or horizontal
    bool isVertical = cityA.position.x == cityB.position.x;
    // Is city A 'first' (ie further south or west)
    bool isCityAFirst = isVertical
      ? cityA.position.y < cityB.position.y
      : cityA.position.x < cityB.position.x;

    CardinalDirection leavingA = isVertical
      // If it's vertical, and city a is first, road goes out of city A northward, into city B
      ? (isCityAFirst ? CardinalDirection.North : CardinalDirection.South)
      // Else it's not vertical, line goes out of city A eastward into city B
      : (isCityAFirst ? CardinalDirection.East : CardinalDirection.West);

    // The road entering city B is always just the opposite of that leaving city A
    CardinalDirection enteringB = CardinalDirection_Util.Opposite(leavingA);

    cityA.insertRoad(r, leavingA);
    cityB.insertRoad(r, enteringB);
    return r;
  }

  // TODO: Make this less public
  public Road findValidConnector(City cityA, City cityB)
  {
    // Step 0: Check if cities are in line
    if (cityA.position.x == cityB.position.x || cityA.position.y == cityB.position.y)
    {
      return connectCitiesInLine(cityA, cityB);
    }

    // Step 1: Determine the relative positions of the two cities
    Box2D citiesOnCorners = new(
      cityA.position.x,
      cityA.position.y,
      cityB.position.x,
      cityB.position.y,
      positiveYDown: false
    );

    CardinalDirection cityAHoriz = citiesOnCorners.IsOnLeftEdge(cityA.position)
      ? // If City A is on the left edge of the box
      CardinalDirection.East
      : // Then the horizontal direction towards cityB must be East
      CardinalDirection.West; // Otherwise, city A is on the right edge and city B is westward
    Debug.Log($"CityAHoriz = {cityAHoriz}");

    CardinalDirection cityAVert = citiesOnCorners.IsOnTopEdge(cityA.position)
      ? // If City A is on the top edge of the box
      CardinalDirection.South
      : // Then the vert direction towards cityB must be South
      CardinalDirection.North; // Otherwise, city A is on the bottom edge and city B is Northwards
    Debug.Log($"cityAVert = {cityAVert}");

    // City B is always in the opposite corner of city A
    CardinalDirection cityBHoriz = CardinalDirection_Util.Opposite(cityAHoriz);
    CardinalDirection cityBVert = CardinalDirection_Util.Opposite(cityAVert);

    // Step 2: Consider what road slots in the cities are already taken.
    // To help visualize: Assume cityA is (1,1) and cityB is (3, 5)
    // Use this ascii art to see all the possible roads (1 and 2 turns)
    /*
      5 |  +--+--B
      4 |  |  |  |
      3 |  +-----+
      2 |  |  |  |
      1 |  A--+--+
      0 +-----------------
        0  1  2  3  4
     */

    List<
      Tuple<
        bool, // leave city A horizontally?
        ElbowLine.ElbowTypes, // Road type
        CardinalDirection, // dir leaving city A
        CardinalDirection // dir entering city b
      >
    > possibleRoads = new();
    // Generate Possible roads leaving City A horizontally
    if (cityA.directionUnoccupied(cityAHoriz))
    {
      bool horizontal = true;
      if (cityB.directionUnoccupied(cityBVert))
      {
        // 1Turn a.East -> b.South is valid
        possibleRoads.Add(new(horizontal, ElbowLine.ElbowTypes.OneTurn, cityAHoriz, cityBVert));
      }
      if (cityB.directionUnoccupied(cityBHoriz))
      {
        // 2turn a.East -> b.West is valid
        possibleRoads.Add(new(horizontal, ElbowLine.ElbowTypes.TwoTurn, cityAHoriz, cityBHoriz));
      }
    }

    // Possible roads leaving City B vertically
    if (cityA.directionUnoccupied(cityAVert))
    {
      bool horizontal = false;
      if (cityB.directionUnoccupied(cityBVert))
      {
        // 2Turn a.North -> b.South is valid
        possibleRoads.Add(new(horizontal, ElbowLine.ElbowTypes.TwoTurn, cityAVert, cityBVert));
      }
      if (cityB.directionUnoccupied(cityBHoriz))
      {
        // 1turn a.North -> b.West is valid
        possibleRoads.Add(new(horizontal, ElbowLine.ElbowTypes.OneTurn, cityAVert, cityBHoriz));
      }
    }

    if (possibleRoads.Count == 0)
    {
      Debug.Log(
        $"No possible roads. Making empty road.\n"
          + $"Position of CityA: {cityA.position}\n"
          + $"Position of CityB: {cityB.position}"
      );
      return new Road(new List<Vector2Int>());
    }

    // All possible paths have been generated. Select one, generate it
    var roadData = NocabRNG.newRNG.randomElem(possibleRoads);
    bool roadHorizontalStart = roadData.Item1;
    ElbowLine.ElbowTypes roadType = roadData.Item2;

    Road road = new Road(
      ElbowLine.ConnectPoints(cityA.position, cityB.position, roadHorizontalStart, roadType)
    );

    // Now, hook that road up to the cities properly.
    // Connecting the correct output direction slot
    CardinalDirection leavingCityA = roadData.Item3;
    CardinalDirection enteringCityB = roadData.Item4;

    cityA.insertRoad(road, leavingCityA);
    cityB.insertRoad(road, enteringCityB);

    return road;
  }

  public List<Road> ConnectCitiesInOrder(List<City> cities)
  {
    List<Road> result = new();
    if (cities.Count == 0)
    {
      return result;
    }
    ElbowLine el = new(NocabRNG.newRNG);

    City cityA = cities[0];
    for (int i = 1; i < cities.Count; i++)
    {
      City cityB = cities[i];
      Road road = findValidConnector(cityA, cityB);
      result.Add(road);
      cityA = cityB;
    }

    return result;
  }
}
