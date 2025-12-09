using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test script demonstrating ATB system with multiple active monsters per team (2v2).
/// Each monster has its own ATB gauge that fills independently based on Speed.
/// </summary>
public class Test_ATBPartyBattle : MonoBehaviour
{
  void Start()
  {
    Debug.Log("=== ATB Party Battle Test (2v2) Start ===");
    RunATBPartyBattle();
    Debug.Log("=== ATB Party Battle Test Complete ===");
  }

  private void RunATBPartyBattle()
  {
    // Create player team: 2 active monsters with different speeds
    var playerMonsters = new List<IMonster>
    {
      MonsterFactory.CreateBirdFast("Speedy"), // Speed: 120 (acts fastest)
      MonsterFactory.CreateBirdBalanced("Balanced"), // Speed: 80
    };

    var playerTeam = new BattleTeam(
      playerMonsters,
      new BattleAI_Random(),
      activeCount: 2, // Both active
      teamId: BattleModel.PLAYER_TEAM_ID
    );

    // Create computer team: 2 active monsters with different speeds
    var computerMonsters = new List<IMonster>
    {
      MonsterFactory.CreateBirdTank("Tank"), // Speed: 60 (acts slowest)
      MonsterFactory.CreateBirdBalanced("Balanced2"), // Speed: 80
    };

    var computerTeam = new BattleTeam(
      computerMonsters,
      new BattleAI_Random(),
      activeCount: 2, // Both active
      teamId: BattleModel.COMPUTER_TEAM_ID
    );

    // Create battle model
    var battleModel = new BattleModel(playerTeam, computerTeam);

    // Create ATB conductor
    var atbConductor = new ATBConductor { IsWaitMode = true };

    Debug.Log("Player Team:");
    Debug.Log($"  - {playerMonsters[0].Nickname} (Speed: {playerMonsters[0].Speed})");
    Debug.Log($"  - {playerMonsters[1].Nickname} (Speed: {playerMonsters[1].Speed})");
    Debug.Log("Computer Team:");
    Debug.Log($"  - {computerMonsters[0].Nickname} (Speed: {computerMonsters[0].Speed})");
    Debug.Log($"  - {computerMonsters[1].Nickname} (Speed: {computerMonsters[1].Speed})");
    Debug.Log("Expected: Speedy acts first (120), then Balanced/Balanced2 (80), then Tank (60)");
    Debug.Log("");

    // Create battle manager with ATB conductor
    var battleManager = new BattleManager(battleModel, atbConductor);

    // Start the battle
    battleManager.StartBattle();
  }
}
