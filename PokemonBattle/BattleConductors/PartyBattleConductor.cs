using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Implements party-based turn-based battle flow with multiple active monsters per team.
///
/// Flow Structure:
/// - ROUND: All combatants select actions, then all execute in speed order
/// - TURN: Each active monster gets one turn per round
/// - ACTION: The move/switch/item performed during a turn
///
/// Rules:
/// - All active monsters choose actions simultaneously (at start of round)
/// - Actions resolve in speed order across ALL active monsters (ties go to player team)
/// - Round ends after all active monsters have taken their turns
/// - Forced switches are handled by BattleManager immediately after each action
///
/// Configuration:
/// - Supports any N vs M configuration (2v2, 3v3, 4v4, etc.)
/// - Active count is determined by BattleTeam configuration
/// </summary>
public class PartyBattleConductor : IBattleConductor
{
  private BattleModel battleModel;
  private Queue<BattleAction> actionsQueue = new Queue<BattleAction>();

  public virtual void Initialize(BattleModel model)
  {
    this.battleModel = model;
    this.actionsQueue.Clear();
  }

  public BattleAction GetNextAction()
  {
    // If queue is empty, collect actions for next round
    if (actionsQueue.Count == 0)
    {
      CollectRoundActions();
    }

    // Get next action from queue
    if (actionsQueue.Count > 0)
    {
      BattleAction action = actionsQueue.Dequeue();

      // If this is the end-of-round sentinel, return null to trigger ProcessEndOfPhase
      if (action.Type == BattleAction.ActionType.EndOfRound)
      {
        return null;
      }

      return action;
    }

    // Queue should never be empty here (CollectRoundActions should add sentinel)
    Debug.LogError("Action queue empty after collection - no sentinel added?");
    return null;
  }

  public bool IsBattleOver()
  {
    // Battle ends when either team is fully defeated (all monsters fainted)
    return battleModel.playerTeam.IsDefeated() || battleModel.computerTeam.IsDefeated();
  }

  public void ProcessEndOfPhase()
  {
    // TODO: Add end-of-round effects here (poison damage, weather effects, etc.)
    // In PartyBattleConductor, a phase = a round (all active monsters have taken their turns)
    // For now, this is a no-op
  }

  public virtual void OnMonsterSwitchedIn(IMonster monster, BattleTeam team)
  {
    // TODO: Trigger switch-in abilities (Intimidate, weather effects, entry hazards, etc.)
    // For now, this is a no-op in the party battle system
  }

  public virtual void OnMonsterSwitchedOut(IMonster monster, BattleTeam team)
  {
    // TODO: Trigger switch-out effects (Baton Pass, U-turn, etc.)
    // For now, this is a no-op in the party battle system
  }

  /// <summary>
  /// Collects moves from all active monsters on both teams and orders them by speed for this round.
  /// Each active monster gets one turn per round.
  /// Adds an EndOfRound sentinel at the end to trigger ProcessEndOfPhase.
  /// </summary>
  private void CollectRoundActions()
  {
    List<BattleAction> allActions = new List<BattleAction>();

    // Collect actions from all player active monsters
    var playerActives = battleModel.playerTeam.GetActiveMonsters();
    var computerActives = battleModel.computerTeam.GetActiveMonsters();

    foreach (var playerMonster in playerActives)
    {
      // For now, randomly pick a target from enemy actives
      // TODO: In future, AI should strategically choose target
      var target = GetRandomTarget(computerActives);

      var playerAI = battleModel.playerTeam.BattleAI;
      var playerMove = playerAI.GetMove(null, playerMonster, target);

      var action = BattleAction.CreateMoveAction(playerMonster, playerMove, target);
      allActions.Add(action);
    }

    // Collect actions from all computer active monsters
    foreach (var computerMonster in computerActives)
    {
      // For now, randomly pick a target from enemy actives
      // TODO: In future, AI should strategically choose target
      var target = GetRandomTarget(playerActives);

      var computerAI = battleModel.computerTeam.BattleAI;
      var computerMove = computerAI.GetMove(null, computerMonster, target);

      var action = BattleAction.CreateMoveAction(computerMonster, computerMove, target);
      allActions.Add(action);
    }

    // Sort all actions by speed (descending), with ties going to player team
    var sortedActions = allActions.OrderByDescending(action =>
    {
      // Add small bonus to player team monsters for tie-breaking
      bool isPlayerTeam = battleModel.playerTeam.AllMonsters.Contains(action.Actor);
      float tieBreaker = isPlayerTeam ? 0.1f : 0.0f;
      return action.Actor.Speed + tieBreaker;
    });

    // Enqueue all actions
    foreach (var action in sortedActions)
    {
      actionsQueue.Enqueue(action);
    }

    // Add end-of-round sentinel
    actionsQueue.Enqueue(BattleAction.CreateEndOfRoundSentinel());
  }

  /// <summary>
  /// Gets a random target from a list of monsters (temporary implementation).
  /// TODO: Replace with strategic AI target selection.
  /// </summary>
  private IMonster GetRandomTarget(List<IMonster> targets)
  {
    if (targets == null || targets.Count == 0)
    {
      Debug.LogError("No targets available!");
      return null;
    }

    // For now, just pick the first alive target
    // TODO: Make this smarter (random, or AI-driven)
    return targets.FirstOrDefault(t => t.Health > 0) ?? targets[0];
  }
}
