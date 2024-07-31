
public class SlickRainMove : IMove
{
    public string Name => "Slick Rain";
    public EBattleType type => EBattleType.Water;

    public void Execute(BattleManager battleManager, IMonster user, IMonster target)
    {
        foreach (var monster in battleManager.GetAllMonsters())
        {
            monster.Speed += 2; // Example speed increase
        }
    }
}
