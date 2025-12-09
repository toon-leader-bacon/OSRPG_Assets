using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EXAMPLE: Pokemon Double Battle
///
/// Battle Configuration:
/// - 4 monsters per team (typical double battle roster)
/// - 2 active monsters at a time
/// - Turn-based with simultaneous action selection
/// - Speed determines action order across all 4 active monsters
/// - Forced switches when any active monster faints
/// - Target selection matters (which of 2 enemies to attack)
///
/// How to Use:
/// 1. Create two teams with List<IMonster> and specify activeCount: 2
/// 2. Create BattleModel with both teams
/// 3. Use PartyBattleConductor (handles any N vs M configuration)
/// 4. Create BattleManager and call StartBattle()
///
/// Note: PartyBattleConductor works for 2v2, 3v3, 4v4, or any configuration!
/// </summary>
public class Test_DoubleBattle : MonoBehaviour
{
  void Start()
  {
    Debug.Log("========================================");
    Debug.Log("POKEMON DOUBLE BATTLE TEST");
    Debug.Log("4v4 roster, 2v2 active combat");
    Debug.Log("========================================\n");

    // ==========================================
    // STEP 1: Create Player Team (4 monsters, 2 active)
    // ==========================================
    List<IMonster> playerMonsters = MonsterFactory.CreateDoubleBattleBirdTeam();
    IBattleAI playerAi = new BattleAI_Random();

    // activeCount: 2 means 2 monsters active simultaneously (Double Battle)
    BattleTeam playerTeam = new BattleTeam(playerMonsters, playerAi, activeCount: 2);

    // ==========================================
    // STEP 2: Create Computer Team (4 monsters, 2 active)
    // ==========================================
    List<IMonster> computerMonsters = MonsterFactory.CreateDoubleBattleCatTeam();
    IBattleAI computerAi = new BattleAI_Random();

    // activeCount: 2 for Double Battles
    BattleTeam computerTeam = new BattleTeam(computerMonsters, computerAi, activeCount: 2);

    // ==========================================
    // STEP 3: Create Battle Model
    // ==========================================
    BattleModel battleModel = new BattleModel(playerTeam: playerTeam, computerTeam: computerTeam);

    // ==========================================
    // STEP 4: Choose Conductor
    // ==========================================
    // PartyBattleConductor handles any N vs M configuration
    // For 2v2, it:
    // - Collects 4 actions (2 per team)
    // - Sorts by speed across all 4 monsters
    // - Executes in speed order
    var conductor = new PartyBattleConductor();

    // ==========================================
    // STEP 5: Create Battle Manager and Start
    // ==========================================
    var battleManager = new BattleManager(battleModel, conductor);

    // Display initial battle state
    BattleTestUtils.LogBattleSetup(playerTeam, computerTeam);

    // Start the battle!
    battleManager.StartBattle();
  }

  void Update() { }
}
