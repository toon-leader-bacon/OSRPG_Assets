using System;
using Unity.Mathematics;

public class Gen4DamageCalculator : BaseDamageCalculator
{
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
    }
    else if (model.currentWeather == BattleWeather.Sunny)
    {
      if (move.type == EBattleType.Fire)
      {
        return 1.5f;
      }
      else if (move.type == EBattleType.Water)
      {
        return 0.5f;
      }
    }

    return 1;
  }

  protected float gen4_mod1(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model,
    bool isCritical
  )
  {
    float brn = burnModifier(caster, move);
    float rl = reflect_lightScreen_modifier(move, model.GetBattleEffects(target), isCritical);
    float tvt = 1; // TODO: 2v2 modifier
    float sr = SunnyDay_RainDance_modifier(move, model);
    float ff = 1; // TODO: flash fire mod

    return brn * rl * tvt * sr * ff;
  }

  protected float gen4_mod2(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    float im = caster.hasItem ? caster.item.DmgMod2(caster, target, move, model) : 1;
    return im;
  }

  public override int CalculateDamage(
    IMonster caster,
    IMonster target,
    IMove move,
    BattleModel model = null
  )
  {
    var ch = Gen4_CriticalHit(caster, move);
    bool isCritical = ch.Item1;
    float criticalHit = ch.Item2;

    float level = caster.level;
    float power = CalculateMovePower(caster, target, move, model);
    float attack = CalculateAtk(caster, target, move, model);
    float defense = CalculateDef(caster, target, move, model);

    float mod1 = gen4_mod1(caster, target, move, model, isCritical);
    float mod2 = gen4_mod2(caster, target, move, model);
    float random = NocabRNG.newRNG.generateInt(85, 100, true, true) / 100;

    float stab = calculate_STAB(caster.Types, move.type);
    float type1 = calculate_typeEffective(target.Types.type1, move.type, model.typeChart);
    float type2 = calculate_typeEffective(target.Types.type2, move.type, model.typeChart);

    float result =
      ((((level * 2 / 5) + 2) * power * attack / 50 / defense * mod1) + 2)
      * criticalHit
      * mod2
      * random
      * stab
      * type1
      * type2;
    return (int)Math.Max(result, 1);
  }
}
