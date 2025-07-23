using System;

public class BasicAttackMove : IMove
{
  public string Name => "Basic Attack";

  public EBattleType type => EBattleType.Normal;

  public int power { get; set; }

  public EMoveMedium moveMedium { get; set; }

  public void Execute(BattleManager battleManager, IMonster user, IMonster target)
  {
    int damage = CalculateDamage(user.Attack, target.Defense);
    target.Health -= damage;
  }

  private int CalculateDamage(int attack, int defense)
  {
    // Simple damage calculation
    return Math.Max(1, attack - defense);
  }
}
