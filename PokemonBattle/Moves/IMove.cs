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
   * It will be called by the BattleManager.
   * The user is the pokemon that is using the move.
   * The target is the pokemon that is being targeted by the move.
   *
   * WARNING: Modifications to the user and target args are REFERENCES so changes to these vars will constitute actual changes
   * to the actual pokemon objects. Useful for simple things like dealing damage or applying status effects to
   * the user or target.
   *
   * The battleManager itself is passed because some moves need access to more rich data than just the user and target.
   * For example, a move that increases the speed of all pokemon on the field,
   * or a move that modifies the weather, etc.
   */
  void Execute(BattleManager battleManager, IMonster user, IMonster target);

  // TODO: Redo the stats of these moves
  int power { get; set; }
}
