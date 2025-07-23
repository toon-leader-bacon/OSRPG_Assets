using System;
using Unity.Mathematics;

public class Gen1DamageCalculator : BaseDamageCalculator
{
  public override int CalculateDamage(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model = null
  )
  {
    int level = caster.level;
    int critical = NocabRNG.newRNG.generateInt(1, 2, true, true);
    int power = move.power;
    int attack = caster.Attack;
    int defense = target.Defense;
    float stab = calculate_STAB(caster.Types, move.type);
    float type = calculate_typeEffective(
      target.Types,
      move.type,
      TypeChart_PokemonGen.buildGen1Chart()
    );

    int random = NocabRNG.newRNG.generateInt(217, 255, true, true) / 255; // integer division

    /**
       (2 * level * critical)                attack
      (---------------------- + 2) * power * -------
                    5                        defense
     (------------------------------------------------ + 2)  * stab * type * random
                                50
     */
    float a = 2 * level * critical;
    float b = (a / 5) + 2;
    float c = b * power * (attack / defense);
    float d = c / 50;
    float result = (d + 2) * stab * type * random;
    return Math.Max(1, (int)result);
  }
}
