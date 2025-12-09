using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test scene for party battles (3v3 with 3 active monsters per team)
/// </summary>
public class Blab_PartyBattle : MonoBehaviour
{
  void Start()
  {
    // Create player team with 6 monsters, 3 active at a time
    List<IMonster> playerMonsters = new List<IMonster>();

    var tweety = new BirdMon(
      speed: 50,
      maxHealth: 100,
      defense: 10,
      attack: 15,
      nickname: "Tweety"
    );
    tweety.Moves.Add(new BasicAttackMove());
    playerMonsters.Add(tweety);

    var pidgey = new BirdMon(speed: 45, maxHealth: 90, defense: 8, attack: 12, nickname: "Pidgey");
    pidgey.Moves.Add(new BasicAttackMove());
    playerMonsters.Add(pidgey);

    var sparrow = new BirdMon(
      speed: 55,
      maxHealth: 85,
      defense: 7,
      attack: 18,
      nickname: "Sparrow"
    );
    sparrow.Moves.Add(new BasicAttackMove());
    playerMonsters.Add(sparrow);

    var eagle = new BirdMon(speed: 60, maxHealth: 110, defense: 12, attack: 20, nickname: "Eagle");
    eagle.Moves.Add(new BasicAttackMove());
    playerMonsters.Add(eagle);

    var falcon = new BirdMon(speed: 65, maxHealth: 95, defense: 9, attack: 22, nickname: "Falcon");
    falcon.Moves.Add(new BasicAttackMove());
    playerMonsters.Add(falcon);

    var hawk = new BirdMon(speed: 58, maxHealth: 105, defense: 11, attack: 19, nickname: "Hawk");
    hawk.Moves.Add(new BasicAttackMove());
    playerMonsters.Add(hawk);

    IBattleAI playerAi = new BattleAI_Random();
    // Create team with 3 active monsters
    BattleTeam playerTeam = new(playerMonsters, playerAi, activeCount: 3);

    // Create computer team with 6 monsters, 3 active at a time
    List<IMonster> computerMonsters = new List<IMonster>();

    var whiskers = new CatMon(
      speed: 40,
      maxHealth: 120,
      defense: 12,
      attack: 18,
      nickname: "Whiskers"
    );
    whiskers.Moves.Add(new BasicAttackMove());
    computerMonsters.Add(whiskers);

    var fluffy = new CatMon(speed: 38, maxHealth: 115, defense: 10, attack: 16, nickname: "Fluffy");
    fluffy.Moves.Add(new BasicAttackMove());
    computerMonsters.Add(fluffy);

    var shadow = new CatMon(speed: 45, maxHealth: 110, defense: 11, attack: 20, nickname: "Shadow");
    shadow.Moves.Add(new BasicAttackMove());
    computerMonsters.Add(shadow);

    var tiger = new CatMon(speed: 42, maxHealth: 125, defense: 13, attack: 22, nickname: "Tiger");
    tiger.Moves.Add(new BasicAttackMove());
    computerMonsters.Add(tiger);

    var panther = new CatMon(
      speed: 48,
      maxHealth: 105,
      defense: 9,
      attack: 21,
      nickname: "Panther"
    );
    panther.Moves.Add(new BasicAttackMove());
    computerMonsters.Add(panther);

    var lynx = new CatMon(speed: 43, maxHealth: 118, defense: 12, attack: 19, nickname: "Lynx");
    lynx.Moves.Add(new BasicAttackMove());
    computerMonsters.Add(lynx);

    IBattleAI computerAi = new BattleAI_Random();
    // Create team with 3 active monsters
    BattleTeam computerTeam = new(computerMonsters, computerAi, activeCount: 3);

    Debug.Log("===== Starting 3v3 Party Battle (6 total monsters per team) =====");
    Debug.Log(
      $"Player Team: {playerTeam.AllMonsters.Count} total, {playerTeam.ActiveCount} active"
    );
    Debug.Log(
      $"Computer Team: {computerTeam.AllMonsters.Count} total, {computerTeam.ActiveCount} active"
    );
    Debug.Log("");
    Debug.Log("Player Active Monsters:");
    foreach (var mon in playerTeam.GetActiveMonsters())
    {
      Debug.Log($"  - {mon.Nickname} (HP: {mon.Health}, Speed: {mon.Speed})");
    }
    Debug.Log("Computer Active Monsters:");
    foreach (var mon in computerTeam.GetActiveMonsters())
    {
      Debug.Log($"  - {mon.Nickname} (HP: {mon.Health}, Speed: {mon.Speed})");
    }
    Debug.Log("");

    BattleModel bm = new BattleModel(playerTeam: playerTeam, computerTeam: computerTeam);

    // Use PartyBattleConductor instead of SimpleTurnConductor
    var conductor = new PartyBattleConductor();
    var battleManager = new BattleManager(bm, conductor);

    battleManager.StartBattle();
  }

  void Update() { }
}
