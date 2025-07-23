
using System;
using System.Collections.Generic;

public class TypeChart
{
  private const float DEFAULT_EFFECTIVENESS = 1f;
  protected Dictionary<EBattleType, Dictionary<EBattleType, float>> chart = new();

  public TypeChart() { }

  public float GetEffectiveness(EBattleType attackingType, IMonster defendingMon) {
    float result = 1f;
    result *= this.GetEffectiveness(attackingType, defendingMon.Types.type1);
    result *= this.GetEffectiveness(attackingType, defendingMon.Types.type2);
    return result;
  }

  public float GetEffectiveness(EBattleType attackingType, EBattleType defendingType)
  {
    if (!chart.ContainsKey(attackingType)) { chart[attackingType] = buildRow(); }
    Dictionary<EBattleType, float> attackerRow = chart[attackingType];

    if (!attackerRow.ContainsKey(defendingType)) { attackerRow[defendingType] = DEFAULT_EFFECTIVENESS; }
    return attackerRow[defendingType];
  }


  protected void AddMapping(EBattleType attackingType, EBattleType defendingType, float effectiveness)
  {
    if (!chart.ContainsKey(attackingType)) { chart[attackingType] = buildRow(DEFAULT_EFFECTIVENESS); }

    Dictionary<EBattleType, float> row = chart[attackingType];
    row[defendingType] = effectiveness;
  }

  protected Dictionary<EBattleType, float> buildRow(float defaultValue = DEFAULT_EFFECTIVENESS)
  {
    return new()
    {
        { EBattleType.Normal, defaultValue },
        { EBattleType.Fighting, defaultValue },
        { EBattleType.Flying, defaultValue },
        { EBattleType.Poison, defaultValue },
        { EBattleType.Ground, defaultValue },

        { EBattleType.Rock, defaultValue },
        { EBattleType.Bug, defaultValue },
        { EBattleType.Ghost, defaultValue },
        { EBattleType.Fire, defaultValue },
        { EBattleType.Water, defaultValue },

        { EBattleType.Grass, defaultValue },
        { EBattleType.Electric, defaultValue },
        { EBattleType.Psychic, defaultValue },
        { EBattleType.Ice, defaultValue },
        { EBattleType.Dragon, defaultValue },
    };
  }
}
