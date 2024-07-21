using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UIElements;

public class blab : MonoBehaviour
{

  public MinimapBuilder minimapBuilder;

  void Start()
  {
    // // List<Vector2Int> points = NocabPixelLine.getPointsAlongLine(new Vector2Int(0, 1), new Vector2Int(4, 1));
    // List<Vector2Int> points = PixelArtShapes.ConnectTheDots(
    //   new List<Vector2Int>{
    //     new Vector2Int(0,0),
    //     new Vector2Int(3,2),
    //     new Vector2Int(6,9),
    //     new Vector2Int(-7,2),
    //   }
    // );
    // foreach (Vector2Int point in points)
    // {
    //   Debug.Log($"({point.x}, {point.y})");
    // }

    minimapBuilder.drawRoad(minimapBuilder.blab());
  }


  void Update()
  {

  }
}
