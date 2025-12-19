using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CD_Util = CardinalDirection_Util;

public class MinimapAuthorOverlapRectangle
{
  const int MAX_DISTANCE = 10_000_000; // There's a while(true) loop, and that scares me :(

  readonly NocabRNG rng;
  readonly AuthorUtilities authorUtil;

  private int XWiggleDelta;
  private int YWiggleDelta;

  public MinimapAuthorOverlapRectangle(NocabRNG rng, int xWiggleDelta = 0, int yWiggleDelta = 0)
  {
    this.rng = rng;
    this.authorUtil = new AuthorUtilities(rng);
    this.XWiggleDelta = Mathf.Abs(xWiggleDelta);
    this.YWiggleDelta = Mathf.Abs(yWiggleDelta);
  }

  public MinimapAuthorOverlapRectangle(int xWiggleDelta = 0, int yWiggleDelta = 0)
    : this(NocabRNG.defaultRNG, xWiggleDelta, yWiggleDelta) { }

  // public (List<City>, List<Road>) GenerateOverlapRectangle(int width, int height)
  // {
  //   /**
  //    * Generate a map that is a rectangle, with other rectangles "under" it.
  //    * Cities will be added at certain points along the entire shape. Typically
  //    * at corners, or intersections of edges.
  //    *
  //    * Example: This city has one main rectangle in the middle, and two rectangles "under" it.
  //    * One rectangle on is under an edge of the main rectangle, and the other is under a corner.
  //    *
  //    *                              +------+
  //    *                              |      |
  //    *                          +---+      |  <--- Main rectangle
  //    * Rectangle under edge --> |   |      |
  //    *                          +---+      +--+
  //    *                              |      |  |
  //    *                              +----+-+  |
  //    *                                   |    |  <--- Rectangle under corner
  //    *                                   +----+
  //    *
  //    * Example with city locations shown as X's:
  //    *      X------X
  //    *      |      |
  //    *  X---X      |
  //    *  |   |      |
  //    *  X---X      X--X
  //    *      |      |  |
  //    *      X----X-X  |
  //    *           |    |
  //    *           X----X
  //    */

  //   /*
  //    * Dev notes: How to implement this?
  //    * - Start by making the main rectangle.
  //    * - For now, lets assume there will be only 2 rectangles "under" the main rectangle.
  //    * - One rectangle will be under an edge of the main rectangle.
  //    * - The other rectangle will be under a corner of the main rectangle.
  //    *
  //    * So, for the corner under rectangle:
  //    * - Pick a corner of the main rectangle.
  //    *   That can be the center of the corner under rectangle.
  //    * - Make the corner-under rectangle (width and height is scaled down from the main rectangle)
  //    * - Calculate the intersection points between the main rectangle and the corner-under rectangle.
  //    * - Draw roads between intersection points, and the corners of the corner-under rectangle in some intelligent way.
  //    *
  //    * For the Edge under rectangle:
  //    * - Pick an edge of the main rectangle (that's not either of the edges associated with the corner under rectangle)
  //    * - Pick the center point of that edge, that will be the kinda center of the edge under rectangle.
  //    * - Make the edge-under rectangle (width and height is scaled down from the main rectangle)
  //    * - Edge-under rectangles are a little easier, because the intersection points for the main rectangle
  //    *   are corners of the edge-under rectangle. So use the point along the edge, the delta up-or-down to
  //    *   compute the intersection points, and the width of the edge-under rectangle to find the external corners.
  //    *   That should be enough to define the edge-under rectangle.
  //    *
  //    * Now that we have all the roads, we should pick city points. For practice, lets discard all the knowledge
  //    * from generating the roads. Let's build some tools that can start at a point along the road, and search for
  //    * corners and intersections. Might be a wast of time, so think about it.
  //    * Then select a few random points for the cities.
  //    * Then create real Road objects and connect them to the cities in the slots.
  //    */

  //   // Assume the center of the map is at (0,0)
  //   // Positive X is up, positive Y is right (Unity coordinate system)
  //   int centerX = 0;
  //   int centerY = 0;

  //   int leftEdgeX = centerX - width / 2;
  //   int rightEdgeX = centerX + width / 2;
  //   int topEdgeY = centerY + height / 2;
  //   int bottomEdgeY = centerY - height / 2;

  //   // Main rectangle
  //   Box2D mainRectangle = new(centerX, centerY, width, height, positiveYDown: false);
  //   List<Road> mainRectangleRoads = new()
  //   {
  //     new(NocabPixelLine.getPointsAlongLine(mainRectangle.TL_v, mainRectangle.TR_v)),
  //     new(NocabPixelLine.getPointsAlongLine(mainRectangle.TR_v, mainRectangle.BR_v)),
  //     new(NocabPixelLine.getPointsAlongLine(mainRectangle.BR_v, mainRectangle.BL_v)),
  //     new(NocabPixelLine.getPointsAlongLine(mainRectangle.BL_v, mainRectangle.TL_v)),
  //   };

  //   // Corner under rectangle
  //   NocabCorner targetCorner = NocabCorner.BottomRight;
  //   Vector2Int targetCornerV = new(0, 0);
  //   switch (targetCorner)
  //   {
  //     case NocabCorner.TopLeft:
  //       targetCornerV = new Vector2Int((int)mainRectangle.TL_v.x, (int)mainRectangle.TL_v.y);
  //       break;
  //     case NocabCorner.TopRight:
  //       targetCornerV = new Vector2Int((int)mainRectangle.TR_v.x, (int)mainRectangle.TR_v.y);
  //       break;
  //     case NocabCorner.BottomRight:
  //       targetCornerV = new Vector2Int((int)mainRectangle.BR_v.x, (int)mainRectangle.BR_v.y);
  //       break;
  //     case NocabCorner.BottomLeft:
  //       targetCornerV = new Vector2Int((int)mainRectangle.BL_v.x, (int)mainRectangle.BL_v.y);
  //       break;
  //     default:
  //       throw new ArgumentException($"Invalid target corner: {targetCorner}");
  //   }
  //   int underRectangleWidth = (int)(mainRectangle.Width / 2);
  //   int underRectangleHeight = (int)(mainRectangle.Height / 2);
  //   Box2D underRectangle = Box2D.Box2D_CenterPt(
  //     targetCornerV.x,
  //     targetCornerV.y,
  //     underRectangleWidth,
  //     underRectangleHeight,
  //     positiveYDown: false
  //   );

  //   throw new NotImplementedException("Not implemented");
  // }

  public (List<City>, List<Road>) GenerateOverlapRectangle_RandomShapes(
    int width,
    int height,
    int numShapes = 3,
    int numCities = 6
  )
  {
    /**
     * Generate a map that is a rectangle, with other rectangles "under" it.
     * The number of shapes is random, and the shapes are randomly placed.
     *
     *
     * @param width - The width of the main rectangle.
     * @param height - The height of the main rectangle.
     * @param numShapes - The number of shapes to generate.
     * @param numCities - The number of cities to generate. (Typically 1-2 per shape)
     * @return A tuple of the cities and roads.
     */

    /*
     * Main strategy:
     * 0) Generate the shapes my layering boxes on top of each other.
     *   - The first box is in the center (0, 0)
     *   - Then, using all the shapes so far, calculate the true outer edge points.
     *   - Pick a random point on the true outer edge points, and use that as the center of the new box.
     *   - Repeat for the number of shapes requested.
     * 1) Generate the road points
     *   - Use in order boxes to generate the roads. Some guarantees:
     *     - The roads will be straight cardinal direction lines.
     *     - Each point will exist in exactly one road (corners or intersections are owned by one road)
     *     - There will be corners, T intersections and possibly (but rarely) + intersections.
     *     - There may be roads with length 1
     *     - There will be no roads with length 0
     *     - There will be no disconnected roads/ points
     *  - Some assumptions/ hopes/ things to improve:
     *     - It is possible for two roads to parallel touch each other, making a "thick" road. This is bad
     *       because it really messes up a lot of the algorithms that look for neighbors/ straight lines.
     * 2) Use the road points to generate the roads. See the guarantees above.
     * 3) Generate the cities (and update roads as needed)
     *   - Pick a random road that is at least 3 tiles long.
     *   - Pick the start or end point of that road as the city position.
     *   - Update all the other roads to connect to this city (including the initially randomly selected road)
     *     - For the road that the city is a start/ end point: Make a new road sub the city point,
     *       and hook up the city in the correct direction.
     *     - For the road that the city is a middle point: Split the road into two roads (why we need at least 3 tiles),
     *       both of those new roads will NOT contain the city point. Then hook up those new roads to the city.
     * 4) Return the cities and roads.
     */

    int centerX = 0;
    int centerY = 0;

    float shapeScaleFactor = 0.75f;

    List<Box2D> inOrderShapes = new();
    // Add the first box to the list,
    Box2D firstBox = Box2D.Box2D_CenterPt(centerX, centerY, width, height, positiveYDown: false);
    inOrderShapes.Add(firstBox);

    // Add the rest of the boxes in order to the list.
    // Each box will be centered at a random point on the true outer edge of the main shape.
    for (int i = 0; i < numShapes; i++)
    {
      Box2D newBox = GenerateValidBox(
        inOrderShapes,
        width * shapeScaleFactor,
        height * shapeScaleFactor
      );
      inOrderShapes.Add(newBox);
    }

    // Now generate the road points
    HashSet<Vector2Int> overlappingEdgePoints = getEdgePoints_TopToBottom(inOrderShapes);

    // Now, convert this list of points into a bunch of roads
    List<Road> allRoads = ExtractRoutes(
      overlappingEdgePoints,
      // Pull a random corner from the top-most box shape for the algorithm to start from.
      rng.randomElem(
        new List<Vector2Int>
        {
          new(Mathf.RoundToInt(firstBox.TL_v.x), Mathf.RoundToInt(firstBox.TL_v.y)),
          new(Mathf.RoundToInt(firstBox.TR_v.x), Mathf.RoundToInt(firstBox.TR_v.y)),
          new(Mathf.RoundToInt(firstBox.BR_v.x), Mathf.RoundToInt(firstBox.BR_v.y)),
          new(Mathf.RoundToInt(firstBox.BL_v.x), Mathf.RoundToInt(firstBox.BL_v.y)),
        }
      )
    );

    // Compute the possible locations for the cities
    HashSet<Vector2Int> collector = new();
    foreach (Road road in allRoads)
    {
      if (road.tilesInOrder.Count < 3)
        continue; // Skip roads that are too short.
      collector.Add(road.StartPoint);
      collector.Add(road.EndPoint);
    }
    List<Vector2Int> possibleCityLocations = new(collector);

    // Now, generate the cities, hook them up to the roads. Some roads may need to be updated/ split.
    List<City> cities = new();
    List<Road> updatedRoads = new(allRoads);
    // Pick a few random roads, and add a city at the start and end of each road.
    for (int i = 0; i < numCities; i++)
    {
      City newCity = new(rng.randomElem(possibleCityLocations));
      // The city position may be the middle of another road (T junctions).
      // So this utility function will look over all the roads, and set everything up properly
      // (IE the T junction road may need to be split into 2 roads)
      updatedRoads = HookUpCityToRoads(newCity, updatedRoads);
      cities.Add(newCity);
    }
    return (cities, updatedRoads);
  }

  HashSet<Vector2Int> getOuterEdgePoints(List<Box2D> inOrderShapes)
  {
    /**
     * Get the outer edge points of the compound shape (inOrderShapes[0] + inOrderShapes[1] + ... + inOrderShapes[n])
     * that are not "covered" by ANY of the other shapes in the list.
     *
     * @param inOrderShapes - The in order shapes.
     * @return A set of outer edge points.
     */
    HashSet<Vector2Int> resultPoints = new();
    for (int currentIndex = 0; currentIndex < inOrderShapes.Count; currentIndex++)
    {
      Box2D currentShape = inOrderShapes[currentIndex];
      foreach (Vector2Int point in currentShape.GetAllEdgePoints())
      {
        bool isCovered = false;
        // Check if the point is covered by any of the other shapes
        for (int otherIndex = 0; otherIndex < inOrderShapes.Count; otherIndex++)
        {
          if (otherIndex == currentIndex)
            continue;
          if (inOrderShapes[otherIndex].Contains(point.x, point.y))
          {
            isCovered = true;
            break;
          }
        } // End looping over other shapes
        if (!isCovered)
        {
          resultPoints.Add(point);
        }
      } // End foreach point in currentShape
    } // End looping over all shapes

    return resultPoints;
  }

  HashSet<Vector2Int> getEdgePoints(ICollection<Box2D> existingShapes, Box2D underShape)
  {
    /**
     * Get the edge points of the under shape that are not "covered" by any of the existing shapes.
     * NOTE: This function ONLY returns the edge points of the under shape, not the entire compound shape shape.
     *
     * @param existingShapes - The existing shapes.
     * @param underShape - The under shape.
     * @return A set of edge points.
     */
    // All edge points of the under shape
    HashSet<Vector2Int> underShapePoints = underShape.GetAllEdgePoints();

    // The edges of the under shape that are not "covered" by the over shape
    HashSet<Vector2Int> resultPoints = new();
    foreach (Vector2Int point in underShapePoints)
    {
      // Check every existing shape, if any of them contain the under shape point, then
      // the point is considered "covered" and should not be added to the result.
      bool isCovered = false;
      foreach (Box2D existingShape in existingShapes)
      {
        if (existingShape.Contains(point.x, point.y))
        {
          // The point is covered by an existing shape, so it should not be added to the result.
          isCovered = true;
          break;
        }
      }
      if (!isCovered)
      {
        resultPoints.Add(point);
      }
    }

    // This should contain all the edge points of the under shape that are not "covered" by the over shape.
    return resultPoints;
  }

  HashSet<Vector2Int> getEdgePoints_TopToBottom(List<Box2D> topToBottomShapes)
  {
    /**
     * Get the edge points of the compound shape (topToBottomShapes[0] + topToBottomShapes[1] + ... + topToBottomShapes[n])
     * that are not "covered" by any of the earlier shapes in the provided list.
     *
     * The shape at index 0 is the top shape, and the shape at index n is the bottom shape.
     *
     * @param topToBottomShapes - The top to bottom shapes.
     * @return A set of edge points.
     */
    HashSet<Vector2Int> resultPoints = new();
    List<Box2D> alreadyProcessedShapes = new();
    for (int i = 0; i < topToBottomShapes.Count; i++)
    {
      Box2D currentShape = topToBottomShapes[i];
      HashSet<Vector2Int> currentShapePoints = getEdgePoints(alreadyProcessedShapes, currentShape);
      resultPoints.UnionWith(currentShapePoints);
      alreadyProcessedShapes.Add(currentShape);
    }
    return resultPoints;
  }

  Box2D GenerateValidBox(List<Box2D> existingShapes, float width, float height)
  {
    /**
     * Generate a new box that does not have edges touching any existing boxes.
     * The box will be centered at a random point on the outer edge of the compound shape.
     *
     * @param existingShapes - The list of existing box shapes.
     * @param width - The width of the new box.
     * @param height - The height of the new box.
     * @return A valid Box2D that does not edge-touch any existing boxes.
     */

    HashSet<Vector2Int> outerEdgePoints = getOuterEdgePoints(existingShapes);
    if (outerEdgePoints.Count == 0)
    {
      Debug.LogWarning("No outer edge points available for box generation.");
      throw new Exception("No outer edge points available for box generation.");
    }
    const int MAX_RETRIES = 100;
    for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
    {
      Vector2Int randomPoint = rng.randomElem_Set(outerEdgePoints);
      Box2D candidateBox = Box2D.Box2D_CenterPt(
        centerX: randomPoint.x,
        centerY: randomPoint.y,
        width: width,
        height: height,
        positiveYDown: false
      );

      // Check if any existing box has "edge touching" (ie, parallel and adjacent, but no overlap or share an edge)
      // with the candidate box.
      bool invalid = existingShapes.Any(existingBox => candidateBox.IsEdgeTouching(existingBox));
      if (!invalid)
        return candidateBox;
    }

    // If we couldn't find a valid box after retries, log a warning and return the last attempt
    // (This shouldn't happen often, but we need to handle the edge case)
    Debug.LogWarning(
      $"Could not generate a valid box after {MAX_RETRIES} attempts. Returning last candidate."
    );
    throw new Exception("Could not generate a valid box after retries.");
  }

  // Dictionary<CardinalDirection, List<Vector2Int>> getLineFromPoint(
  //   HashSet<Vector2Int> points,
  //   Vector2Int point
  // )
  // {
  //   /**
  //    * Get the line from the point to the nearest point in the set of points.
  //    *
  //    * @param points - The set of points.
  //    * @param point - The point to get the line from.
  //    * @return A dictionary of the line from the point to the nearest point in the set of points.
  //    */

  //   Dictionary<CardinalDirection, List<Vector2Int>> result = new()
  //   {
  //     { CardinalDirection.North, new() },
  //     { CardinalDirection.South, new() },
  //     { CardinalDirection.East, new() },
  //     { CardinalDirection.West, new() },
  //   };
  //   if (!points.Contains(point))
  //   {
  //     return result;
  //   }

  //   // Check north direction. Greedily go in that direction until we don't find a point in the set.
  //   foreach (CardinalDirection dir in CD_Util.ALL_DIRECTIONS)
  //   {
  //     List<Vector2Int> line = new();
  //     int distance = 0;

  //     while (true)
  //     {
  //       Vector2Int nextPoint = point + CD_Util.GetVector(dir, distance);
  //       if (!points.Contains(nextPoint) || distance > MAX_DISTANCE)
  //         break;
  //       line.Add(nextPoint);
  //       distance++;
  //     }
  //     result[dir] = line;
  //   }
  //   return result;
  // }

  List<Road> ExtractRoutes(HashSet<Vector2Int> points, Vector2Int startingPoint)
  {
    /**
     * Extracts axis-aligned road segments from a set of grid points.
     *
     * Each road is a straight-line segment (only North/South or East/West). Direction
     * changes at corners or intersections define segment boundaries, similar to how routes
     * work in Pokemon games where each route is a single straight path.
     *
     * EXAMPLE: Given this shape, the algorithm extracts 6 routes:
     *
     *       col: 123456
     *            ------
     *   row 1:   222223    Route 2: top horizontal (almost full width)
     *   row 2:   1    3    Route 1: left vertical (almost full height)
     *   row 3:   1    3    Route 3: right vertical (rows 1-4)
     *   row 4:   1    3
     *   row 5:   155555    Route 5: middle horizontal (almost full width)
     *   row 6:   1    6    Route 6: single point (vertical connector)
     *   row 7:   144444    Route 4: bottom horizontal (almost full width)
     *
     * CORNER vs INTERSECTION:
     * - Corner: A point with exactly 2 orthogonal neighbors (e.g., where route 2 meets route 3)
     * - Intersection: A point with 3+ neighbors (e.g., where routes 5, 3, and 6 meet)
     *
     * ROUTE OWNERSHIP:
     * Each point belongs to exactly one route. When multiple routes could claim a corner or
     * intersection, the first route to reach it (in BFS order) claims it. This is deterministic
     * based on exploration order from the starting point.
     *
     * ALGORITHM (BFS-based):
     * 1. Start from a corner point with exactly 2 neighbors.
     * 2. Initialize frontier with: (startingPoint, firstDirection) and (firstNeighborInSecondDir, secondDirection)
     * 3. For each frontier entry, walk in that direction, claiming points until:
     *    - We step off the edge (next point not in set), OR
     *    - We hit an already-claimed point (route collision)
     * 4. While walking, check orthogonal directions for branches; add unclaimed neighbors to frontier.
     * 5. Repeat until frontier is empty.
     *
     * LIMITATIONS:
     * - Starting point MUST have exactly 2 neighbors (be a corner). Intersections/endpoints not supported.
     * - Only explores points reachable from startingPoint. Disconnected regions are ignored.
     * - Routes may be as short as 1 point (see route 6 in example), but will never generate 0 length route.
     *
     * @param points - Set of grid points forming the shape's edges.
     * @param startingPoint - A corner point (exactly 2 neighbors) to begin extraction from.
     * @return List of Roads, where each Road is a straight segment of the original shape.
     */

    List<Road> result = new();
    Queue<Tuple<Vector2Int, CardinalDirection>> frontier = new();
    HashSet<Vector2Int> alreadyExplored = new();

    if (!points.Contains(startingPoint))
    {
      Debug.LogWarning(
        $"Starting point {startingPoint} is not in the points set. This is probably a bug."
      );
      return result;
    }

    // Start with the starting point
    // Check all 4 cardinal directions for neighbors that are in the points set.
    int neighborsFound = 0;
    foreach (CardinalDirection dir in CD_Util.ALL_DIRECTIONS)
    {
      Vector2Int neighbor = startingPoint + CD_Util.GetVector(dir);
      if (points.Contains(neighbor))
      {
        if (neighborsFound == 0)
        {
          // Add the starting point itself. After this, the neighbor will be added to the frontier.
          frontier.Enqueue(new(startingPoint, dir));
        }
        else
        {
          frontier.Enqueue(new(neighbor, dir));
        }
        neighborsFound++;
      }
    }

    if (neighborsFound == 0)
    {
      Debug.LogWarning(
        $"No neighbors found for starting point {startingPoint}. This is probably a bug. You gave me a starting point on an island by itself?."
      );
      return result;
    }
    else if (neighborsFound != 2)
    {
      Debug.LogWarning(
        $"Starting point {startingPoint} has {neighborsFound} neighbors. This is probably a bug. You gave me a starting point that's not a corner?."
      );
      return result;
    }

    // BFS traversal: process each (point, direction) pair from the frontier.
    // For each pair, walk in that direction building a route until we step off the edge or hit a claimed point.

    while (frontier.Count > 0)
    {
      Tuple<Vector2Int, CardinalDirection> current = frontier.Dequeue();
      Vector2Int routeStartPoint = current.Item1;
      Vector2Int currentPoint = routeStartPoint;
      CardinalDirection currentDirection = current.Item2;
      List<Vector2Int> routePoints = new();

      // Walk in currentDirection, claiming points until we exit the shape or hit a claimed point.
      while (points.Contains(currentPoint))
      {
        if (alreadyExplored.Contains(currentPoint))
        {
          break; // Hit an already-claimed point (route intersection).
        }

        routePoints.Add(currentPoint);
        alreadyExplored.Add(currentPoint);

        // Check orthogonal neighbors for branches (corners/intersections) and add to frontier.
        foreach (CardinalDirection orthogonalDir in CD_Util.OrthogonalTo(currentDirection))
        {
          Vector2Int orthogonalPoint = currentPoint + CD_Util.GetVector(orthogonalDir);
          if (points.Contains(orthogonalPoint))
          {
            frontier.Enqueue(new(orthogonalPoint, orthogonalDir));
          }
        }

        currentPoint += CD_Util.GetVector(currentDirection);
      }
      // Possible exit conditions:
      // Current point is not in the points set.
      // The current point is in the alreadyExplored set.
      // In either case, the current point was NOT added to the routePoints collector.
      if (routePoints.Count == 0)
      {
        // No route was found, this probably means we've attempted to travel one edge of a route that's
        // already been explored from the other side. Ignore this route.
        continue;
      }
      result.Add(new Road(routePoints));
    }

    return result;
  }

  List<Road> HookUpCityToRoads(City city, List<Road> roads)
  {
    /**
     * Hook up a city to a list of roads.
     * The city will be hooked up to the roads in a way that is compatible with the roads.
     *
     * @param city - The city to hook up to the roads.
     * @param roads - The list of roads to hook up the city to.
     */
    // Loop through all the roads, and check if the city is at the start or end of the road.
    // To deal with the case where the city is in the middle of the road, we need to see if the city
    // is along any of the points of the road. And if so, we need to break the road into two roads.

    Vector2Int cityPos = city.position;
    List<Road> resultRoads = new(roads); // Make a copy of the roads list.
    foreach (Road road in roads)
    {
      if (road.StartPoint == cityPos || road.EndPoint == cityPos)
      {
        Road newRoad = InsertCityToRoad_StartOrEnd(city, road);
        resultRoads.Remove(road); // Remove the old road (with the city point)
        resultRoads.Add(newRoad); // Add the new road (without the city point)
      }
      else if (road.tilesInOrder.Contains(cityPos))
      {
        // Create two new roads:
        // -start of road to city
        // -city to end of road
        List<Road> newRoads = InsertCityToRoad_Middle(city, road);

        // Remove the old road from the result roads list, and add the two new roads.
        resultRoads.Remove(road);
        resultRoads.AddRange(newRoads);
      }
    } // End of foreach (Road road in roads)
    return resultRoads;
  }

  Road InsertCityToRoad_StartOrEnd(City city, Road road)
  {
    /**
     * Hook up the city with the road. It's required that the city point is
     * at the start or end of the road.
     * A new road will be created that does not contain the city point.
     * The city will be connected in proper direction slots to the new road.
     *
     * @param city - The city to insert into the road.
     * @param road - The road to insert the city into.
     * @return The new road.
     */
    // Figure out the direction of the road relative to the city.
    // Check the neighboring points of the city, and see which one is on the road still.
    // that'll give us the direction of the road relative to the city.
    CardinalDirection cityExitDirection = CardinalDirection.North;
    foreach (CardinalDirection dir in CD_Util.ALL_DIRECTIONS)
    {
      Vector2Int neighbor = city.position + CD_Util.GetVector(dir);
      if (road.tilesInOrder.Contains(neighbor))
      {
        cityExitDirection = dir;
        break; // Don't need to check the other directions
      }
    }

    // Create a new road that does not contain the city point.
    Road newRoad = road.RemovePoint(city.position, quiet: true);
    city.insertRoad(newRoad, cityExitDirection); // Hook up the city to the new road.
    return newRoad;
  }

  List<Road> InsertCityToRoad_Middle(City city, Road road)
  {
    /**
     * Insert a city into a road. This will split the road into two roads.
     * The two new roads will not contain the City point.
     * The city will be connected in proper direction slots to the two new roads.
     *
     * The city MUST be along the road, and the road MUST be a straight line.
     * The road MUST have at least 3 tiles.
     *
     * @param city - The city to insert into the road.
     * @param road - The road to insert the city into.
     * @return The two new roads.
     */

    if (road.tilesInOrder.Count < 3)
    {
      Debug.LogWarning($"Road {road} has less than 3 tiles. This is probably a bug.");
      return new List<Road>();
    }

    // Determine if the road is traveling east/ west or north/ south.
    bool isVertical = road.StartPoint.x == road.EndPoint.x;
    bool isHorizontal = road.StartPoint.y == road.EndPoint.y;

    // Now, create two new roads. One from the city to the first direction, and one from the city to the second direction.
    // These directions are the direction to the Start/End from the city.
    CardinalDirection dirToStart = CardinalDirection.North;
    CardinalDirection dirToEnd = CardinalDirection.South;
    if (isHorizontal)
    {
      // City is along a road traveling east/ west
      // Figure out if the start is west or east of the city
      if (road.StartPoint.x < city.position.x)
      {
        // Start is west of the city
        // Create a new road from the city to the start
        dirToStart = CardinalDirection.West;
        dirToEnd = CardinalDirection.East;
      }
      else
      {
        // Start is east of the city
        dirToStart = CardinalDirection.East;
        dirToEnd = CardinalDirection.West;
      }
    }
    else if (isVertical)
    {
      // City is along a road traveling north/ south
      // Figure out if the start is north or south of the city
      // Assume positive Y is upwards.
      if (road.StartPoint.y < city.position.y)
      {
        // Start is south of the city
        // Create a new road from the city to the start
        dirToStart = CardinalDirection.South;
        dirToEnd = CardinalDirection.North;
      }
      else
      {
        // Start is north of the city
        dirToStart = CardinalDirection.North;
        dirToEnd = CardinalDirection.South;
      }
    }
    else
    {
      // Something is wrong, we should have either west/ east or north/ south.
      Debug.LogWarning(
        $"City {city.position} is along a road traveling in an unexpected direction. This is probably a bug."
      );
      return new List<Road>();
    }

    Vector2Int cityStartSide = city.position + CD_Util.GetVector(dirToStart);
    Road newRoadStartToCity = new(
      NocabPixelLine.getPointsAlongLine(cityStartSide, road.StartPoint)
    );
    city.insertRoad(newRoadStartToCity, dirToStart);

    Vector2Int cityEndSide = city.position + CD_Util.GetVector(dirToEnd);
    Road newRoadCityToEnd = new(NocabPixelLine.getPointsAlongLine(cityEndSide, road.EndPoint));
    city.insertRoad(newRoadCityToEnd, dirToEnd);

    return new List<Road> { newRoadStartToCity, newRoadCityToEnd };
  }
}
