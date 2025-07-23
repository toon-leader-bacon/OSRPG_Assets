using System;
using Unity.Mathematics;

public class Gen3DamageCalculator : BaseDamageCalculator
{
  public override int CalculateDamage(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model = null
  )
  {
    /**
       (2 * level)                attack
      (----------- + 2) * power * -------
            5                     defense
     (----------------------------------- + (Burn * screen * targets * weather * ff + 2)) * Stockpile * Critical * doubleDmg * charg * HH * STAB * type * random
                                50
     */
    int level = caster.level;
    int attack = caster.Attack;
    int defense = target.Defense;
    int power = move.power;

    float burn = 1f; // Or 0.5
    float screen = 1f; // Or 0.5
    float targets = 1; // Or 0.5 in certain double battle cases
    float weather = 1; // Values 0.5, 1, 1.5
    float ff = 1; // Flash Fire pokemon may have 1.5 here

    int stockpile = 1; // Used for some moves
    int critical = 1; // Or 2

    int doubleDmg = 1; // Some moves conditionally deal double damage
    int charge = 1; // Some electric moves have 2
    float hh = 1; // 1.5 in double battle
    float stab = calculate_STAB(caster.Types, move.type);
    float type = calculate_typeEffective(
      target.Types,
      move.type,
      TypeChart_PokemonGen.buildGen3Chart()
    );
    float random = NocabRNG.newRNG.generateInt(85, 100, true, true) / 100.0f;

    float a = 2 * level;
    float b = (a / 5) + 2;
    float c = b * power * (attack / defense);
    float d = c / 50;
    float result =
      (d * burn * screen * targets * weather * ff + 2)
      * stockpile
      * critical
      * doubleDmg
      * charge
      * hh
      * stab
      * type
      * random;
    return Math.Max(1, (int)result);
  }
}
