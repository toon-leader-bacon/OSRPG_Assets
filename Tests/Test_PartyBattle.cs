using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EXAMPLE: Final Fantasy style party battle
///
/// Battle Configuration:
/// - Player: 5 heroes (all active, no reserves)
/// - Computer: 3 enemies (all active, no reserves)
/// - Turn-based with simultaneous action selection
/// - Speed determines action order across ALL active monsters (8 total actions per round)
/// - Battle ends when one team is fully defeated (no switching)
///
/// How to Use:
/// 1. Create two teams with List<IMonster> and specify activeCount (5 for heroes, 3 for enemies)
/// 2. Create BattleModel with both teams
/// 3. Use PartyBattleConductor for multi-active battles
/// 4. Create BattleManager and call StartBattle()
/// </summary>
public class Test_PartyBattle : MonoBehaviour
{
  void Start()
  {
    Debug.Log("========================================");
    Debug.Log("FINAL FANTASY PARTY BATTLE TEST");
    Debug.Log("5 heroes vs 3 enemies (all active)");
    Debug.Log("========================================\n");

    // ==========================================
    // STEP 1: Create Player Team (5 heroes, all active)
    // ==========================================
    List<IMonster> playerMonsters = MonsterFactory.CreateHeroParty();
    IBattleAI playerAi = new BattleAI_Random();

    // activeCount: 5 means all 5 heroes are active (typical FF party)
    BattleTeam playerTeam = new BattleTeam(playerMonsters, playerAi, activeCount: 5);

    // ==========================================
    // STEP 2: Create Computer Team (3 enemies, all active)
    // ==========================================
    List<IMonster> computerMonsters = MonsterFactory.CreateEnemyGroup();
    IBattleAI computerAi = new BattleAI_Random();

    // activeCount: 3 means all 3 enemies are active
    BattleTeam computerTeam = new BattleTeam(computerMonsters, computerAi, activeCount: 3);

    // ==========================================
    // STEP 3: Create Battle Model
    // ==========================================
    BattleModel battleModel = new BattleModel(playerTeam: playerTeam, computerTeam: computerTeam);

    // ==========================================
    // STEP 4: Choose Conductor
    // ==========================================
    // PartyBattleConductor = Multi-active party battles
    // - All active monsters select moves simultaneously
    // - Actions execute in speed order across ALL combatants
    // - Round ends after all active monsters have acted
    var conductor = new PartyBattleConductor();

    // ==========================================
    // STEP 5: Create Battle Manager and Start
    // ==========================================
    var battleManager = new BattleManager(battleModel, conductor);

    // Display initial battle state (with detailed stats for FF-style battles)
    BattleTestUtils.LogBattleSetupDetailed(playerTeam, computerTeam);

    // Start the battle!
    battleManager.StartBattle();
  }

  void Update() { }
}
