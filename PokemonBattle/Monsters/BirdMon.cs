using System.Collections.Generic;

public class BirdMon : IMonster
{
    public const string CLASS_NAME = "BirdMon";
    public string ClassName => CLASS_NAME;
    public string Nickname { get; set; }
    public int Speed { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; }
    public int Defense { get; set; }
    public int Attack { get; set; }
    public List<IMove> Moves { get; }


    public BirdMon(int speed, int maxHealth, int defense, int attack, string nickname = CLASS_NAME)
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
