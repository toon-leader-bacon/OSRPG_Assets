# ATB (Active Time Battle) System

This directory contains the implementation of the Active Time Battle system, inspired by Final Fantasy IV-IX.

## Overview

The ATB system replaces traditional turn-based combat with real-time gauge-filling mechanics. Each combatant has an individual gauge that fills continuously based on their Speed stat. When a gauge reaches 100%, that combatant can act.

## Core Components

### AtbTimeline.cs (Data Model)

**AtbGaugeState** - Pure state data for a single combatant's ATB gauge:

- `CurrentCharge` (0-100): Current gauge fill level
- `Phase`: Current state (Charging, Ready, AwaitingInput, Casting)
- **Note**: No references to monsters, no computed behavior - just data

**AtbTimeline** - Container managing all gauge states, attached to `BattleModel`

- Maps monsters to their gauge state
- Pure model data - can be serialized, saved, or transferred

### ATBConductor.cs (Flow Controller)

Implements `ITimeDrivenConductor` to provide ATB battle flow:

- `Tick(deltaTime)`: Advances all gauges based on elapsed time
- `GetNextAction()`: Returns queued actions from ready combatants
- `SubmitPlayerAction()`: Receives player input and queues action
- `OnMonsterSwitchedIn/Out()`: Handles gauge state during switches (required by IBattleConductor)

### ITimeDrivenConductor.cs (Interface)

Optional interface for conductors requiring time advancement. Extends `IBattleConductor` with `Tick(deltaTime)` method.

## Battle Flow

### Initialization

1. `ATBConductor.Initialize()` called by BattleManager
2. All monsters get fresh gauges at 0%, Phase = Charging
3. Charge rates calculated from Speed stats

### Main Loop (BattleManager.StartTimeDrivenBattle)

```
while (!battle over):
    1. Tick(deltaTime) - advance all gauges
    2. GetNextAction() - check for ready actions
    3. If action ready:
        - ExecuteAction()
        - CheckAndHandleForcedSwitches()
    4. Repeat
```

### Gauge Filling

- **Charging Phase**: Gauge fills at `ChargeRate * deltaTime` per tick
- **Reaches 100%**: `OnGaugeFilled()` triggered
  - **AI monsters**: Action requested immediately, gauge resets to 0%
  - **Player monsters**: Phase = AwaitingInput, gauge pauses

### Wait Mode vs Active Mode

- **Wait Mode** (default): Time freezes for ALL combatants when any player monster awaits input
- **Active Mode**: Time continues even during player input (more challenging)

### Player Input Flow

1. Gauge reaches 100% → Phase = AwaitingInput
2. UI system detects awaiting monsters via `GetMonstersAwaitingInput()`
3. Player selects action in battle menu
4. UI calls `SubmitPlayerAction(monster, action)`
5. Action queued, gauge resets to 0%, time resumes

### Forced Switches

When a monster faints:

1. `CheckAndHandleForcedSwitches()` detects fainted active
2. `OnMonsterSwitchedOut(oldMonster)` called - clears awaiting input, preserves gauge charge
3. Reserve monster selected
4. `OnMonsterSwitchedIn(newMonster)` called - resets gauge to 0%

## Speed & Timing Math

**Charge Rate Formula**: `ChargeRate = Max(Speed stat, 1)`

- Calculated by ATBConductor each Tick (not cached)
- Automatically reflects Speed buffs/debuffs with no manual sync

**Time to Fill**: `100 / ChargeRate` seconds

Examples:

- Speed 100: Fills in 1.0 second
- Speed 50: Fills in 2.0 seconds  
- Speed 200: Fills in 0.5 seconds

**Speed Advantages**:

- 2x Speed = 2x action frequency
- 120 Speed vs 60 Speed → Player acts twice for every enemy action
- Speed buffs immediately affect next Tick (no manual sync needed)

## Integration with BattleManager

BattleManager auto-detects time-driven conductors:

```csharp
if (conductor is ITimeDrivenConductor)
{
    StartTimeDrivenBattle(); // Uses Tick() loop
}
else
{
    StartTurnBasedBattle(); // Synchronous loop
}
```

## Usage Examples

### Basic 1v1 ATB Battle

```csharp
var playerTeam = new BattleTeam(fastMonster, new BattleAI_Random());
var computerTeam = new BattleTeam(slowMonster, new BattleAI_Random());
var battleModel = new BattleModel(playerTeam, computerTeam);

var atbConductor = new ATBConductor { IsWaitMode = true };
var battleManager = new BattleManager(battleModel, atbConductor);

battleManager.StartBattle();
```

### 2v2 ATB Party Battle

```csharp
var playerMonsters = new List<IMonster> { monster1, monster2 };
var playerTeam = new BattleTeam(playerMonsters, ai, activeCount: 2);

var computerMonsters = new List<IMonster> { monster3, monster4 };
var computerTeam = new BattleTeam(computerMonsters, ai, activeCount: 2);

var battleModel = new BattleModel(playerTeam, computerTeam);
var atbConductor = new ATBConductor { IsWaitMode = true };
var battleManager = new BattleManager(battleModel, atbConductor);

battleManager.StartBattle();
```

## Test Scripts

- **Test_ATBBattle.cs** - Simple 1v1 demonstration
- **Test_ATBPartyBattle.cs** - 2v2 multi-active demonstration

## Current Limitations & Future Work

### Implemented ✓

- Individual gauge filling per monster
- Speed-based charge rates
- Wait Mode / Active Mode
- AI auto-action queuing
- Forced switch handling
- Multi-active monster support (2v2, 3v3, etc.)
- Speed stat change handling

### Not Yet Implemented

- **Player Input Integration**: Currently both sides use AI. Need UI system to call `SubmitPlayerAction()`
- **Cast Time**: Gauges could support "casting" phase where actions take time to execute
- **Pause/Resume**: Global battle pause (for menus, cutscenes)
- **Haste/Slow Effects**: Status effects that modify ChargeRate multiplier
- **Visual Gauge UI**: Display gauge bars for all combatants
- **Action Priority**: Some actions could execute at different gauge thresholds (90% vs 100%)

### Design Decisions

**Why preserve gauge charge on switch-out?**

- Allows strategic switching without penalty
- Alternative: Reset to 0 (uncomment line in `OnMonsterSwitchedOut`)

**Why use 0-100 gauge instead of 0-1?**

- More intuitive for designers and UI
- Easier to think about "50% charged" than "0.5 charged"

**Why Sleep in StartTimeDrivenBattle?**

- Simulation loop runs headless without Unity's Update()
- In real game, this would be a MonoBehaviour calling Tick() in Update()
- Sleep prevents CPU pegging in test scripts

## Architecture Notes

**Separation of Concerns**:

- `AtbTimeline` = pure data (gauges, state)
- `ATBConductor` = flow logic (when to act, who goes next)
- `BattleManager` = execution (damage, effects, switches)
- UI layer (future) = input, visualization

**Why extend IBattleConductor?**

- Turn-based conductors don't need Tick()
- ITimeDrivenConductor is opt-in for real-time systems
- Allows conductor pattern to support both paradigms cleanly

**Thread Safety**:

- Current implementation is single-threaded
- If moving to async/coroutines, gauges would need locking
