using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using CD_Util = CardinalDirection_Util;

public class MinimapAuthorLoop_Polygon
{
  readonly NocabRNG rng;
  readonly AuthorUtilities authorUtil;
  private int XWiggleDelta;
  private int YWiggleDelta;

  public MinimapAuthorLoop_Polygon(NocabRNG rng, int xWiggleDelta = 0, int yWiggleDelta = 0)
  {
    this.rng = rng;
    this.authorUtil = new AuthorUtilities(rng);
    this.XWiggleDelta = Mathf.Abs(xWiggleDelta);
    this.YWiggleDelta = Mathf.Abs(yWiggleDelta);
  }

  public MinimapAuthorLoop_Polygon(int xWiggleDelta = 0, int yWiggleDelta = 0)
    : this(NocabRNG.defaultRNG, xWiggleDelta, yWiggleDelta) { }

  public (List<City>, List<Road>) GenerateLoop_CitiesAtCorners(
    int loopWidth,
    int loopHeight,
    int numSides
  )
  {
    /**
     * A loop map, basically a polygon.
     * Cities will be placed at the corners of the polygon,
     * edges will connect them defining the loop.
     */
    List<City> cornerCities = getCityPositions_Corners(loopWidth, loopHeight, numSides);
    List<Road> roads = new();
    City previousCorner = cornerCities[cornerCities.Count - 1];
    foreach (var currentCorner in cornerCities)
    {
      Road road = authorUtil.connectCities_Automatic(previousCorner, currentCorner);
      roads.Add(road);
      previousCorner = currentCorner;
    }

    return (cornerCities, roads);
  }

  public (List<City>, List<Road>) GenerateLoop_CitiesAlongEdges(
    int loopWidth,
    int loopHeight,
    int numSides
  )
  {
    /**
     * A loop map, basically a polygon.
     * Cities will be placed along the edges of the polygon,
     * connecting will define the loop.
     */
    List<City> edgeCities = getCityPositions_Edges(loopWidth, loopHeight, numSides);
    List<Road> roads = new();
    City previousEdge = edgeCities[edgeCities.Count - 1];
    foreach (var edgeCity in edgeCities)
    {
      // Connect the previous city to the current city
      // (HashSet<CardinalDirection> exitDirectionsA, HashSet<CardinalDirection> enterDirectionsB) =
      //   getEnterExitDirections(previousEdge, edgeCity);
      // Road road = authorUtil.connectCities(
      //   previousEdge,
      //   edgeCity,
      //   exitDirectionsA,
      //   enterDirectionsB,
      //   ElbowLine.ElbowTypes.TwoTurn
      // );
      Road road = authorUtil.connectCities_Automatic(previousEdge, edgeCity);
      roads.Add(road);

      previousEdge = edgeCity;
    }

    return (edgeCities, roads);
  }

  protected List<City> getCityPositions_Corners(
    int loopWidth,
    int loopHeight,
    int numSides,
    float rotationOffsetDegrees = 0f
  )
  {
    /**
     * Make City objects at the corners of the loop.
     * Return a list of City objects in order:
     * Right most (along the X axis), counter-clockwise around the polygon, rotationOffset note below.
     *
     * If a rotational offset is provided, the cities will be rotated around the center of the polygon.
     * This will impact the relative positions of the cities.
     * For example, if the rotation offset is 0 for a triangle, cities will make a triangle pointing Right (positive X axis)
     * The first city will be the rightmost city, following the counter-clockwise order.
     * If the rotation offset is 90 degrees, cities will make a triangle pointing Up (positive Y axis)
     * The first city will be the topmost city, following the counter-clockwise order.
     *
     * @param loopWidth - The width of the loop.
     * @param loopHeight - The height of the loop.
     * @param numSides - The number of sides of the polygon.
     * @param rotationOffsetDegrees - The rotation offset in degrees.
     * @return A list of City objects in order: Right most (along the X axis), counter-clockwise around the polygon, rotationOffset note below.
     */

    float rotationOffsetRadians = (rotationOffsetDegrees * Mathf.Deg2Rad) % (2 * Mathf.PI);
    if (numSides < 0)
    {
      Debug.LogError("Number of sides must be greater than 0");
      numSides = math.abs(numSides);
    }
    if (numSides == 0)
    {
      throw new Exception("Number of sides must be greater than 0");
    }

    // Assume the polygon is centered at (0,0)
    int centerX = 0;
    int centerY = 0;

    int radius = (int)(Mathf.Min(loopWidth, loopHeight) / 2);
    float angleIncrement = 2 * Mathf.PI / numSides;

    List<City> corners = new();
    for (int i = 0; i < numSides; i++)
    {
      float angle = i * angleIncrement;
      int x = centerX + (int)(radius * Mathf.Cos(angle + rotationOffsetRadians));
      int y = centerY + (int)(radius * Mathf.Sin(angle + rotationOffsetRadians));
      corners.Add(new City(x, y));
    }

    corners = authorUtil.wiggleCityPositions(corners, XWiggleDelta, YWiggleDelta);
    return corners;
  }

  protected List<City> getCityPositions_Edges(
    int loopWidth,
    int loopHeight,
    int numSides,
    float rotationOffsetDegrees = 0f
  )
  {
    /**
     * Make City objects along the edges of the loop.
     * Return a list of City objects in counter clockwise order around the polygon.
     *
     * First, the corners of the polygon are determined (via helper function)
     * Then, a point is picked along each edge between the corners.
     *
     * @param loopWidth - The width of the loop.
     * @param loopHeight - The height of the loop.
     * @param numSides - The number of sides of the polygon.
     * @param rotationOffsetDegrees - The rotation offset in degrees.
     * @return A list of City objects in counter clockwise order around the polygon.
     *
     * @throws Exception if the number of sides is less than 3.
     */
    if (numSides <= 2)
    {
      throw new Exception("Number of sides must be greater than 2");
    }

    // Assume the polygon is centered at (0,0)
    int centerX = 0;
    int centerY = 0;

    List<City> corners = getCityPositions_Corners(
      loopWidth,
      loopHeight,
      numSides,
      rotationOffsetDegrees
    );

    List<City> result = new();
    // Loop over the corners to establish the edges. Pick a point along that edge for the actual result city.
    City previousCorner = corners[corners.Count - 1];
    for (int i = 0; i < corners.Count; i++)
    {
      City currentCorner = corners[i];
      // Pick a point along the edge between the previous and current corner.
      float lerpPercentage = rng.generateFloat(0.25f, 0.75f);
      Vector2 lerpedPoint = Vector2.Lerp(
        previousCorner.position,
        currentCorner.position,
        lerpPercentage
      );
      result.Add(new City((int)lerpedPoint.x, (int)lerpedPoint.y));

      previousCorner = currentCorner;
    }

    result = authorUtil.wiggleCityPositions(result, XWiggleDelta, YWiggleDelta);
    return result;
  }

  private (
    HashSet<CardinalDirection> exitDirectionsA,
    HashSet<CardinalDirection> enterDirectionsB
  ) getEnterExitDirections(City cityA, City cityB)
  {
    /**
    * Find the enter and exit directions for a road between two cities.
    * Return a tuple of the exit and enter directions for city A and city B.
    *
    * City A is the source of the road, city B is the destination.
    *
    * @param cityA - The city to connect from.
    * @param cityB - The city to connect to.
    * @return A tuple of the exit and enter directions.
    */

    // Assume positive Y is up, positive X is right (Unity coordinate system)
    HashSet<CardinalDirection> exitDirectionsA = new();
    HashSet<CardinalDirection> enterDirectionsB = new();

    // North/South (same Y gets both)
    if (cityA.position.y >= cityB.position.y)
    {
      // If city A is north of City B, then A can exit south into B enter north.
      exitDirectionsA.Add(CardinalDirection.South);
      enterDirectionsB.Add(CardinalDirection.North);
    }
    if (cityA.position.y <= cityB.position.y)
    {
      // If city A is south of City B, then A can exit north into B enter south.
      exitDirectionsA.Add(CardinalDirection.North);
      enterDirectionsB.Add(CardinalDirection.South);
    }

    // East/West (Same X gets both)
    if (cityA.position.x >= cityB.position.x)
    {
      // If city A is east of City B, then A can exit west into B enter east.
      exitDirectionsA.Add(CardinalDirection.West);
      enterDirectionsB.Add(CardinalDirection.East);
    }
    if (cityA.position.x <= cityB.position.x)
    {
      // If city A is west of City B, then A can exit east into B enter west.
      exitDirectionsA.Add(CardinalDirection.East);
      enterDirectionsB.Add(CardinalDirection.West);
    }

    return (exitDirectionsA, enterDirectionsB);
  }
}
