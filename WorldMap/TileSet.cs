using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSet : ScriptableObject
{
  public string tileSetName;
  public TileAssociation[] tileAssociations;
}
