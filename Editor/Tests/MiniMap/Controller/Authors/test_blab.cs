using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class test_blab
{
  private string filePath;

  [SetUp]
  public void SetUp()
  {
    this.filePath = Path.Combine(Application.dataPath, "tmp/tests/test_MinimapAuthorLoop/");
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
  public void Test_blab()
  {
    int squareWidth = 20;
    int squareHeight = 8;
    NocabRNG rng = new NocabRNG(System.DateTime.UtcNow.Ticks);
    // NocabRNG rng = new NocabRNG(1111);
    int xWiggleDelta = 0;
    int yWiggleDelta = 2;
    // MinimapAuthorLoop_Polygon authorLoop = new MinimapAuthorLoop_Polygon(rng);
    MinimapAuthorSquiggle authorLoop = new MinimapAuthorSquiggle(
      rng: rng,
      xWiggleDelta: xWiggleDelta,
      yWiggleDelta: yWiggleDelta
    );
    (List<City>, List<Road>) result = authorLoop.GenerateSquiggle(
      width: squareWidth,
      height: squareHeight,
      numCities: 4,
      pattern: SquigglePattern.M
    );

    int loopLeft = -squareWidth / 2;
    int loopRight = squareWidth / 2;
    int loopTop = squareHeight / 2;
    int loopBottom = -squareHeight / 2;

    PictureView pictureView = new(
      minX: loopLeft - 5,
      maxX: loopRight + 5,
      minY: loopBottom - 5,
      maxY: loopTop + 5
    );

    #region Debug min and max extent roads
    List<Road> boxRoads = new()
    {
      // Main box roads
      new(NocabPixelLine.getPointsAlongLine(new(loopLeft, loopTop), new(loopRight, loopTop))), // TL to TR
      new(NocabPixelLine.getPointsAlongLine(new(loopRight, loopTop), new(loopRight, loopBottom))), // TR to BR
      new(NocabPixelLine.getPointsAlongLine(new(loopRight, loopBottom), new(loopLeft, loopBottom))), // BR to BL
      new(NocabPixelLine.getPointsAlongLine(new(loopLeft, loopBottom), new(loopLeft, loopTop))), // BL to TL
    };

    List<Road> maxExtentRoads = new()
    {
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopLeft - xWiggleDelta, loopTop + yWiggleDelta),
          new(loopRight + xWiggleDelta, loopTop + yWiggleDelta)
        )
      ), // TL to TR
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopRight + xWiggleDelta, loopTop + yWiggleDelta),
          new(loopRight + xWiggleDelta, loopBottom - yWiggleDelta)
        )
      ), // TR to BR
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopRight + xWiggleDelta, loopBottom - yWiggleDelta),
          new(loopLeft - xWiggleDelta, loopBottom - yWiggleDelta)
        )
      ), // BR to BL
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopLeft - xWiggleDelta, loopBottom - yWiggleDelta),
          new(loopLeft - xWiggleDelta, loopTop + yWiggleDelta)
        )
      ), // BL to TL
    };

    List<Road> minExtentRoads = new()
    {
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopLeft + xWiggleDelta, loopTop - yWiggleDelta),
          new(loopRight - xWiggleDelta, loopTop - yWiggleDelta)
        )
      ), // TL to TR
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopRight - xWiggleDelta, loopTop - yWiggleDelta),
          new(loopRight - xWiggleDelta, loopBottom + yWiggleDelta)
        )
      ), // TR to BR
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopRight - xWiggleDelta, loopBottom + yWiggleDelta),
          new(loopLeft + xWiggleDelta, loopBottom + yWiggleDelta)
        )
      ), // BR to BL
      new(
        NocabPixelLine.getPointsAlongLine(
          new(loopLeft + xWiggleDelta, loopBottom + yWiggleDelta),
          new(loopLeft + xWiggleDelta, loopTop - yWiggleDelta)
        )
      ), // BL to TL
    };

    foreach (var road in maxExtentRoads)
    {
      pictureView.AddRoad(road, color: new Color(1f, 1f, 1f, 0.85f));
    }
    foreach (var road in minExtentRoads)
    {
      pictureView.AddRoad(road, color: new Color(1f, 1f, 1f, 0.85f));
    }

    // Draw this one last so it's on top
    foreach (var road in boxRoads)
    {
      pictureView.AddRoad(road, color: new Color(1f, 1f, 1f, 0.6f));
    }
    #endregion Debug min and max extent roads

    float percentageColor = 1f;
    int percentagePerRoad = 100 / result.Item2.Count;
    foreach (var road in result.Item2)
    {
      pictureView.AddRoad(road, color: Color.Lerp(Color.green, Color.blue, percentageColor));
      percentageColor -= (float)percentagePerRoad / 100f;
    }

    foreach (var city in result.Item1)
    {
      pictureView.AddCity(city);
    }
    pictureView.Render(filename: Path.Combine(filePath, "test_blab.png"), positiveYIsUp: true);

    Assert.IsTrue(true);
  }
}
