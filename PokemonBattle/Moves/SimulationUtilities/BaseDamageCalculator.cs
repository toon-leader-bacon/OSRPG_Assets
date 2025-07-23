using System;
using Unity.Mathematics;

public abstract class BaseDamageCalculator
{
  protected float calculate_STAB(MonsterBattleType monType, EBattleType moveType)
  {
    if (moveType == EBattleType.None)
    {
      return 1;
    }
    bool anyMatch = (monType.type1 == moveType) || (monType.type2 == moveType);
    return anyMatch ? 1.5f : 1.0f;
  }

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
      EMoveMedium.Physical => target.hasItem ? target.item.DefMod(target, move, model) : 1,
      EMoveMedium.Special => target.hasItem ? target.item.SpecialDefMod(target, move, model) : 1,
      _ => 1,
    };
    float mod = im; // From items/ abilities ect.
    float sx = 1; // self destruct or explosion modi (0.5)

    return stat * sm * mod * sx;
  }

  // Abstract method that each generation must implement
  public abstract int CalculateDamage(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model = null
  );
}
