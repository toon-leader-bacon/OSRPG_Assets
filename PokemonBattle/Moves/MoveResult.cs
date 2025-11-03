using System.Collections.Generic;

/// <summary>
/// Result of executing a move. Contains all state changes that the Controller (BattleManager)
/// should apply to the Model (monsters, battle state).
///
/// This class is part of the Model layer and represents a data-driven description of effects,
/// maintaining separation between Model (what should happen) and Controller (making it happen).
/// </summary>
public class MoveResult
{
  /// <summary>
  /// All targets and their effects from this move.
  /// Can include multiple opponents, allies, or even the user (for recoil/self-buffs).
  /// </summary>
  public List<TargetEffect> TargetEffects { get; set; } = new();

  /// <summary>
  /// Field-wide effects that should be applied to the BattleModel.
  ///
  /// Examples:
  ///   { EFieldEffect.Weather, BattleWeather.Rainy } => Set weather to rainy
  ///   { EFieldEffect.ElectricTerrain, true } => Enable electric terrain
  ///   { EFieldEffect.TrickRoom, true } => Activate trick room
  ///
  /// The Controller interprets these and applies them to the appropriate model state.
  /// </summary>
  public Dictionary<EFieldEffect, object> FieldEffects { get; set; } = new();

  /// <summary>
  /// Side-wide effects that apply to a team (not individual monsters or the whole field).
  ///
  /// Examples:
  ///   { team, { ESideEffect.Reflect, true } } => Enable Reflect for the team
  ///   { team, { ESideEffect.StealthRock, 1 } } => Set stealth rock layers
  ///
  /// Key: The team reference
  /// Value: Dictionary of side effect to effect data (typically bool or int)
  ///
  /// TODO: Consider using team ID (int) instead of BattleTeam reference to reduce coupling.
  /// </summary>
  public Dictionary<BattleTeam, Dictionary<ESideEffect, object>> SideEffects { get; set; } = new();

  /// <summary>
  /// Helper method to add a single target effect.
  /// </summary>
  public void AddTargetEffect(TargetEffect effect)
  {
    TargetEffects.Add(effect);
  }

  /// <summary>
  /// Helper method to quickly add a simple damage effect.
  /// NOTE: the `damage` param here should be POSITIVE (the amount of damage to deal).
  /// The TargetEffect will automatically negate the damage. This means:
  /// Update the Target Monster's Health by -damage.
  /// </summary>
  public void AddDamage(IMonster target, int damage)
  {
    TargetEffects.Add(
      new TargetEffect
      {
        Target = target,
        AttributeDeltas = new() { { EMonsterAttribute.Health, -damage } },
      }
    );
  }

  /// <summary>
  /// Helper method to quickly add a healing effect.
  /// NOTE: the `healing` param here should be POSITIVE (the amount of healing to apply).
  /// The TargetEffect will automatically negate the healing. This means:
  /// Update the Target Monster's Health by +healing.
  /// </summary>
  public void AddHealing(IMonster target, int healing)
  {
    TargetEffects.Add(
      new TargetEffect
      {
        Target = target,
        AttributeDeltas = new() { { EMonsterAttribute.Health, healing } },
      }
    );
  }
}
