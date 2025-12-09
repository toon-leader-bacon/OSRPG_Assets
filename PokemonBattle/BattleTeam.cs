using System;
using System.Collections.Generic;
using System.Linq;

public class BattleTeam
{
  public int TeamId { get; private set; }
  public IBattleAI BattleAI { get; private set; }

  // Multi-monster support
  private List<IMonster> allMonsters;
  private List<int> activeMonsterIndices; // Indices of active monsters (size = activeCount)
  private int activeCount; // How many monsters are simultaneously active

  /// <summary>
  /// Gets the currently active monster in battle (for single-active battles).
  /// For multi-active battles, this returns the first active monster.
  /// Use GetActiveMonsters() for multi-active battles.
  /// </summary>
  public IMonster ActiveMonster
  {
    get { return allMonsters[activeMonsterIndices[0]]; }
  }

  /// <summary>
  /// Gets all monsters on this team (active + reserves)
  /// </summary>
  public List<IMonster> AllMonsters
  {
    get { return allMonsters; }
  }

  /// <summary>
  /// Gets the index of the currently active monster (for single-active battles).
  /// For multi-active battles, this returns the first active index.
  /// </summary>
  public int ActiveMonsterIndex
  {
    get { return activeMonsterIndices[0]; }
  }

  /// <summary>
  /// Gets the number of monsters that are simultaneously active in battle
  /// </summary>
  public int ActiveCount
  {
    get { return activeCount; }
  }

  /// <summary>
  /// Gets all currently active monsters in battle
  /// </summary>
  public List<IMonster> GetActiveMonsters()
  {
    return activeMonsterIndices.Select(index => allMonsters[index]).ToList();
  }

  /// <summary>
  /// Gets the indices of all currently active monsters
  /// </summary>
  public List<int> GetActiveIndices()
  {
    return new List<int>(activeMonsterIndices);
  }

  /// <summary>
  /// Constructor for single-monster team (backward compatibility)
  /// </summary>
  public BattleTeam(IMonster monster, IBattleAI battleAI, int teamId = -1)
    : this(new List<IMonster> { monster }, battleAI, activeCount: 1, teamId) { }

  /// <summary>
  /// Constructor for multi-monster team with single active (6v6 with 1 active)
  /// </summary>
  public BattleTeam(List<IMonster> monsters, IBattleAI battleAI, int teamId = -1)
    : this(monsters, battleAI, activeCount: 1, teamId) { }

  /// <summary>
  /// Constructor for multi-monster team with configurable active count
  /// </summary>
  public BattleTeam(List<IMonster> monsters, IBattleAI battleAI, int activeCount, int teamId = -1)
  {
    if (monsters == null || monsters.Count == 0)
    {
      throw new ArgumentException("Team must have at least one monster");
    }

    if (activeCount < 1 || activeCount > monsters.Count)
    {
      throw new ArgumentException(
        $"activeCount ({activeCount}) must be between 1 and team size ({monsters.Count})"
      );
    }

    TeamId = teamId;
    this.allMonsters = new List<IMonster>(monsters);
    this.activeCount = activeCount;
    this.activeMonsterIndices = new List<int>();

    // Initialize with first N monsters as active
    for (int i = 0; i < activeCount; i++)
    {
      this.activeMonsterIndices.Add(i);
    }

    BattleAI = battleAI;
  }

  public IMove GetNextMove(BattleManager battleManager, IMonster opposingMonster)
  {
    return BattleAI.GetMove(battleManager, ActiveMonster, opposingMonster);
  }

  /// <summary>
  /// Gets all reserve monsters (non-active monsters on the team)
  /// </summary>
  public List<IMonster> GetReserveMonsters()
  {
    return allMonsters.Where((mon, index) => !activeMonsterIndices.Contains(index)).ToList();
  }

  /// <summary>
  /// Gets all conscious (non-fainted) reserve monsters that can be switched in
  /// </summary>
  public List<IMonster> GetAvailableReserves()
  {
    return allMonsters
      .Where((mon, index) => !activeMonsterIndices.Contains(index) && mon.Health > 0)
      .ToList();
  }

  /// <summary>
  /// Switches the active monster to a different one by index (for single-active battles).
  /// For multi-active battles, this switches the first active slot.
  /// Use SwitchActiveMonster(slot, newIndex) for multi-active battles.
  /// </summary>
  public void SwitchActiveMonster(int newIndex)
  {
    SwitchActiveMonster(0, newIndex);
  }

  /// <summary>
  /// Switches a specific active slot to a different monster
  /// </summary>
  /// <param name="slot">Which active slot to switch (0 to activeCount-1)</param>
  /// <param name="newIndex">Index in AllMonsters of the monster to switch in</param>
  public void SwitchActiveMonster(int slot, int newIndex)
  {
    if (slot < 0 || slot >= activeCount)
    {
      throw new ArgumentException($"Invalid slot: {slot} (activeCount = {activeCount})");
    }

    if (newIndex < 0 || newIndex >= allMonsters.Count)
    {
      throw new ArgumentException($"Invalid monster index: {newIndex}");
    }

    if (activeMonsterIndices.Contains(newIndex))
    {
      throw new ArgumentException("Cannot switch to already active monster");
    }

    if (allMonsters[newIndex].Health <= 0)
    {
      throw new ArgumentException("Cannot switch to fainted monster");
    }

    activeMonsterIndices[slot] = newIndex;
  }

  /// <summary>
  /// Checks if the entire team is defeated (all monsters fainted)
  /// </summary>
  public bool IsDefeated()
  {
    return allMonsters.All(mon => mon.Health <= 0);
  }

  /// <summary>
  /// Gets the count of conscious (non-fainted) monsters remaining
  /// </summary>
  public int GetRemainingMonsterCount()
  {
    return allMonsters.Count(mon => mon.Health > 0);
  }

  /// <summary>
  /// Checks if a forced switch is needed (any active monster fainted but team not defeated)
  /// </summary>
  public bool NeedsForcedSwitch()
  {
    // Check if any active monster has fainted
    bool hasFaintedActive = GetActiveMonsters().Any(mon => mon.Health <= 0);
    return hasFaintedActive && !IsDefeated();
  }

  /// <summary>
  /// Gets the slot index of the first fainted active monster, or -1 if none fainted
  /// </summary>
  public int GetFaintedActiveSlot()
  {
    for (int slot = 0; slot < activeCount; slot++)
    {
      if (allMonsters[activeMonsterIndices[slot]].Health <= 0)
      {
        return slot;
      }
    }
    return -1;
  }

  public int _SetTeamID(int newTeamId)
  {
    // SHOULD ONLY be used by the Battle Manger
    int oldTeamId = this.TeamId;
    this.TeamId = newTeamId;
    return oldTeamId;
  }

  public static implicit operator BattleTeam(BattleWeather v)
  {
    throw new NotImplementedException();
  }
}
