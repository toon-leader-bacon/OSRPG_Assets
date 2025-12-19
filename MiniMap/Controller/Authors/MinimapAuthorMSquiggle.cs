using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum SquigglePattern
{
  M, // Start bottom, zigzag up/down while going right
  W, // Start top, zigzag down/up while going right
  S, // Start top, zigzag left/right while going down
  Z, // Start bottom, zigzag left/right while going up
}

public class MinimapAuthorSquiggle
{
  readonly NocabRNG rng;
  readonly AuthorUtilities authorUtil;

  private int XWiggleDelta;
  private int YWiggleDelta;

  public MinimapAuthorSquiggle(NocabRNG rng, int xWiggleDelta = 0, int yWiggleDelta = 0)
  {
    this.rng = rng;
    this.authorUtil = new AuthorUtilities(rng);
    this.XWiggleDelta = math.abs(xWiggleDelta);
    this.YWiggleDelta = math.abs(yWiggleDelta);
  }

  public MinimapAuthorSquiggle(int xWiggleDelta = 0, int yWiggleDelta = 0)
    : this(NocabRNG.defaultRNG, xWiggleDelta, yWiggleDelta) { }

  /**
   * Generate a squiggle map with the specified pattern.
   *
   * @param width - The width of the map area.
   * @param height - The height of the map area.
   * @param numCities - The number of cities to generate (must be at least 3).
   * @param pattern - The squiggle pattern to use (M, W, S, or Z).
   * @return A tuple containing the list of cities and the list of roads.
   */
  public (List<City>, List<Road>) GenerateSquiggle(
    int width,
    int height,
    int numCities,
    SquigglePattern pattern
  )
  {
    if (numCities < 3)
    {
      throw new Exception("At least 3 cities are required for a squiggle map.");
    }

    // Assume the center of the map is at (0,0)
    // Positive X is up, positive Y is right (Unity coordinate system)
    int centerX = 0;
    int centerY = 0;

    int leftEdgeX = centerX - width / 2;
    int rightEdgeX = centerX + width / 2;
    int topEdgeY = centerY + height / 2;
    int bottomEdgeY = centerY - height / 2;

    List<City> cities = getCityPositions_Squiggle(
      leftEdgeX,
      rightEdgeX,
      topEdgeY,
      bottomEdgeY,
      numCities,
      pattern
    );

    // Generate the roads
    List<Road> roads = new();
    City previousCity = cities[0];
    for (int cityIndex = 1; cityIndex < numCities; cityIndex++)
    {
      City currentCity = cities[cityIndex];
      Road road = authorUtil.connectCities_Automatic(previousCity, currentCity);
      roads.Add(road);
      previousCity = currentCity;
    }

    return (cities, roads);
  }

  /**
   * Generate an M-shaped squiggle map (backward compatibility).
   * Starts at bottom, zigzags up/down while going right.
   */
  public (List<City>, List<Road>) GenerateMSquiggle(int width, int height, int numCities)
  {
    return GenerateSquiggle(width, height, numCities, SquigglePattern.M);
  }

  /**
   * Generate city positions based on the specified squiggle pattern.
   *
   * @param leftEdgeX - The left edge X coordinate.
   * @param rightEdgeX - The right edge X coordinate.
   * @param topEdgeY - The top edge Y coordinate.
   * @param bottomEdgeY - The bottom edge Y coordinate.
   * @param numCities - The number of cities to generate.
   * @param pattern - The squiggle pattern to use.
   * @return A list of cities positioned according to the pattern.
   */
  protected List<City> getCityPositions_Squiggle(
    int leftEdgeX,
    int rightEdgeX,
    int topEdgeY,
    int bottomEdgeY,
    int numCities,
    SquigglePattern pattern
  )
  {
    List<City> cities = new();

    switch (pattern)
    {
      case SquigglePattern.M:
        // Start bottom, zigzag up/down while going right
        for (int cityIndex = 0; cityIndex < numCities; cityIndex++)
        {
          float cityXF = Mathf.Lerp(leftEdgeX, rightEdgeX, cityIndex / (float)(numCities - 1));
          int cityX = Mathf.RoundToInt(cityXF);
          int cityY = cityIndex % 2 == 0 ? bottomEdgeY : topEdgeY;
          cities.Add(new City(cityX, cityY));
        }
        break;

      case SquigglePattern.W:
        // Start top, zigzag down/up while going right
        for (int cityIndex = 0; cityIndex < numCities; cityIndex++)
        {
          float cityXF = Mathf.Lerp(leftEdgeX, rightEdgeX, cityIndex / (float)(numCities - 1));
          int cityX = Mathf.RoundToInt(cityXF);
          int cityY = cityIndex % 2 == 0 ? topEdgeY : bottomEdgeY;
          cities.Add(new City(cityX, cityY));
        }
        break;

      case SquigglePattern.S:
        // Start top, zigzag left/right while going down
        for (int cityIndex = 0; cityIndex < numCities; cityIndex++)
        {
          int cityX = cityIndex % 2 == 0 ? leftEdgeX : rightEdgeX;
          float cityYF = Mathf.Lerp(topEdgeY, bottomEdgeY, cityIndex / (float)(numCities - 1));
          int cityY = Mathf.RoundToInt(cityYF);
          cities.Add(new City(cityX, cityY));
        }
        break;

      case SquigglePattern.Z:
        // Start bottom, zigzag left/right while going up
        for (int cityIndex = 0; cityIndex < numCities; cityIndex++)
        {
          int cityX = cityIndex % 2 == 0 ? leftEdgeX : rightEdgeX;
          float cityYF = Mathf.Lerp(bottomEdgeY, topEdgeY, cityIndex / (float)(numCities - 1));
          int cityY = Mathf.RoundToInt(cityYF);
          cities.Add(new City(cityX, cityY));
        }
        break;

      default:
        throw new Exception($"Unknown squiggle pattern: {pattern}");
    }

    if (XWiggleDelta == 0 && YWiggleDelta == 0)
    {
      return cities;
    }
    return authorUtil.wiggleCityPositions(cities, XWiggleDelta, YWiggleDelta);
  }
}
