public class PkmItem
{
  public virtual float BasePowerMod(
    IMonster itemHolder,
    IMonster target,
    IMove move,
    BattleModel model
  )
  {
    return 1;
  }

  public virtual float AtkMod(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float DefMod(IMonster itemHolder, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float SpecialAtkMod(
    IMonster itemHolder,
    IMonster target,
    IMove move,
    BattleModel model
  )
  {
    return 1;
  }

  public virtual float SpecialDefMod(IMonster itemHolder, IMove move, BattleModel model)
  {
    return 1;
  }

  /// <summary>
  /// The basic pokemon damage formula is:
  /// ((attack / defense) * Mod1 + 2) * Mod2
  ///
  /// This function DmgMod1 will add a pipe to the Mod1 part.
  /// </summary>
  /// <param name="itemHolder">The pokemon holding the item</param>
  /// <param name="target">The pokemon being targeted</param>
  /// <param name="move">The move being used</param>
  /// <param name="model">The battle model</param>
  public virtual float DmgMod1(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }

  /// <summary>
  /// The basic pokemon damage formula is:
  /// ((attack / defense) * Mod1 + 2) * Mod2
  ///
  /// This function DmgMod2 will add a pipe to the Mod2 part.
  /// </summary>
  /// <param name="itemHolder">The pokemon holding the item</param>
  /// <param name="target">The pokemon being targeted</param>
  /// <param name="move">The move being used</param>
  /// <param name="model">The battle model</param>
  public virtual float DmgMod2(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }

  public virtual float DmgMod3(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    return 1;
  }
}

public class BustedItem : PkmItem
{
  public override float BasePowerMod(
    IMonster itemHolder,
    IMonster target,
    IMove move,
    BattleModel model
  )
  {
    // Adamant Orb	1.2 if the user is Dialga and the move used is Steel- or Dragon-type, 1 otherwise.
    if (
      itemHolder.ClassName == "Dialga"
      && (move.type == EBattleType.Dragon || move.type == EBattleType.Steel)
    )
    {
      return 1.2f;
    }
    return 1;
  }

  public override float AtkMod(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    // Thick Club	2 if the user is Cubone or Marowak, 1 otherwise.
    if (itemHolder.ClassName == "Cubone" || itemHolder.ClassName == "Marowak")
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

  public override float SpecialAtkMod(
    IMonster itemHolder,
    IMonster target,
    IMove move,
    BattleModel model
  )
  {
    //Light Ball	2 if the user is Pikachu, 1 otherwise.
    if (itemHolder.ClassName == "Pikachu")
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

  public override float DmgMod2(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    // 1.3 if the user is holding the item Life Orb.
    return 1.3f;
  }
}

public class SilkScarf : PkmItem
{
  public override float DmgMod1(IMonster itemHolder, IMonster target, IMove move, BattleModel model)
  {
    // 1.1x if the move used is normal type.
    return move.type == EBattleType.Normal ? 1.1f : 1f;
  }
}
