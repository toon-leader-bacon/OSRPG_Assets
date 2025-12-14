using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEngine;

public class test_StringView
{
  [SetUp]
  public void SetUp() { }

  [Test]
  public void test_unityCoordsToGridCoords()
  {
    StringView stringView = new(minX: -5, maxX: 5, minY: -5, maxY: 5);

    // Test the center of the grid
    Tuple<int, int> gridCoords = stringView.unityCoordsToGridCoords(0, 0);
    Assert.AreEqual(5, gridCoords.Item1);
    Assert.AreEqual(5, gridCoords.Item2);

    // Corners of grid
    Assert.AreEqual(new Tuple<int, int>(0, 10), stringView.unityCoordsToGridCoords(-5, 5)); // Top left
    Assert.AreEqual(new Tuple<int, int>(10, 10), stringView.unityCoordsToGridCoords(5, 5)); // Top right
    Assert.AreEqual(new Tuple<int, int>(0, 0), stringView.unityCoordsToGridCoords(-5, -5)); // Bottom left
    Assert.AreEqual(new Tuple<int, int>(10, 0), stringView.unityCoordsToGridCoords(5, -5)); // Bottom right
  }

  [Test]
  public void test_RenderEmptyGrid()
  {
    StringView stringView = new(minX: -5, maxX: 5, minY: -5, maxY: 5);
    string result = stringView.Render();

    // Expected grid is a 11x11 grid of dots (with a trailing newline)
    string expected =
      @"...........
...........
...........
...........
...........
...........
...........
...........
...........
...........
...........
";
    Assert.AreEqual(expected, result);
  }

  [Test]
  public void test_AddRoad()
  {
    StringView stringView = new(minX: -5, maxX: 5, minY: -5, maxY: 5);

    // Roads are in Unity Coordinate System (0,0) is the center of the grid
    Road road = new(
      new List<Vector2Int>
      {
        new(-4, 0),
        new(-3, 0),
        new(-2, 1),
        new(-1, 1),
        new(0, 2),
        new(1, 2),
        new(2, 3),
        new(3, 3),
        new(4, 4),
      }
    );
    stringView.AddRoad(road);

    string result = stringView.Render(positiveYIsUp: true);

    // Expected grid is a 11x11 grid of dots (with a trailing newline)
    string expected =
      @"...........
.........X.
.......XX..
.....XX....
...XX......
.XX........
...........
...........
...........
...........
...........
";
    Assert.AreEqual(expected, result);

    // Try positiveYIsUp = false
    expected =
      @"...........
...........
...........
...........
...........
.XX........
...XX......
.....XX....
.......XX..
.........X.
...........
";
    result = stringView.Render(positiveYIsUp: false);
    Assert.AreEqual(expected, result);
  }

  [Test]
  public void test_AddCityCentered()
  {
    StringView stringView = new(minX: -2, maxX: 2, minY: -2, maxY: 2);
    City city = new(new Vector2Int(0, 0));
    stringView.AddCity(city);
    string result = stringView.Render(positiveYIsUp: true);
    string expected =
      @".....
.....
..C..
.....
.....
";
    Assert.AreEqual(expected, result);

    // Try positiveYIsUp = false
    expected =
      @".....
.....
..C..
.....
.....
";
    result = stringView.Render(positiveYIsUp: false);
    Assert.AreEqual(expected, result);
  }

  [Test]
  public void test_AddCityCorner()
  {
    StringView stringView = new(minX: -2, maxX: 2, minY: -2, maxY: 2);
    City city = new(new Vector2Int(2, 2));
    stringView.AddCity(city);
    string result = stringView.Render(positiveYIsUp: true);
    string expected =
      @"....C
.....
.....
.....
.....
";
    Assert.AreEqual(expected, result);

    // Try positiveYIsUp = false
    expected =
      @".....
.....
.....
.....
....C
";
    result = stringView.Render(positiveYIsUp: false);
    Assert.AreEqual(expected, result);
  }
}
