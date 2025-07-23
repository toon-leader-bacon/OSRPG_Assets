
using System;
using System.Collections.Generic;

public class TypeChart_PokemonGen : TypeChart
{

  public static TypeChart_PokemonGen buildGen1Chart()
  {
    TypeChart_PokemonGen result = new();
    result.buildChart_gen1();
    return result;
  }

  public static TypeChart_PokemonGen buildGen2Chart()
  {
    TypeChart_PokemonGen result = new();
    result.buildChart_gen2_5();
    return result;
  }

  public static TypeChart_PokemonGen buildGen3Chart()
  {
    return buildGen2Chart();
  }

  public static TypeChart_PokemonGen buildGen4Chart()
  {
    return buildGen2Chart();
  }

  public static TypeChart_PokemonGen buildGen5Chart()
  {
    return buildGen2Chart();
  }

  public static TypeChart_PokemonGen buildGen6Chart()
  {
    TypeChart_PokemonGen result = new();
    result.buildChart_gen6();
    return result;
  }

  protected void buildChart_gen1()
  {
    // Based on this: https://bulbapedia.bulbagarden.net/wiki/Type/Type_chart
    List<Tuple<EBattleType, EBattleType, float>> atkDefEffectiveness = new()
    {
        // Attacker = Normal
        new(EBattleType.Normal, EBattleType.Rock, 0.5f),
        new(EBattleType.Normal, EBattleType.Ghost, 0f),

        // Attacker = Fighting
        new(EBattleType.Fighting, EBattleType.Normal, 2f),
        new(EBattleType.Fighting, EBattleType.Flying, 0.5f),
        new(EBattleType.Fighting, EBattleType.Poison, 0.5f),
        new(EBattleType.Fighting, EBattleType.Rock, 2f),
        new(EBattleType.Fighting, EBattleType.Bug, 0.5f),
        new(EBattleType.Fighting, EBattleType.Ghost, 0f),
        new(EBattleType.Fighting, EBattleType.Psychic, 0.5f),
        new(EBattleType.Fighting, EBattleType.Ice, 2f),

        // Attacker = Flying
        new(EBattleType.Flying, EBattleType.Fighting, 2f),
        new(EBattleType.Flying, EBattleType.Rock, 0.5f),
        new(EBattleType.Flying, EBattleType.Bug, 0.5f),
        new(EBattleType.Flying, EBattleType.Grass, 2f),
        new(EBattleType.Flying, EBattleType.Electric, 0.5f),

        // Attacker = Poison
        new(EBattleType.Poison, EBattleType.Poison, 0.5f),
        new(EBattleType.Poison, EBattleType.Ground, 0.5f),
        new(EBattleType.Poison, EBattleType.Rock, 0.5f),
        new(EBattleType.Poison, EBattleType.Bug, 2f),
        new(EBattleType.Poison, EBattleType.Ghost, 0.5f),
        new(EBattleType.Poison, EBattleType.Grass, 2f),
        new(EBattleType.Poison, EBattleType.Electric, 0.5f),

        // Attacker = Ground
        new(EBattleType.Ground, EBattleType.Flying, 0f),
        new(EBattleType.Ground, EBattleType.Poison, 2f),
        new(EBattleType.Ground, EBattleType.Rock, 2f),
        new(EBattleType.Ground, EBattleType.Bug, 0.5f),
        new(EBattleType.Ground, EBattleType.Fire, 2f),
        new(EBattleType.Ground, EBattleType.Grass, 0.5f),
        new(EBattleType.Ground, EBattleType.Electric, 2f),

        // Attacker = Rock
        new(EBattleType.Rock, EBattleType.Fighting, 0.5f),
        new(EBattleType.Rock, EBattleType.Flying, 2f),
        new(EBattleType.Rock, EBattleType.Ground, 0.5f),
        new(EBattleType.Rock, EBattleType.Bug, 2f),
        new(EBattleType.Rock, EBattleType.Fire, 2f),
        new(EBattleType.Rock, EBattleType.Ice, 2f),

        // Attacker = Bug
        new(EBattleType.Bug, EBattleType.Fighting, 0.5f),
        new(EBattleType.Bug, EBattleType.Flying, 0.5f),
        new(EBattleType.Bug, EBattleType.Poison, 2f),
        new(EBattleType.Bug, EBattleType.Ghost, 0.5f),
        new(EBattleType.Bug, EBattleType.Fire, 0.5f),
        new(EBattleType.Bug, EBattleType.Grass, 2f),
        new(EBattleType.Bug, EBattleType.Psychic, 2f),

        // Attacker = Ghost
        new(EBattleType.Ghost, EBattleType.Normal, 0f),
        new(EBattleType.Ghost, EBattleType.Ghost, 2f),
        new(EBattleType.Ghost, EBattleType.Psychic, 0f),

        // Attacker = Fire
        new(EBattleType.Fire, EBattleType.Rock, 0.5f),
        new(EBattleType.Fire, EBattleType.Bug, 2f),
        new(EBattleType.Fire, EBattleType.Fire, 0.5f),
        new(EBattleType.Fire, EBattleType.Water, 0.5f),
        new(EBattleType.Fire, EBattleType.Grass, 2f),
        new(EBattleType.Fire, EBattleType.Ice, 2f),
        new(EBattleType.Fire, EBattleType.Dragon, 0.5f),

        // Attacker = Water
        new(EBattleType.Water, EBattleType.Ground, 2f),
        new(EBattleType.Water, EBattleType.Rock, 2f),
        new(EBattleType.Water, EBattleType.Fire, 2f),
        new(EBattleType.Water, EBattleType.Water, 0.5f),
        new(EBattleType.Water, EBattleType.Dragon, 0.5f),

        // Attacker = Grass
        new(EBattleType.Grass, EBattleType.Flying, 0.5f),
        new(EBattleType.Grass, EBattleType.Poison, 0.5f),
        new(EBattleType.Grass, EBattleType.Ground, 2f),
        new(EBattleType.Grass, EBattleType.Rock, 2f),
        new(EBattleType.Grass, EBattleType.Bug, 0.5f),
        new(EBattleType.Grass, EBattleType.Fire, 0.5f),
        new(EBattleType.Grass, EBattleType.Water, 2f),
        new(EBattleType.Grass, EBattleType.Grass, 0.5f),
        new(EBattleType.Grass, EBattleType.Dragon, 0.5f),

        // Attacker = Electric
        new(EBattleType.Electric, EBattleType.Flying, 2f),
        new(EBattleType.Electric, EBattleType.Ground, 0f),
        new(EBattleType.Electric, EBattleType.Water, 2f),
        new(EBattleType.Electric, EBattleType.Grass, 0.5f),
        new(EBattleType.Electric, EBattleType.Electric, 0.5f),
        new(EBattleType.Electric, EBattleType.Dragon, 0.5f),

        // Attacker = Psychic
        new(EBattleType.Psychic, EBattleType.Fighting, 2f),
        new(EBattleType.Psychic, EBattleType.Poison, 2f),
        new(EBattleType.Psychic, EBattleType.Psychic, 0.5f),

        // Attacker = Ice
        new(EBattleType.Ice, EBattleType.Flying, 2f),
        new(EBattleType.Ice, EBattleType.Ground, 2f),
        new(EBattleType.Ice, EBattleType.Water, 0.5f),
        new(EBattleType.Ice, EBattleType.Grass, 2f),
        new(EBattleType.Ice, EBattleType.Ice, 0.5f),
        new(EBattleType.Ice, EBattleType.Dragon, 0.5f),

        // Attacker = Dragon
        new(EBattleType.Dragon, EBattleType.Dragon, 2f),
    };
    applyUpdates(atkDefEffectiveness);
  }

  protected void buildChart_gen2_5()
  {
    buildChart_gen1();
    // Based on this: https://bulbapedia.bulbagarden.net/wiki/Type/Type_chart
    List<Tuple<EBattleType, EBattleType, float>> atkDefEffectiveness = new()
    {
        // Changes from Gen 1
        new(EBattleType.Bug, EBattleType.Poison, 0.5f),
        new(EBattleType.Poison, EBattleType.Bug, 1f),
        new(EBattleType.Ghost, EBattleType.Psychic, 2f),
        new(EBattleType.Ice, EBattleType.Fire, 0.5f),

        // Attacker = Dark
        new(EBattleType.Dark, EBattleType.Fighting, 0.5f),
        new(EBattleType.Dark, EBattleType.Ghost, 2f),
        new(EBattleType.Dark, EBattleType.Steel, 0.5f),
        new(EBattleType.Dark, EBattleType.Psychic, 2f),
        new(EBattleType.Dark, EBattleType.Dark, 0.5f),

        // Attacker = Steel
        new(EBattleType.Steel, EBattleType.Rock, 2f),
        new(EBattleType.Steel, EBattleType.Steel, 0.5f),
        new(EBattleType.Steel, EBattleType.Fire, 0.5f),
        new(EBattleType.Steel, EBattleType.Water, 0.5f),
        new(EBattleType.Steel, EBattleType.Electric, 0.5f),
        new(EBattleType.Steel, EBattleType.Ice, 2f),

        // Defender = Dark
        new(EBattleType.Fighting, EBattleType.Dark, 2f),
        new(EBattleType.Bug, EBattleType.Dark, 2f),
        new(EBattleType.Ghost, EBattleType.Dark, 0.5f),
        new(EBattleType.Psychic, EBattleType.Dark, 0.5f),
        // new(EBattleType.Dark, EBattleType.Dark, 0.5f),

        // Defender = Steel
        new(EBattleType.Normal, EBattleType.Steel, 0.5f),
        new(EBattleType.Fighting, EBattleType.Steel, 2f),
        new(EBattleType.Flying, EBattleType.Steel, 0.5f),
        new(EBattleType.Poison, EBattleType.Steel, 0f),
        new(EBattleType.Ground, EBattleType.Steel, 2f),
        new(EBattleType.Rock, EBattleType.Steel, 0.5f),
        new(EBattleType.Bug, EBattleType.Steel, 0.5f),
        new(EBattleType.Ghost, EBattleType.Steel, 0.5f),
        // new(EBattleType.Steel, EBattleType.Steel, 0.5f),
        new(EBattleType.Fire, EBattleType.Steel, 2f),
        new(EBattleType.Grass, EBattleType.Steel, 0.5f),
        new(EBattleType.Psychic, EBattleType.Steel, 0.5f),
        new(EBattleType.Ice, EBattleType.Steel, 0.5f),
        new(EBattleType.Dragon, EBattleType.Steel, 0.5f),
        new(EBattleType.Dark, EBattleType.Steel, 0.5f),
    };
    applyUpdates(atkDefEffectiveness);
  }


  protected void buildChart_gen6()
  {
    buildChart_gen2_5();
    // Based on this: https://bulbapedia.bulbagarden.net/wiki/Type/Type_chart
    List<Tuple<EBattleType, EBattleType, float>> atkDefEffectiveness = new()
    {
        // Changes from Previous gen
        new(EBattleType.Ghost, EBattleType.Steel, 1f),
        new(EBattleType.Dark, EBattleType.Steel, 1f),

        // Attacker = Fairy
        new(EBattleType.Fairy, EBattleType.Fighting, 2f),
        new(EBattleType.Fairy, EBattleType.Poison, 0.5f),
        new(EBattleType.Fairy, EBattleType.Fire, 0.5f),
        new(EBattleType.Fairy, EBattleType.Dragon, 2f),
        new(EBattleType.Fairy, EBattleType.Dark, 2f),

        // Defender = Fairy
        new(EBattleType.Fighting, EBattleType.Fairy, 0.5f),
        new(EBattleType.Poison, EBattleType.Fairy, 2f),
        new(EBattleType.Bug, EBattleType.Fairy, 0.5f),
        new(EBattleType.Steel, EBattleType.Fairy, 2f),
        new(EBattleType.Dragon, EBattleType.Fairy, 0f),
        new(EBattleType.Dark, EBattleType.Fairy, 0.5f),
    };
    applyUpdates(atkDefEffectiveness);
  }

  protected void applyUpdates(List<Tuple<EBattleType, EBattleType, float>> updates)
  {
    foreach (var elem in updates)
    {
      EBattleType attackType = elem.Item1;
      EBattleType defenseType = elem.Item2;
      float effectiveness = elem.Item3;

      this.AddMapping(attackType, defenseType, effectiveness);
    }
  }

}