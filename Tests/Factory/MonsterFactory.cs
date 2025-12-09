using System.Collections.Generic;

/// <summary>
/// Factory for creating pre-configured monsters for testing.
/// Provides consistent monster types with balanced stats for different roles.
/// </summary>
public static class MonsterFactory
{
  // ==========================================
  // BIRD MONSTERS (Player/Hero archetypes)
  // ==========================================

  /// <summary>
  /// Creates a Tank bird - high HP and Defense, moderate Attack
  /// </summary>
  public static IMonster CreateBirdTank(string nickname = "Tweety")
  {
    var monster = new BirdMon(
      speed: 50,
      maxHealth: 120,
      defense: 15,
      attack: 15,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Support bird - balanced stats, slightly lower HP
  /// </summary>
  public static IMonster CreateBirdSupport(string nickname = "Pidgey")
  {
    var monster = new BirdMon(speed: 45, maxHealth: 90, defense: 8, attack: 12, nickname: nickname);
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a DPS bird - high Attack, lower Defense
  /// </summary>
  public static IMonster CreateBirdDPS(string nickname = "Sparrow")
  {
    var monster = new BirdMon(speed: 55, maxHealth: 85, defense: 7, attack: 22, nickname: nickname);
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Balanced bird - all-around good stats
  /// </summary>
  public static IMonster CreateBirdBalanced(string nickname = "Eagle")
  {
    var monster = new BirdMon(
      speed: 60,
      maxHealth: 110,
      defense: 12,
      attack: 18,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Fast Striker bird - very high Speed, good Attack
  /// </summary>
  public static IMonster CreateBirdFast(string nickname = "Falcon")
  {
    var monster = new BirdMon(speed: 70, maxHealth: 85, defense: 8, attack: 20, nickname: nickname);
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Standard bird - basic stats for testing
  /// </summary>
  public static IMonster CreateBirdStandard(string nickname = "Hawk")
  {
    var monster = new BirdMon(
      speed: 58,
      maxHealth: 105,
      defense: 11,
      attack: 19,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  // ==========================================
  // CAT MONSTERS (Enemy/Boss archetypes)
  // ==========================================

  /// <summary>
  /// Creates a Heavy Hitter cat - high Attack and HP
  /// </summary>
  public static IMonster CreateCatHeavy(string nickname = "Whiskers")
  {
    var monster = new CatMon(
      speed: 40,
      maxHealth: 140,
      defense: 14,
      attack: 24,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Fast Attacker cat - high Speed, good Attack
  /// </summary>
  public static IMonster CreateCatFast(string nickname = "Shadow")
  {
    var monster = new CatMon(
      speed: 52,
      maxHealth: 100,
      defense: 10,
      attack: 20,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Boss cat - very high HP and Attack, lower Speed
  /// </summary>
  public static IMonster CreateCatBoss(string nickname = "Tiger")
  {
    var monster = new CatMon(
      speed: 35,
      maxHealth: 180,
      defense: 16,
      attack: 26,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Standard cat - balanced enemy stats
  /// </summary>
  public static IMonster CreateCatStandard(string nickname = "Fluffy")
  {
    var monster = new CatMon(
      speed: 38,
      maxHealth: 115,
      defense: 10,
      attack: 16,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Balanced cat - all-around good enemy stats
  /// </summary>
  public static IMonster CreateCatBalanced(string nickname = "Panther")
  {
    var monster = new CatMon(speed: 48, maxHealth: 105, defense: 9, attack: 21, nickname: nickname);
    AddBasicMoves(monster);
    return monster;
  }

  /// <summary>
  /// Creates a Defender cat - high Defense, moderate HP
  /// </summary>
  public static IMonster CreateCatDefender(string nickname = "Lynx")
  {
    var monster = new CatMon(
      speed: 43,
      maxHealth: 118,
      defense: 12,
      attack: 19,
      nickname: nickname
    );
    AddBasicMoves(monster);
    return monster;
  }

  // ==========================================
  // HELPER METHODS
  // ==========================================

  /// <summary>
  /// Adds a standard set of moves to a monster
  /// </summary>
  private static void AddBasicMoves(IMonster monster)
  {
    monster.Moves.Add(new BasicAttackMove());
    // Add SlickRainMove for variety on some monsters
    if (monster.Speed >= 55 || monster.Attack >= 20)
    {
      monster.Moves.Add(new SlickRainMove());
    }
  }

  // ==========================================
  // TEAM CREATION HELPERS
  // ==========================================

  /// <summary>
  /// Creates a standard 6-monster bird team (for Pokemon Red style)
  /// </summary>
  public static List<IMonster> CreateStandardBirdTeam()
  {
    return new List<IMonster>
    {
      CreateBirdTank("Tweety"),
      CreateBirdSupport("Pidgey"),
      CreateBirdDPS("Sparrow"),
      CreateBirdBalanced("Eagle"),
      CreateBirdFast("Falcon"),
      CreateBirdStandard("Hawk"),
    };
  }

  /// <summary>
  /// Creates a standard 6-monster cat team (for Pokemon Red style)
  /// </summary>
  public static List<IMonster> CreateStandardCatTeam()
  {
    return new List<IMonster>
    {
      CreateCatHeavy("Whiskers"),
      CreateCatStandard("Fluffy"),
      CreateCatFast("Shadow"),
      CreateCatBoss("Tiger"),
      CreateCatBalanced("Panther"),
      CreateCatDefender("Lynx"),
    };
  }

  /// <summary>
  /// Creates a 5-hero bird party (for Final Fantasy style)
  /// </summary>
  public static List<IMonster> CreateHeroParty()
  {
    return new List<IMonster>
    {
      CreateBirdTank("Tweety"),
      CreateBirdSupport("Pidgey"),
      CreateBirdDPS("Sparrow"),
      CreateBirdBalanced("Eagle"),
      CreateBirdFast("Falcon"),
    };
  }

  /// <summary>
  /// Creates a 3-enemy cat group (for Final Fantasy style)
  /// </summary>
  public static List<IMonster> CreateEnemyGroup()
  {
    return new List<IMonster>
    {
      CreateCatHeavy("Whiskers"),
      CreateCatFast("Shadow"),
      CreateCatBoss("Tiger"),
    };
  }

  /// <summary>
  /// Creates a 4-monster bird team (for Double Battles)
  /// </summary>
  public static List<IMonster> CreateDoubleBattleBirdTeam()
  {
    return new List<IMonster>
    {
      CreateBirdTank("Tweety"),
      CreateBirdSupport("Pidgey"),
      CreateBirdDPS("Sparrow"),
      CreateBirdBalanced("Eagle"),
    };
  }

  /// <summary>
  /// Creates a 4-monster cat team (for Double Battles)
  /// </summary>
  public static List<IMonster> CreateDoubleBattleCatTeam()
  {
    return new List<IMonster>
    {
      CreateCatHeavy("Whiskers"),
      CreateCatStandard("Fluffy"),
      CreateCatFast("Shadow"),
      CreateCatBoss("Tiger"),
    };
  }
}
