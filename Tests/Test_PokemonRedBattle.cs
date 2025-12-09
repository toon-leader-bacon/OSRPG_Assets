using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EXAMPLE: Pokemon Red/Blue style battle
///
/// Battle Configuration:
/// - 6 monsters per team (full Pokemon roster)
/// - 1 active monster at a time
/// - Turn-based with simultaneous move selection
/// - Speed determines action order within round
/// - Forced switches when active monster faints
///
/// How to Use:
/// 1. Create two teams with List<IMonster> and specify activeCount: 1
/// 2. Create BattleModel with both teams
/// 3. Use SimpleTurnConductor for Pokemon Red style turn order
/// 4. Create BattleManager and call StartBattle()
/// </summary>
public class Test_PokemonRedBattle : MonoBehaviour
{
  void Start()
  {
    Debug.Log("========================================");
    Debug.Log("POKEMON RED BATTLE TEST");
    Debug.Log("6v6 roster, 1v1 active combat");
    Debug.Log("========================================\n");

    // ==========================================
    // STEP 1: Create Player Team (6 monsters, 1 active)
    // ==========================================
    List<IMonster> playerMonsters = MonsterFactory.CreateStandardBirdTeam();
    IBattleAI playerAi = new BattleAI_Random();

    // activeCount: 1 means only 1 monster active at a time (Pokemon Red style)
    BattleTeam playerTeam = new BattleTeam(playerMonsters, playerAi, activeCount: 1);

    // ==========================================
    // STEP 2: Create Computer Team (6 monsters, 1 active)
    // ==========================================
    List<IMonster> computerMonsters = MonsterFactory.CreateStandardCatTeam();
    IBattleAI computerAi = new BattleAI_Random();

    // activeCount: 1 for single-active battles
    BattleTeam computerTeam = new BattleTeam(computerMonsters, computerAi, activeCount: 1);

    // ==========================================
    // STEP 3: Create Battle Model
    // ==========================================
    BattleModel battleModel = new BattleModel(playerTeam: playerTeam, computerTeam: computerTeam);

    // ==========================================
    // STEP 4: Choose Conductor
    // ==========================================
    // SimpleTurnConductor = Pokemon Red/Blue style
    // - Both teams select moves simultaneously
    // - Actions execute in speed order
    // - Round ends after both have acted
    var conductor = new SimpleTurnConductor();

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
