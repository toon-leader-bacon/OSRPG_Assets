using System;
using System.Collections.Generic;
using System.Text;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.Playables;
using UnityEngine;
using CD_Util = CardinalDirection_Util;

public class MinimapAuthorLoop
{
  readonly NocabRNG rng;

  public MinimapAuthorLoop(NocabRNG rng)
  {
    this.rng = rng;
  }

  public MinimapAuthorLoop()
    : this(NocabRNG.defaultRNG) { }

  public (List<City>, List<Road>) GenerateLoop_CitiesAtCorners(int loopWidth, int loopHeight)
  {
    /**
     * A loop map, basically a box.
     * Cities will be placed at the corners of the box,
     * edges will connect them defining the loop.
     * NOTE: Edge connections for this technique must ALWAYS be 2-turn.
     */

    // For now, assume the center of the loop is at (0,0)
    // Positive X is up, positive Y is right (Unity coordinate system)

    List<City> cities = getCityPositions_Corners(loopWidth, loopHeight);
    City TL_city = cities[0];
    City TR_city = cities[1];
    City BL_city = cities[2];
    City BR_city = cities[3];

    // Connect the cities with roads
    // Let's focus on connection TL to TR first.
    // Each city has a Cardinal Direction for each of its 4 sides.
    // The TL to TR connection should either be a North or East connection (from TL)
    // connection to a North or West connection (into TR)
    Road TL_TR_road = connectCities(
      cityA: TL_city,
      cityB: TR_city,
      exitDirectionsA: new() { CardinalDirection.North, CardinalDirection.East },
      enterDirectionsB: new() { CardinalDirection.North, CardinalDirection.West },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );

    Road TR_BR_road = connectCities(
      cityA: TR_city,
      cityB: BR_city,
      exitDirectionsA: new() { CardinalDirection.East, CardinalDirection.South },
      enterDirectionsB: new() { CardinalDirection.North, CardinalDirection.East },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );

    Road BR_BL_road = connectCities(
      cityA: BR_city,
      cityB: BL_city,
      exitDirectionsA: new() { CardinalDirection.South, CardinalDirection.West },
      enterDirectionsB: new() { CardinalDirection.South, CardinalDirection.East },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );

    Road BL_TL_road = connectCities(
      cityA: BL_city,
      cityB: TL_city,
      exitDirectionsA: new() { CardinalDirection.West, CardinalDirection.North },
      enterDirectionsB: new() { CardinalDirection.West, CardinalDirection.South },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );

    List<Road> roads = new() { TL_TR_road, TR_BR_road, BR_BL_road, BL_TL_road };

    return new(cities, roads);
  }

  public (List<City>, List<Road>) GenerateLoop_CitiesAlongEdges(int loopWidth, int loopHeight)
  {
    /**
     * A loop map, basically a box.
     * Cities will be placed along the edges of the box,
     * connecting will define the loop.
     */

    // For now, assume the center of the loop is at (0,0)
    // Positive X is up, positive Y is right (Unity coordinate system)

    List<City> cities = getCityPositions_Edges(loopWidth, loopHeight);
    City topCity = cities[0];
    City bottomCity = cities[1];
    City rightCity = cities[2];
    City leftCity = cities[3];

    // Connect the cities with roads
    // Let's focus on connection top to right connection first
    Road TopRightRoad = connectCities(
      cityA: topCity,
      cityB: rightCity,
      exitDirectionsA: new() { CardinalDirection.South, CardinalDirection.East },
      enterDirectionsB: new() { CardinalDirection.North, CardinalDirection.West },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );
    Road RightBottomRoad = connectCities(
      cityA: rightCity,
      cityB: bottomCity,
      exitDirectionsA: new() { CardinalDirection.West, CardinalDirection.South },
      enterDirectionsB: new() { CardinalDirection.East, CardinalDirection.North },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );
    Road BottomLeftRoad = connectCities(
      cityA: bottomCity,
      cityB: leftCity,
      exitDirectionsA: new() { CardinalDirection.North, CardinalDirection.West },
      enterDirectionsB: new() { CardinalDirection.South, CardinalDirection.East },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );
    Road LeftTopRoad = connectCities(
      cityA: leftCity,
      cityB: topCity,
      exitDirectionsA: new() { CardinalDirection.East, CardinalDirection.North },
      enterDirectionsB: new() { CardinalDirection.South, CardinalDirection.West },
      roadType: ElbowLine.ElbowTypes.TwoTurn
    );

    List<Road> roads = new() { TopRightRoad, RightBottomRoad, BottomLeftRoad, LeftTopRoad };
    return new(cities, roads);
  }

  public (List<City>, List<Road>) GenerateLoop_Cities_Square(int loopWidth, int loopHeight)
  {
    /**
     * A loop map, basically a box.
     * Cities will be placed along the edges of the box,
     * connecting will be One Turn connections making a box
     */

    List<City> cities = getCityPositions_Edges(loopWidth, loopHeight);
    City topCity = cities[0];
    City bottomCity = cities[1];
    City rightCity = cities[2];
    City leftCity = cities[3];

    // Connect the cities with roads
    Road TopRightRoad = connectCities(
      cityA: topCity,
      cityB: rightCity,
      exitDirectionsA: new() { CardinalDirection.East },
      enterDirectionsB: new() { CardinalDirection.North },
      roadType: ElbowLine.ElbowTypes.OneTurn
    );
    Road RightBottomRoad = connectCities(
      cityA: rightCity,
      cityB: bottomCity,
      exitDirectionsA: new() { CardinalDirection.South },
      enterDirectionsB: new() { CardinalDirection.East },
      roadType: ElbowLine.ElbowTypes.OneTurn
    );
    Road BottomLeftRoad = connectCities(
      cityA: bottomCity,
      cityB: leftCity,
      exitDirectionsA: new() { CardinalDirection.West },
      enterDirectionsB: new() { CardinalDirection.South },
      roadType: ElbowLine.ElbowTypes.OneTurn
    );
    Road LeftTopRoad = connectCities(
      cityA: leftCity,
      cityB: topCity,
      exitDirectionsA: new() { CardinalDirection.North },
      enterDirectionsB: new() { CardinalDirection.West },
      roadType: ElbowLine.ElbowTypes.OneTurn
    );

    List<Road> roads = new() { TopRightRoad, RightBottomRoad, BottomLeftRoad, LeftTopRoad };
    return new(cities, roads);
  }

  protected List<City> getCityPositions_Corners(int loopWidth, int loopHeight)
  {
    /**
     * Make City objects at the corners of the loop.
     * Return a list of City objects in order:
     * Top-Left, Top-Right, Bottom-Left, Bottom-Right
     */

    int centerX = 0;
    int centerY = 0;

    int leftEdgeX = centerX - (loopWidth / 2);
    int rightEdgeX = centerX + (loopWidth / 2);
    int topEdgeY = centerY + (loopHeight / 2);
    int bottomEdgeY = centerY - (loopHeight / 2);

    // Cities in the corners
    City TL = new(leftEdgeX, topEdgeY);
    City TR = new(rightEdgeX, topEdgeY);
    City BL = new(leftEdgeX, bottomEdgeY);
    City BR = new(rightEdgeX, bottomEdgeY);

    return new() { TL, TR, BL, BR };
  }

  protected List<City> getCityPositions_Edges(int loopWidth, int loopHeight)
  {
    /**
     * Make City objects along the edges of the loop.
     * Return a list of City objects in order:
     * Top, Bottom, Right, Left
     */

    int centerX = 0;
    int centerY = 0;

    int leftEdgeX = centerX - (loopWidth / 2);
    int rightEdgeX = centerX + (loopWidth / 2);
    int topEdgeY = centerY + (loopHeight / 2);
    int bottomEdgeY = centerY - (loopHeight / 2);

    // Cities along the edges middle 75% of the edge
    // Top
    int topCityX = rng.generateInt(
      (int)(leftEdgeX + (loopWidth * 0.125f)),
      (int)(rightEdgeX - (loopWidth * 0.125f))
    );
    int topCityY = topEdgeY;
    City topCity = new(topCityX, topCityY);

    // Bottom
    int bottomCityX = rng.generateInt(
      (int)(leftEdgeX + (loopWidth * 0.125f)),
      (int)(rightEdgeX - (loopWidth * 0.125f))
    );
    int bottomCityY = bottomEdgeY;
    City bottomCity = new(bottomCityX, bottomCityY);

    // Right
    int rightCityX = rightEdgeX;
    int rightCityY = rng.generateInt(
      (int)(bottomEdgeY + (loopHeight * 0.125f)),
      (int)(topEdgeY - (loopHeight * 0.125f))
    );
    City rightCity = new(rightCityX, rightCityY);

    // Left
    int leftCityX = leftEdgeX;
    int leftCityY = rng.generateInt(
      (int)(bottomEdgeY + (loopHeight * 0.125f)),
      (int)(topEdgeY - (loopHeight * 0.125f))
    );
    City leftCity = new(leftCityX, leftCityY);

    return new() { topCity, bottomCity, rightCity, leftCity };
  }

  protected Road connectCities(
    City cityA,
    City cityB,
    HashSet<CardinalDirection> exitDirectionsA,
    HashSet<CardinalDirection> enterDirectionsB,
    ElbowLine.ElbowTypes roadType
  )
  {
    /**
     * Connect two cities with a road.
     * Return the road object.
     *
     * This method will create an ElbowLine road between the two cities.
     * The exit and enter directions are intersected with the unoccupied directions of the cities.
     * If no compatible directions are found, an exception is thrown.
     *
     # The exit tile is the position of the city plus the exit direction. For example
     * if the city is at (0,0) and the exit direction is North, the first tile of the road will be (0,1).
     *
     * @param cityA - The city to connect from.
     * @param cityB - The city to connect to.
     * @param exitDirectionsA - The directions that city A can exit from.
     * @param enterDirectionsB - The directions that city B can enter from.
     * @param roadType - The type of road to create.
     * @return The road object.
     *
     * WARNING: If the cities and the provided directions are not compatible, this will throw an exception.
     * @throws Exception if the cities and the provided directions are not compatible.
     */
    exitDirectionsA.IntersectWith(cityA.UnoccupiedDirections());
    if (exitDirectionsA.Count == 0)
    {
      throw new Exception("No compatible exit directions found for city A.");
    }
    CardinalDirection AExitDirection = rng.randomElem_Set(exitDirectionsA);
    Vector2Int AExitTile = cityA.position + CD_Util.GetVector(AExitDirection);

    enterDirectionsB.IntersectWith(cityB.UnoccupiedDirections());
    if (enterDirectionsB.Count == 0)
    {
      throw new Exception("No compatible enter directions found for city B.");
    }
    CardinalDirection BEnterDirection = rng.randomElem_Set(enterDirectionsB);
    Vector2Int BEnterTile = cityB.position + CD_Util.GetVector(BEnterDirection);

    Road road;
    switch (roadType)
    {
      case ElbowLine.ElbowTypes.OneTurn:
        road = new(ElbowLine.OneTurn_static(AExitTile, BEnterTile));
        break;
      case ElbowLine.ElbowTypes.TwoTurn:
        road = new(ElbowLine.TwoTurn_static(AExitTile, BEnterTile));
        break;
      default:
        throw new Exception("Invalid road type.");
    }
    cityA.insertRoad(road, AExitDirection);
    cityB.insertRoad(road, BEnterDirection);
    return road;
  }

  protected Road connectCities(
    City cityA,
    City cityB,
    HashSet<CardinalDirection> exitDirectionsA,
    HashSet<CardinalDirection> enterDirectionsB
  )
  {
    // See full method for documentation
    return connectCities(
      cityA,
      cityB,
      exitDirectionsA,
      enterDirectionsB,
      rng.randomElem(ElbowLine.AllElbowTypes)
    );
  }
}
