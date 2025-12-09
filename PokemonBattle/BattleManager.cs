using System;
using System.Collections.Generic;
using UnityEngine;

/**
The BattleManager is the central class that orchestrates the battle between two teams of monsters.
It delegates battle flow logic (turn order, timing, etc.) to an IBattleConductor implementation.
At a high level, this class will:
 - Use a conductor to determine action order and timing
 - Execute the moves and actions
 - Handle the effects of moves
 - Handle the status of the monsters
 - Announce the winner
*/
public class BattleManager
{
  protected BattleModel battleModel;
  protected IBattleConductor conductor;

  private System.Random random = new System.Random();

  public BattleManager(BattleModel model, IBattleConductor conductor)
  {
    model.playerTeam._SetTeamID(BattleModel.PLAYER_TEAM_ID);
    model.computerTeam._SetTeamID(BattleModel.COMPUTER_TEAM_ID);
    this.battleModel = model;
    this.conductor = conductor;
    this.conductor.Initialize(model);
  }

  /// <summary>
  /// Convenience constructor that uses SimpleTurnConductor by default
  /// </summary>
  public BattleManager(BattleModel model)
    : this(model, new SimpleTurnConductor()) { }

  public void StartBattle()
  {
    Debug.Log("Battle Start");

    // Check if conductor is time-driven (ATB, real-time systems)
    bool isTimeDriven = conductor is ITimeDrivenConductor;

    if (isTimeDriven)
    {
      StartTimeDrivenBattle();
    }
    else
    {
      StartTurnBasedBattle();
    }

    AnnounceWinner();
  }

  /// <summary>
  /// Main loop for turn-based battles (SimpleTurnConductor, PartyBattleConductor, etc.)
  /// Synchronous action execution - wait for each action to be requested and executed.
  /// </summary>
  private void StartTurnBasedBattle()
  {
    // Main battle loop using conductor pattern
    while (!conductor.IsBattleOver())
    {
      // Get next action from conductor (move, switch, item, etc.)
      BattleAction action = conductor.GetNextAction();

      if (action == null)
      {
        // Phase is complete (round/segment/cycle depending on conductor type)
        // Process end-of-phase effects (poison, weather, etc.)
        conductor.ProcessEndOfPhase();
        continue;
      }

      // Execute the action (one turn)
      ExecuteAction(action);

      // After executing ANY action, check if forced switches are needed
      // (e.g., monster fainted from attack, or from recoil damage)
      CheckAndHandleForcedSwitches();
    }
  }

  /// <summary>
  /// Main loop for time-driven battles (ATB, real-time systems).
  /// Continuously advances time and executes actions as they become ready.
  /// NOTE: This is a simplified simulation loop. In a real Unity game, this would
  /// be driven by MonoBehaviour.Update() instead of a while loop.
  /// </summary>
  private void StartTimeDrivenBattle()
  {
    var timeDrivenConductor = conductor as ITimeDrivenConductor;
    const float TICK_INTERVAL = 0.016f; // ~60 FPS simulation

    Debug.Log("Starting time-driven battle (ATB mode)");

    while (!conductor.IsBattleOver())
    {
      // Advance time (in real Unity, this would be Time.deltaTime)
      timeDrivenConductor.Tick(TICK_INTERVAL);

      // Try to execute any ready actions
      BattleAction action = conductor.GetNextAction();

      if (action != null)
      {
        ExecuteAction(action);
        CheckAndHandleForcedSwitches();
      }

      // Small delay to prevent infinite loop in headless simulation
      // In real Unity with Update(), this wouldn't be needed
      System.Threading.Thread.Sleep((int)(TICK_INTERVAL * 1000));
    }
  }

  /// <summary>
  /// Checks if any team needs a forced switch (active monster fainted) and handles it immediately.
  /// This is called after every action execution, so recoil damage triggers immediate switches.
  /// </summary>
  private void CheckAndHandleForcedSwitches()
  {
    // Check player team
    if (battleModel.playerTeam.NeedsForcedSwitch())
    {
      HandleForcedSwitch(battleModel.playerTeam);
    }

    // Check computer team
    if (battleModel.computerTeam.NeedsForcedSwitch())
    {
      HandleForcedSwitch(battleModel.computerTeam);
    }
  }

  /// <summary>
  /// Handles a forced switch for a team whose active monster(s) fainted.
  /// Handles all fainted active slots at once.
  /// </summary>
  private void HandleForcedSwitch(BattleTeam team)
  {
    // Find all fainted active slots and switch them
    int faintedSlot;
    while ((faintedSlot = team.GetFaintedActiveSlot()) != -1)
    {
      var oldMonster = team.GetActiveMonsters()[faintedSlot];
      Debug.Log($"{oldMonster.Nickname} fainted! Forced switch required.");

      // Notify conductor of switch-out
      conductor.OnMonsterSwitchedOut(oldMonster, team);

      // Get available reserves
      var availableReserves = team.GetAvailableReserves();
      if (availableReserves.Count == 0)
      {
        // This should never happen - IsBattleOver() should have caught this
        Debug.LogError($"Team {team.TeamId} has no available reserves but isn't defeated!");
        return;
      }

      // For now, pick first available monster
      // TODO: Ask AI/player to make strategic choice
      var chosenMonster = availableReserves[0];
      int switchIndex = team.AllMonsters.IndexOf(chosenMonster);

      // Perform the switch for this specific slot
      team.SwitchActiveMonster(faintedSlot, switchIndex);
      Debug.Log($"{oldMonster.Nickname} is recalled!");
      Debug.Log($"Go, {chosenMonster.Nickname}!");

      // Notify conductor of switch-in
      conductor.OnMonsterSwitchedIn(chosenMonster, team);

      // TODO: Handle switch-in abilities (this will eventually move to conductor implementations)
    }
  }

  /// <summary>
  /// Executes a battle action (move, switch, item, etc.)
  /// </summary>
  private void ExecuteAction(BattleAction action)
  {
    switch (action.Type)
    {
      case BattleAction.ActionType.Move:
        ExecuteMove(action.Move, action.Actor, action.Target);
        break;

      case BattleAction.ActionType.Switch:
        ExecuteSwitch(action);
        break;

      case BattleAction.ActionType.Item:
        // TODO: Implement item logic in future phase
        Debug.LogWarning("Item action not yet implemented");
        break;

      case BattleAction.ActionType.Defend:
        // TODO: Implement defend logic in future phase
        Debug.LogWarning("Defend action not yet implemented");
        break;

      case BattleAction.ActionType.Flee:
        // TODO: Implement flee logic in future phase
        Debug.LogWarning("Flee action not yet implemented");
        break;

      case BattleAction.ActionType.EndOfRound:
        // This is a sentinel value that should never reach ExecuteAction
        // GetNextAction should intercept it and return null instead
        Debug.LogError("EndOfRound sentinel reached ExecuteAction - this should never happen!");
        break;

      default:
        Debug.LogError($"Unknown action type: {action.Type}");
        break;
    }
  }

  /// <summary>
  /// Executes a voluntary switch action - player/AI chooses to swap out their active monster.
  /// Note: Forced switches (when monster faints) are handled by CheckAndHandleForcedSwitches().
  /// </summary>
  private void ExecuteSwitch(BattleAction action)
  {
    // Determine which team is switching
    BattleTeam team = GetTeamForMonster(action.Actor);

    if (team == null)
    {
      Debug.LogError("Switch action with null Actor - cannot identify team.");
      return;
    }

    var oldMonster = team.ActiveMonster;
    var newMonster = team.AllMonsters[action.SwitchToIndex];

    // Notify conductor of switch-out
    conductor.OnMonsterSwitchedOut(oldMonster, team);

    // Perform the switch
    team.SwitchActiveMonster(action.SwitchToIndex);

    Debug.Log($"{oldMonster.Nickname} is recalled!");
    Debug.Log($"Go, {newMonster.Nickname}!");

    // Notify conductor of switch-in
    conductor.OnMonsterSwitchedIn(newMonster, team);

    // TODO: Switch-in abilities and effects will eventually be handled in conductor implementations
  }

  /// <summary>
  /// Finds which team a monster belongs to
  /// </summary>
  private BattleTeam GetTeamForMonster(IMonster monster)
  {
    if (monster == null)
      return null;

    // Check if monster is in player team
    if (battleModel.playerTeam.AllMonsters.Contains(monster))
    {
      return battleModel.playerTeam;
    }

    // Check if monster is in computer team
    if (battleModel.computerTeam.AllMonsters.Contains(monster))
    {
      return battleModel.computerTeam;
    }

    return null;
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
    if (!battleModel.playerTeam.IsDefeated())
    {
      Debug.Log($"Player team wins!");
      Debug.Log(
        $"Remaining monsters: {battleModel.playerTeam.GetRemainingMonsterCount()}/{battleModel.playerTeam.AllMonsters.Count}"
      );
    }
    else if (!battleModel.computerTeam.IsDefeated())
    {
      Debug.Log($"Computer team wins!");
      Debug.Log(
        $"Remaining monsters: {battleModel.computerTeam.GetRemainingMonsterCount()}/{battleModel.computerTeam.AllMonsters.Count}"
      );
    }
    else
    {
      Debug.Log("Both teams defeated - Draw!");
    }
  }

  /// <summary>
  /// Gets all monsters from both teams (active + reserves)
  /// </summary>
  public IMonster[] GetAllMonsters()
  {
    var allMonsters = new System.Collections.Generic.List<IMonster>();
    allMonsters.AddRange(battleModel.playerTeam.AllMonsters);
    allMonsters.AddRange(battleModel.computerTeam.AllMonsters);
    return allMonsters.ToArray();
  }

  /// <summary>
  /// Gets only the currently active monsters from both teams
  /// </summary>
  public IMonster[] GetActiveMonsters()
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
    // Apply all target effects. This includes:
    // - Damage/healing
    // - Status effects
    // - Attribute de/buffs
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

    // Apply all attribute deltas. Typically damage/healing, but could also be speed changes, etc.
    foreach (KeyValuePair<EMonsterAttribute, int> attributeDelta in effect.AttributeDeltas)
    {
      ApplyAttributeDelta(effect.Target, attributeDelta.Key, attributeDelta.Value);
    }

    // Apply status effects. Example: Burn, Poison, Sleep, etc.
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
