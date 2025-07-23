using System.Collections.Generic;

public enum EBattleType
{
  // Gen 1
  Normal,
  Fire,
  Water,
  Electric,
  Grass,
  Ice,
  Fighting,
  Poison,
  Ground,
  Flying,
  Psychic,
  Bug,
  Rock,
  Ghost,
  Dragon,

  // Gen 2-5
  Dark,
  Steel,

  // Gen 6
  Fairy,

  // Misc:
  None
}

public static class EBattleType_Util
{
  public static readonly HashSet<EBattleType> GEN1_BATTLE_TYPES = new()
  {
    EBattleType.Normal,
    EBattleType.Fighting,
    EBattleType.Flying,
    EBattleType.Poison,
    EBattleType.Ground,

    EBattleType.Rock,
    EBattleType.Bug,
    EBattleType.Ghost,
    EBattleType.Fighting,
    EBattleType.Water,

    EBattleType.Grass,
    EBattleType.Electric,
    EBattleType.Psychic,
    EBattleType.Ice,
    EBattleType.Dragon
    };

  public static readonly HashSet<EBattleType> GEN2_5_BATTLE_TYPES = new()
  {
    EBattleType.Normal,
    EBattleType.Fighting,
    EBattleType.Flying,
    EBattleType.Poison,
    EBattleType.Ground,

    EBattleType.Rock,
    EBattleType.Bug,
    EBattleType.Ghost,
    EBattleType.Fighting,
    EBattleType.Water,

    EBattleType.Grass,
    EBattleType.Electric,
    EBattleType.Psychic,
    EBattleType.Ice,
    EBattleType.Dragon,

    EBattleType.Steel,
    EBattleType.Dark
  };

  public static readonly HashSet<EBattleType> GEN6_BATTLE_TYPES = new()
  {
    EBattleType.Normal,
    EBattleType.Fighting,
    EBattleType.Flying,
    EBattleType.Poison,
    EBattleType.Ground,

    EBattleType.Rock,
    EBattleType.Bug,
    EBattleType.Ghost,
    EBattleType.Fighting,
    EBattleType.Water,

    EBattleType.Grass,
    EBattleType.Electric,
    EBattleType.Psychic,
    EBattleType.Ice,
    EBattleType.Dragon,

    EBattleType.Steel,
    EBattleType.Dark,

    EBattleType.Fairy
  };

}

