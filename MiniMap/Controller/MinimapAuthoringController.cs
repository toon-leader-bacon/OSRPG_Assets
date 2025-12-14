using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for authoring/building maps. Handles map generation, city placement,
/// road creation, and connecting cities. This is the authoring-time controller.
/// </summary>
public class MinimapAuthoringController
{
  private MapData mapData;

  public MinimapAuthoringController(MapData mapData)
  {
    this.mapData = mapData;
  }

  /// <summary>
  /// Places a city at the specified position.
  /// </summary>
  public City PlaceCity(Vector2Int position)
  {
    City city = new City(position);
    mapData.AddCity(city);
    return city;
  }

  /// <summary>
  /// Places a road along the specified path.
  /// </summary>
  public Road PlaceRoad(List<Vector2Int> path)
  {
    Road road = new Road(path);
    mapData.AddRoad(road);
    return road;
  }

  /// <summary>
  /// Connects two cities with a road. Uses the existing logic from MinimapBuilder.
  /// </summary>
  public Road ConnectCities(City cityA, City cityB)
  {
    Road road = FindValidConnector(cityA, cityB);
    mapData.AddRoad(road);
    return road;
  }

  /// <summary>
  /// Connects a list of cities in order, creating roads between consecutive cities.
  /// </summary>
  public List<Road> ConnectCitiesInOrder(List<City> cities)
  {
    List<Road> roads = new List<Road>();
    if (cities.Count == 0)
    {
      return roads;
    }

    City cityA = cities[0];
    for (int i = 1; i < cities.Count; i++)
    {
      City cityB = cities[i];
      Road road = ConnectCities(cityA, cityB);
      roads.Add(road);
      cityA = cityB;
    }

    return roads;
  }

  /// <summary>
  /// Generates cities in a circular/loop pattern. Based on existing CircleOfCities logic.
  /// </summary>
  public List<City> GenerateCircleOfCities(int loopWidth, int loopHeight, int centerX, int centerY)
  {
    NocabRNG rng = NocabRNG.defaultRNG;
    Box2D mainLoop = new(centerX, centerY, loopWidth, loopHeight, positiveYDown: true);

    List<City> cityPoints = new List<City>();

    // Left edge
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

    // Bottom edge
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

    // Right edge
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

    // Top edge
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

    // Add all cities to the map
    foreach (City city in cityPoints)
    {
      mapData.AddCity(city);
    }

    return cityPoints;
  }

  /// <summary>
  /// Clears the entire map.
  /// </summary>
  public void ClearMap()
  {
    mapData.Clear();
  }

  // Private helper methods from MinimapBuilder
  private Road connectCitiesInLine(City cityA, City cityB)
  {
    Road r = new Road(
      ElbowLine.ConnectPoints(
        cityA.position,
        cityB.position,
        false, // Ignored for straight lines
        ElbowLine.ElbowTypes.OneTurn
      )
    );

    bool isVertical = cityA.position.x == cityB.position.x;
    bool isCityAFirst = isVertical
      ? cityA.position.y < cityB.position.y
      : cityA.position.x < cityB.position.x;

    CardinalDirection leavingA = isVertical
      ? (isCityAFirst ? CardinalDirection.North : CardinalDirection.South)
      : (isCityAFirst ? CardinalDirection.East : CardinalDirection.West);

    CardinalDirection enteringB = CardinalDirection_Util.opposite(leavingA);

    cityA.insertRoad(r, leavingA);
    cityB.insertRoad(r, enteringB);
    return r;
  }

  private Road FindValidConnector(City cityA, City cityB)
  {
    // Check if cities are in line
    if (cityA.position.x == cityB.position.x || cityA.position.y == cityB.position.y)
    {
      return connectCitiesInLine(cityA, cityB);
    }

    // Determine relative positions
    Box2D citiesOnCorners = new(
      cityA.position.x,
      cityA.position.y,
      cityB.position.x,
      cityB.position.y,
      positiveYDown: false
    );

    CardinalDirection cityAHoriz = citiesOnCorners.IsOnLeftEdge(cityA.position)
      ? CardinalDirection.East
      : CardinalDirection.West;

    CardinalDirection cityAVert = citiesOnCorners.IsOnTopEdge(cityA.position)
      ? CardinalDirection.South
      : CardinalDirection.North;

    CardinalDirection cityBHoriz = CardinalDirection_Util.opposite(cityAHoriz);
    CardinalDirection cityBVert = CardinalDirection_Util.opposite(cityAVert);

    // Generate possible roads
    List<Tuple<bool, ElbowLine.ElbowTypes, CardinalDirection, CardinalDirection>> possibleRoads =
      new List<Tuple<bool, ElbowLine.ElbowTypes, CardinalDirection, CardinalDirection>>();

    // Roads leaving City A horizontally
    if (cityA.directionUnoccupied(cityAHoriz))
    {
      bool horizontal = true;
      if (cityB.directionUnoccupied(cityBVert))
      {
        possibleRoads.Add(
          new Tuple<bool, ElbowLine.ElbowTypes, CardinalDirection, CardinalDirection>(
            horizontal,
            ElbowLine.ElbowTypes.OneTurn,
            cityAHoriz,
            cityBVert
          )
        );
      }
      if (cityB.directionUnoccupied(cityBHoriz))
      {
        possibleRoads.Add(
          new Tuple<bool, ElbowLine.ElbowTypes, CardinalDirection, CardinalDirection>(
            horizontal,
            ElbowLine.ElbowTypes.TwoTurn,
            cityAHoriz,
            cityBHoriz
          )
        );
      }
    }

    // Roads leaving City A vertically
    if (cityA.directionUnoccupied(cityAVert))
    {
      bool horizontal = false;
      if (cityB.directionUnoccupied(cityBVert))
      {
        possibleRoads.Add(
          new Tuple<bool, ElbowLine.ElbowTypes, CardinalDirection, CardinalDirection>(
            horizontal,
            ElbowLine.ElbowTypes.TwoTurn,
            cityAVert,
            cityBVert
          )
        );
      }
      if (cityB.directionUnoccupied(cityBHoriz))
      {
        possibleRoads.Add(
          new Tuple<bool, ElbowLine.ElbowTypes, CardinalDirection, CardinalDirection>(
            horizontal,
            ElbowLine.ElbowTypes.OneTurn,
            cityAVert,
            cityBHoriz
          )
        );
      }
    }

    if (possibleRoads.Count == 0)
    {
      Debug.LogWarning(
        $"No possible roads between cities at {cityA.position} and {cityB.position}. Making empty road."
      );
      return new Road(new List<Vector2Int>());
    }

    // Select a random road
    var roadData = NocabRNG.newRNG.randomElem(possibleRoads);
    bool roadHorizontalStart = roadData.Item1;
    ElbowLine.ElbowTypes roadType = roadData.Item2;

    Road road = new Road(
      ElbowLine.ConnectPoints(cityA.position, cityB.position, roadHorizontalStart, roadType)
    );

    CardinalDirection leavingCityA = roadData.Item3;
    CardinalDirection enteringCityB = roadData.Item4;

    cityA.insertRoad(road, leavingCityA);
    cityB.insertRoad(road, enteringCityB);

    return road;
  }
}
