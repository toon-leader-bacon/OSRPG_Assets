using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface IMove
{
  string Name { get; }
  EBattleType type { get; } // Elemental
  EMoveMedium moveMedium { get; } // Physical vs Special vs Status

  /**
   * This is the main method that will be called to execute the move.
   * It will be called by the BattleManager (Controller).
   * The user is the pokemon that is using the move.
   * The target is the pokemon that is being targeted by the move.
   *
   * IMPORTANT: This method should NOT directly modify monsters or battle state.
   * Instead, it returns a MoveResult that describes what changes should happen.
   * The BattleManager (Controller) is responsible for applying those changes.
   *
   * This separation maintains MVC architecture:
   * - Model (IMove): Calculates and describes effects
   * - Controller (BattleManager): Applies effects and coordinates responses
   * - View: Observes and displays results
   *
   * The battleManager itself is passed because some moves need access to context data
   * (e.g., all monsters on field, weather, side effects) to determine their effects.
   */
  MoveResult Execute(BattleManager battleManager, IMonster user, IMonster target);

  // TODO: Redo the stats of these moves
  int power { get; set; }

  public int criticalMod_thresholdPipe(int threshold)
  {
    // Larger threshold => easier to crit
    return threshold; // Most moves don't modify the crit rate threshold
  }

  public float DmgMod1(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    /**
     * The basic pokemon damage formula is:
     * ((attack / defense) * Mod1 + 2) * Mod2
     *
     * This function DmgMod1 will add a pipe to the Mod1 part.
     */
    return 1;
  }

  public float DmgMod2(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    /**
     * The basic pokemon damage formula is:
     * ((attack / defense) * Mod1 + 2) * Mod2
     *
     * This function DmgMod2 will add a pipe to the Mod2 part.
     */
    return 1;
  }
}
