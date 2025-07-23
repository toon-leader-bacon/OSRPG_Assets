using System;
using Unity.Mathematics;

public class PokemonGoDamageCalculator : BaseDamageCalculator
{
  public override int CalculateDamage(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model = null
  )
  {
    float power = move.power;
    float attack = caster.Attack;
    float defense = target.Defense;

    float stab = calculate_STAB(caster.Types, move.type);
    float type = calculate_typeEffective(
      target.Types,
      move.type,
      TypeChart_PokemonGen.buildGen6Chart()
    );
    float weather = 1; // Or 1.5 or 0.5
    float friendship = 1; // 1.03, 1.05, 1.07, 1.1
    float dodged = 1; // Sometimes 0.25
    float mega = 1; // Sometimes 1.1, 1.3
    float trainer = 1; // Some battles it's 1.3
    float charge = 1; // charge timing mini game in range [0.75, 1), [0.5, 0.75), [0.25, 0.5)

    float modifier = type * stab * weather * friendship * dodged * mega * trainer * charge;
    return math.max((int)(math.floor(0.5 * power * (attack / defense) * modifier) + 1), 1);
  }
}
