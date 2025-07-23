# Pokemon Battle System - Developer Documentation

This directory contains a comprehensive Pokemon battle system implementation for Unity, focusing on recreating the mechanics from Pokemon Red/Blue (Generation 1). The system is designed with modularity and extensibility in mind, allowing for easy expansion to support multiple Pokemon generations and battle mechanics.

## Core Battle System

### BattleManager.cs

The central orchestrator of the battle system that manages turn execution, move resolution, and battle state. It handles speed-based turn order, executes moves between monsters, and determines battle outcomes. The manager operates on a BattleModel and coordinates between player and computer teams.

### BattleModel.cs

Data container that holds the current battle state including both teams, weather conditions, side effects, and the type chart. It provides utility methods to access battle effects for specific teams and monsters, serving as the single source of truth for battle information.

### BattleTeam.cs

Represents a team of monsters in battle, currently supporting single-monster teams with plans for multi-monster expansion. Each team has an active monster, battle AI, and team ID. The class includes TODO comments for future features like multiple monsters per team, team-wide effects, and item management.

## Monsters/

### IMonster.cs

Core interface defining monster properties including stats (HP, Attack, Defense, Speed, etc.), moves, items, battle effects, and type information. This interface serves as the foundation for all monster implementations in the battle system.

### CatMon.cs & BirdMon.cs

Concrete monster implementations providing example monster classes with different type combinations and stat distributions. These serve as templates for creating new monster types.

### MonsterBattleType.cs

Simple data structure representing a monster's type combination, supporting dual-typing with primary and secondary types.

### BattleEffects/

Contains BattleEffects.cs which manages status conditions, stat modifications, and other temporary effects that can be applied to monsters during battle. This system handles effects like paralysis, burn, stat boosts, and other battle modifiers.

## Moves/

### IMove.cs

Extensive interface and implementation containing the complete damage calculation system for multiple Pokemon generations (Gen 1-5 and Pokemon GO). Includes STAB (Same Type Attack Bonus) calculations, type effectiveness, critical hits, weather effects, and various damage modifiers. The file contains comprehensive damage formulas that accurately replicate the original games' mechanics.

### BasicAttackMove.cs

Simple move implementation that deals physical damage using the basic attack formula, serving as a template for creating new moves.

### SlickRainMove.cs

Example move implementation demonstrating how to create moves with special effects, in this case a weather-based move.

## BattleTypes/

### EBattleType.cs

Enumeration defining all Pokemon types from Generation 1 through Generation 6, including utility methods to filter types by generation. Supports the complete type system evolution across Pokemon generations.

### TypeChart.cs

Base type chart interface and implementation for calculating type effectiveness between different Pokemon types.

### TypeChart_PokemonGen.cs

Comprehensive type chart implementation containing the complete type effectiveness matrix for Generation 1 Pokemon, including all type matchups and their effectiveness multipliers.

### EMoveMedium.cs

Enumeration defining move categories (Physical, Special, Status) which affects how damage is calculated and which stats are used in battle.

## Items/

### PkmItem.cs

Base class for Pokemon items with virtual methods for modifying various battle calculations including base power, attack, defense, and damage modifiers. Includes BustedItem as an example implementation showing how to create items like Adamant Orb, Thick Club, and Life Orb with their specific effects.

## BattleAI/

### IBattleAi.cs

Interface defining the contract for battle AI implementations, requiring a GetMove method that determines which move an AI-controlled monster should use.

### BattleAI_Random.cs

Simple AI implementation that randomly selects moves from the monster's available move pool, serving as a basic AI template for testing and development.

## BattleWeather/

### BattleWeather.cs

Minimal implementation for weather effects in battle. Currently contains basic weather state management with plans for expansion to include weather-specific move modifications and effects.

## UI/

### BattleMenu.cs

Unity MonoBehaviour that handles battle menu navigation using arrow keys and enter for selection. Integrates with UINavigationGrid for button management.

### UINavigationGrid.cs

Grid-based navigation system for UI elements, supporting 2D navigation with wrap-around functionality. Handles keyboard input and visual indication of selected buttons.

## Blab_PokemonBattle.cs

Utility file containing miscellaneous battle-related functions and debugging tools for the Pokemon battle system.

---

## Architecture Notes

The system follows a modular design pattern with clear separation of concerns:

- **Data Layer**: BattleModel, IMonster, BattleTeam
- **Logic Layer**: BattleManager, damage calculations in IMove
- **AI Layer**: IBattleAI implementations
- **UI Layer**: BattleMenu and navigation components

The codebase includes extensive TODO comments indicating planned features like multi-monster teams, enhanced AI, and expanded item systems. The damage calculation system is particularly comprehensive, supporting multiple Pokemon generations with accurate formulas.
