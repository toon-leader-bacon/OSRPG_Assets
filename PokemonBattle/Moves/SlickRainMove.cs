public class SlickRainMove : IMove
{
  public string Name => "Slick Rain";
  public EBattleType type => EBattleType.Water;

  public int power { get; set; }

  public EMoveMedium moveMedium { get; set; }

  public MoveResult Execute(BattleManager battleManager, IMonster user, IMonster target)
  {
    var result = new MoveResult();

    // Increase speed of all monsters on the field
    foreach (var monster in battleManager.GetAllMonsters())
    {
      result.TargetEffects.Add(
        new TargetEffect
        {
          Target = monster,
          AttributeDeltas = new() { { EMonsterAttribute.Speed, 2 } },
        }
      );
    }

    return result;
  }
}
