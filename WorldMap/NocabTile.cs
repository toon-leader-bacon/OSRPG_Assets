using UnityEngine;
using UnityEngine.Tilemaps;

public class NocabTile : MonoBehaviour
{
  public string tileType;
  public bool isWalkable;
  public int someValue = 0;

  public void OnPlayerEnter()
  {
    // Custom behavior when player enters the tile
    Debug.Log($"Player entered {tileType} tile.");
  }

  public void OnPlayerExit()
  {
    // Custom behavior when player exits the tile
    Debug.Log($"Player exited {tileType} tile.");
  }
}
