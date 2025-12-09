using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Implements Active Time Battle (ATB) system similar to Final Fantasy IV-IX.
/// Each combatant has an individual gauge that fills based on their Speed stat.
/// When a gauge fills, the combatant becomes ready to act.
///
/// Flow:
/// - All combatants start with empty gauges (0%)
/// - Gauges fill continuously at rate determined by Speed stat
/// - When gauge reaches 100%, combatant is ready to act:
///   - AI monsters: Action is queued immediately, gauge resets
///   - Player monsters: Enter AwaitingInput state, wait for player choice
/// - In Wait Mode: Time freezes when player is choosing action
/// - In Active Mode: Time continues during player input
///
/// Integration:
/// - BattleManager must call Tick(deltaTime) each frame to advance gauges
/// - Player input provided via SubmitPlayerAction()
/// - Forced switches notify via OnMonsterSwitchedIn/Out()
/// </summary>
public class ATBConductor : ITimeDrivenConductor
{
  private BattleModel battleModel;
  private Queue<BattleAction> readyActionsQueue = new Queue<BattleAction>();
  private HashSet<IMonster> monstersAwaitingInput = new HashSet<IMonster>();

  /// <summary>
  /// Wait Mode: Pause time when player is making a choice (classic ATB)
  /// Active Mode: Time continues during player input (more challenging)
  /// </summary>
  public bool IsWaitMode { get; set; } = true;

  public void Initialize(BattleModel model)
  {
    battleModel = model;
    readyActionsQueue.Clear();
    monstersAwaitingInput.Clear();

    // Initialize gauges for all monsters (active + reserves)
    InitializeGaugesForTeam(battleModel.playerTeam);
    InitializeGaugesForTeam(battleModel.computerTeam);

    Debug.Log("ATBConductor initialized. Wait Mode: " + (IsWaitMode ? "ON" : "OFF"));
  }

  private void InitializeGaugesForTeam(BattleTeam team)
  {
    foreach (var monster in team.AllMonsters)
    {
      var gauge = battleModel.atbTimeline.GetGauge(monster);
      gauge.CurrentCharge = 0f;
      gauge.Phase = AtbPhase.Charging;
    }
  }

  /// <summary>
  /// Advances ATB gauges by deltaTime.
  /// Should be called every frame by BattleManager or Update loop.
  /// </summary>
  public void Tick(float deltaTime)
  {
    // Get all active monsters from both teams
    var allActiveMonsters = battleModel
      .playerTeam.GetActiveMonsters()
      .Concat(battleModel.computerTeam.GetActiveMonsters())
      .ToList();

    foreach (var monster in allActiveMonsters)
    {
      // Skip fainted monsters
      if (monster.Health <= 0)
        continue;

      var gauge = battleModel.atbTimeline.GetGauge(monster);

      // Only advance gauges in Charging phase
      if (gauge.Phase == AtbPhase.Charging)
      {
        // In Wait Mode, pause all charging if any player monster is awaiting input
        if (IsWaitMode && monstersAwaitingInput.Count > 0)
        {
          continue; // Don't advance this gauge
        }

        // Calculate charge rate from monster's current Speed stat
        // Formula: Speed stat = units per second (100 speed fills gauge in 1 second)
        float chargeRate = Mathf.Max(monster.Speed, 1);

        // Advance the gauge
        gauge.CurrentCharge += chargeRate * deltaTime;

        // Check if gauge filled (reached 100%)
        if (gauge.CurrentCharge >= 100f)
        {
          gauge.CurrentCharge = 100f;
          OnGaugeFilled(monster, gauge);
        }
      }
    }
  }

  /// <summary>
  /// Called when a monster's ATB gauge reaches 100%.
  /// Handles differently for player-controlled vs AI-controlled monsters.
  /// </summary>
  private void OnGaugeFilled(IMonster monster, AtbGaugeState gauge)
  {
    // Determine if this monster is player or AI controlled
    bool isPlayerTeam = battleModel.playerTeam.AllMonsters.Contains(monster);

    if (isPlayerTeam)
    {
      // Player-controlled: Wait for input from UI
      gauge.Phase = AtbPhase.AwaitingInput;
      monstersAwaitingInput.Add(monster);

      // TODO: Signal UI system to show battle menu for this monster
      Debug.Log($"[ATB] {monster.Nickname}'s turn is ready! Awaiting player input.");
    }
    else
    {
      // AI-controlled: Get action immediately and queue it
      gauge.Phase = AtbPhase.Ready;
      BattleAction action = GetAIAction(monster);
      readyActionsQueue.Enqueue(action);

      // Reset gauge for next turn
      gauge.CurrentCharge = 0f;
      gauge.Phase = AtbPhase.Charging;

      Debug.Log($"[ATB] {monster.Nickname}'s turn is ready! AI action queued.");
    }
  }

  /// <summary>
  /// Requests an action from AI for the specified monster.
  /// </summary>
  private BattleAction GetAIAction(IMonster aiMonster)
  {
    var aiTeam = battleModel.computerTeam;
    var targetTeam = battleModel.playerTeam;

    // Pick a random alive target from enemy team
    var aliveTargets = targetTeam.GetActiveMonsters().Where(t => t.Health > 0).ToList();

    if (aliveTargets.Count == 0)
    {
      // No valid targets - this should not happen if victory conditions are checked
      Debug.LogError($"AI monster {aiMonster.Nickname} has no valid targets!");
      // Return a dummy action that will be handled gracefully
      return BattleAction.CreateMoveAction(aiMonster, aiMonster.Moves[0], targetTeam.ActiveMonster);
    }

    var target = aliveTargets[Random.Range(0, aliveTargets.Count)];

    // Ask AI for move choice
    var move = aiTeam.BattleAI.GetMove(null, aiMonster, target);

    return BattleAction.CreateMoveAction(aiMonster, move, target);
  }

  public BattleAction GetNextAction()
  {
    // Drain ready actions queue first
    if (readyActionsQueue.Count > 0)
    {
      return readyActionsQueue.Dequeue();
    }

    // No actions ready - need more time to pass (Tick must be called)
    return null;
  }

  /// <summary>
  /// Called by UI/input system when player makes a choice.
  /// Queues the action and resets the monster's gauge.
  /// </summary>
  public void SubmitPlayerAction(IMonster playerMonster, BattleAction action)
  {
    var gauge = battleModel.atbTimeline.GetGauge(playerMonster);

    if (gauge.Phase != AtbPhase.AwaitingInput)
    {
      Debug.LogWarning(
        $"Received input for {playerMonster.Nickname} but not awaiting input (phase: {gauge.Phase})!"
      );
      return;
    }

    // Enqueue the action
    readyActionsQueue.Enqueue(action);

    // Reset gauge for next turn
    gauge.CurrentCharge = 0f;
    gauge.Phase = AtbPhase.Charging;
    monstersAwaitingInput.Remove(playerMonster);

    Debug.Log($"[ATB] {playerMonster.Nickname} action submitted! Gauge reset.");
  }

  public bool IsBattleOver()
  {
    // Battle ends when either team is fully defeated
    return battleModel.playerTeam.IsDefeated() || battleModel.computerTeam.IsDefeated();
  }

  public void ProcessEndOfPhase()
  {
    // In ATB, there's no traditional "phase" or "round"
    // This could be used for periodic effects (e.g., every N seconds of battle time)
    // For now, it's a no-op
  }

  public void OnMonsterSwitchedIn(IMonster monster, BattleTeam team)
  {
    var gauge = battleModel.atbTimeline.GetGauge(monster);
    gauge.CurrentCharge = 0f;
    gauge.Phase = AtbPhase.Charging;
    Debug.Log($"[ATB] {monster.Nickname} switched in. Gauge reset to 0%.");
  }

  public void OnMonsterSwitchedOut(IMonster monster, BattleTeam team)
  {
    var gauge = battleModel.atbTimeline.GetGauge(monster);

    // Option: Reset charge when switched out
    // gauge.CurrentCharge = 0f;

    // Always set to Charging phase and remove from input queue
    gauge.Phase = AtbPhase.Charging;
    monstersAwaitingInput.Remove(monster);

    Debug.Log($"[ATB] {monster.Nickname} switched out.");
  }

  /// <summary>
  /// Gets list of monsters currently awaiting player input.
  /// Useful for UI to display battle menus.
  /// </summary>
  public List<IMonster> GetMonstersAwaitingInput()
  {
    return new List<IMonster>(monstersAwaitingInput);
  }

  /// <summary>
  /// Gets gauge fill percentage for a monster (0-100).
  /// Useful for UI gauge display.
  /// </summary>
  public float GetGaugePercentage(IMonster monster)
  {
    var gauge = battleModel.atbTimeline.GetGauge(monster);
    return gauge.CurrentCharge;
  }
}
