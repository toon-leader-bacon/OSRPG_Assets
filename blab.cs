using System.Collections.Generic;
using UnityEngine;

public class blab : MonoBehaviour
{

  public MinimapBuilder minimapBuilder;

  void Start()
  {
    List<Vector2Int> cityPts = minimapBuilder.CircleOfCities();
    cityPts.Add(cityPts[0]);
    List<Vector2Int> connectedRoads = minimapBuilder.ConnectCitiesInOrder(cityPts);
    minimapBuilder.DrawRoad(connectedRoads);
    minimapBuilder.DrawCity(cityPts);

  }


  void Update()
  {

  }
}
