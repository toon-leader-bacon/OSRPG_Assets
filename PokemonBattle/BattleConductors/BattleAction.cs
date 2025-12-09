using System.Collections.Generic;

/// <summary>
/// Represents a single action in battle (move, switch, item, etc.)
/// </summary>
public class BattleAction
{
  public enum ActionType
  {
    Move, // Use a move/ability
    Switch, // Swap active monster
    Item, // Use a consumable item
    Defend, // Reduce damage taken
    Flee, // Attempt to escape

    // Utility Actions, used by the Conductor/ BattleManager to coreograph the battle flow
    EndOfRound, // Sentinel value marking end of round (not executed, triggers ProcessEndOfPhase)
  }

  public ActionType Type { get; set; }
  public IMonster Actor { get; set; }
  public IMonster Target { get; set; } // Single target (can be null for multi-target or self-target moves)
  public List<IMonster> Targets { get; set; } // For multi-target moves
  public IMove Move { get; set; } // if Type == Move
  public int ItemId { get; set; } // if Type == Item
  public int SwitchToIndex { get; set; } // if Type == Switch

  /// <summary>
  /// Creates a Move action
  /// </summary>
  public static BattleAction CreateMoveAction(IMonster actor, IMove move, IMonster target)
  {
    return new BattleAction
    {
      Type = ActionType.Move,
      Actor = actor,
      Move = move,
      Target = target,
    };
  }

  /// <summary>
  /// Creates a Switch action
  /// </summary>
  public static BattleAction CreateSwitchAction(IMonster actor, int switchToIndex)
  {
    return new BattleAction
    {
      Type = ActionType.Switch,
      Actor = actor,
      SwitchToIndex = switchToIndex,
    };
  }

  /// <summary>
  /// Creates an Item action
  /// </summary>
  public static BattleAction CreateItemAction(IMonster actor, int itemId, IMonster target)
  {
    return new BattleAction
    {
      Type = ActionType.Item,
      Actor = actor,
      ItemId = itemId,
      Target = target,
    };
  }

  /// <summary>
  /// Creates a Defend action
  /// </summary>
  public static BattleAction CreateDefendAction(IMonster actor)
  {
    return new BattleAction { Type = ActionType.Defend, Actor = actor };
  }

  /// <summary>
  /// Creates a Flee action
  /// </summary>
  public static BattleAction CreateFleeAction(IMonster actor)
  {
    return new BattleAction { Type = ActionType.Flee, Actor = actor };
  }

  /// <summary>
  /// Creates an EndOfRound sentinel action.
  /// This is not executed - it signals the conductor to call ProcessEndOfPhase.
  /// </summary>
  public static BattleAction CreateEndOfRoundSentinel()
  {
    return new BattleAction { Type = ActionType.EndOfRound };
  }
}
