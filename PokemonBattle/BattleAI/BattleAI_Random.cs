
using System;
using System.Collections.Generic;

public class BattleAI_Random : IBattleAI
{
  private Random random = new Random();

  public IMove GetMove(BattleManager battleManager, IMonster friendlyMonster, IMonster opposingMonster)
  {
    List<IMove> availableMoves = friendlyMonster.Moves;
    if (availableMoves.Count == 0)
    {
      throw new InvalidOperationException("The monster has no available moves.");
    }

    int randomIndex = random.Next(availableMoves.Count);
    return availableMoves[randomIndex];
  }
}