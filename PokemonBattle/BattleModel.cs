public class BattleModel
{
  public const int PLAYER_TEAM_ID = 0;
  public const int COMPUTER_TEAM_ID = 0;

  public BattleTeam playerTeam;
  public BattleTeam computerTeam;
  public BattleWeather currentWeather = BattleWeather.None;

  public BattleEffects playerSideEffects = new();
  public BattleEffects computerSideEffects = new();

  public TypeChart typeChart = TypeChart_PokemonGen.buildGen1Chart();

  public BattleModel(BattleTeam playerTeam, BattleTeam computerTeam)
  {
    // NOTE Team IDs are set by the BattleManager before this constructor is called
    this.playerTeam = playerTeam;
    this.computerTeam = computerTeam;

    this.currentWeather = BattleWeather.None;
    this.typeChart = TypeChart_PokemonGen.buildGen1Chart();
  }

  public BattleEffects GetBattleEffects(BattleTeam team)
  {
    return team.TeamId == PLAYER_TEAM_ID ? playerSideEffects : computerSideEffects;
  }

  public BattleEffects GetBattleEffects(IMonster mon)
  {
    // TODO: I'm not sure this function really works.
    // I need a better way to connect IMonster instances to a Battle Team
    if (playerTeam.ActiveMonster == mon)
    {
      return playerSideEffects;
    }
    return computerSideEffects;
  }
}
