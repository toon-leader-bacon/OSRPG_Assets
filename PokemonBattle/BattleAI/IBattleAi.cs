using System;
using System.Collections.Generic;

public interface IBattleAI
{
  IMove GetMove(BattleManager battleManager, IMonster friendlyMonster, IMonster opposingMonster);
}
