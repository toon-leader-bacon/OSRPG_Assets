using System;
using System.Collections.Generic;

public interface IBattleAI
{
  /**
   * This method will be called to get the move that the AI will use.
   * NOTE: There is an "AI" for the player to select, which is a UI thing, not a true AI. But the interface is the same.
   *
   * The battleManager is passed because some complex decision making may consider more rich data than just the user and target.
   * For example, considering the weather, what turn this is, etc.
   *
   * The friendlyMonster is the pokemon that is currently selecting a move.
   * The opposingMonster is the pokemon that is opposing the friendlyMonster.
   *
   * The return value is the move that the friendly monster will use. Again: The human player's implementation of this
   * is a UI thing, not a true AI.
   */
  IMove GetMove(BattleManager battleManager, IMonster friendlyMonster, IMonster opposingMonster);
}
