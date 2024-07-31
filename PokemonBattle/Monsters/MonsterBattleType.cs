public readonly struct MonsterBattleType
{
  public readonly EBattleType type1;
  public readonly EBattleType type2;

  public MonsterBattleType(EBattleType type1, EBattleType type2)
  {
    this.type1 = type1;
    this.type2 = type2;
  }
  public MonsterBattleType(EBattleType type1) : this(type1, EBattleType.None) { }
}