#nullable enable
using System.Collections.Generic;

/// <summary>
/// Interface for battle flow controllers that orchestrate the timing and order of actions.
/// Different implementations support different battle systems (turn-based, ATB, CTB, etc.)
///
/// Terminology:
/// - Action: Single move/switch/item performed by one combatant
/// - Turn: One combatant taking their action
/// - Phase: System-agnostic grouping (round in turn-based, time segment in ATB, etc.)
/// </summary>
public interface IBattleConductor
{
  /// <summary>
  /// Initializes the conductor with the battle model.
  /// Sets up any flow-specific state (timers, queues, etc.)
  /// </summary>
  void Initialize(BattleModel model);

  /// <summary>
  /// Gets the next action that should be executed in the battle.
  /// Returns null when the current phase is complete (signals end of round/segment/cycle).
  /// </summary>
  BattleAction? GetNextAction();

  /// <summary>
  /// Checks if the battle is over based on victory/defeat conditions.
  /// May vary by battle type.
  /// </summary>
  bool IsBattleOver();

  /// <summary>
  /// Processes end-of-phase effects (poison damage, weather, stat changes, etc.)
  /// Called by BattleManager when GetNextAction returns null.
  /// What constitutes a "phase" depends on the conductor implementation:
  /// - SimpleTurnConductor: End of round (both teams acted)
  /// - ATBConductor: Might process every N time units
  /// - TimelineConductor: Might process after each action
  /// </summary>
  void ProcessEndOfPhase();

  /// <summary>
  /// Called when a monster is switched into battle (becomes active).
  /// Use cases:
  /// - ATB: Reset gauge to 0%
  /// - Turn-based: Trigger switch-in abilities (Intimidate, weather effects)
  /// - Timeline: Recalculate turn queue
  /// </summary>
  /// <param name="monster">The monster being switched in</param>
  /// <param name="team">The team the monster belongs to</param>
  void OnMonsterSwitchedIn(IMonster monster, BattleTeam team);

  /// <summary>
  /// Called when a monster is switched out of battle (no longer active).
  /// Use cases:
  /// - ATB: Preserve or reset gauge state
  /// - Turn-based: Trigger switch-out effects
  /// - Timeline: Remove from turn queue
  /// </summary>
  /// <param name="monster">The monster being switched out</param>
  /// <param name="team">The team the monster belongs to</param>
  void OnMonsterSwitchedOut(IMonster monster, BattleTeam team);
}
