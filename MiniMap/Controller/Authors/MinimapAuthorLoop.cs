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

    int centerX = 0;
    int centerY = 0;

    int leftEdgeX = centerX - (loopWidth / 2);
    int rightEdgeX = centerX + (loopWidth / 2);
    int topEdgeY = centerY + (loopHeight / 2);
    int bottomEdgeY = centerY - (loopHeight / 2);

    // Compute corners
    Vector2Int TL = new(leftEdgeX, topEdgeY);
    Vector2Int TR = new(rightEdgeX, topEdgeY);
    Vector2Int BL = new(leftEdgeX, bottomEdgeY);
    Vector2Int BR = new(rightEdgeX, bottomEdgeY);

    // For now, put a city at each corner
    City TL_city = new(TL);
    City TR_city = new(TR);
    City BL_city = new(BL);
    City BR_city = new(BR);
    List<City> cities = new() { TL_city, TR_city, BL_city, BR_city };

    // Connect the cities with roads
    // Let's focus on connection TL to TR first.
    // Each city has a Cardinal Direction for each of its 4 sides.
    // The TL to TR connection should either be a North or East connection (from TL)
    // connection to a North or West connection (into TR)
    #region TL to TR
    CardinalDirection TL_Exit_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.North, CardinalDirection.East }
    );
    Vector2Int TL_Exit_tile = TL_city.position + CD_Util.GetVector(TL_Exit_Direction);

    CardinalDirection TR_Enter_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.North, CardinalDirection.West }
    );
    Vector2Int TR_Enter_tile = TR_city.position + CD_Util.GetVector(TR_Enter_Direction);

    Road TL_TR_road = new(ElbowLine.TwoTurn_static(TL_Exit_tile, TR_Enter_tile));
    TL_city.insertRoad(TL_TR_road, TL_Exit_Direction);
    TR_city.insertRoad(TL_TR_road, TR_Enter_Direction);
    #endregion

    #region TR to BR
    // TODO: We need to be smarter about picking the exit direction.
    // The Entering direction (from the other city) should be considered, so we effectively
    // need to find what directions are empty for the city, and unit it with the specified
    // exit directions before we pick a random one.
    HashSet<CardinalDirection> possibleExitDirections = new()
    {
      CardinalDirection.East,
      CardinalDirection.South,
    };
    // Compute union of available directions
    possibleExitDirections.IntersectWith(TR_city.UnoccupiedDirections());
    CardinalDirection TR_Exit_Direction = rng.randomElem_Set(possibleExitDirections);
    Vector2Int TR_Exit_tile = TR_city.position + CD_Util.GetVector(TR_Exit_Direction);

    CardinalDirection BR_Enter_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.North, CardinalDirection.East }
    );
    Vector2Int BR_Enter_tile = BR_city.position + CD_Util.GetVector(BR_Enter_Direction);

    Road TR_BR_road = new(ElbowLine.TwoTurn_static(TR_Exit_tile, BR_Enter_tile));
    TR_city.insertRoad(TR_BR_road, TR_Exit_Direction);
    BR_city.insertRoad(TR_BR_road, BR_Enter_Direction);

    #endregion

    #region BR to BL
    possibleExitDirections = new() { CardinalDirection.South, CardinalDirection.West };
    possibleExitDirections.IntersectWith(BR_city.UnoccupiedDirections());
    CardinalDirection BR_Exit_Direction = rng.randomElem_Set(possibleExitDirections);
    Vector2Int BR_Exit_tile = BR_city.position + CD_Util.GetVector(BR_Exit_Direction);

    CardinalDirection BL_Enter_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.South, CardinalDirection.East }
    );
    Vector2Int BL_Enter_tile = BL_city.position + CD_Util.GetVector(BL_Enter_Direction);

    Road BR_BL_road = new(ElbowLine.TwoTurn_static(BR_Exit_tile, BL_Enter_tile));
    BR_city.insertRoad(BR_BL_road, BR_Exit_Direction);
    BL_city.insertRoad(BR_BL_road, BL_Enter_Direction);

    #endregion

    #region BL to TL
    possibleExitDirections = new() { CardinalDirection.West, CardinalDirection.North };
    possibleExitDirections.IntersectWith(BL_city.UnoccupiedDirections());
    CardinalDirection BL_Exit_Direction = rng.randomElem_Set(possibleExitDirections);
    Vector2Int BL_Exit_tile = BL_city.position + CD_Util.GetVector(BL_Exit_Direction);

    CardinalDirection TL_Enter_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.West, CardinalDirection.South }
    );
    Vector2Int TL_Enter_tile = TL_city.position + CD_Util.GetVector(TL_Enter_Direction);

    Road BL_TL_road = new(ElbowLine.TwoTurn_static(BL_Exit_tile, TL_Enter_tile));
    BL_city.insertRoad(BL_TL_road, BL_Exit_Direction);
    TL_city.insertRoad(BL_TL_road, TL_Enter_Direction);
    #endregion

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

    int centerX = 0;
    int centerY = 0;

    int leftEdgeX = centerX - (loopWidth / 2);
    int rightEdgeX = centerX + (loopWidth / 2);
    int topEdgeY = centerY + (loopHeight / 2);
    int bottomEdgeY = centerY - (loopHeight / 2);

    // Each city will be a random point on the edge of the box
    // Valid points are in the middle 75% of the edge
    // Start with the top edge:
    int topCityX = rng.generateInt(
      (int)(leftEdgeX + (loopWidth * 0.125f)),
      (int)(rightEdgeX - (loopWidth * 0.125f))
    );
    int topCityY = topEdgeY;
    Debug.Log("topCityX: " + topCityX + ", topCityY: " + topCityY);
    City topCity = new(topCityX, topCityY);

    // Bottom edge:
    int bottomCityX = rng.generateInt(
      (int)(leftEdgeX + (loopWidth * 0.125f)),
      (int)(rightEdgeX - (loopWidth * 0.125f))
    );
    int bottomCityY = bottomEdgeY;
    Debug.Log("bottomCityX: " + bottomCityX + ", bottomCityY: " + bottomCityY);
    City bottomCity = new(bottomCityX, bottomCityY);

    // Right edge:
    int rightCityX = rightEdgeX;
    int rightCityY = rng.generateInt(
      (int)(bottomEdgeY + (loopHeight * 0.125f)),
      (int)(topEdgeY - (loopHeight * 0.125f))
    );
    Debug.Log(
      $"bottomEdgeY + (loopHeight * 0.125f) = {bottomEdgeY + (loopHeight * 0.125f)}, topEdgeY - (loopHeight * 0.125f) = {topEdgeY - (loopHeight * 0.125f)}"
    );
    Debug.Log("rightCityX: " + rightCityX + ", rightCityY: " + rightCityY);
    City rightCity = new(rightCityX, rightCityY);

    // Left edge:
    int leftCityX = leftEdgeX;
    int leftCityY = rng.generateInt(
      (int)(bottomEdgeY + (loopHeight * 0.125f)),
      (int)(topEdgeY - (loopHeight * 0.125f))
    );
    Debug.Log(
      $"bottomEdgeY + (loopHeight * 0.125f) = {bottomEdgeY + (loopHeight * 0.125f)}, topEdgeY - (loopHeight * 0.125f) = {topEdgeY - (loopHeight * 0.125f)}"
    );
    Debug.Log("leftCityX: " + leftCityX + ", leftCityY: " + leftCityY);
    City leftCity = new(leftCityX, leftCityY);

    List<City> cities = new() { topCity, bottomCity, rightCity, leftCity };

    // Connect the cities with roads
    // Let's focus on connection top to right connection first
    #region Top to Right
    HashSet<CardinalDirection> possibleExitDirections = new()
    {
      CardinalDirection.South,
      CardinalDirection.East,
    };
    possibleExitDirections.IntersectWith(topCity.UnoccupiedDirections());
    CardinalDirection Top_Exit_Direction = rng.randomElem_Set(possibleExitDirections);
    Vector2Int TopExitTile = topCity.position + CD_Util.GetVector(Top_Exit_Direction);

    HashSet<CardinalDirection> possibleEnterDirections = new()
    {
      CardinalDirection.North,
      CardinalDirection.West,
    };
    possibleEnterDirections.IntersectWith(rightCity.UnoccupiedDirections());
    CardinalDirection Right_Enter_Direction = rng.randomElem_Set(possibleEnterDirections);
    Vector2Int RightEnterTile = rightCity.position + CD_Util.GetVector(Right_Enter_Direction);

    Road TopRightRoad = new(ElbowLine.ConnectPoints_static(TopExitTile, RightEnterTile));
    topCity.insertRoad(TopRightRoad, Top_Exit_Direction);
    rightCity.insertRoad(TopRightRoad, Right_Enter_Direction);
    #endregion

    #region Right to Bottom
    possibleExitDirections = new() { CardinalDirection.West, CardinalDirection.South };
    possibleExitDirections.IntersectWith(rightCity.UnoccupiedDirections());
    CardinalDirection Right_Exit_Direction = rng.randomElem_Set(possibleExitDirections);
    Vector2Int RightExitTile = rightCity.position + CD_Util.GetVector(Right_Exit_Direction);

    CardinalDirection Bottom_Enter_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.East, CardinalDirection.North }
    );
    Vector2Int BottomEnterTile = bottomCity.position + CD_Util.GetVector(Bottom_Enter_Direction);

    Road RightBottomRoad = new(ElbowLine.ConnectPoints_static(RightExitTile, BottomEnterTile));
    rightCity.insertRoad(RightBottomRoad, Right_Exit_Direction);
    bottomCity.insertRoad(RightBottomRoad, Bottom_Enter_Direction);
    #endregion

    #region Bottom to Left
    possibleExitDirections = new() { CardinalDirection.North, CardinalDirection.West };
    possibleExitDirections.IntersectWith(bottomCity.UnoccupiedDirections());
    CardinalDirection Bottom_Exit_Direction = rng.randomElem_Set(possibleExitDirections);
    Vector2Int BottomExitTile = bottomCity.position + CD_Util.GetVector(Bottom_Exit_Direction);

    CardinalDirection Left_Enter_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.South, CardinalDirection.East }
    );
    Vector2Int LeftEnterTile = leftCity.position + CD_Util.GetVector(Left_Enter_Direction);

    Road BottomLeftRoad = new(ElbowLine.ConnectPoints_static(BottomExitTile, LeftEnterTile));
    bottomCity.insertRoad(BottomLeftRoad, Bottom_Exit_Direction);
    leftCity.insertRoad(BottomLeftRoad, Left_Enter_Direction);
    #endregion

    #region Left to Top
    possibleExitDirections = new() { CardinalDirection.East, CardinalDirection.North };
    possibleExitDirections.IntersectWith(leftCity.UnoccupiedDirections());
    CardinalDirection Left_Exit_Direction = rng.randomElem_Set(possibleExitDirections);
    Vector2Int LeftExitTile = leftCity.position + CD_Util.GetVector(Left_Exit_Direction);

    CardinalDirection Top_Enter_Direction = rng.randomElem(
      new List<CardinalDirection> { CardinalDirection.South, CardinalDirection.West }
    );
    Vector2Int TopEnterTile = topCity.position + CD_Util.GetVector(Top_Enter_Direction);

    Road LeftTopRoad = new(ElbowLine.ConnectPoints_static(LeftExitTile, TopEnterTile));
    leftCity.insertRoad(LeftTopRoad, Left_Exit_Direction);
    topCity.insertRoad(LeftTopRoad, Top_Enter_Direction);
    #endregion

    List<Road> roads = new() { TopRightRoad, RightBottomRoad, BottomLeftRoad, LeftTopRoad };
    return new(cities, roads);
  }
}
