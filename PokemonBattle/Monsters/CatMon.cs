
using System.Collections.Generic;

public class CatMon : IMonster
{
  public const string CLASS_NAME = "CatMon";
  public int level { get; set; }
  public string ClassName => CLASS_NAME;
  public string Nickname { get; set; }
  public int Speed { get; set; }
  public int Health { get; set; }
  public int MaxHealth { get; }
  public int Defense { get; set; }
  public int Attack { get; set; }
  public List<IMove> Moves { get; }
  public int SpecialDefense { get; set; }
  public int SpecialAttack { get; set; }

  private PkmItem _item = null;
  PkmItem IMonster.item { get { return _item; } set { _item = value; } }

  public MonsterBattleType Types => new(EBattleType.Fire);

  public BattleEffects BattleEffects { get => battleEffects; set => battleEffects = value; }
  private BattleEffects battleEffects = new();

  public CatMon(int speed, int maxHealth, int defense, int attack, string nickname = CLASS_NAME)
  {
    Nickname = nickname;
    Speed = speed;
    MaxHealth = maxHealth;
    Health = MaxHealth;
    Defense = defense;
    Attack = attack;
    Moves = new List<IMove>();
  }
}