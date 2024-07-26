using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class blab : MonoBehaviour
{

  public MinimapBuilder minimapBuilder;

  void Start()
  {
    regenerateCityLoop();
    /**
    TODO: Write a one pass filter that does the following:
     - Assigns route numbers/ names to each route
     - Detects squares of path and replaces them with a larger "zone" route
     - Different city shapes

     Reach?
     - Detects T junctions in paths and makes an appropriate branch
       - T into other path
       - T into City
       R R T T R R R    R=Road T=Junction C=city
           T C
    */
  }


  void Update()
  {
    if (Input.GetKeyUp(KeyCode.Space))
    {
      regenerateCityLoop();
    }
  }

  void clearExistingLoopTiles()
  {

  }

  void regenerateCityLoop()
  {

    List<City> cityPts = minimapBuilder.CircleOfCities();
    cityPts.Add(cityPts[0]);
    List<Road> connectedRoads = minimapBuilder.ConnectCitiesInOrder(cityPts);
    minimapBuilder.DrawRoad(connectedRoads);
    minimapBuilder.DrawCity(cityPts);

    PositionTileDict ptd = new();
    foreach (Road r in connectedRoads)
    {
      ptd.AddRoad(r, minimapBuilder.pathTile);
    }
    TileLaydownLinter tll = new();
    PositionTileDict lintedTiles = tll.SmartDrawTiles(ptd);
    lintedTiles.drawSelf(minimapBuilder.tilemap);
  }
}
