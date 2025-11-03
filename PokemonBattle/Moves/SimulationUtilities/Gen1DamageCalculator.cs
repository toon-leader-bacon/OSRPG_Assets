using System;
using Unity.Mathematics;
using UnityEditor.UI;

/**
Generation I

    Damage = ((((2 × Level × Critical) / 5) + 2) × Power × A/D) / 50 + 2) × STAB × Type1 × Type2 × random

where:
    Level is the level of the attacking Pokémon.
    Critical is 2 for a critical hit, and 1 otherwise.
    A is the effective Attack stat of the attacking Pokémon if the used move is a physical move, or the effective Special stat of the attacking Pokémon if the used move is a special move (for a critical hit, all modifiers are ignored, and the unmodified Attack or Special is used instead). If either this or D are greater than 255, both are divided by 4 and rounded down.
    D is the effective Defense stat of the target if the used move is a physical move, or the effective Special stat of the target if the used move is an other special move (for a critical hit, all modifiers are ignored, and the unmodified Defense or Special is used instead). If the move is physical and the target has Reflect up, or if the move is special and the target has Light Screen up, this value is doubled (unless it is a critical hit). If the move is Explosion or Selfdestruct, this value is halved (rounded down, with a minimum of 1). If either this or A are greater than 255, both are divided by 4 and rounded down. Unlike future Generations, if this is 0, the division is not made equal to 0; rather, the game will try to divide by 0 and softlock, hanging indefinitely until it is turned off.
    Power is the power of the used move.
    STAB is the same-type attack bonus. This is equal to 1.5 if the move's type matches any of the user's types, and 1 if otherwise. Internally, it is recognized as an addition of the damage calculated thus far divided by 2, rounded down, then added to the damage calculated thus far.
    Type1 is the type effectiveness of the used move against the target's type that comes first in the type matchup table, or only type if it only has one type. This can be 0.5 (not very effective), 1 (normally effective), 2 (super effective).
    Type2 is the type effectiveness of the used move against the target's type that comes second in the type matchup table. This can be 0.5 (not very effective), 1 (normally effective), 2 (super effective). If the target only has one type, Type2 is 1. If this would result in 0 damage, the calculation ends here and the move is stated to have missed, even if it would've hit.
    random is realized as a multiplication by a random uniformly distributed integer between 217 and 255 (inclusive), followed by an integer division by 255. If the calculated damage thus far is 1, random is always 1.
*/
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
    Tuple<bool, float> criticalMod = this.criticalMod_Gen1(caster, move, model);
    float critical = criticalMod.Item1 ? criticalMod.Item2 : 1.0f;
    int power = move.power;
    // TODO: Add in temporary stat modifiers here?
    int attack = move.moveMedium == EMoveMedium.Physical ? caster.Attack : caster.SpecialAttack;
    int defense = move.moveMedium == EMoveMedium.Physical ? target.Defense : target.SpecialDefense;
    defense = math.max(1, defense);

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

  public Tuple<bool, float> criticalMod_Gen1(IMonster caster, IMove move, BattleModel model)
  {
    /**
    Damage
    When a move lands a critical hit, the attacker's level will be doubled during damage calculation. A rough
    approximation for the damage multiplier is 2L+5L+5 where L is the attacker's level; as a result, lower-leveled
    Pokémon have a smaller critical hit boost than higher-leveled Pokémon. For example, a level 5 Pokémon will inflict
    about 1.5× damage on a critical hit, while a level 20 Pokémon will inflict 1.8× and a level 95 Pokémon will inflict
    1.95×.

    Critical hits use the attacker and defender's original stats with no modifications. This means it is possible for a
    critical hit to deal less damage than a non-critical hit if the attacker's stat modifications were high enough and
    the defender's were low enough. If the player's Pokémon has transformed, its untransformed stats will be used (but
    enemies use the stats of their transformation).[1]
    
    Probability
    Whether a move scores a critical hit is determined by comparing a 1-byte random number (0 to 255) against a 1-byte
    threshold value (also 0 to 255). If the random number is strictly less than the threshold, the Pokémon scores a
    critical hit. For a given threshold value T, the probability of scoring a critical hit is T/256.

    If the threshold would exceed 255, it instead becomes 255. Consequently, the maximum possible chance of landing a
    critical hit is 255/256. (If the generated random number is 255, that number can never be less than the threshold,
    regardless of the value of the threshold.)
    
    Core series
    In the Generation I core series games, the threshold is normally equal to half the user's base Speed. If the move
    used has a high critical-hit ratio (Crabhammer, Karate Chop, Razor Leaf, or Slash), the move is 8 times more likely
    to be a critical hit (up to a maximum of 255/256). If the Pokémon has used Focus Energy or a Dire Hit, due to a bug,
    the user is 1/4 as likely to land a critical hit.

    The threshold has the following values, depending on the move used and whether the user is under the effect of Focus
    Energy or a Dire Hit. The probability of landing a critical hit is simply these values divided by 256.
    */

    // First, determine if the hit is a critical hit
    int threshold = math.clamp(caster.criticalHitThreshold, 0, 255);
    threshold = math.clamp(move.criticalMod_thresholdPipe(threshold), 0, 255);

    int random = NocabRNG.newRNG.generateInt(0, 255, true, true);
    bool isCritical = random < threshold;

    // If it's a critical hit, compute the approximate damage multiplier
    float multiplier = 1.0f;
    if (isCritical)
    {
      // TODO: Consider putting more complex crit rate logic here. Really, any more complex logic should
      // live either in the move itself, or the monster, or the battle model or something...
      multiplier = ((2.0f * caster.level) + 5.0f) / (caster.level + 5.0f);
    }

    // Return the multiplier
    return Tuple.Create(isCritical, multiplier);
  }

  public int CalculateDamag_blab(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model = null
  )
  {
    // The basic formula is:
    // ((attack / defense) * mod1 + 2) * mod2
    // There are several possible sources for mod1 and mod2 (in the form of pipes/ simple functions)
    // 1. Item
    // 2. Move
    // 3. Status
    // 4. Field
    // 5. Badge

    float standard_damage = standard_damage_gen2(caster, target, move, model);

    // Compute the mod1 pipeline
    float mod1 = 1f;
    mod1 = caster.hasItem ? caster.item.DmgMod1(caster, target, move, model) : mod1;
    mod1 = move.DmgMod1(caster, target, move, model);

    // Compute the mod2 pipeline
    float mod2 = 1f;
    mod2 = caster.hasItem ? caster.item.DmgMod2(caster, target, move, model) : mod2;
    mod2 = move.DmgMod2(caster, target, move, model);

    return -1; // TODO: Implement this
  }

  protected float standard_damage_gen2(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model
  )
  {
    // ((((2 × Level) / 5) + 2) × Power × A/D) / 50)
    int level = caster.level;
    int power = move.power;
    // TODO: How to deal with temporary stat modifiers here?
    int attack = move.moveMedium == EMoveMedium.Physical ? caster.Attack : caster.SpecialAttack;
    int defense = move.moveMedium == EMoveMedium.Physical ? target.Defense : target.SpecialDefense;
    defense = math.max(1, defense);

    return ((2 * level / 5) + 2) * power * (attack / defense) / 50;
  }
}
