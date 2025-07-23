using System;
using Unity.Mathematics;

public class Gen5DamageCalculator : BaseDamageCalculator
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
    (----------------------------------- +  2) * Targets * PB * Weather * GlaiveRush * Critical * random * STAB * Type * Burn * other * ZMove * TeraShield
                                50
    */
    int level = caster.level;
    int attack = caster.Attack;
    int defense = target.Defense;
    int power = move.power;

    float targets = 1; // Or 0.75 if multiple targets
    float pb = 1; // Used only for Parental Bond move
    float weather = 1; // Or 1.5 or 0.5
    float glaiveRush = 1; // 2 for special move
    float critical = 1; // or (1.5 or 2)
    float random = NocabRNG.newRNG.generateInt(85, 100, true, true) / 100.0f;
    float stab = calculate_STAB(caster.Types, move.type);
    float type = calculate_typeEffective(
      target.Types,
      move.type,
      TypeChart_PokemonGen.buildGen5Chart()
    );

    float burn = 1f; // Or 0.5
    float other = 1; // Modified by special conditions
    float zmove = 1; // Allows for penetration of protect
    float teraShield = 1; // Only relevant for Tera Raid Battles

    float a = 2 * level;
    float b = (a / 5) + 2;
    float c = b * power * (attack / defense);
    float d = (c / 50) + 2;
    float result =
      d
      * targets
      * pb
      * weather
      * glaiveRush
      * critical
      * random
      * stab
      * type
      * burn
      * other
      * zmove
      * teraShield;
    return Math.Max(1, (int)result);
  }
}
