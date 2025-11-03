using System;
using Unity.Mathematics;

/**
 * This is a class that contains the logic for the damage calculation of various pokemon generations.
 * This is mostly a static utility class that contains the logic for the damage calculation of the move.
 */
public class IMove_Blab
{
  public int pkmGen1(MonsterBattleType casterTypes, MonsterBattleType targetTypes, IMove move)
  {
    int level = 10;
    int critical = NocabRNG.newRNG.generateInt(1, 2, true, true);
    int power = 100;
    int attack = 50;
    int defense = 70;
    float stab = calculate_STAB(casterTypes, move.type);
    float type = calculate_typeEffective(
      targetTypes,
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

  public float calculate_STAB(MonsterBattleType monType, EBattleType moveType)
  {
    if (moveType == EBattleType.None)
    {
      return 1;
    }
    bool anyMatch = (monType.type1 == moveType) || (monType.type2 == moveType);
    return anyMatch ? 1.5f : 1.0f;
  }

  public float calculate_typeEffective(
    MonsterBattleType targetTypes,
    EBattleType moveType,
    TypeChart typeChart
  )
  {
    if (moveType == EBattleType.None)
    {
      return 1;
    }

    // TypeChart typeChart = TypeChart_PokemonGen.buildGen1Chart();
    float result = 1;
    if (targetTypes.type1 != EBattleType.None)
    {
      result *= typeChart.GetEffectiveness(moveType, targetTypes.type1);
    }
    if (targetTypes.type2 != EBattleType.None)
    {
      result *= typeChart.GetEffectiveness(moveType, targetTypes.type2);
    }
    return result;
  }

  public float calculate_typeEffective(
    EBattleType targetType,
    EBattleType moveType,
    TypeChart typeChart
  )
  {
    if (moveType == EBattleType.None)
    {
      return 1;
    }
    return typeChart.GetEffectiveness(moveType, targetType);
  }

  public int pkmGen2(IMonster caster, IMonster target, IMove move)
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

  public int pkmGen3(IMonster caster, IMonster target, IMove move)
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

  public int pkmGen4(IMonster caster, IMonster target, IMove move)
  {
    /**
       (2 * level)                attack
      (----------- + 2) * power * -------
            5                     defense
     (----------------------------------- + (Burn * screen * targets * weather * ff + 2)) * Critical * item * first * random * stab * type * srf * eb * TL * berry
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

    int critical = 1; // Or 2

    float item = 1; // Some items have an ability
    float first = 1; // Some moves set this to 1.5
    float random = NocabRNG.newRNG.generateInt(85, 100, true, true) / 100.0f;
    float stab = calculate_STAB(caster.Types, move.type);
    float type = calculate_typeEffective(
      target.Types,
      move.type,
      TypeChart_PokemonGen.buildGen4Chart()
    );

    float srf = 1; // supper effective vs Pokemon attributes affect this
    float eb = 1; // supper effective vs item
    float tl = 1; // not very effective vs item
    float berry = 1; // Some berries change this

    float a = 2 * level;
    float b = (a / 5) + 2;
    float c = b * power * (attack / defense);
    float d = c / 50;
    float result =
      d
      * (burn * screen * targets * weather * ff + 2)
      * critical
      * item
      * first
      * random
      * stab
      * type
      * srf
      * eb
      * tl
      * berry;
    return Math.Max(1, (int)result);
  }

  protected float CalculateMovePower(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model
  )
  {
    float hh = 1; // 1.5 if Helping Hand boost
    float bp = move.power; // Base Power. Some moves have variable power
    float it = caster.hasItem ? caster.item.BasePowerMod(caster, target, move, model) : 1; // Item multiplier
    float chg = 1; // 2 if pervious move was charge & electric
    float ms = 1; // sometimes 0.5 if mud sport and electric
    float ws = 1; // sometimes 0.5 if water sport and fire
    float ua = 1; // user ability.
    float fa = 1; // foe ability
    return hh * bp * it * chg * ms * ws * ua * fa;
  }

  protected float CalculateAtk(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    float stat = move.moveMedium switch
    {
      EMoveMedium.Physical => caster.Attack,
      EMoveMedium.Special => caster.SpecialAttack,
      _ => 1,
    };

    float sm = 1; // Stat modifier
    float am = 1; // ability modifier
    float im = move.moveMedium switch
    {
      EMoveMedium.Physical => caster.hasItem ? caster.item.AtkMod(caster, target, move, model) : 1,
      EMoveMedium.Special => caster.hasItem
        ? caster.item.SpecialAtkMod(caster, target, move, model)
        : 1,
      _ => 1,
    };

    return stat * sm * am * im;
  }

  protected float CalculateDef(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    float stat = move.moveMedium switch
    {
      EMoveMedium.Physical => target.Defense,
      EMoveMedium.Special => target.SpecialDefense,
      _ => 1,
    };

    float sm = 1; // Stat modifier
    float im = move.moveMedium switch
    {
      // Remember: We consider the _targets_ item here, not the caster's item.
      EMoveMedium.Physical => target.hasItem ? target.item.DefMod(target, move, model) : 1,
      EMoveMedium.Special => target.hasItem ? target.item.SpecialDefMod(target, move, model) : 1,
      _ => 1,
    };
    float mod = im; // From items/ abilities ect.
    float sx = 1; // self destruct or explosion modi (0.5)

    return stat * sm * mod * sx;
  }

  protected Tuple<bool, float> Gen4_CriticalHit(IMonster attacker, IMove move)
  {
    // TODO: Implement critical hit thresholds based on attacker, items, move and possible stage modifiers
    float stageThreshold = 0.0625f; // 6.25%
    bool isCritical = NocabRNG.defaultRNG.unitFloat <= stageThreshold;
    float criticalMultiplier = isCritical ? 1 : 2;
    return new(false, 1);
  }

  protected float burnModifier(IMonster monster, IMove move)
  {
    // BRN is 0.5 if:
    //  - the move performed is physical,
    //  - the user is affected by the burn special condition
    //  - and the user's ability is not Guts, and 1 otherwise.
    // Inverting the conditional allows for faster returns for the majority of cases.

    if (!monster.BattleEffects.ContainsTag("Burn"))
    {
      return 1;
    }
    if (move.moveMedium != EMoveMedium.Physical)
    {
      return 1;
    }

    // TODO: Pokemon abilities here
    return 0.5f;
  }

  protected float reflect_lightScreen_modifier(
    IMove move,
    BattleEffects defendingSideEffects,
    bool isCritical
  )
  {
    /**
        RL is:
        0.5 if the move performed is physical, the foe has setup a Reflect and the game is 1vs1.
        0.5 if the move performed is special, the foe has setup a Light Screen and the game is 1vs1.
        2/3 if the move performed is physical, the foe has setup a Reflect and the game is 2v2.
        2/3 if the move performed is special, the foe has setup a Light Screen and the game is 2v2.
        1 otherwise.
        Also, if the move is a critical hit, RL is made equal to 1 no matter what.
     */
    // Exit early, most moves will not have these defensive effects up
    bool reflectUp = defendingSideEffects.ContainsTag(ESideEffect.Reflect.ToEffectString());
    bool lightScreenUp = defendingSideEffects.ContainsTag(ESideEffect.LightScreen.ToEffectString());
    if ((!reflectUp && !lightScreenUp) || isCritical)
    {
      return 1;
    }

    if (reflectUp && move.moveMedium == EMoveMedium.Physical)
    {
      return 0.5f;
    }
    if (lightScreenUp && move.moveMedium == EMoveMedium.Special)
    {
      return 0.5f;
    }

    return 1;
  }

  protected float SunnyDay_RainDance_modifier(IMove move, BattleModel model)
  {
    /*
        1.5 if Sunny Day is in effect and the move is of Fire-type.
        1.5 if Rain Dance is in effect and the move is of Water-type.
        0.5 if Sunny Day is in effect and the move is of Water-type.
        0.5 if Rain Dance is in effect and the move is of Fire-type.
     */
    // Common case first
    if (model.currentWeather == BattleWeather.None)
    {
      return 1;
    }

    if (model.currentWeather == BattleWeather.Rainy)
    {
      if (move.type == EBattleType.Water)
      {
        return 1.5f;
      }
      else if (move.type == EBattleType.Fire)
      {
        return 0.5f;
      }
      else
      {
        return 1;
      }
    }
    if (model.currentWeather == BattleWeather.Sunny)
    {
      if (move.type == EBattleType.Fire)
      {
        return 1.5f;
      }
      else if (move.type == EBattleType.Water)
      {
        return 0.5f;
      }
      else
      {
        return 1;
      }
    }

    return 1;
  }

  protected float gen4_2_mod1(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model,
    bool isCritical
  )
  {
    // Attacker Status
    float brn = burnModifier(caster, move); // Sometimes 0.5
    // Defender field modifier
    float rl = reflect_lightScreen_modifier(move, model.GetBattleEffects(target), isCritical);
    // Battle Style modifier
    float tvt = 1; // TODO: 2v2 modifier
    // Battle weather modifier
    float sr = SunnyDay_RainDance_modifier(move, model); // Sunny/ Rainy Day mod
    // Attacker ability modifier
    float ff = 1; // TODO: flash fire mod

    // Other modifiers to consider:
    // Defender status/ ability
    // Attacker field modifier

    return brn * rl * tvt * sr * ff;
  }

  protected float gen4_2_mod2(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    // Special cases depending on previous moves/ items/ abilities
    float im = caster.hasItem ? caster.item.DmgMod2(caster, target, move, model) : 1;
    return im;
  }

  protected float gen4_2_mod3(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    float srf = 1; // solid rock/ filter
    float im = caster.hasItem ? caster.item.DmgMod3(caster, target, move, model) : 1;
    float tl = 1; // tinted lense mod
    float trb = 1; // type resiting berry mod

    return srf * im * tl * trb;
  }

  public int pkmGen4_2(IMonster caster, IMonster target, IMove move, BattleModel bm)
  {
    // Based on this: https://www.smogon.com/dp/articles/damage_formula
    var ch = Gen4_CriticalHit(caster, move);
    bool isCritical = ch.Item1;
    float criticalHit = ch.Item2; // Typically 1, sometimes 2, rarely 3

    float level = caster.level;
    float power = CalculateMovePower(caster, target, move, bm);
    float attack = CalculateAtk(caster, target, move, bm);
    float defense = CalculateDef(caster, target, move, bm);

    float mod1 = gen4_2_mod1(caster, target, move, bm, isCritical);
    float mod2 = gen4_2_mod2(caster, target, move, bm);
    float random = NocabRNG.newRNG.generateInt(85, 100, true, true);

    float stab = calculate_STAB(caster.Types, move.type);
    float type1 = calculate_typeEffective(target.Types.type1, move.type, bm.typeChart);
    float type2 = calculate_typeEffective(target.Types.type2, move.type, bm.typeChart);
    float mod3 = gen4_2_mod3(caster, target, move, bm);

    float result =
      ((((level * 2 / 5) + 2) * power * attack / 50 / defense * mod1) + 2)
      * criticalHit
      * mod2
      * random
      / 100
      * stab
      * type1
      * type2
      * mod3;
    return (int)Math.Max(result, 1);
  }

  public int pkmGen5(IMonster caster, IMonster target, IMove move)
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

  public int pkmGo(IMonster caster, IMonster target, IMove move)
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
