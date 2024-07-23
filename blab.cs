using System.Collections.Generic;
using UnityEngine;

public class blab : MonoBehaviour
{

  public MinimapBuilder minimapBuilder;

  void Start()
  {
    List<City> cityPts = minimapBuilder.CircleOfCities();
    cityPts.Add(cityPts[0]);
    List<Road> connectedRoads = minimapBuilder.ConnectCitiesInOrder(cityPts);
    minimapBuilder.DrawRoad(connectedRoads);
    minimapBuilder.DrawCity(cityPts);

    // City a = new City(1, 1);
    // City b = new City(5, 9);
    // Road r = minimapBuilder.findValidConnector(a, b);
    // minimapBuilder.DrawRoad(r.tilesInOrder);
    // Debug.Log(b.directionOccupied(CardinalDirection.North));
    // Debug.Log(b.directionOccupied(CardinalDirection.South));
    // Debug.Log(b.directionOccupied(CardinalDirection.East));
    // Debug.Log(b.directionOccupied(CardinalDirection.West));
    // Debug.Log("==================================");

    // City c = new City(10, 4);
    // r = minimapBuilder.findValidConnector(b, c);
    // minimapBuilder.DrawRoad(r.tilesInOrder);
    // Debug.Log(b.directionOccupied(CardinalDirection.North));
    // Debug.Log(b.directionOccupied(CardinalDirection.South));
    // Debug.Log(b.directionOccupied(CardinalDirection.East));
    // Debug.Log(b.directionOccupied(CardinalDirection.West));


    // minimapBuilder.DrawCity(new List<City>() { a, b, c });
  }


  void Update()
  {

  }
}
