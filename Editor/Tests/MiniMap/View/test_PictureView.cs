using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;

public class test_PictureView
{
  string filePath;

  [SetUp]
  public void SetUp()
  {
    this.filePath = Path.Combine(Application.dataPath, "tmp/tests/test_PictureView/");
    Debug.Log("filePath: " + filePath);
    Directory.CreateDirectory(filePath);
    // Clean out this directory
    foreach (string file in Directory.GetFiles(filePath))
    {
      File.Delete(file);
    }
    Debug.Log("Files deleted");
  }

  [TearDown]
  public void TearDown()
  {
    // Clean out this directory
    foreach (string file in Directory.GetFiles(filePath))
    {
      File.Delete(file);
    }
  }

  [Test]
  public void test_Render()
  {
    PictureView pictureView = new(width: 10, height: 10);
    pictureView.Render(Path.Combine(filePath, "test_Render.png"));
    Assert.IsTrue(File.Exists(Path.Combine(filePath, "test_Render.png")));
    // Read the file and check the contents
    byte[] fileContents = File.ReadAllBytes(Path.Combine(filePath, "test_Render.png"));
    Texture2D texture = new Texture2D(10, 10);
    texture.LoadImage(fileContents);
    Assert.IsTrue(texture.width == 10);
    Assert.IsTrue(texture.height == 10);
    Assert.IsTrue(texture.GetPixel(0, 0) == Color.white);
    Assert.IsTrue(texture.GetPixel(9, 9) == Color.white);
  }

  [Test]
  public void test_AddCityCenter()
  {
    PictureView pictureView = new(minX: -5, maxX: 5, minY: -5, maxY: 5);
    City city = new(new Vector2Int(0, 0));
    pictureView.AddCity(city);
    pictureView.Render(Path.Combine(filePath, "test_AddCityCenter.png"));
    Assert.IsTrue(File.Exists(Path.Combine(filePath, "test_AddCityCenter.png")));
    // Read the file and check the contents
    byte[] fileContents = File.ReadAllBytes(Path.Combine(filePath, "test_AddCityCenter.png"));
    Texture2D texture = new Texture2D(11, 11);
    texture.LoadImage(fileContents);
    Assert.IsTrue(texture.width == 11);
    Assert.IsTrue(texture.height == 11);
    Assert.IsTrue(texture.GetPixel(5, 5) == Color.red);
  }

  [Test]
  public void test_AddCityCorner()
  {
    PictureView pictureView = new(minX: -5, maxX: 5, minY: -5, maxY: 5);
    City city1 = new(new Vector2Int(5, 5)); // Top Right Unity Coords
    City city2 = new(new Vector2Int(5, -5)); // Bottom Right Unity Coords
    City city3 = new(new Vector2Int(-5, 5)); // Top Left Unity Coords
    City city4 = new(new Vector2Int(-5, -5)); // Bottom Left Unity Coords
    pictureView.AddCity(city1);
    pictureView.AddCity(city2);
    pictureView.AddCity(city3);
    pictureView.AddCity(city4);
    pictureView.Render(Path.Combine(filePath, "test_AddCityCorner.png"));

    Assert.IsTrue(File.Exists(Path.Combine(filePath, "test_AddCityCorner.png")));
    // Read the file and check the contents
    byte[] fileContents = File.ReadAllBytes(Path.Combine(filePath, "test_AddCityCorner.png"));
    Texture2D texture = new Texture2D(11, 11);
    texture.LoadImage(fileContents);
    Assert.IsTrue(texture.width == 11);
    Assert.IsTrue(texture.height == 11);

    Assert.IsTrue(texture.GetPixel(10, 10) == Color.red); // Top Right Grid Coords
    Assert.IsTrue(texture.GetPixel(10, 0) == Color.red); // Bottom Right Grid Coords
    Assert.IsTrue(texture.GetPixel(0, 10) == Color.red); // Top Left Grid Coords
    Assert.IsTrue(texture.GetPixel(0, 0) == Color.red); // Bottom Left Grid Coords
  }

  [Test]
  public void test_AddRoad()
  {
    PictureView pictureView = new(minX: -5, maxX: 5, minY: -5, maxY: 5);
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
    pictureView.AddRoad(road);
    pictureView.Render(Path.Combine(filePath, "test_AddRoad_yUp.png"), positiveYIsUp: true);
    Assert.IsTrue(File.Exists(Path.Combine(filePath, "test_AddRoad_yUp.png")));

    pictureView.Render(Path.Combine(filePath, "test_AddRoad_yDown.png"), positiveYIsUp: false);
    Assert.IsTrue(File.Exists(Path.Combine(filePath, "test_AddRoad_yDown.png")));
  }
}
