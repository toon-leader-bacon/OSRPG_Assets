using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class ElbowLine
{
  readonly NocabRNG rng;

  public ElbowLine()
  {
    this.rng = NocabRNG.defaultRNG;
  }
  public ElbowLine(NocabRNG rng)
  {
    this.rng = rng;
  }

  public List<Vector2Int> getPts(Vector2Int start, Vector2Int end)
  {
    return ElbowLine.getPts(start, end, rng.generateBool());
  }

  public static List<Vector2Int> getPts_static(Vector2Int start, Vector2Int end)
  {
    return ElbowLine.getPts(start, end, NocabRNG.defaultRNG.generateBool());
  }

  public static List<Vector2Int> getPts(Vector2Int start, Vector2Int end, bool prioritizeHorizontal)
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

    Vector2Int elbowPt = Vector2Int.zero;

    if (start.x < end.x)
    { // start is left
      if (start.y < end.y)
      { // start is bot-left
        // end must be top-right\
        // Elbow point is either bot-right OR top-left
        // Horizontal from BL is BR
        Vector2Int horizElbow = new((int)box.BR.x, (int)box.BR.y);
        Vector2Int vertElbow = new((int)box.TL.x, (int)box.TL.y);
        elbowPt = prioritizeHorizontal ? horizElbow : vertElbow;
      }
      else
      { // start is top-left
        // end must be bot-right
        // Elbow point is either bot-left OR top-right
        // Horizontal from TL is TR
        Vector2Int horizElbow = new((int)box.TR.x, (int)box.TR.y);
        Vector2Int vertElbow = new((int)box.BL.x, (int)box.BL.y);
        elbowPt = prioritizeHorizontal ? horizElbow : vertElbow;
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
        elbowPt = prioritizeHorizontal ? horizElbow : vertElbow;
      }
      else
      { // start is top-right
        // end must be bot-left
        // Elbow point is either bot-right OR top-left
        // Horizontal from BL is BR
        Vector2Int horizElbow = new((int)box.BR.x, (int)box.BR.y);
        Vector2Int vertElbow = new((int)box.TL.x, (int)box.TL.y);
        elbowPt = prioritizeHorizontal ? horizElbow : vertElbow;
      }
    }

    return PixelArtShapes.ConnectTheDots(new List<Vector2Int>{
      start,
      elbowPt,
      end
    });
  }
}