public class PkmItem
{
  public virtual float BasePowerMod(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float AtkMod(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float DefMod(IMonster itemHolder, IMove move, BattleModel model)
  {
    return 1;
  }
  public virtual float SpecialAtkMod(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float SpecialDefMod(IMonster itemHolder, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float DmgMod2(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float DmgMod3(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }
}

public class BustedItem : PkmItem
{
  public override float BasePowerMod(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    // Adamant Orb	1.2 if the user is Dialga and the move used is Steel- or Dragon-type, 1 otherwise.
    if (caster.ClassName == "Dialga" && (move.type == EBattleType.Dragon || move.type == EBattleType.Steel))
    {
      return 1.2f;
    }
    return 1;
  }

  public override float AtkMod(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    // Thick Club	2 if the user is Cubone or Marowak, 1 otherwise.
    if (caster.ClassName == "Cubone" || caster.ClassName == "Marowak")
    {
      return 2;
    }
    return 1;
  }

  public override float DefMod(IMonster itemHolder, IMove move, BattleModel model)
  {
    // Defense is weird because the opponent's item is what needs to be pulled in
    // So the itemHolder is usually the defending pokemon, ie the target. 

    // Metal Powder	1.5 if the foe is Ditto, is holding the item Metal Powder and has not used the move Transform.
    if (itemHolder.ClassName == "Ditto")
    {
      return 1.5f;
    }
    return 2;
  }

  public override float SpecialAtkMod(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    //Light Ball	2 if the user is Pikachu, 1 otherwise.
    if (caster.ClassName == "Pikachu")
    {
      return 2;
    }
    return 1;
  }

  public override float SpecialDefMod(IMonster itemHolder, IMove move, BattleModel model)
  {
    // Defense is weird because the opponent's item is what needs to be pulled in
    // So the itemHolder is usually the defending pokemon, ie the target. 

    // Metal Powder	1.5 if the foe is Ditto, is holding the item Metal Powder and has not used the move Transform.
    if (itemHolder.ClassName == "Ditto")
    {
      return 1.5f;
    }
    return 2;
  }

  public override float DmgMod2(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    // 1.3 if the user is holding the item Life Orb.
    return 1.3f;
  }

  public override float DmgMod3(IMonster caster, IMonster target, IMove move, BattleModel model)
  {
    // EB is 1.2 if the user is holding the item Expert Belt and the move used is super effective against the foe, and 1 otherwise.
    if (model.typeChart.GetEffectiveness(move.type, target) >= 2.0f)
    {
      return 1.2f;
    }
    return 1;
  }
}