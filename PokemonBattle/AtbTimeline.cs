using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pure ATB state data for a single combatant.
/// Stores only charge level and phase - no behavior, no references to monsters.
/// Charge rate is calculated by the ATBConductor based on the monster's Speed stat.
/// This is model data - it can be serialized, copied, or transferred between battles.
/// </summary>
public class AtbGaugeState
{
  /// <summary>
  /// Current charge level (0.0 to 100.0)
  /// </summary>
  public float CurrentCharge { get; set; }

  /// <summary>
  /// Current phase in the ATB cycle
  /// </summary>
  public AtbPhase Phase { get; set; }

  public AtbGaugeState()
  {
    CurrentCharge = 0f;
    Phase = AtbPhase.Charging;
  }

  /// <summary>
  /// Resets gauge to start charging from 0
  /// </summary>
  public void Reset()
  {
    CurrentCharge = 0f;
    Phase = AtbPhase.Charging;
  }
}

/// <summary>
/// Phases in the ATB gauge cycle
/// </summary>
public enum AtbPhase
{
  /// <summary>
  /// Gauge is actively charging towards 100%
  /// </summary>
  Charging,

  /// <summary>
  /// Gauge is full and action is queued (AI monsters)
  /// </summary>
  Ready,

  /// <summary>
  /// Gauge is full, waiting for player input
  /// </summary>
  AwaitingInput,

  /// <summary>
  /// Action is being cast/executed (future: add cast time support)
  /// </summary>
  Casting,
}

/// <summary>
/// Manages ATB gauge states for all combatants in battle.
/// Attached to BattleModel to persist gauge data.
/// </summary>
public class AtbTimeline
{
  private Dictionary<IMonster, AtbGaugeState> monsterGauges =
    new Dictionary<IMonster, AtbGaugeState>();

  /// <summary>
  /// Gets or creates the gauge state for a monster
  /// </summary>
  public AtbGaugeState GetGauge(IMonster monster)
  {
    if (!monsterGauges.ContainsKey(monster))
    {
      monsterGauges[monster] = new AtbGaugeState();
    }
    return monsterGauges[monster];
  }

  /// <summary>
  /// Checks if a gauge exists for a monster
  /// </summary>
  public bool HasGauge(IMonster monster)
  {
    return monsterGauges.ContainsKey(monster);
  }

  /// <summary>
  /// Removes a monster's gauge (e.g., when permanently removed from battle)
  /// </summary>
  public void RemoveGauge(IMonster monster)
  {
    monsterGauges.Remove(monster);
  }

  /// <summary>
  /// Resets all gauges (useful for battle initialization)
  /// </summary>
  public void ResetAll()
  {
    foreach (var gauge in monsterGauges.Values)
    {
      gauge.Reset();
    }
  }

  /// <summary>
  /// Gets all gauge states (useful for debugging/UI)
  /// </summary>
  public IEnumerable<AtbGaugeState> GetAllGauges()
  {
    return monsterGauges.Values;
  }
}
