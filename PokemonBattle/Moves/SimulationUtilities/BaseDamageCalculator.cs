using System;
using Unity.Mathematics;

public abstract class BaseDamageCalculator
{
  /// <summary>
  /// Calculates Same Type Attack Bonus (STAB) multiplier based on if the move type matches either of the monster's types.
  /// Returns either 1.0 (no match) or 1.5 (type match).
  /// </summary>
  protected float calculate_STAB(MonsterBattleType monType, EBattleType moveType)
  {
    if (moveType == EBattleType.None)
    {
      return 1;
    }
    bool anyMatch = (monType.type1 == moveType) || (monType.type2 == moveType);
    return anyMatch ? 1.5f : 1.0f;
  }

  /// <summary>
  /// Calculates type effectiveness multiplier against a monster with two possible types.
  /// Returns a value between 0.0 (no effect) to 4.0 (double super effective).
  /// </summary>
  protected float calculate_typeEffective(
    MonsterBattleType targetTypes,
    EBattleType moveType,
    TypeChart typeChart
  )
  {
    if (moveType == EBattleType.None)
    {
      return 1;
    }

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

  /// <summary>
  /// Calculates type effectiveness multiplier against a single type target.
  /// Returns a value of 0.0 (no effect), 0.5 (not very effective), 1.0 (normal), or 2.0 (super effective).
  /// </summary>
  protected float calculate_typeEffective(
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

  /// <summary>
  /// Calculates the final power of a move after applying all relevant battle modifiers (items, abilities, field effects).
  /// Returns a value typically between 0.0 and 200.0, though some combinations can exceed this range.
  /// </summary>
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

  /// <summary>
  /// Calculates the final attack stat after applying all relevant modifiers based on the move's medium (Physical/Special).
  /// Returns a value typically between 1.0 and 999.0 depending on base stats and modifiers.
  /// </summary>
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

  /// <summary>
  /// Calculates the final defense stat after applying all relevant modifiers based on the move's medium (Physical/Special).
  /// Returns a value typically between 1.0 and 999.0 depending on base stats and modifiers.
  /// </summary>
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
      EMoveMedium.Physical => target.hasItem ? target.item.DefMod(target, move, model) : 1,
      EMoveMedium.Special => target.hasItem ? target.item.SpecialDefMod(target, move, model) : 1,
      _ => 1,
    };
    float mod = im; // From items/ abilities ect.
    float sx = 1; // self destruct or explosion modi (0.5)

    return stat * sm * mod * sx;
  }

  /// <summary>
  /// Main damage calculation method that each generation must implement with its specific formula.
  /// Returns an integer damage value of at least 1, with upper bound varying by generation and modifiers.
  /// </summary>
  public abstract int CalculateDamage(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model = null
  );
}
