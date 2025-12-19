using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class test_Box2D
{
  [SetUp]
  public void SetUp() { }

  #region Constructor Tests

  [Test]
  public void test_Constructor_TwoPoints_PositiveYDown()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.AreEqual(0, box.Left_X);
    Assert.AreEqual(0, box.Top_Y);
    Assert.AreEqual(10, box.Right_X);
    Assert.AreEqual(10, box.Bottom_Y);
    Assert.AreEqual(true, box.PositiveYDown);
  }

  [Test]
  public void test_Constructor_TwoPoints_PositiveYUp()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: false);
    Assert.AreEqual(0, box.Left_X);
    Assert.AreEqual(10, box.Top_Y);
    Assert.AreEqual(10, box.Right_X);
    Assert.AreEqual(0, box.Bottom_Y);
    Assert.AreEqual(false, box.PositiveYDown);
  }

  [Test]
  public void test_Constructor_ReversedPoints()
  {
    // Constructor should handle reversed points correctly
    Box2D box = new Box2D(10, 10, 0, 0, positiveYDown: true);
    Assert.AreEqual(0, box.Left_X);
    Assert.AreEqual(0, box.Top_Y);
    Assert.AreEqual(10, box.Right_X);
    Assert.AreEqual(10, box.Bottom_Y);
  }

  [Test]
  public void test_Box2D_TL_WithHeight_PositiveYDown()
  {
    Box2D box = Box2D.Box2D_TL_WithHeight(0, 0, 10, 5, positiveYDown: true);
    Assert.AreEqual(0, box.Left_X);
    Assert.AreEqual(0, box.Top_Y);
    Assert.AreEqual(10, box.Right_X);
    Assert.AreEqual(5, box.Bottom_Y);
  }

  [Test]
  public void test_Box2D_TL_WithHeight_PositiveYUp()
  {
    Box2D box = Box2D.Box2D_TL_WithHeight(0, 10, 10, 5, positiveYDown: false);
    Assert.AreEqual(0, box.Left_X);
    Assert.AreEqual(10, box.Top_Y);
    Assert.AreEqual(10, box.Right_X);
    Assert.AreEqual(5, box.Bottom_Y);
  }

  [Test]
  public void test_Box2D_CenterPt_PositiveYDown()
  {
    Box2D box = Box2D.Box2D_CenterPt(5, 5, 10, 10, positiveYDown: true);
    Assert.AreEqual(0, box.Left_X);
    Assert.AreEqual(0, box.Top_Y);
    Assert.AreEqual(10, box.Right_X);
    Assert.AreEqual(10, box.Bottom_Y);
  }

  [Test]
  public void test_Box2D_CenterPt_PositiveYUp()
  {
    Box2D box = Box2D.Box2D_CenterPt(5, 5, 10, 10, positiveYDown: false);
    Assert.AreEqual(0, box.Left_X);
    Assert.AreEqual(10, box.Top_Y);
    Assert.AreEqual(10, box.Right_X);
    Assert.AreEqual(0, box.Bottom_Y);
  }

  #endregion

  #region Dimension Tests

  [Test]
  public void test_Width()
  {
    Box2D box = new Box2D(0, 0, 10, 5, positiveYDown: true);
    Assert.AreEqual(10, box.Width);
  }

  [Test]
  public void test_Height()
  {
    Box2D box = new Box2D(0, 0, 10, 5, positiveYDown: true);
    Assert.AreEqual(5, box.Height);
  }

  [Test]
  public void test_SideLengths()
  {
    Box2D box = new Box2D(0, 0, 10, 5, positiveYDown: true);
    Assert.AreEqual(10, box.TopSideLength);
    Assert.AreEqual(10, box.BottomSideLength);
    Assert.AreEqual(5, box.LeftSideLength);
    Assert.AreEqual(5, box.RightSideLength);
  }

  #endregion

  #region Corner Point Tests

  [Test]
  public void test_CornerPoints_PositiveYDown()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.AreEqual((0f, 0f), box.TopLeft);
    Assert.AreEqual((10f, 0f), box.TopRight);
    Assert.AreEqual((0f, 10f), box.BottomLeft);
    Assert.AreEqual((10f, 10f), box.BottomRight);
  }

  [Test]
  public void test_CornerPoints_PositiveYUp()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: false);
    Assert.AreEqual((0f, 10f), box.TopLeft);
    Assert.AreEqual((10f, 10f), box.TopRight);
    Assert.AreEqual((0f, 0f), box.BottomLeft);
    Assert.AreEqual((10f, 0f), box.BottomRight);
  }

  [Test]
  public void test_Center()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.AreEqual((5f, 5f), box.Center);
  }

  [Test]
  public void test_VectorProperties()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.AreEqual(new Vector2(0, 0), box.TL_v);
    Assert.AreEqual(new Vector2(10, 0), box.TR_v);
    Assert.AreEqual(new Vector2(0, 10), box.BL_v);
    Assert.AreEqual(new Vector2(10, 10), box.BR_v);
    Assert.AreEqual(new Vector2(5, 5), box.Center_v);
  }

  #endregion

  #region Contains Tests

  [Test]
  public void test_Contains_CenterPoint()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(box.Contains(5, 5));
  }

  [Test]
  public void test_Contains_CornerPoints()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(box.Contains(0, 0));
    Assert.IsTrue(box.Contains(10, 0));
    Assert.IsTrue(box.Contains(0, 10));
    Assert.IsTrue(box.Contains(10, 10));
  }

  [Test]
  public void test_Contains_OutsidePoints()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsFalse(box.Contains(-1, 5));
    Assert.IsFalse(box.Contains(11, 5));
    Assert.IsFalse(box.Contains(5, -1));
    Assert.IsFalse(box.Contains(5, 11));
  }

  [Test]
  public void test_Contains_PositiveYUp()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: false);
    Assert.IsTrue(box.Contains(5, 5));
    Assert.IsTrue(box.Contains(0, 0));
    Assert.IsTrue(box.Contains(10, 10));
    Assert.IsFalse(box.Contains(5, 11));
    Assert.IsFalse(box.Contains(5, -1));
  }

  #endregion

  #region Edge Detection Tests

  [Test]
  public void test_IsOnLeftEdge()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(box.IsOnLeftEdge(new Vector2Int(0, 5)));
    Assert.IsTrue(box.IsOnLeftEdge(new Vector2Int(0, 0)));
    Assert.IsTrue(box.IsOnLeftEdge(new Vector2Int(0, 10)));
    Assert.IsFalse(box.IsOnLeftEdge(new Vector2Int(1, 5)));
    Assert.IsFalse(box.IsOnLeftEdge(new Vector2Int(0, 11)));
  }

  [Test]
  public void test_IsOnRightEdge()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(box.IsOnRightEdge(new Vector2Int(10, 5)));
    Assert.IsTrue(box.IsOnRightEdge(new Vector2Int(10, 0)));
    Assert.IsTrue(box.IsOnRightEdge(new Vector2Int(10, 10)));
    Assert.IsFalse(box.IsOnRightEdge(new Vector2Int(9, 5)));
  }

  [Test]
  public void test_IsOnTopEdge()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(box.IsOnTopEdge(new Vector2Int(5, 0)));
    Assert.IsTrue(box.IsOnTopEdge(new Vector2Int(0, 0)));
    Assert.IsTrue(box.IsOnTopEdge(new Vector2Int(10, 0)));
    Assert.IsFalse(box.IsOnTopEdge(new Vector2Int(5, 1)));
  }

  [Test]
  public void test_IsOnBottomEdge()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(box.IsOnBottomEdge(new Vector2Int(5, 10)));
    Assert.IsTrue(box.IsOnBottomEdge(new Vector2Int(0, 10)));
    Assert.IsTrue(box.IsOnBottomEdge(new Vector2Int(10, 10)));
    Assert.IsFalse(box.IsOnBottomEdge(new Vector2Int(5, 9)));
  }

  [Test]
  public void test_IsOnEdge()
  {
    Box2D box = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(box.IsOnEdge(new Vector2Int(0, 5)));
    Assert.IsTrue(box.IsOnEdge(new Vector2Int(10, 5)));
    Assert.IsTrue(box.IsOnEdge(new Vector2Int(5, 0)));
    Assert.IsTrue(box.IsOnEdge(new Vector2Int(5, 10)));
    Assert.IsFalse(box.IsOnEdge(new Vector2Int(5, 5)));
  }

  #endregion

  #region GetAllEdgePoints Tests

  [Test]
  public void test_GetAllEdgePoints_SmallBox()
  {
    Box2D box = new Box2D(0, 0, 2, 2, positiveYDown: true);
    HashSet<Vector2Int> edgePoints = box.GetAllEdgePoints();

    // Should have 8 edge points (4 corners + 4 edge midpoints)
    // (0,0), (1,0), (2,0)
    // (0,1),       (2,1)
    // (0,2), (1,2), (2,2)
    Assert.AreEqual(8, edgePoints.Count);
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(0, 0)));
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(1, 0)));
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(2, 0)));
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(0, 1)));
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(2, 1)));
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(0, 2)));
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(1, 2)));
    Assert.IsTrue(edgePoints.Contains(new Vector2Int(2, 2)));
  }

  [Test]
  public void test_GetAllEdgePoints_DoesNotIncludeInterior()
  {
    Box2D box = new Box2D(0, 0, 4, 4, positiveYDown: true);
    HashSet<Vector2Int> edgePoints = box.GetAllEdgePoints();

    // Interior points should not be included
    Assert.IsFalse(edgePoints.Contains(new Vector2Int(2, 2)));
    Assert.IsFalse(edgePoints.Contains(new Vector2Int(1, 1)));
  }

  #endregion

  #region SharesEdge Tests

  [Test]
  public void test_SharesEdge_VerticalEdge_LeftRight()
  {
    // Box A on left, Box B on right, sharing vertical edge
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: true);
    Box2D boxB = new Box2D(5, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(boxA.SharesEdge(boxB));
    Assert.IsTrue(boxB.SharesEdge(boxA));
  }

  [Test]
  public void test_SharesEdge_HorizontalEdge_TopBottom()
  {
    // Box A on top, Box B on bottom, sharing horizontal edge
    Box2D boxA = new Box2D(0, 0, 10, 5, positiveYDown: true);
    Box2D boxB = new Box2D(0, 5, 10, 10, positiveYDown: true);
    Assert.IsTrue(boxA.SharesEdge(boxB));
    Assert.IsTrue(boxB.SharesEdge(boxA));
  }

  [Test]
  public void test_SharesEdge_PartialOverlap()
  {
    // Boxes share edge but only partially overlap in range
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: true);
    Box2D boxB = new Box2D(5, 3, 10, 8, positiveYDown: true);
    Assert.IsTrue(boxA.SharesEdge(boxB));
    Assert.IsTrue(boxB.SharesEdge(boxA));
  }

  [Test]
  public void test_SharesEdge_NoSharedEdge_Separated()
  {
    // Boxes are separated by 1 unit
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: true);
    Box2D boxB = new Box2D(6, 0, 10, 10, positiveYDown: true);
    Assert.IsFalse(boxA.SharesEdge(boxB));
    Assert.IsFalse(boxB.SharesEdge(boxA));
  }

  [Test]
  public void test_SharesEdge_NoSharedEdge_NoOverlap()
  {
    // Boxes share X or Y coordinate but don't overlap in the other dimension
    Box2D boxA = new Box2D(0, 0, 5, 5, positiveYDown: true);
    Box2D boxB = new Box2D(5, 6, 10, 10, positiveYDown: true);
    Assert.IsFalse(boxA.SharesEdge(boxB));
    Assert.IsFalse(boxB.SharesEdge(boxA));
  }

  [Test]
  public void test_SharesEdge_OverlappingBoxes()
  {
    // Boxes overlap in interior (also share edge technically)
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Box2D boxB = new Box2D(5, 0, 15, 10, positiveYDown: true);
    Assert.IsFalse(boxA.SharesEdge(boxB)); // They overlap in interior, so they don't share an edge.

    Box2D boxC = new Box2D(10, 10, 20, 20, positiveYDown: true);
    Assert.IsTrue(boxA.SharesEdge(boxC));
    Assert.IsTrue(boxC.SharesEdge(boxA));
  }

  [Test]
  public void test_SharesEdge_PositiveYUp()
  {
    // Boxes overlap in interior (also share edge technically)
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: false);
    Box2D boxB = new Box2D(5, 0, 15, 10, positiveYDown: false);
    Assert.IsFalse(boxA.SharesEdge(boxB)); // They overlap in interior, so they don't share an edge.

    Box2D boxC = new Box2D(10, 10, 20, 20, positiveYDown: false);
    Assert.IsTrue(boxA.SharesEdge(boxC));
    Assert.IsTrue(boxC.SharesEdge(boxA));
  }

  #endregion

  #region IsEdgeTouching Tests

  [Test]
  public void test_IsEdgeTouching_VerticalEdges_OneApart()
  {
    // Box A right edge at x=5, Box B left edge at x=6 (1 apart)
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: true);
    Box2D boxB = new Box2D(6, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(boxA.IsEdgeTouching(boxB));
    Assert.IsTrue(boxB.IsEdgeTouching(boxA));
  }

  [Test]
  public void test_IsEdgeTouching_HorizontalEdges_OneApart()
  {
    // Box A bottom edge at y=5, Box B top edge at y=6 (1 apart)
    Box2D boxA = new Box2D(0, 0, 10, 5, positiveYDown: true);
    Box2D boxB = new Box2D(0, 6, 10, 10, positiveYDown: true);
    Assert.IsTrue(boxA.IsEdgeTouching(boxB));
    Assert.IsTrue(boxB.IsEdgeTouching(boxA));
  }

  [Test]
  public void test_IsEdgeTouching_PartialOverlap()
  {
    // Edges 1 apart but only partially overlapping in range
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: true);
    Box2D boxB = new Box2D(6, 5, 10, 15, positiveYDown: true);
    Assert.IsTrue(boxA.IsEdgeTouching(boxB));
  }

  [Test]
  public void test_IsEdgeTouching_SharedEdge_NotTouching()
  {
    // Boxes share an edge (0 apart), not 1 apart
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: true);
    Box2D boxB = new Box2D(5, 0, 10, 10, positiveYDown: true);
    Assert.IsFalse(boxA.IsEdgeTouching(boxB));
    Assert.IsFalse(boxB.IsEdgeTouching(boxA));
  }

  [Test]
  public void test_IsEdgeTouching_TwoApart_NotTouching()
  {
    // Edges are 2 apart, not 1 apart
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: true);
    Box2D boxB = new Box2D(7, 0, 10, 10, positiveYDown: true);
    Assert.IsFalse(boxA.IsEdgeTouching(boxB));
    Assert.IsFalse(boxB.IsEdgeTouching(boxA));
  }

  [Test]
  public void test_IsEdgeTouching_CornerTouching_NotEdgeTouching()
  {
    // Boxes are diagonally adjacent (corner to corner), Y ranges don't overlap
    Box2D boxA = new Box2D(0, 0, 5, 5, positiveYDown: true);
    Box2D boxB = new Box2D(6, 6, 10, 10, positiveYDown: true);
    Assert.IsFalse(boxA.IsEdgeTouching(boxB));
  }

  [Test]
  public void test_IsEdgeTouching_NoRangeOverlap_NotTouching()
  {
    // Edges are 1 apart but no overlap in perpendicular range
    Box2D boxA = new Box2D(0, 0, 5, 5, positiveYDown: true);
    Box2D boxB = new Box2D(6, 10, 10, 15, positiveYDown: true);
    Assert.IsFalse(boxA.IsEdgeTouching(boxB));
  }

  [Test]
  public void test_IsEdgeTouching_PositiveYUp()
  {
    // Test with positiveYDown = false
    Box2D boxA = new Box2D(0, 0, 5, 10, positiveYDown: false);
    Box2D boxB = new Box2D(6, 0, 10, 10, positiveYDown: false);
    Assert.IsTrue(boxA.IsEdgeTouching(boxB));

    // Horizontal edges 1 apart
    Box2D boxC = new Box2D(0, 0, 10, 5, positiveYDown: false);
    Box2D boxD = new Box2D(0, 6, 10, 10, positiveYDown: false);
    Assert.IsTrue(boxC.IsEdgeTouching(boxD));
  }

  #endregion

  #region Ranges Overlap Tests

  [Test]
  public void test_YRangesOverlap()
  {
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: true);

    // Box that is overlapping and above
    Box2D overlapAbove = new Box2D(0, -5, 10, 5, positiveYDown: true);
    Box2D overlapBelow = new Box2D(0, 5, 10, 15, positiveYDown: true);

    Box2D noOverlapAbove = new Box2D(0, -15, 10, -5, positiveYDown: true);
    Box2D noOverlapBelow = new Box2D(0, 15, 10, 25, positiveYDown: true);

    Box2D sharedEdgeAbove = new Box2D(0, -5, 10, 0, positiveYDown: true);
    Box2D sharedEdgeBelow = new Box2D(0, 10, 10, 15, positiveYDown: true);

    Box2D inside = new Box2D(1, 1, 9, 9, positiveYDown: true);

    Assert.IsTrue(boxA.YRangesOverlap(overlapAbove));
    Assert.IsTrue(overlapAbove.YRangesOverlap(boxA));
    Assert.IsTrue(boxA.YRangesOverlap(overlapBelow));
    Assert.IsTrue(overlapBelow.YRangesOverlap(boxA));

    Assert.IsFalse(boxA.YRangesOverlap(noOverlapAbove));
    Assert.IsFalse(noOverlapAbove.YRangesOverlap(boxA));
    Assert.IsFalse(boxA.YRangesOverlap(noOverlapBelow));
    Assert.IsFalse(noOverlapBelow.YRangesOverlap(boxA));

    Assert.IsTrue(boxA.YRangesOverlap(sharedEdgeAbove));
    Assert.IsTrue(sharedEdgeAbove.YRangesOverlap(boxA));
    Assert.IsTrue(boxA.YRangesOverlap(sharedEdgeBelow));
    Assert.IsTrue(sharedEdgeBelow.YRangesOverlap(boxA));
    Assert.IsTrue(boxA.YRangesOverlap(inside));
    Assert.IsTrue(inside.YRangesOverlap(boxA));
  }

  [Test]
  public void test_YRangesOverlap_IdenticalBoxes()
  {
    // A box should overlap with itself (reflexive property)
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(boxA.YRangesOverlap(boxA));

    Box2D boxB = new Box2D(-5, -5, 5, 5, positiveYDown: false);
    Assert.IsTrue(boxB.YRangesOverlap(boxB));
  }

  [Test]
  public void test_YRangesOverlap_ThinBoxes()
  {
    // Test with boxes that have height=1 (single-line boxes)
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Box2D thinBox = new Box2D(2, 5, 8, 6, positiveYDown: true); // Height=1
    Assert.IsTrue(boxA.YRangesOverlap(thinBox));
    Assert.IsTrue(thinBox.YRangesOverlap(boxA));

    Box2D thinBoxNoOverlap = new Box2D(2, 15, 8, 16, positiveYDown: true); // Height=1, no overlap
    Assert.IsFalse(boxA.YRangesOverlap(thinBoxNoOverlap));
  }

  [Test]
  public void test_YRangesOverlap_CompletelyWrappingBox()
  {
    // Box that completely wraps around the other
    Box2D boxA = new Box2D(5, 5, 8, 8, positiveYDown: true);
    Box2D wrappingBox = new Box2D(0, 0, 15, 15, positiveYDown: true);
    Assert.IsTrue(boxA.YRangesOverlap(wrappingBox));
    Assert.IsTrue(wrappingBox.YRangesOverlap(boxA));
  }

  [Test]
  public void test_YRangesOverlap_AllNegativeCoordinates()
  {
    // Test with boxes entirely in negative coordinate space
    Box2D boxA = new Box2D(-10, -10, -5, -5, positiveYDown: true);
    Box2D overlapBox = new Box2D(-8, -8, -3, -3, positiveYDown: true);
    Box2D noOverlapBox = new Box2D(-20, -20, -15, -15, positiveYDown: true);

    Assert.IsTrue(boxA.YRangesOverlap(overlapBox));
    Assert.IsFalse(boxA.YRangesOverlap(noOverlapBox));
  }

  [Test]
  public void test_YRangesOverlap_Above()
  {
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: false);

    // Box that is overlapping and above
    Box2D overlapAbove = new Box2D(0, 5, 10, 15, positiveYDown: false);
    Box2D overlapBelow = new Box2D(0, -5, 10, 5, positiveYDown: false);

    Box2D noOverlapAbove = new Box2D(0, 15, 10, 25, positiveYDown: false);
    Box2D noOverlapBelow = new Box2D(0, -15, 10, -5, positiveYDown: false);

    Box2D sharedEdgeAbove = new Box2D(0, 10, 10, 15, positiveYDown: false);
    Box2D sharedEdgeBelow = new Box2D(0, -5, 10, 0, positiveYDown: false);
    Box2D inside = new Box2D(1, 1, 9, 9, positiveYDown: false);

    Assert.IsTrue(boxA.YRangesOverlap(overlapAbove));
    Assert.IsTrue(overlapAbove.YRangesOverlap(boxA));
    Assert.IsTrue(boxA.YRangesOverlap(overlapBelow));
    Assert.IsTrue(overlapBelow.YRangesOverlap(boxA));

    Assert.IsFalse(boxA.YRangesOverlap(noOverlapAbove));
    Assert.IsFalse(noOverlapAbove.YRangesOverlap(boxA));
    Assert.IsFalse(boxA.YRangesOverlap(noOverlapBelow));
    Assert.IsFalse(noOverlapBelow.YRangesOverlap(boxA));

    Assert.IsTrue(boxA.YRangesOverlap(sharedEdgeAbove));
    Assert.IsTrue(sharedEdgeAbove.YRangesOverlap(boxA));
    Assert.IsTrue(boxA.YRangesOverlap(sharedEdgeBelow));
    Assert.IsTrue(sharedEdgeBelow.YRangesOverlap(boxA));
    Assert.IsTrue(boxA.YRangesOverlap(inside));
    Assert.IsTrue(inside.YRangesOverlap(boxA));
  }

  [Test]
  public void test_XRangesOverlap()
  {
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: true);

    // Box that is overlapping and to the left
    Box2D overlapLeft = new Box2D(-5, 0, 5, 10, positiveYDown: true);
    Box2D overlapRight = new Box2D(5, 0, 15, 10, positiveYDown: true);

    Box2D noOverlapLeft = new Box2D(-15, 0, -5, 10, positiveYDown: true);
    Box2D noOverlapRight = new Box2D(15, 0, 25, 10, positiveYDown: true);

    Box2D sharedEdgeLeft = new Box2D(-5, 0, 0, 10, positiveYDown: true);
    Box2D sharedEdgeRight = new Box2D(10, 0, 15, 10, positiveYDown: true);

    Box2D inside = new Box2D(1, 1, 9, 9, positiveYDown: true);

    Assert.IsTrue(boxA.XRangesOverlap(overlapLeft));
    Assert.IsTrue(overlapLeft.XRangesOverlap(boxA));
    Assert.IsTrue(boxA.XRangesOverlap(overlapRight));
    Assert.IsTrue(overlapRight.XRangesOverlap(boxA));

    Assert.IsFalse(boxA.XRangesOverlap(noOverlapLeft));
    Assert.IsFalse(noOverlapLeft.XRangesOverlap(boxA));
    Assert.IsFalse(boxA.XRangesOverlap(noOverlapRight));
    Assert.IsFalse(noOverlapRight.XRangesOverlap(boxA));

    Assert.IsTrue(boxA.XRangesOverlap(sharedEdgeLeft));
    Assert.IsTrue(sharedEdgeLeft.XRangesOverlap(boxA));
    Assert.IsTrue(boxA.XRangesOverlap(sharedEdgeRight));
    Assert.IsTrue(sharedEdgeRight.XRangesOverlap(boxA));
    Assert.IsTrue(boxA.XRangesOverlap(inside));
    Assert.IsTrue(inside.XRangesOverlap(boxA));
  }

  [Test]
  public void test_XRangesOverlap_PositiveYUp()
  {
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: false);

    // Box that is overlapping and to the left
    Box2D overlapLeft = new Box2D(-5, 0, 5, 10, positiveYDown: false);
    Box2D overlapRight = new Box2D(5, 0, 15, 10, positiveYDown: false);

    Box2D noOverlapLeft = new Box2D(-15, 0, -5, 10, positiveYDown: false);
    Box2D noOverlapRight = new Box2D(15, 0, 25, 10, positiveYDown: false);

    Box2D sharedEdgeLeft = new Box2D(-5, 0, 0, 10, positiveYDown: false);
    Box2D sharedEdgeRight = new Box2D(10, 0, 15, 10, positiveYDown: false);
    Box2D inside = new Box2D(1, 1, 9, 9, positiveYDown: false);

    Assert.IsTrue(boxA.XRangesOverlap(overlapLeft));
    Assert.IsTrue(overlapLeft.XRangesOverlap(boxA));
    Assert.IsTrue(boxA.XRangesOverlap(overlapRight));
    Assert.IsTrue(overlapRight.XRangesOverlap(boxA));

    Assert.IsFalse(boxA.XRangesOverlap(noOverlapLeft));
    Assert.IsFalse(noOverlapLeft.XRangesOverlap(boxA));
    Assert.IsFalse(boxA.XRangesOverlap(noOverlapRight));
    Assert.IsFalse(noOverlapRight.XRangesOverlap(boxA));

    Assert.IsTrue(boxA.XRangesOverlap(sharedEdgeLeft));
    Assert.IsTrue(sharedEdgeLeft.XRangesOverlap(boxA));
    Assert.IsTrue(boxA.XRangesOverlap(sharedEdgeRight));
    Assert.IsTrue(sharedEdgeRight.XRangesOverlap(boxA));
    Assert.IsTrue(boxA.XRangesOverlap(inside));
    Assert.IsTrue(inside.XRangesOverlap(boxA));
  }

  [Test]
  public void test_XRangesOverlap_IdenticalBoxes()
  {
    // A box should overlap with itself (reflexive property)
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Assert.IsTrue(boxA.XRangesOverlap(boxA));

    Box2D boxB = new Box2D(-5, -5, 5, 5, positiveYDown: false);
    Assert.IsTrue(boxB.XRangesOverlap(boxB));
  }

  [Test]
  public void test_XRangesOverlap_ThinBoxes()
  {
    // Test with boxes that have width=1 (single-line boxes)
    Box2D boxA = new Box2D(0, 0, 10, 10, positiveYDown: true);
    Box2D thinBox = new Box2D(5, 2, 6, 8, positiveYDown: true); // Width=1
    Assert.IsTrue(boxA.XRangesOverlap(thinBox));
    Assert.IsTrue(thinBox.XRangesOverlap(boxA));

    Box2D thinBoxNoOverlap = new Box2D(15, 2, 16, 8, positiveYDown: true); // Width=1, no overlap
    Assert.IsFalse(boxA.XRangesOverlap(thinBoxNoOverlap));
  }

  [Test]
  public void test_XRangesOverlap_CompletelyWrappingBox()
  {
    // Box that completely wraps around the other
    Box2D boxA = new Box2D(5, 5, 8, 8, positiveYDown: true);
    Box2D wrappingBox = new Box2D(0, 0, 15, 15, positiveYDown: true);
    Assert.IsTrue(boxA.XRangesOverlap(wrappingBox));
    Assert.IsTrue(wrappingBox.XRangesOverlap(boxA));
  }

  [Test]
  public void test_XRangesOverlap_AllNegativeCoordinates()
  {
    // Test with boxes entirely in negative coordinate space
    Box2D boxA = new Box2D(-10, -10, -5, -5, positiveYDown: true);
    Box2D overlapBox = new Box2D(-8, -8, -3, -3, positiveYDown: true);
    Box2D noOverlapBox = new Box2D(-20, -20, -15, -15, positiveYDown: true);

    Assert.IsTrue(boxA.XRangesOverlap(overlapBox));
    Assert.IsFalse(boxA.XRangesOverlap(noOverlapBox));
  }

  #endregion
}
