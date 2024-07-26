

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;

public class SmartPathTileLookup
{

  Dictionary<int, TileBase> tileBases = new();

  public SmartPathTileLookup(string assetDirPath)
  {
    loadTilesFromDirectory(assetDirPath);
  }

  void loadTilesFromDirectory(string assetDirPath)
  {
    string[] filePaths = Directory.GetFiles($"Assets/{assetDirPath}/", "*.asset");
    foreach (string filePath in filePaths)
    {
      string fileName = Path.GetFileNameWithoutExtension(filePath);
      TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(filePath);
      if (tile == null)
      {
        Debug.LogWarning($"Could not load tile asset \'{filePath}\'");
        continue;
      }

      processTile(fileName, tile);
    }
  }

  void processTile(string fileName, TileBase tb)
  {
    string[] ids = fileName.Split('_');
    if (ids.Length <= 1)
    {
      Debug.LogWarning($"Could not load tile asset \'{fileName}\' is mislabeled (expected \'_\' character)");
      return;
    }
    string connections = ids[1];
    int hash = getHash(connections);
    tileBases[hash] = tb;
  }

  public TileBase getTileBasedOnNeighbors(HashSet<CardinalDirection> neighbors)
  {
    return tileBases[getHash(neighbors)];
  }

  int getHash(string connections)
  {
    HashSet<char> chars = new HashSet<char>(connections.Length);
    foreach (char c in connections) { chars.Add(c); }

    bool up = chars.Contains('u');
    bool down = chars.Contains('d');
    bool left = chars.Contains('l');
    bool right = chars.Contains('r');

    // Order MUST match other getHash functions 
    int hash = NocabHashUtility.hashBools(up, down, left, right);
    return hash;
  }

  int getHash(HashSet<CardinalDirection> neighbors)
  {
    bool up = neighbors.Contains(CardinalDirection.North);
    bool down = neighbors.Contains(CardinalDirection.South);
    bool left = neighbors.Contains(CardinalDirection.West);
    bool right = neighbors.Contains(CardinalDirection.East);

    // Order MUST match other getHash functions 
    int hash = NocabHashUtility.hashBools(up, down, left, right);
    return hash;
  }

}