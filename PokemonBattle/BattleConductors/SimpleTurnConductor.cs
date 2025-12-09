using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements classic Pokemon Red/Blue turn-based battle flow.
///
/// Flow Structure:
/// - ROUND: Both teams select actions, then both execute in speed order
/// - TURN: Each team gets one turn per round (player turn + computer turn = 1 round)
/// - ACTION: The move/switch/item performed during a turn
///
/// Rules:
/// - Both sides choose actions simultaneously (at start of round)
/// - Actions resolve in speed order (ties go to player)
/// - Round ends after both teams have taken their turns
/// - Forced switches are handled by BattleManager immediately after each action
/// </summary>
public class SimpleTurnConductor : IBattleConductor
{
  private BattleModel battleModel;
  private Queue<BattleAction> actionsQueue = new Queue<BattleAction>();

  public void Initialize(BattleModel model)
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
    // In SimpleTurnConductor, a phase = a round (both teams have taken their turns)
    // For now, this is a no-op in the simple turn system
  }

  public void OnMonsterSwitchedIn(IMonster monster, BattleTeam team)
  {
    // TODO: Trigger switch-in abilities (Intimidate, weather effects, etc.)
    // For now, this is a no-op in the simple turn system
  }

  public void OnMonsterSwitchedOut(IMonster monster, BattleTeam team)
  {
    // TODO: Trigger switch-out effects (Baton Pass stat transfers, etc.)
    // For now, this is a no-op in the simple turn system
  }

  /// <summary>
  /// Collects moves from both teams and orders them by speed for this round.
  /// Each team gets one turn per round.
  /// Adds an EndOfRound sentinel at the end to trigger ProcessEndOfPhase.
  /// </summary>
  private void CollectRoundActions()
  {
    var playerMonster = battleModel.playerTeam.ActiveMonster;
    var computerMonster = battleModel.computerTeam.ActiveMonster;

    var playerAI = battleModel.playerTeam.BattleAI;
    var computerAI = battleModel.computerTeam.BattleAI;

    // Request moves from both AIs
    // Note: We still pass BattleManager reference through battleModel temporarily
    // This will be refactored in future phases
    var playerMove = playerAI.GetMove(null, playerMonster, computerMonster);
    var computerMove = computerAI.GetMove(null, computerMonster, playerMonster);

    // Create battle actions
    var playerAction = BattleAction.CreateMoveAction(playerMonster, playerMove, computerMonster);
    var computerAction = BattleAction.CreateMoveAction(
      computerMonster,
      computerMove,
      playerMonster
    );

    // Determine order based on speed (ties go to player)
    if (playerMonster.Speed >= computerMonster.Speed)
    {
      actionsQueue.Enqueue(playerAction);
      actionsQueue.Enqueue(computerAction);
    }
    else
    {
      actionsQueue.Enqueue(computerAction);
      actionsQueue.Enqueue(playerAction);
    }

    // Add end-of-round sentinel
    // This marks the round as complete and triggers ProcessEndOfPhase
    actionsQueue.Enqueue(BattleAction.CreateEndOfRoundSentinel());
  }
}
