using System;

public class StruggleMove : IMove
{
  public string Name => "Struggle";
  public EBattleType type => EBattleType.Normal;
  public EMoveMedium moveMedium => EMoveMedium.Physical;

  public int power { get; set; } = 50;

  public MoveResult Execute(BattleManager battleManager, IMonster user, IMonster target)
  {
    // Struggle move is a special move that does damage to the user.
    // It is used when the user has no moves left or when the user is in an error state.
    int damage = CalculateDamage(user.Attack, target.Defense);
    int recoil = (int)(user.MaxHealth * 0.1);

    var result = new MoveResult();
    result.AddDamage(target, damage);
    result.AddDamage(user, recoil); // Recoil damage to the user
    return result;
  }

  private int CalculateDamage(int attack, int defense)
  {
    // Simple damage calculation
    return Math.Max(1, attack - defense);
  }
}