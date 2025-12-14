using UnityEngine;

/// <summary>
/// Coordinator controller that manages the map data lifecycle and provides
/// access to both authoring and runtime controllers. This can be attached
/// to a Unity GameObject if needed.
/// </summary>
public class MinimapController
{
  private MapData mapData;
  private MinimapAuthoringController authoringController;
  private MinimapRuntimeController runtimeController;

  public MinimapAuthoringController Authoring => authoringController;
  public MinimapRuntimeController Runtime => runtimeController;
  public MapData Data => mapData;

  public MinimapController(int width, int height)
  {
    mapData = new MapData(width, height);
    authoringController = new MinimapAuthoringController(mapData);
    runtimeController = new MinimapRuntimeController(mapData);
  }

  /// <summary>
  /// Creates a new map with the specified dimensions, clearing any existing data.
  /// </summary>
  public void CreateNewMap(int width, int height)
  {
    mapData = new MapData(width, height);
    authoringController = new MinimapAuthoringController(mapData);
    runtimeController = new MinimapRuntimeController(mapData);
  }
}

