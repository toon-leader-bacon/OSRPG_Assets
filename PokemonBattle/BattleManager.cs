using System;
using System.Collections.Generic;
using UnityEngine;

/**
The BattleManager is the central class that orchestrates the battle between two teams of monsters.
At time of writing, this is based off of the Pokemon style of battle, where each team has a single monster.
At a high level, this class will:
 - Ask each team for their move.
 - Handle the battle logic
   -- Determine move order
   -- Execute the moves
   -- Handle the effects of moves
   -- Handle the status of the monsters
 - Announce the winner
*/
public class BattleManager
{
  // private IBattleAI playerAi;
  // private IMonster playerMonster;

  // private IBattleAI computerAi;
  // private IMonster computerMonster;

  // BattleTeam playerTeam;
  // BattleTeam computerTeam;
  // public BattleWeather currentWeather = BattleWeather.None;

  protected BattleModel battleModel;

  private System.Random random = new System.Random();

  // public BattleManager(BattleTeam playerTeam, BattleTeam computerTeam)
  // {
  //   this.playerTeam = playerTeam;
  //   this.battleModel.playerTeam._SetTeamID(0);

  //   this.computerTeam = computerTeam;
  //   this.battleModel.computerTeam._SetTeamID(1);
  // }

  public BattleManager(BattleModel model)
  {
    model.playerTeam._SetTeamID(BattleModel.PLAYER_TEAM_ID);
    model.computerTeam._SetTeamID(BattleModel.COMPUTER_TEAM_ID);
    this.battleModel = model;
  }

  public void StartBattle()
  {
    Debug.Log("Battle Start");

    // Simple pokemon battle loop: Ask each team for their move. Then execute the move in the engine.
    // The engine will handle the battle logic, including the order of moves, the effects of moves, and the
    // status of the monsters.
    while (!IsBattleOver())
    {
      var playerAI = battleModel.playerTeam.BattleAI;
      var computerAI = battleModel.computerTeam.BattleAI;

      var playerMonster = battleModel.playerTeam.ActiveMonster;
      var computerMonster = battleModel.computerTeam.ActiveMonster;

      var playerMove = playerAI.GetMove(this, playerMonster, computerMonster);
      var computerMove = computerAI.GetMove(this, computerMonster, playerMonster);

      ExecuteTurn(playerMove, computerMove);
    }

    AnnounceWinner();
  }

  private bool IsBattleOver()
  {
    return battleModel.playerTeam.ActiveMonster.Health <= 0
      || battleModel.computerTeam.ActiveMonster.Health <= 0;
  }

  private void ExecuteTurn(IMove playerMove, IMove computerMove)
  {
    var playerMonster = battleModel.playerTeam.ActiveMonster;
    var computerMonster = battleModel.computerTeam.ActiveMonster;
    // Determine the order of moves.
    // Ties go to the player (more fun that way :D)
    if (playerMonster.Speed >= computerMonster.Speed)
    {
      // Player goes first.
      ExecuteMove(playerMove, playerMonster, computerMonster);
      if (!IsBattleOver())
      {
        // Computer goes second.
        ExecuteMove(computerMove, computerMonster, playerMonster);
      }
    }
    else
    {
      // Computer goes first.
      ExecuteMove(computerMove, computerMonster, playerMonster);
      if (!IsBattleOver())
      {
        // Player goes second.
        ExecuteMove(playerMove, playerMonster, computerMonster);
      }
    }
  }

  private void ExecuteMove(IMove move, IMonster user, IMonster target)
  {
    Debug.Log($"{user.Nickname} uses {move.Name}!");

    // Get the move result (what should happen)
    MoveResult result = move.Execute(this, user, target);

    // Apply the result (make it happen)
    ApplyMoveResult(result);

    // TODO: This log is View logic and should be moved to a View layer
    // For now, we'll keep basic logging but move detailed HP tracking into ApplyMoveResult
    Debug.Log($"{target.Nickname}'s HP: {target.Health}/{target.MaxHealth}");
  }

  private void AnnounceWinner()
  {
    Debug.Log("Battle over");
    if (battleModel.playerTeam.ActiveMonster.Health > 0)
    {
      Debug.Log($"{battleModel.playerTeam.ActiveMonster.Nickname} wins!");
    }
    else
    {
      Debug.Log($"{battleModel.computerTeam.ActiveMonster.Nickname} wins!");
    }
  }

  public IMonster[] GetAllMonsters()
  {
    return new IMonster[]
    {
      battleModel.playerTeam.ActiveMonster,
      battleModel.computerTeam.ActiveMonster,
    };
  }

  /// <summary>
  /// Applies a MoveResult to the battle state. This is where the Controller actually
  /// mutates the Model based on the move's described effects.
  /// </summary>
  private void ApplyMoveResult(MoveResult result)
  {
    // Apply all target effects
    foreach (var targetEffect in result.TargetEffects)
    {
      ApplyTargetEffect(targetEffect);
    }

    // Apply field effects
    foreach (KeyValuePair<EFieldEffect, object> fieldEffect in result.FieldEffects)
    {
      ApplyFieldEffect(fieldEffect.Key, fieldEffect.Value);
    }

    // Apply side effects
    foreach (
      KeyValuePair<BattleTeam, Dictionary<ESideEffect, object>> sideEffect in result.SideEffects
    )
    {
      ApplySideEffects(sideEffect.Key, sideEffect.Value);
    }
  }

  /// <summary>
  /// Applies a single TargetEffect to a monster.
  /// </summary>
  private void ApplyTargetEffect(TargetEffect effect)
  {
    if (effect.Target == null)
    {
      Debug.LogWarning("TargetEffect has null Target, skipping.");
      return;
    }

    // Apply all attribute deltas
    foreach (KeyValuePair<EMonsterAttribute, int> attributeDelta in effect.AttributeDeltas)
    {
      ApplyAttributeDelta(effect.Target, attributeDelta.Key, attributeDelta.Value);
    }

    // Apply status effects
    foreach (var status in effect.StatusEffectsToApply)
    {
      effect.Target.BattleEffects.AddTag(status.ToStatusString());
      Debug.Log($"{effect.Target.Nickname} is now affected by {status}!");
    }

    // Remove status effects
    foreach (var status in effect.StatusEffectsToRemove)
    {
      effect.Target.BattleEffects.RemoveTag(status.ToStatusString());
      Debug.Log($"{effect.Target.Nickname} is no longer affected by {status}!");
    }
  }

  /// <summary>
  /// Applies a delta to a monster's attribute.
  /// Handles both standard IMonster properties and custom attributes via BattleEffects.
  /// </summary>
  private void ApplyAttributeDelta(IMonster target, EMonsterAttribute attribute, int delta)
  {
    // TODO: Now that the attributes are enums, is a switch statement really needed here?
    switch (attribute)
    {
      case EMonsterAttribute.Health:
        target.Health += delta;
        // Clamp to valid range [0, MaxHealth]
        target.Health = System.Math.Clamp(target.Health, 0, target.MaxHealth);

        // TODO: Move this logging to View layer
        if (delta < 0)
          Debug.Log($"{target.Nickname} takes {-delta} damage!");
        else if (delta > 0)
          Debug.Log($"{target.Nickname} heals {delta} HP!");
        break;

      case EMonsterAttribute.Speed:
        target.Speed += delta;
        // NOTE: No clamping for speed - it can go negative in some game systems
        // TODO: Consider if speed should have a minimum value (e.g., 1)
        Debug.Log($"{target.Nickname}'s Speed changed by {delta:+0;-#}!");
        break;

      case EMonsterAttribute.Attack:
        target.Attack += delta;
        Debug.Log($"{target.Nickname}'s Attack changed by {delta:+0;-#}!");
        break;

      case EMonsterAttribute.Defense:
        target.Defense += delta;
        Debug.Log($"{target.Nickname}'s Defense changed by {delta:+0;-#}!");
        break;

      case EMonsterAttribute.SpecialAttack:
        target.SpecialAttack += delta;
        Debug.Log($"{target.Nickname}'s Special Attack changed by {delta:+0;-#}!");
        break;

      case EMonsterAttribute.SpecialDefense:
        target.SpecialDefense += delta;
        Debug.Log($"{target.Nickname}'s Special Defense changed by {delta:+0;-#}!");
        break;

      case EMonsterAttribute.Accuracy:
      case EMonsterAttribute.Evasion:
      case EMonsterAttribute.CriticalRate:
        // These stats are stored in BattleEffects since they're not direct IMonster properties
        string attrName = attribute.ToAttributeString();
        int currentValue = target.BattleEffects.ContainsTaggedInt(attrName)
          ? target.BattleEffects.GetTaggedInt(attrName)
          : 0;
        target.BattleEffects.SetTaggedInt(attrName, currentValue + delta);
        Debug.Log($"{target.Nickname}'s {attribute} changed by {delta:+0;-#}!");
        break;

      default:
        Debug.LogWarning($"Unknown attribute: {attribute}");
        break;
    }
  }

  /// <summary>
  /// Applies a field-wide effect to the BattleModel.
  /// </summary>
  private void ApplyFieldEffect(EFieldEffect effect, object effectValue)
  {
    switch (effect)
    {
      case EFieldEffect.Weather:
        if (effectValue is BattleWeather weather)
        {
          battleModel.currentWeather = weather;
          Debug.Log($"Weather changed to {weather}!");
        }
        else
        {
          Debug.LogWarning($"Invalid weather value: {effectValue}");
        }
        break;

      case EFieldEffect.ElectricTerrain:
      case EFieldEffect.GrassyTerrain:
      case EFieldEffect.MistyTerrain:
      case EFieldEffect.PsychicTerrain:
        // TODO: Implement terrain storage in BattleModel
        Debug.Log($"{effect} activated!");
        break;

      case EFieldEffect.TrickRoom:
      case EFieldEffect.MagicRoom:
      case EFieldEffect.WonderRoom:
        // TODO: Implement room effects in BattleModel
        Debug.Log($"{effect} activated!");
        break;

      case EFieldEffect.Gravity:
      case EFieldEffect.MudSport:
      case EFieldEffect.WaterSport:
      case EFieldEffect.IonDeluge:
      case EFieldEffect.FairyLock:
        // TODO: Implement these field conditions in BattleModel
        Debug.Log($"{effect} activated!");
        break;

      default:
        Debug.LogWarning($"Unknown field effect: {effect}");
        break;
    }
  }

  /// <summary>
  /// Applies side-wide effects to a team's BattleEffects.
  /// </summary>
  private void ApplySideEffects(BattleTeam team, Dictionary<ESideEffect, object> effects)
  {
    // Get the team's side effects
    BattleEffects sideEffects = battleModel.GetBattleEffects(team);

    foreach (KeyValuePair<ESideEffect, object> effect in effects)
    {
      // Convert enum to string for storage in BattleEffects
      string effectName = effect.Key.ToEffectString();

      // Store the effect in the team's BattleEffects
      // The type of storage depends on the effect value type
      if (effect.Value is int intValue)
      {
        sideEffects.SetTaggedInt(effectName, intValue);
      }
      else if (effect.Value is bool boolValue)
      {
        sideEffects.SetTaggedBool(effectName, boolValue);
      }
      else if (effect.Value is string stringValue)
      {
        sideEffects.SetTag(effectName, stringValue);
      }
      else
      {
        // For other types, just add a tag
        sideEffects.AddTag(effectName);
      }

      Debug.Log($"Side effect {effect.Key} applied to team {team.TeamId}!");
    }
  }
}
