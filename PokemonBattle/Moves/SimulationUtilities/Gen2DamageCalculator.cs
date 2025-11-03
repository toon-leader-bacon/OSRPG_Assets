using System;
using Unity.Mathematics;
using UnityEditor;

/**
Generation II
    Damage=(((((2×Level/5)+2)×Power×A/D) / 50)×Item×Critical+2)×TK×Weather×Badge×STAB×Type×MoveMod×random×DoubleDmg

where:
    Level is the level of the attacking Pokémon. If the used move is Beat Up, L is instead the level of the Pokémon performing the strike.
    A is the effective Attack stat of the attacking Pokémon if the used move is a physical move, or the effective Special Attack stat of the attacking Pokémon if the used move is a special move (for a critical hit, if the target's Defense or Special Defense stat stage is greater than or equal to the attacker's Attack or Special Attack stat stage, all modifiers are ignored, and the unmodified Attack or Special is used instead). If the used move is Beat Up, A is instead the base Attack of the Pokémon performing the strike.
    D is the effective Defense stat of the target if the used move is a physical move, or the effective Special stat of the target if the used move is a special move (for a critical hit, all modifiers are ignored, and the unmodified Defense or Special is used instead). If the move is physical and the target has Reflect up, or if the move is special and the target has Light Screen up, this value is doubled (unless it is a critical hit). If the move is Explosion or Selfdestruct, this value is halved (rounded down, with a minimum of 1). If the used move is Beat Up, D is instead the base Defense of the target.
    Power is the power of the used move.
    Item is 1.1 if the attacker is holding an type-enhancing held item corresponding to the attack type (for instance, the Magnet for an Electric-type move). Otherwise, this value is simply 1.
    Critical is 2 for a critical hit, and 1 otherwise. It is always is 1 if the used move is Flail, Reversal, or Future Sight.
    TK is 1, 2, or 3 for each successive hit of Triple Kick, or always 1 if the used move is not Triple Kick.
    Weather is 1.5 if a Water-type move is being used during rain or a Fire-type move during harsh sunlight, and 0.5 if a Water-type move is used during harsh sunlight or SolarBeam or any Fire-type move during rain, and 1 otherwise.
    Badge is 1.125 if the attacking Pokémon is controlled by the player and if the player has obtained the Badge corresponding to the used move's type, and 1 otherwise. This bonus is not applied in link battles or the Battle Tower.
    STAB is the same-type attack bonus. This is equal to 1.5 if the move's type matches any of the user's types, and 1 if otherwise.
    Type is the type effectiveness. This can be 0.25, 0.5 (not very effective), 1 (normally effective), 2, or 4 (super effective), depending on both the move's and target's types. If the used move is Struggle, Future Sight, or Beat Up, Type is always 1.
    MoveMod can be (and if the used move is not any of these, MoveMod is 1):
        If Rollout is used, 2(n+d), where n is the amount of successful and consecutive hits of the move, up to 4 (for the fifth hit), and d is 1 if Defense Curl was used beforehand and 0 otherwise.
        If Fury Cutter is used, 2n, where n is the number of successful and consecutive uses of the move, up to 4.
        If Rage is used, an integer value corresponding to the Rage counter, i.e. the number of times the user of Rage has been damaged by an attack while using Rage.
    random is realized as a multiplication by a random uniformly distributed integer between 217 and 255 (inclusive), followed by an integer division by 255. random is always 1 if Flail or Reversal is used.
    DoubleDmg is 2 if the used move is Pursuit and the target is attempting to switch out, Stomp and the target has previously used Minimize, Gust or Twister and the target is in the semi-invulnerable turn of Fly, or Earthquake or Magnitude and the target is in the semi-invulnerable turn of Dig, and 1 otherwise.
*/
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
     (----------------------------------- * (item_mod1 * critical + 2)) * TK * Weather * Badge  * stab * type * MoveMod * random * doubleDmg
                                50
     */
    int level = caster.level;
    int attack = caster.Attack;
    int defense = target.Defense;
    int power = move.power;
    float item = caster.hasItem ? caster.item.DmgMod1(caster, target, move, model) : 1f;
    Tuple<bool, float> criticalMod = this.criticalMod_Gen2(caster, move, model);
    bool isCritical = criticalMod.Item1;
    float critical = isCritical ? criticalMod.Item2 : 1f;

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

    // Math
    float a = 2 * level;
    float b = (a / 5) + 2;
    float c = b * power * (attack / defense);
    float d = c / 50;
    float result =
      (d * item * critical + 2) * tk * weather * badge * stab * type * moveMod * random * doubleDmg;
    return Math.Max(1, (int)result);
  }

  public Tuple<bool, float> criticalMod_Gen2(IMonster caster, IMove move, BattleModel model)
  {
    /**
    Similar to stats, there are temporary in-battle stages used to determine the probability that a particular move will
    be a critical hit.

    +0 	17/256 (≈6.64%)
    +1 	1/8 (12.5%)
    +2 	1/4 (25%)
    +3 	85/256
    +4 and above 	1/2 (50%)
    */
    // TODO: Implement the stage logic for this
    // First, determine if the hit is a critical hit
    // Larger threshold => easier to crit
    int threshold = 17;
    int random = NocabRNG.newRNG.generateInt(1, 256, true, true);
    bool isCritical = random <= threshold;

    return isCritical
      ? Tuple.Create(true, 2.0f) // In gen 2, crits are always 2x damage
      : Tuple.Create(false, 1.0f);
  }
}
