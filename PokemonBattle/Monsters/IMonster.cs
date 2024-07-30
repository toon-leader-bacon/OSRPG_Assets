using System.Collections.Generic;

public interface IMonster
{
    string ClassName { get; }
    string Nickname { get; set; }
    int Speed { get; set; }
    int Health { get; set; }
    int MaxHealth { get; }
    int Defense { get; set; }
    int Attack { get; set; }
    List<IMove> Moves { get; }
}

