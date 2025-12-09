using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Verbose ATB battle test that logs gauge states periodically.
/// Useful for understanding ATB timing and debugging gauge fill rates.
/// </summary>
public class Test_ATBVerbose : MonoBehaviour
{
  void Start()
  {
    Debug.Log("=== Verbose ATB Battle Test Start ===");
    RunVerboseATBBattle();
    Debug.Log("=== Verbose ATB Battle Test Complete ===");
  }

  private void RunVerboseATBBattle()
  {
    // Create teams with distinct speeds for clear visualization
    var playerMonster = MonsterFactory.CreateBirdFast("Flash"); // Speed: 120
    var playerTeam = new BattleTeam(
      playerMonster,
      new BattleAI_Random(),
      BattleModel.PLAYER_TEAM_ID
    );

    var computerMonster = MonsterFactory.CreateBirdTank("Boulder"); // Speed: 60
    var computerTeam = new BattleTeam(
      computerMonster,
      new BattleAI_Random(),
      BattleModel.COMPUTER_TEAM_ID
    );

    var battleModel = new BattleModel(playerTeam, computerTeam);
    var atbConductor = new ATBConductor { IsWaitMode = true };

    Debug.Log("Battle Setup:");
    Debug.Log($"  {playerMonster.Nickname}: Speed {playerMonster.Speed} (fills in ~0.83s)");
    Debug.Log($"  {computerMonster.Nickname}: Speed {computerMonster.Speed} (fills in ~1.67s)");
    Debug.Log(
      $"  Expected: {playerMonster.Nickname} should act twice before {computerMonster.Nickname} acts once"
    );
    Debug.Log("");

    var battleManager = new VerboseATBBattleManager(battleModel, atbConductor);
    battleManager.StartBattle();
  }
}

/// <summary>
/// Extended BattleManager that logs gauge states during ATB battles.
/// </summary>
public class VerboseATBBattleManager : BattleManager
{
  private ATBConductor atbConductor;
  private float elapsedTime = 0f;
  private float lastLogTime = 0f;
  private const float LOG_INTERVAL = 0.5f; // Log every 0.5 seconds
  private int actionCount = 0;

  public VerboseATBBattleManager(BattleModel model, ATBConductor conductor)
    : base(model, conductor)
  {
    this.atbConductor = conductor;
  }

  /// <summary>
  /// Override StartBattle to add gauge logging
  /// </summary>
  public new void StartBattle()
  {
    Debug.Log("[TIME] Battle Start - Gauges initialized to 0%");
    Debug.Log("");

    const float TICK_INTERVAL = 0.016f; // ~60 FPS
    elapsedTime = 0f;
    lastLogTime = 0f;
    actionCount = 0;

    // Custom ATB loop with logging
    while (!atbConductor.IsBattleOver())
    {
      // Advance time
      atbConductor.Tick(TICK_INTERVAL);
      elapsedTime += TICK_INTERVAL;

      // Log gauge states periodically
      if (elapsedTime - lastLogTime >= LOG_INTERVAL)
      {
        LogGaugeStates();
        lastLogTime = elapsedTime;
      }

      // Try to execute ready actions
      BattleAction action = atbConductor.GetNextAction();

      if (action != null)
      {
        actionCount++;
        Debug.Log($"[TIME {elapsedTime:F2}s] ACTION #{actionCount}:");

        // Use base class execution logic
        base.StartBattle(); // This won't re-enter the loop, just runs the single action

        // Log updated gauge states after action
        LogGaugeStatesCompact();
        Debug.Log("");
      }

      // Simulate frame delay
      System.Threading.Thread.Sleep((int)(TICK_INTERVAL * 1000));
    }

    Debug.Log($"[TIME {elapsedTime:F2}s] Battle Over - Total Actions: {actionCount}");
    Debug.Log("");
  }

  private void LogGaugeStates()
  {
    Debug.Log($"[TIME {elapsedTime:F2}s] Gauge States:");

    // Get all active monsters
    var allActives = GetActiveMonsters();

    foreach (var monster in allActives)
    {
      var gauge = GetBattleModel().atbTimeline.GetGauge(monster);
      string bar = CreateGaugeBar(gauge.CurrentCharge);
      Debug.Log(
        $"  {monster.Nickname, -12} [{bar}] {gauge.CurrentCharge, 5:F1}% | Phase: {gauge.Phase, -13} | HP: {monster.Health}/{monster.MaxHealth}"
      );
    }

    Debug.Log("");
  }

  private void LogGaugeStatesCompact()
  {
    var allActives = GetActiveMonsters();
    foreach (var monster in allActives)
    {
      var gauge = GetBattleModel().atbTimeline.GetGauge(monster);
      Debug.Log(
        $"  {monster.Nickname}: Gauge reset to {gauge.CurrentCharge:F1}% (HP: {monster.Health}/{monster.MaxHealth})"
      );
    }
  }

  private string CreateGaugeBar(float percentage)
  {
    int filled = (int)(percentage / 10); // 0-10 segments
    int empty = 10 - filled;
    return new string('█', filled) + new string('░', empty);
  }

  private BattleModel GetBattleModel()
  {
    // Access protected battleModel via reflection (only for debugging)
    var field = typeof(BattleManager).GetField(
      "battleModel",
      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
    );
    return (BattleModel)field.GetValue(this);
  }

  private new IMonster[] GetActiveMonsters()
  {
    var model = GetBattleModel();
    var actives = new List<IMonster>();
    actives.AddRange(model.playerTeam.GetActiveMonsters());
    actives.AddRange(model.computerTeam.GetActiveMonsters());
    return actives.ToArray();
  }
}
