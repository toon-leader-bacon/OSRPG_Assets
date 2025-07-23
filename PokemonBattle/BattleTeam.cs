using System;

public class BattleTeam
{
  public int TeamId { get; private set; }
  public IMonster ActiveMonster { get; private set; }
  public IBattleAI BattleAI { get; private set; }

  public BattleTeam(IMonster monster, IBattleAI battleAI, int teamId = -1)
  {
    TeamId = teamId;
    ActiveMonster = monster;
    BattleAI = battleAI;
  }

  public IMove GetNextMove(BattleManager battleManager, IMonster opposingMonster)
  {
    return BattleAI.GetMove(battleManager, ActiveMonster, opposingMonster);
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

  // AI generated:
  // TODO: Implement the following features and enhancements:
  // 1. Add a List<IMonster> to support multiple monsters per team
  // 2. Implement a method to check if the team is defeated (all monsters fainted)
  // 3. Add a method to select the next active monster for multi-monster battles
  // 4. Implement team-wide effects and a method to apply them
  // 5. Add a team name or identifier for better recognition
  // 6. Create a list of available items for future item use implementation
  // 7. Add a team strategy property (e.g., aggressive, defensive, balanced)
  // 8. Implement the Observer pattern with events for team status changes
  // 9. Add methods to receive and handle battle updates
  // 10. Consider creating an ITeam interface for different team implementations
  // 11. Implement a factory method for creating teams of different sizes/types
}
