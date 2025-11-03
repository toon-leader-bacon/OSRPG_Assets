using System.Collections.Generic;

/// <summary>
/// Represents the changes to apply to a single monster as a result of a move.
/// This is part of the Model layer - it describes WHAT should change, not HOW to change it.
/// </summary>
public class TargetEffect
{
  /// <summary>
  /// The monster that will receive these effects.
  /// </summary>
  public IMonster Target { get; set; }

  /// <summary>
  /// Dictionary mapping monster attributes to deltas.
  /// - Negative values represent damage/reduction to that attribute
  /// - Positive values represent healing/increases to that attribute
  ///
  /// Examples:
  ///   { EMonsterAttribute.Health, -25 }  => Target takes 25 damage
  ///   { EMonsterAttribute.Health, 15 }   => Target heals 15 HP
  ///   { EMonsterAttribute.Speed, 2 }     => Target gains +2 Speed
  ///   { EMonsterAttribute.Defense, -1 }  => Target loses 1 Defense (stat debuff)
  /// </summary>
  public Dictionary<EMonsterAttribute, int> AttributeDeltas { get; set; } = new();

  /// <summary>
  /// Status effects to add to the target.
  /// The Controller (BattleManager) is responsible for applying these.
  /// </summary>
  public HashSet<EStatusEffect> StatusEffectsToApply { get; set; } = new();

  /// <summary>
  /// Status effects to remove from the target.
  /// </summary>
  public HashSet<EStatusEffect> StatusEffectsToRemove { get; set; } = new();
}
