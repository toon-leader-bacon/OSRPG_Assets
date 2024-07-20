using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileAssociation", menuName = "ScriptableObjects/TileAssociation", order = 2)]
public class TileAssociation : ScriptableObject
{
  public string associationName;
  public TileBase[] tiles;
  public NocabTile nocabTile;
}
