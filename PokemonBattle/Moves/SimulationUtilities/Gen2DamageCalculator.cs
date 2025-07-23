using System;
using Unity.Mathematics;

public class Gen2DamageCalculator : BaseDamageCalculator
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
     (----------------------------------- * (item * critical + 2)) * TK * Weather * Badge  * stab * type * MoveMod * random * doubleDmg
                                50
     */
    int level = caster.level;
    int attack = caster.Attack;
    int defense = target.Defense;
    int power = move.power;
    float item = 1.1f; // or 1
    int critical = 2; // Or 1
    int tk = 1; // Only useful for tipple kick; Values 1, 2 and 3
    float weather = 1; // Values 0.5, 1, 1.5
    float badge = 1.25f; // Only if the pokemon level is controlled by badge, and you have the appropriate badge type
    float stab = calculate_STAB(caster.Types, move.type);
    float type = calculate_typeEffective(
      target.Types,
      move.type,
      TypeChart_PokemonGen.buildGen2Chart()
    );
    float moveMod = 1; // Certain moves have their own move mod
    float random = NocabRNG.newRNG.generateInt(217, 255, true, true) / 255.0f; // integer division
    int doubleDmg = 1; // Some moves conditionally deal double damage

    float a = 2 * level;
    float b = (a / 5) + 2;
    float c = b * power * (attack / defense);
    float d = c / 50;
    float result =
      (d * item * critical + 2) * tk * weather * badge * stab * type * moveMod * random * doubleDmg;
    return Math.Max(1, (int)result);
  }
}
