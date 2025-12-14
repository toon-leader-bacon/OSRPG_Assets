using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for MinimapAuthorLoop class.
/// These are Edit Mode tests that run in the Unity Editor without entering Play Mode.
/// </summary>
public class test_MinimapAuthorLoop
{
  private MinimapAuthorLoop authorLoop;
  private NocabRNG testRNG;
  private string filePath;

  [SetUp]
  public void SetUp()
  {
    // Use a seeded RNG for deterministic test results
    testRNG = new NocabRNG(12345);
    authorLoop = new MinimapAuthorLoop(testRNG);

    this.filePath = Path.Combine(Application.dataPath, "tmp/tests/test_MinimapAuthorLoop/");
    Debug.Log("filePath: " + filePath);
    Directory.CreateDirectory(filePath);
    // Clean out this directory
    foreach (string file in Directory.GetFiles(filePath))
    {
      File.Delete(file);
    }
  }

  [TearDown]
  public void TearDown()
  {
    // Clean out this directory
    foreach (string file in Directory.GetFiles(filePath))
    {
      // File.Delete(file);
    }
  }

  [Test]
  public void blab()
  {
    // Arrange
    int loopWidth = 10;
    int loopHeight = 10;

    // Act
    authorLoop = new MinimapAuthorLoop(new NocabRNG(1111));
    var (cities, roads) = authorLoop.GenerateLoop_CitiesAlongEdges(loopWidth, loopHeight);

    int loopLeft = -5;
    int loopRight = 5;
    int loopTop = 5;
    int loopBottom = -5;
    List<Road> boxRoads = new()
    {
      new(NocabPixelLine.getPointsAlongLine(new(loopLeft, loopTop), new(loopRight, loopTop))), // TL to TR
      new(NocabPixelLine.getPointsAlongLine(new(loopRight, loopTop), new(loopRight, loopBottom))), // TR to BR
      new(NocabPixelLine.getPointsAlongLine(new(loopRight, loopBottom), new(loopLeft, loopBottom))), // BR to BL
      new(NocabPixelLine.getPointsAlongLine(new(loopLeft, loopBottom), new(loopLeft, loopTop))), // BL to TL
    };

    PictureView pictureView = new(minX: -8, maxX: 8, minY: -8, maxY: 8);

    foreach (var road in boxRoads)
    {
      pictureView.AddRoad(road, color: Color.black);
    }

    List<Color> colors = new()
    {
      Color.green,
      Color.green * 0.75f,
      Color.green * 0.5f,
      Color.green * 0.25f,
    };
    int colorIndex = 0;
    foreach (var road in roads)
    {
      pictureView.AddRoad(road, color: colors[colorIndex]);
      colorIndex++;
      colorIndex %= colors.Count;
    }
    foreach (var city in cities)
    {
      pictureView.AddCity(city);
    }
    pictureView.Render(filename: Path.Combine(filePath, "test_blab.png"), positiveYIsUp: true);
  }

  [Test]
  public void GenerateLoop_WithValidDimensions_CreatesFourCities()
  {
    // Arrange
    int loopWidth = 10;
    int loopHeight = 8;

    // Act
    var (cities, roads) = authorLoop.GenerateLoop_CitiesAtCorners(loopWidth, loopHeight);

    // Assert
    Assert.IsNotNull(cities, "Cities list should not be null");
    Assert.AreEqual(4, cities.Count, "Should create exactly 4 cities (one at each corner)");

    // Verify all cities have valid positions
    foreach (var city in cities)
    {
      Assert.IsNotNull(city, "City should not be null");
      Assert.IsNotNull(city.position, "City position should not be null");
    }
  }

  [Test]
  public void GenerateLoop_WithValidDimensions_CreatesFourRoads()
  {
    // Arrange
    int loopWidth = 10;
    int loopHeight = 8;

    // Act
    var (cities, roads) = authorLoop.GenerateLoop_CitiesAtCorners(loopWidth, loopHeight);

    // Assert
    Assert.IsNotNull(roads, "Roads list should not be null");
    Assert.AreEqual(4, roads.Count, "Should create exactly 4 roads (connecting the 4 cities)");

    // Verify all roads have valid tile lists
    foreach (var road in roads)
    {
      Assert.IsNotNull(road, "Road should not be null");
      Assert.IsNotNull(road.tilesInOrder, "Road tilesInOrder should not be null");
      Assert.Greater(road.tilesInOrder.Count, 0, "Road should have at least one tile");
    }
  }

  [Test]
  public void GenerateLoop_WithValidDimensions_CitiesAreAtCorners()
  {
    // Arrange
    int loopWidth = 10;
    int loopHeight = 8;
    int centerX = 0;
    int centerY = 0;
    int leftEdgeX = centerX - (loopWidth / 2);
    int rightEdgeX = centerX + (loopWidth / 2);
    int topEdgeY = centerY + (loopHeight / 2);
    int bottomEdgeY = centerY - (loopHeight / 2);

    Vector2Int expectedTL = new(leftEdgeX, topEdgeY);
    Vector2Int expectedTR = new(rightEdgeX, topEdgeY);
    Vector2Int expectedBL = new(leftEdgeX, bottomEdgeY);
    Vector2Int expectedBR = new(rightEdgeX, bottomEdgeY);

    // Act
    var (cities, roads) = authorLoop.GenerateLoop_CitiesAtCorners(loopWidth, loopHeight);

    // Assert
    HashSet<Vector2Int> cityPositions = new();
    foreach (var city in cities)
    {
      cityPositions.Add(city.position);
    }

    Assert.IsTrue(cityPositions.Contains(expectedTL), "Should have a city at top-left corner");
    Assert.IsTrue(cityPositions.Contains(expectedTR), "Should have a city at top-right corner");
    Assert.IsTrue(cityPositions.Contains(expectedBL), "Should have a city at bottom-left corner");
    Assert.IsTrue(cityPositions.Contains(expectedBR), "Should have a city at bottom-right corner");
  }

  [Test]
  public void GenerateLoop_WithValidDimensions_AllCitiesHaveRoads()
  {
    // Arrange
    int loopWidth = 10;
    int loopHeight = 8;

    // Act
    var (cities, roads) = authorLoop.GenerateLoop_CitiesAtCorners(loopWidth, loopHeight);

    // Assert
    foreach (var city in cities)
    {
      var occupiedDirections = city.OccupiedDirections();
      Assert.Greater(
        occupiedDirections.Count,
        0,
        $"City at {city.position} should have at least one road connected"
      );
      Assert.LessOrEqual(
        occupiedDirections.Count,
        2,
        $"City at {city.position} should have at most 2 roads (one in, one out)"
      );
    }
  }

  [Test]
  public void GenerateLoop_WithValidDimensions_RoadsConnectCities()
  {
    // Arrange
    int loopWidth = 10;
    int loopHeight = 8;

    // Act
    var (cities, roads) = authorLoop.GenerateLoop_CitiesAtCorners(loopWidth, loopHeight);

    // Assert - Each road should connect two cities
    foreach (var road in roads)
    {
      Assert.IsNotNull(road.tilesInOrder, "Road tiles should not be null");
      Assert.GreaterOrEqual(
        road.tilesInOrder.Count,
        2,
        "Road should have at least 2 tiles (start and end)"
      );

      // Verify the road has a valid path (non-empty tile list)
      bool hasValidPath = false;
      foreach (var city in cities)
      {
        // Check if road connects to this city by checking if any road tile is adjacent to city
        foreach (var tile in road.tilesInOrder)
        {
          Vector2Int cityPos = city.position;
          int distanceX = Mathf.Abs(tile.x - cityPos.x);
          int distanceY = Mathf.Abs(tile.y - cityPos.y);

          // Road should be adjacent to city (within 1 tile)
          if (distanceX <= 1 && distanceY <= 1 && (distanceX + distanceY) <= 1)
          {
            hasValidPath = true;
            break;
          }
        }
        if (hasValidPath)
          break;
      }
      Assert.IsTrue(hasValidPath, "Road should connect to at least one city");
    }
  }

  [Test]
  public void GenerateLoop_WithDifferentDimensions_ScalesCorrectly()
  {
    // Arrange
    int loopWidth1 = 10;
    int loopHeight1 = 8;
    int loopWidth2 = 20;
    int loopHeight2 = 16;

    // Act
    var (cities1, roads1) = authorLoop.GenerateLoop_CitiesAtCorners(loopWidth1, loopHeight1);
    var (cities2, roads2) = authorLoop.GenerateLoop_CitiesAtCorners(loopWidth2, loopHeight2);

    // Assert
    Assert.AreEqual(4, cities1.Count, "First loop should have 4 cities");
    Assert.AreEqual(4, cities2.Count, "Second loop should have 4 cities");
    Assert.AreEqual(4, roads1.Count, "First loop should have 4 roads");
    Assert.AreEqual(4, roads2.Count, "Second loop should have 4 roads");

    // Verify cities are positioned correctly for different sizes
    int centerX = 0,
      centerY = 0;

    int leftEdgeX1 = centerX - (loopWidth1 / 2);
    int rightEdgeX1 = centerX + (loopWidth1 / 2);
    int leftEdgeX2 = centerX - (loopWidth2 / 2);
    int rightEdgeX2 = centerX + (loopWidth2 / 2);

    HashSet<Vector2Int> positions1 = new();
    foreach (var city in cities1)
      positions1.Add(city.position);

    HashSet<Vector2Int> positions2 = new();
    foreach (var city in cities2)
      positions2.Add(city.position);

    Assert.IsTrue(positions1.Contains(new Vector2Int(leftEdgeX1, centerY + (loopHeight1 / 2))));
    Assert.IsTrue(positions2.Contains(new Vector2Int(leftEdgeX2, centerY + (loopHeight2 / 2))));
    Assert.IsTrue(positions1.Contains(new Vector2Int(rightEdgeX1, centerY + (loopHeight1 / 2))));
    Assert.IsTrue(positions2.Contains(new Vector2Int(rightEdgeX2, centerY + (loopHeight2 / 2))));
  }

  [Test]
  public void GenerateLoop_WithDefaultConstructor_UsesDefaultRNG()
  {
    // Arrange & Act
    var authorLoopDefault = new MinimapAuthorLoop();
    var (cities, roads) = authorLoopDefault.GenerateLoop_CitiesAtCorners(10, 8);

    // Assert
    Assert.IsNotNull(cities, "Should work with default RNG");
    Assert.AreEqual(4, cities.Count, "Should still create 4 cities");
    Assert.AreEqual(4, roads.Count, "Should still create 4 roads");
  }

  [Test]
  public void GenerateLoop_WithSameSeed_ProducesDeterministicResults()
  {
    // Arrange
    var rng1 = new NocabRNG(999);
    var rng2 = new NocabRNG(999);
    var authorLoop1 = new MinimapAuthorLoop(rng1);
    var authorLoop2 = new MinimapAuthorLoop(rng2);

    // Act
    var (cities1, roads1) = authorLoop1.GenerateLoop_CitiesAtCorners(10, 8);
    var (cities2, roads2) = authorLoop2.GenerateLoop_CitiesAtCorners(10, 8);

    // Assert - With same seed, results should be identical
    Assert.AreEqual(cities1.Count, cities2.Count, "Same seed should produce same number of cities");
    Assert.AreEqual(roads1.Count, roads2.Count, "Same seed should produce same number of roads");

    // Verify city positions match
    for (int i = 0; i < cities1.Count; i++)
    {
      Assert.AreEqual(
        cities1[i].position,
        cities2[i].position,
        $"City {i} positions should match with same seed"
      );
    }
  }
}
