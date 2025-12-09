using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility methods for battle testing and logging
/// </summary>
public static class BattleTestUtils
{
  /// <summary>
  /// Logs detailed battle setup information to console
  /// </summary>
  public static void LogBattleSetup(BattleTeam playerTeam, BattleTeam computerTeam)
  {
    Debug.Log("BATTLE SETUP:");
    Debug.Log(
      $"Player Team: {playerTeam.AllMonsters.Count} monsters, {playerTeam.ActiveCount} active"
    );
    Debug.Log("  Active Monsters:");
    foreach (var mon in playerTeam.GetActiveMonsters())
    {
      Debug.Log($"    - {mon.Nickname} (HP: {mon.Health}, Speed: {mon.Speed})");
    }

    var playerReserves = playerTeam.GetReserveMonsters();
    if (playerReserves.Count > 0)
    {
      Debug.Log($"  Reserves: {string.Join(", ", playerReserves.ConvertAll(m => m.Nickname))}");
    }
    else
    {
      Debug.Log("  Reserves: None (all active)");
    }

    Debug.Log("");
    Debug.Log(
      $"Computer Team: {computerTeam.AllMonsters.Count} monsters, {computerTeam.ActiveCount} active"
    );
    Debug.Log("  Active Monsters:");
    foreach (var mon in computerTeam.GetActiveMonsters())
    {
      Debug.Log($"    - {mon.Nickname} (HP: {mon.Health}, Speed: {mon.Speed})");
    }

    var computerReserves = computerTeam.GetReserveMonsters();
    if (computerReserves.Count > 0)
    {
      Debug.Log($"  Reserves: {string.Join(", ", computerReserves.ConvertAll(m => m.Nickname))}");
    }
    else
    {
      Debug.Log("  Reserves: None (all active)");
    }

    Debug.Log("\n========================================");
    Debug.Log("BATTLE START!");
    Debug.Log("========================================\n");
  }

  /// <summary>
  /// Logs detailed battle setup with extended stats (Attack, Defense)
  /// </summary>
  public static void LogBattleSetupDetailed(BattleTeam playerTeam, BattleTeam computerTeam)
  {
    Debug.Log("BATTLE SETUP:");
    Debug.Log(
      $"Player Team: {playerTeam.AllMonsters.Count} monsters, {playerTeam.ActiveCount} active"
    );
    Debug.Log("  Active Monsters:");
    foreach (var mon in playerTeam.GetActiveMonsters())
    {
      Debug.Log(
        $"    - {mon.Nickname} (HP: {mon.Health}, Speed: {mon.Speed}, Atk: {mon.Attack}, Def: {mon.Defense})"
      );
    }

    var playerReserves = playerTeam.GetReserveMonsters();
    if (playerReserves.Count > 0)
    {
      Debug.Log($"  Reserves: {string.Join(", ", playerReserves.ConvertAll(m => m.Nickname))}");
    }
    else
    {
      Debug.Log("  Reserves: None (all active)");
    }

    Debug.Log("");
    Debug.Log(
      $"Computer Team: {computerTeam.AllMonsters.Count} monsters, {computerTeam.ActiveCount} active"
    );
    Debug.Log("  Active Monsters:");
    foreach (var mon in computerTeam.GetActiveMonsters())
    {
      Debug.Log(
        $"    - {mon.Nickname} (HP: {mon.Health}, Speed: {mon.Speed}, Atk: {mon.Attack}, Def: {mon.Defense})"
      );
    }

    var computerReserves = computerTeam.GetReserveMonsters();
    if (computerReserves.Count > 0)
    {
      Debug.Log($"  Reserves: {string.Join(", ", computerReserves.ConvertAll(m => m.Nickname))}");
    }
    else
    {
      Debug.Log("  Reserves: None (all active)");
    }

    Debug.Log("\n========================================");
    Debug.Log("BATTLE START!");
    Debug.Log("========================================\n");
  }
}
