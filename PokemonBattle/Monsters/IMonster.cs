using System.Collections.Generic;

public interface IMonster
{

    // TODO: Redo these stats
    string ClassName { get; }
    string Nickname { get; set; }
    int Speed { get; set; }
    int Health { get; set; }
    int MaxHealth { get; }
    int Defense { get; set; }
    int SpecialDefense { get; set; }
    int Attack { get; set; }
    int SpecialAttack { get; set; }

    int level { get; set; }
    List<IMove> Moves { get; }

    PkmItem item { get; set; }
    bool hasItem { get { return this.item != null; } }

    BattleEffects BattleEffects{ get; set; }

    MonsterBattleType Types { get; }
}

