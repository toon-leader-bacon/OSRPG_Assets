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

  #region ATB System

  // ATB system support - only used when ATBConductor is active
  public AtbTimeline atbTimeline = new AtbTimeline();

  #endregion ATB System

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
    // Check if monster is in player team (active or reserve)
    if (playerTeam.AllMonsters.Contains(mon))
    {
      return playerSideEffects;
    }

    // Check if monster is in computer team (active or reserve)
    if (computerTeam.AllMonsters.Contains(mon))
    {
      return computerSideEffects;
    }

    // Fallback - assume computer side if not found
    UnityEngine.Debug.LogWarning(
      $"Monster {mon.Nickname} not found in either team, defaulting to computer side effects"
    );
    return computerSideEffects;
  }
}
