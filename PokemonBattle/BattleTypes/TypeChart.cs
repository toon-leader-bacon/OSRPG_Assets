
using System.Collections.Generic;

public class TypeChart
{
  protected Dictionary<EBattleType, Dictionary<EBattleType, float>> chart = new();

  public TypeChart() {}

  public float getEffectiveness(EBattleType attackingType, EBattleType defendingType)
  {
    Dictionary<EBattleType, float> attackerRow = chart[attackingType];
    return attackerRow[defendingType];
  }
}

public class TypeChart_Gen1 : TypeChart{

  // public TypeChart_Gen1()  {
  //   this.buildChart()
  // }

  public void buildChart() {

  }

}