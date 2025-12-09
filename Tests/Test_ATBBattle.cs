using UnityEngine;

/// <summary>
/// Test script demonstrating the ATB (Active Time Battle) system.
/// Creates a simple 1v1 battle where gauges fill based on Speed stats.
/// AI controls both sides for demonstration purposes.
/// </summary>
public class Test_ATBBattle : MonoBehaviour
{
  void Start()
  {
    Debug.Log("=== ATB Battle Test Start ===");
    RunATBBattle();
    Debug.Log("=== ATB Battle Test Complete ===");
  }

  private void RunATBBattle()
  {
    // Create player team with a fast monster
    var playerMonster = MonsterFactory.CreateBirdFast("Tweety"); // Speed: 120
    var playerTeam = new BattleTeam(
      playerMonster,
      new BattleAI_Random(),
      BattleModel.PLAYER_TEAM_ID
    );

    // Create computer team with a slower tank
    var computerMonster = MonsterFactory.CreateBirdTank("Tank"); // Speed: 60
    var computerTeam = new BattleTeam(
      computerMonster,
      new BattleAI_Random(),
      BattleModel.COMPUTER_TEAM_ID
    );

    // Create battle model
    var battleModel = new BattleModel(playerTeam, computerTeam);

    // Create ATB conductor with Wait Mode enabled
    var atbConductor = new ATBConductor { IsWaitMode = true };

    Debug.Log($"Player: {playerMonster.Nickname} (Speed: {playerMonster.Speed})");
    Debug.Log($"Computer: {computerMonster.Nickname} (Speed: {computerMonster.Speed})");
    Debug.Log("Expected: Player should act twice as fast (120 speed vs 60 speed)");
    Debug.Log("");

    // Create battle manager with ATB conductor
    var battleManager = new BattleManager(battleModel, atbConductor);

    // Start the battle
    battleManager.StartBattle();
  }
}
