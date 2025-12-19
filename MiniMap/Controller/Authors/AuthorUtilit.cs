using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using CD_Util = CardinalDirection_Util;

public class AuthorUtilities
{
  NocabRNG rng;

  public AuthorUtilities()
  {
    this.rng = NocabRNG.defaultRNG;
  }

  public AuthorUtilities(NocabRNG rng)
  {
    this.rng = rng;
  }

  public List<City> wiggleCityPositions(List<City> cities, int xWiggleDelta, int yWiggleDelta)
  {
    if (xWiggleDelta == 0 && yWiggleDelta == 0)
    {
      return cities;
    }
    xWiggleDelta = math.abs(xWiggleDelta);
    yWiggleDelta = math.abs(yWiggleDelta);
    List<City> wiggedCities = new();
    foreach (City city in cities)
    {
      wiggedCities.Add(
        new City(
          city.position.x + rng.generateInt(-xWiggleDelta, xWiggleDelta),
          city.position.y + rng.generateInt(-yWiggleDelta, yWiggleDelta)
        )
      );
    }
    return wiggedCities;
  }

  public Road connectCities(
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
     * The exit tile is the position of the city plus the exit direction. For example
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
    // Determine startHorizontal based on exit direction
    // If exiting horizontally (East/West), start horizontal; otherwise start vertical
    bool startHorizontal =
      AExitDirection == CardinalDirection.East || AExitDirection == CardinalDirection.West;
    switch (roadType)
    {
      case ElbowLine.ElbowTypes.OneTurn:
        road = new(ElbowLine.OneTurn(AExitTile, BEnterTile, startHorizontal));
        break;
      case ElbowLine.ElbowTypes.TwoTurn:
        road = new(ElbowLine.TwoTurn(AExitTile, BEnterTile, startHorizontal));
        break;
      default:
        throw new Exception("Invalid road type.");
    }
    cityA.insertRoad(road, AExitDirection);
    cityB.insertRoad(road, BEnterDirection);
    return road;
  }

  public Road connectCities(
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

  #region Connect Cities Automatic

  public Road connectCities_Automatic(City sourceCity, City destinationCity)
  {
    /**
     * Connect two cities with a road.
     * Return the road object.
     *
     */
    // Check if cities are the same
    if (sourceCity.position == destinationCity.position)
    {
      throw new Exception("Source and destination cities are the same.");
    }

    // Check for horizontal or vertical line
    if (sourceCity.position.y == destinationCity.position.y)
    {
      return connectCities_AutomaticHorizontal(sourceCity, destinationCity);
    }
    else if (sourceCity.position.x == destinationCity.position.x)
    {
      return connectCities_AutomaticVertical(sourceCity, destinationCity);
    }

    // Determine the relative positions of the two cities
    Box2D citiesOnCorners = new(
      sourceCity.position.x,
      sourceCity.position.y,
      destinationCity.position.x,
      destinationCity.position.y,
      positiveYDown: false
    );

    if (
      citiesOnCorners.IsOnLeftEdge(sourceCity.position)
      && citiesOnCorners.IsOnBottomEdge(sourceCity.position)
    )
    {
      // Road is traveling North East
      return connectCities_AutomaticNorthEast(sourceCity, destinationCity);
    }
    else if (
      citiesOnCorners.IsOnLeftEdge(sourceCity.position)
      && citiesOnCorners.IsOnTopEdge(sourceCity.position)
    )
    {
      // Road is traveling South East
      return connectCities_AutomaticSouthEast(sourceCity, destinationCity);
    }
    else if (
      citiesOnCorners.IsOnRightEdge(sourceCity.position)
      && citiesOnCorners.IsOnBottomEdge(sourceCity.position)
    )
    {
      // Road is traveling North West
      // Flip the source and destination cities and try again (should be South East)
      return connectCities_AutomaticSouthEast(destinationCity, sourceCity);
    }
    else if (
      citiesOnCorners.IsOnRightEdge(sourceCity.position)
      && citiesOnCorners.IsOnTopEdge(sourceCity.position)
    )
    {
      // Road is traveling South West
      // Flip the source and destination cities and try again (should be North East)
      return connectCities_AutomaticNorthEast(destinationCity, sourceCity);
    }
    else
    {
      throw new Exception("No compatible road found for the given cities.");
    }
  }

  #region Connect Cities Automatic Straight Lines
  protected Road connectCities_AutomaticHorizontal(City sourceCity, City destinationCity)
  {
    // Cities have same y coordinate, so the road is traveling horizontally
    // Determine the direction of the road
    if (sourceCity.position.x < destinationCity.position.x)
    {
      // Road is traveling East
      HashSet<CardinalDirection> sourceExitDirections = new()
      {
        CardinalDirection.North,
        CardinalDirection.East,
        CardinalDirection.South,
      };
      sourceExitDirections.IntersectWith(sourceCity.UnoccupiedDirections());
      if (sourceExitDirections.Count == 0)
      {
        throw new Exception("No compatible exit directions found for source city.");
      }
      CardinalDirection sourceExitDirection = rng.randomElem_Set(sourceExitDirections);
      Vector2Int sourceExitTile = sourceCity.position + CD_Util.GetVector(sourceExitDirection);

      HashSet<CardinalDirection> destinationEnterDirections = new()
      {
        CardinalDirection.North,
        CardinalDirection.West,
        CardinalDirection.South,
      };
      destinationEnterDirections.IntersectWith(destinationCity.UnoccupiedDirections());
      if (destinationEnterDirections.Count == 0)
      {
        throw new Exception("No compatible enter directions found for destination city.");
      }
      CardinalDirection destinationEnterDirection = rng.randomElem_Set(destinationEnterDirections);
      Vector2Int destinationEnterTile =
        destinationCity.position + CD_Util.GetVector(destinationEnterDirection);

      // Possible combos of enter exit:
      /*
       * Straight Line:
       * North -> North
       * East -> West
       * South -> South
       * 2 Turn:
       * North -> West
       * North -> South
       * East -> North
       * East -> South
       * South -> North
       * South -> East
       */
      Road road;
      if (
        sourceExitDirection == CardinalDirection.North
          && destinationEnterDirection == CardinalDirection.North
        || sourceExitDirection == CardinalDirection.East
          && destinationEnterDirection == CardinalDirection.West
        || sourceExitDirection == CardinalDirection.South
          && destinationEnterDirection == CardinalDirection.South
      )
      {
        road = new(NocabPixelLine.getPointsAlongLine(sourceExitTile, destinationEnterTile));
      }
      else
      {
        road = new(ElbowLine.TwoTurn(sourceExitTile, destinationEnterTile, startHorizontal: true));
      }
      sourceCity.insertRoad(road, sourceExitDirection);
      destinationCity.insertRoad(road, destinationEnterDirection);
      return road;
    }
    else
    {
      // Road is traveling West flip it around and try again (should be East)
      return connectCities_AutomaticHorizontal(destinationCity, sourceCity);
    }
  }

  protected Road connectCities_AutomaticVertical(City sourceCity, City destinationCity)
  {
    /**
     * The road from source to destination is traveling vertically.
     * Assume Positive Y is upwards.
     */
    if (sourceCity.position.y < destinationCity.position.y)
    {
      // Road is traveling North
      HashSet<CardinalDirection> sourceExitDirections = new()
      {
        CardinalDirection.East,
        CardinalDirection.North,
        CardinalDirection.West,
      };
      sourceExitDirections.IntersectWith(sourceCity.UnoccupiedDirections());
      if (sourceExitDirections.Count == 0)
      {
        throw new Exception("No compatible exit directions found for source city.");
      }
      CardinalDirection sourceExitDirection = rng.randomElem_Set(sourceExitDirections);
      Vector2Int sourceExitTile = sourceCity.position + CD_Util.GetVector(sourceExitDirection);

      HashSet<CardinalDirection> destinationEnterDirections = new()
      {
        CardinalDirection.East,
        CardinalDirection.South,
        CardinalDirection.West,
      };
      destinationEnterDirections.IntersectWith(destinationCity.UnoccupiedDirections());
      if (destinationEnterDirections.Count == 0)
      {
        throw new Exception("No compatible enter directions found for destination city.");
      }
      CardinalDirection destinationEnterDirection = rng.randomElem_Set(destinationEnterDirections);
      Vector2Int destinationEnterTile =
        destinationCity.position + CD_Util.GetVector(destinationEnterDirection);

      // Possible combos of enter exit:
      /*
       * Straight Line:
       * East -> East
       * North -> South
       * West -> West
       * 2 Turn:
       * East -> South
       * East -> West
       * North -> East
       * North -> West
       * West -> South
       * West -> East
       */
      Road road;
      if (
        sourceExitDirection == CardinalDirection.East
          && destinationEnterDirection == CardinalDirection.East
        || sourceExitDirection == CardinalDirection.North
          && destinationEnterDirection == CardinalDirection.South
        || sourceExitDirection == CardinalDirection.West
          && destinationEnterDirection == CardinalDirection.West
      )
      {
        road = new(NocabPixelLine.getPointsAlongLine(sourceExitTile, destinationEnterTile));
      }
      else
      {
        road = new(ElbowLine.TwoTurn(sourceExitTile, destinationEnterTile, startHorizontal: false));
      }
      sourceCity.insertRoad(road, sourceExitDirection);
      destinationCity.insertRoad(road, destinationEnterDirection);
      return road;
    }
    else
    {
      // Road is traveling South flip it around and try again (should be North)
      return connectCities_AutomaticVertical(destinationCity, sourceCity);
    }
  }

  #endregion

  #region Connect Cities Automatic North East
  protected Road connectCities_AutomaticNorthEast(City sourceCity, City destinationCity)
  {
    /**
     * The road from source to destination is traveling North East.
     */
    // Source city stuff
    HashSet<CardinalDirection> sourceExitDirections = new()
    {
      CardinalDirection.North,
      CardinalDirection.East,
    };
    sourceExitDirections.IntersectWith(sourceCity.UnoccupiedDirections());
    if (sourceExitDirections.Count == 0)
    {
      throw new Exception("No compatible exit directions found for source city.");
    }
    CardinalDirection sourceExitDirection = rng.randomElem_Set(sourceExitDirections);
    Vector2Int sourceExitTile = sourceCity.position + CD_Util.GetVector(sourceExitDirection);

    // Dest city stuff
    HashSet<CardinalDirection> destinationEnterDirections = new()
    {
      CardinalDirection.South,
      CardinalDirection.West,
    };
    destinationEnterDirections.IntersectWith(destinationCity.UnoccupiedDirections());
    if (destinationEnterDirections.Count == 0)
    {
      throw new Exception("No compatible enter directions found for destination city.");
    }
    CardinalDirection destinationEnterDirection = rng.randomElem_Set(destinationEnterDirections);
    Vector2Int destinationEnterTile =
      destinationCity.position + CD_Util.GetVector(destinationEnterDirection);

    // Determine the road type
    ElbowLine.ElbowTypes roadType = ElbowLine.ElbowTypes.TwoTurn; // Default to 2 turn road
    if (
      sourceExitDirection == CardinalDirection.North
      && destinationEnterDirection == CardinalDirection.West
    )
    {
      roadType = ElbowLine.ElbowTypes.OneTurn;
    }
    else if (
      sourceExitDirection == CardinalDirection.East
      && destinationEnterDirection == CardinalDirection.South
    )
    {
      roadType = ElbowLine.ElbowTypes.OneTurn;
    }
    else if (
      sourceExitDirection == CardinalDirection.North
      && destinationEnterDirection == CardinalDirection.South
    )
    {
      roadType = ElbowLine.ElbowTypes.TwoTurn;
    }
    else if (
      sourceExitDirection == CardinalDirection.East
      && destinationEnterDirection == CardinalDirection.West
    )
    {
      roadType = ElbowLine.ElbowTypes.TwoTurn;
    }

    Road road;
    // If exiting horizontally (East/West), start horizontal; otherwise start vertical
    bool startHorizontal =
      sourceExitDirection == CardinalDirection.East
      || sourceExitDirection == CardinalDirection.West;
    if (roadType == ElbowLine.ElbowTypes.TwoTurn)
    {
      road = new(ElbowLine.TwoTurn(sourceExitTile, destinationEnterTile, startHorizontal));
    }
    else
    {
      road = new(ElbowLine.OneTurn(sourceExitTile, destinationEnterTile, startHorizontal));
    }
    sourceCity.insertRoad(road, sourceExitDirection);
    destinationCity.insertRoad(road, destinationEnterDirection);
    return road;
  }
  #endregion

  #region Connect Cities Automatic South East
  protected Road connectCities_AutomaticSouthEast(City sourceCity, City destinationCity)
  {
    /**
     * The road from source to destination is traveling South East.
     */
    // Source city stuff
    HashSet<CardinalDirection> sourceExitDirections = new()
    {
      CardinalDirection.South,
      CardinalDirection.East,
    };
    sourceExitDirections.IntersectWith(sourceCity.UnoccupiedDirections());
    if (sourceExitDirections.Count == 0)
    {
      throw new Exception("No compatible exit directions found for source city.");
    }
    CardinalDirection sourceExitDirection = rng.randomElem_Set(sourceExitDirections);
    Vector2Int sourceExitTile = sourceCity.position + CD_Util.GetVector(sourceExitDirection);

    // Dest city stuff
    HashSet<CardinalDirection> destinationEnterDirections = new()
    {
      CardinalDirection.North,
      CardinalDirection.West,
    };
    destinationEnterDirections.IntersectWith(destinationCity.UnoccupiedDirections());
    if (destinationEnterDirections.Count == 0)
    {
      throw new Exception("No compatible enter directions found for destination city.");
    }
    CardinalDirection destinationEnterDirection = rng.randomElem_Set(destinationEnterDirections);
    Vector2Int destinationEnterTile =
      destinationCity.position + CD_Util.GetVector(destinationEnterDirection);

    // Determine the road type
    ElbowLine.ElbowTypes roadType = ElbowLine.ElbowTypes.TwoTurn; // Default to 2 turn road
    if (
      sourceExitDirection == CardinalDirection.South
      && destinationEnterDirection == CardinalDirection.West
    )
    {
      roadType = ElbowLine.ElbowTypes.OneTurn;
    }
    else if (
      sourceExitDirection == CardinalDirection.East
      && destinationEnterDirection == CardinalDirection.North
    )
    {
      roadType = ElbowLine.ElbowTypes.OneTurn;
    }
    else if (
      sourceExitDirection == CardinalDirection.South
      && destinationEnterDirection == CardinalDirection.North
    )
    {
      roadType = ElbowLine.ElbowTypes.TwoTurn;
    }
    else if (
      sourceExitDirection == CardinalDirection.East
      && destinationEnterDirection == CardinalDirection.West
    )
    {
      roadType = ElbowLine.ElbowTypes.TwoTurn;
    }

    Road road;
    // Determine startHorizontal based on exit direction
    // If exiting horizontally (East/West), start horizontal; otherwise start vertical
    bool startHorizontal =
      sourceExitDirection == CardinalDirection.East
      || sourceExitDirection == CardinalDirection.West;
    if (roadType == ElbowLine.ElbowTypes.TwoTurn)
    {
      road = new(ElbowLine.TwoTurn(sourceExitTile, destinationEnterTile, startHorizontal));
    }
    else
    {
      road = new(ElbowLine.OneTurn(sourceExitTile, destinationEnterTile, startHorizontal));
    }
    sourceCity.insertRoad(road, sourceExitDirection);
    destinationCity.insertRoad(road, destinationEnterDirection);
    return road;
  }
  #endregion

  #endregion
}
