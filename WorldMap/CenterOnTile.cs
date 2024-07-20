using UnityEngine;

public class CenterOnTile : MonoBehaviour
{
  public float gridSize = 1f; // Size of the tile grid

  void Start()
  {
    transform.position = TileUtility.SnapToGrid(transform.position);
  }

}
