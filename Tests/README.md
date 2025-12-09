# Battle System Tests & Examples

This directory contains test/example files that demonstrate how to use the battle system for different battle styles.

## Test Files

### Test_PokemonRedBattle.cs

**Battle Style:** Pokemon Red/Blue Classic  
**Configuration:** 6v6 roster, 1v1 active combat  
**Conductor:** `SimpleTurnConductor`  

Demonstrates:

- Creating teams with 6 monsters each
- Setting `activeCount: 1` for single-active battles
- Using SimpleTurnConductor for turn-based flow
- Forced switches when active monster faints

**Key Code:**

```csharp
BattleTeam team = new BattleTeam(monsters, ai, activeCount: 1);
var conductor = new SimpleTurnConductor();
var battleManager = new BattleManager(battleModel, conductor);
```

---

### Test_DoubleBattle.cs

**Battle Style:** Pokemon Double Battles  
**Configuration:** 4v4 roster, 2v2 active combat  
**Conductor:** `PartyBattleConductor`  

Demonstrates:

- Creating teams with 4 monsters each
- Setting `activeCount: 2` for double battles
- Using PartyBattleConductor for multi-active combat
- Speed-based ordering across all 4 active monsters

**Key Code:**

```csharp
BattleTeam team = new BattleTeam(monsters, ai, activeCount: 2);
var conductor = new PartyBattleConductor();
var battleManager = new BattleManager(battleModel, conductor);
```

---

### Test_ATBBattle.cs

**Battle Style:** Active Time Battle (Final Fantasy IV-IX)  
**Configuration:** 1v1 combat with real-time gauge filling  
**Conductor:** `ATBConductor`  

Demonstrates:

- Creating ATB battles with gauge-based timing
- Speed stat determining action frequency (120 speed vs 60 speed)
- Wait Mode enabled (time freezes during player input)
- Individual gauge filling per combatant

**Key Code:**

```csharp
var atbConductor = new ATBConductor { IsWaitMode = true };
var battleManager = new BattleManager(battleModel, atbConductor);
battleManager.StartBattle(); // Auto-detects time-driven conductor
```

---

### Test_ATBPartyBattle.cs

**Battle Style:** ATB with Multiple Active Monsters  
**Configuration:** 2v2 active combat, each with independent gauges  
**Conductor:** `ATBConductor`  

Demonstrates:

- Multi-active ATB battles (2v2)
- Each monster has independent gauge filling based on Speed
- Actions execute as soon as individual gauges fill
- No synchronized rounds - continuous action flow

**Key Code:**

```csharp
BattleTeam team = new BattleTeam(monsters, ai, activeCount: 2);
var atbConductor = new ATBConductor { IsWaitMode = true };
var battleManager = new BattleManager(battleModel, atbConductor);
```

---

### Test_ATBVerbose.cs

**Battle Style:** ATB with Detailed Logging  
**Configuration:** 1v1 with gauge state visualization  
**Purpose:** Debugging and understanding ATB timing  

Demonstrates:

- Logging gauge percentages every 0.5 seconds
- Visual gauge bars (█████░░░░░)
- Tracking elapsed time and action counts
- Verifying charge rate calculations

Example Output:

```text
[TIME 0.50s] Gauge States:
  Flash       [████░░░░░░]  40.0% | Phase: Charging     | HP: 100/100
  Boulder     [██░░░░░░░░]  20.0% | Phase: Charging     | HP: 120/120
```

---

### Test_PartyBattle.cs

**Battle Style:** Dragon Quest / Final Fantasy Party  
**Configuration:** 6v6 roster, 3v3 active combat  
**Conductor:** `PartyBattleConductor`  

Demonstrates:

- Creating teams with 6 monsters each
- Setting `activeCount: 3` for party-style battles
- Using PartyBattleConductor for larger parties
- Multiple forced switches when multiple monsters faint

**Key Code:**

```csharp
BattleTeam team = new BattleTeam(monsters, ai, activeCount: 3);
var conductor = new PartyBattleConductor();
var battleManager = new BattleManager(battleModel, conductor);
```

---

## Factory Classes

To reduce code duplication, test utilities are provided in the `Factory/` subdirectory:

### MonsterFactory.cs

Pre-configured monster creation:

```csharp
// Individual monster creation
MonsterFactory.CreateBirdTank("Tweety")      // Tank role
MonsterFactory.CreateBirdDPS("Sparrow")      // DPS role
MonsterFactory.CreateCatBoss("Tiger")        // Boss enemy

// Team creation shortcuts
MonsterFactory.CreateStandardBirdTeam()      // 6 birds for Pokemon Red
MonsterFactory.CreateStandardCatTeam()       // 6 cats for Pokemon Red
MonsterFactory.CreateHeroParty()             // 5 heroes for FF
MonsterFactory.CreateEnemyGroup()            // 3 enemies for FF
MonsterFactory.CreateDoubleBattleBirdTeam()  // 4 birds for Double Battle
MonsterFactory.CreateDoubleBattleCatTeam()   // 4 cats for Double Battle
```

### BattleTestUtils.cs

Common testing utilities:

```csharp
// Log battle setup (standard)
BattleTestUtils.LogBattleSetup(playerTeam, computerTeam);

// Log battle setup (detailed stats)
BattleTestUtils.LogBattleSetupDetailed(playerTeam, computerTeam);
```

---

## How to Run Tests

1. **In Unity Editor:**
   - Attach one of the test scripts to a GameObject in your scene
   - Press Play
   - Watch the Console for battle output

2. **Change Active Count:**
   - Modify `activeCount` parameter to test different configurations
   - `activeCount: 1` = Single-active (Pokemon Red style)
   - `activeCount: 2` = Double battles
   - `activeCount: 3+` = Party battles

3. **Change Conductor:**
   - `SimpleTurnConductor`: For 1v1 battles (Pokemon Red)
   - `PartyBattleConductor`: For any N vs M battles (2v2, 3v3, 4v4, etc.)

---

## Battle System Components

### BattleTeam

Represents one team in battle.

- `List<IMonster>`: All monsters (active + reserves)
- `int activeCount`: How many monsters are simultaneously active
- `IBattleAI`: AI that selects actions

### IBattleConductor

Controls battle flow (turn order, timing, etc.)

- `SimpleTurnConductor`: Pokemon Red style (1v1)
- `PartyBattleConductor`: Multi-active battles (NvM)

### BattleManager

Executes actions and manages battle state.

- Delegates flow control to conductor
- Handles move execution and effects
- Manages forced switches

### BattleModel

Data container for battle state.

- Both teams
- Weather
- Field effects
- Side effects

---

## Creating Custom Battle Configurations

```csharp
// Example: 4v6 battle (4 heroes vs 6 monsters)
List<IMonster> heroes = CreateHeroes(4);
List<IMonster> monsters = CreateMonsters(6);

// All 4 heroes active, all 6 monsters active
BattleTeam heroTeam = new BattleTeam(heroes, playerAI, activeCount: 4);
BattleTeam monsterTeam = new BattleTeam(monsters, computerAI, activeCount: 6);

BattleModel model = new BattleModel(heroTeam, monsterTeam);
var conductor = new PartyBattleConductor();
var manager = new BattleManager(model, conductor);

manager.StartBattle();
```

---

## Future Battle Styles

Coming soon:

- **ATBConductor**: Active Time Battle (Final Fantasy 4-9)
- **TimelineConductor**: Turn order queue (Final Fantasy X)
- **PressTurnConductor**: Turn economy (Persona/SMT)

Each will use the same BattleTeam and BattleManager infrastructure!
