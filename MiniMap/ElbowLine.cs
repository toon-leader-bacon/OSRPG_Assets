using System;
using System.Collections.Generic;
using UnityEngine;


public class ElbowLine
{

  public enum ElbowTypes
  {
    OneTurn = 1,
    TwoTurn = 2
  }
  public static readonly List<ElbowTypes> AllElbowTypes = new() {
    ElbowTypes.OneTurn, ElbowTypes.TwoTurn };

  readonly NocabRNG rng;

  #region Constructors

  public ElbowLine()
  {
    this.rng = NocabRNG.defaultRNG;
  }

  public ElbowLine(NocabRNG rng)
  {
    this.rng = rng;
  }

  #endregion

  #region Connect Points

  public static List<Vector2Int> ConnectPoints(Vector2Int start, Vector2Int end, bool startHorizontal, ElbowTypes connectionType)
  {
    // Vertical or Horizontal line
    if (start.x == end.x || start.y == end.y)
    { return NocabPixelLine.getPointsAlongLine(start, end); }

    switch (connectionType)
    {
      default:
      case ElbowTypes.OneTurn:
        return OneTurn(start, end, startHorizontal);
      case ElbowTypes.TwoTurn:
        return TwoTurn(start, end, startHorizontal);
    }
  }

  public static List<Vector2Int> ConnectPoints(Vector2Int start, Vector2Int end, bool startHorizontal, NocabRNG rng)
  {
    // Vertical or Horizontal line
    if (start.x == end.x || start.y == end.y)
    { return NocabPixelLine.getPointsAlongLine(start, end); }

    return ConnectPoints(start, end, startHorizontal, rng.randomElem(AllElbowTypes));
  }

  public static List<Vector2Int> ConnectPoints_static(Vector2Int start, Vector2Int end, bool startHorizontal)
  { return ConnectPoints(start, end, startHorizontal, NocabRNG.defaultRNG); }
  public static List<Vector2Int> ConnectPoints_static(Vector2Int start, Vector2Int end)
  {
    NocabRNG rng = NocabRNG.defaultRNG;
    return ConnectPoints(start, end, rng.generateBool(), rng);
  }

  public List<Vector2Int> ConnectPoints(Vector2Int start, Vector2Int end, bool startHorizontal)
  { return ConnectPoints(start, end, startHorizontal, this.rng); }

  public List<Vector2Int> ConnectPoints(Vector2Int start, Vector2Int end)
  { return ConnectPoints(start, end, this.rng.generateBool()); }

  #endregion 

  #region OneTurn
  public List<Vector2Int> OneTurn(Vector2Int start, Vector2Int end)
  { return OneTurn(start, end, rng.generateBool()); }

  public static List<Vector2Int> OneTurn_static(Vector2Int start, Vector2Int end)
  { return OneTurn(start, end, NocabRNG.defaultRNG.generateBool()); }

  public static List<Vector2Int> OneTurn(Vector2Int start, Vector2Int end, bool startHorizontal)
  {

    if (start.x == end.x || start.y == end.y)
    {
      // Vertical or Horizontal line
      // No elbow possible
      return NocabPixelLine.getPointsAlongLine(start, end);
    }

    Box2D box = Box2D.Box2D_CornerPoints(start.x, start.y,
                                         end.x, end.y,
                                         positiveYDown: false);

    Vector2Int elbowPt;

    if (start.x < end.x)
    { // start is left
      if (start.y < end.y)
      { // start is bot-left
        // end must be top-right\
        // Elbow point is either bot-right OR top-left
        // Horizontal from BL is BR
        Vector2Int horizElbow = new((int)box.BR.x, (int)box.BR.y);
        Vector2Int vertElbow = new((int)box.TL.x, (int)box.TL.y);
        elbowPt = startHorizontal ? horizElbow : vertElbow;
      }
      else
      { // start is top-left
        // end must be bot-right
        // Elbow point is either bot-left OR top-right
        // Horizontal from TL is TR
        Vector2Int horizElbow = new((int)box.TR.x, (int)box.TR.y);
        Vector2Int vertElbow = new((int)box.BL.x, (int)box.BL.y);
        elbowPt = startHorizontal ? horizElbow : vertElbow;
      }
    }
    else
    { // start is right
      if (start.y < end.y)
      { // start is bot-right
        // end must be top-left
        // Elbow point is either bot-left OR top-right
        // Horizontal from BR is BL
        Vector2Int horizElbow = new((int)box.BL.x, (int)box.BL.y);
        Vector2Int vertElbow = new((int)box.TR.x, (int)box.TR.y);
        elbowPt = startHorizontal ? horizElbow : vertElbow;
      }
      else
      { // start is top-right
        // end must be bot-left
        // Elbow point is either bot-right OR top-left
        // Horizontal from BL is BR
        Vector2Int horizElbow = new((int)box.BR.x, (int)box.BR.y);
        Vector2Int vertElbow = new((int)box.TL.x, (int)box.TL.y);
        elbowPt = startHorizontal ? horizElbow : vertElbow;
      }
    }

    return PixelArtShapes.ConnectTheDots(new List<Vector2Int>{
      start,
      elbowPt,
      end
    });
  }

  #endregion

  #region TwoTurn

  public List<Vector2Int> TwoTurn(Vector2Int start, Vector2Int end)
  { return TwoTurn(start, end, this.rng.generateBool()); }

  public static List<Vector2Int> TwoTurn_static(Vector2Int start, Vector2Int end)
  { return TwoTurn(start, end, NocabRNG.defaultRNG.generateBool()); }

  public static List<Vector2Int> TwoTurn(Vector2Int start, Vector2Int end, bool startHorizontal)
  {
    if (start.x == end.x || start.y == end.y)
    {
      // Vertical or Horizontal line
      // No elbow possible
      return NocabPixelLine.getPointsAlongLine(start, end);
    }

    Vector2Int pt1;
    Vector2Int pt2;

    if (startHorizontal)
    {
      // Go horizontally (pos x direction) then at a certain point cut up 
      int deltaX = end.x - start.x;
      float percentage = NocabRNG.defaultRNG.generateFloat(0.333f, 0.666f);
      int cutAcrossX = start.x + ((int)(deltaX * percentage));

      pt1 = new(cutAcrossX, start.y);
      pt2 = new(cutAcrossX, end.y);
    }
    else
    {
      // Start Vertical
      // Go vertically (pos y direction) then at a certain point cut over
      int deltaY = end.y - start.y;
      float percentage = NocabRNG.defaultRNG.generateFloat(0.333f, 0.666f);
      int cutAcrossY = start.y + ((int)(deltaY * percentage));

      pt1 = new(start.x, cutAcrossY);
      pt2 = new(end.x, cutAcrossY);
    }

    return PixelArtShapes.ConnectTheDots(new List<Vector2Int>{
      start,
      pt1,
      pt2,
      end
    });

  }

  #endregion
}