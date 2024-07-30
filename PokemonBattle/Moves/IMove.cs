public interface IMove
{
    string Name { get; }
    void Execute(BattleManager battleManager, IMonster user, IMonster target);
}
