# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
SimHammer is a .NET-based combat simulation tool for tabletop wargaming (Warhammer 40k style). It simulates combat between units with different weapons, calculating hits, wounds, saves, and casualties across multiple combat rounds.

## Common Development Commands

### Build and Run
```bash
# Build entire solution
dotnet build SimHammer.sln

# Build specific project
dotnet build SimHammer.Core/SimHammer.Core.csproj
dotnet build SimHammer.Console/SimHammer.Console.csproj

# Run the console application
dotnet run --project SimHammer.Console

# Clean build artifacts
dotnet clean SimHammer.sln
```

### Testing
```bash
# Currently no test projects exist - tests would use:
dotnet test SimHammer.sln
```

## Architecture Overview

### Project Structure
- **SimHammer.Core** (.NET 8.0): Core business logic library containing models and services
- **SimHammer.Console** (.NET 9.0): Console application using Spectre.Console for UI

### Key Components

**Models:**
- `Unit`: Represents a combat unit with stats (Movement, Strength, Toughness, Wounds, Save, etc.) and weapon collections
- `MeleeWeapon` / `RangedWeapon`: Different weapon types with combat stats
- `CombatRound`: Results of a single combat round with weapon results and casualties
- `SimulationResult`: Complete simulation outcome across multiple rounds
- `WeaponResult`: Detailed results for individual weapon attacks (hits, wounds, saves, damage)

**Services:**
- `SimulationService` (implements `ISimulationService`): Core combat simulation logic
- `DiceRoller`: Static utility for dice rolling mechanics

### Combat Flow
1. `BeginSimulation()` orchestrates multiple combat rounds
2. Each round calls either `SimulateRangedCombatRound()` or `SimulateMeleeCombatRound()`
3. For each weapon in attacking unit:
   - Roll to hit (based on Ballistic Skill)
   - Roll to wound (Strength vs Toughness comparison)
   - Roll armor saves (Save vs Armor Piercing)
   - Calculate damage and models killed

### Dependencies
- **Microsoft.Data.Sqlite** (9.0.6): Database support in Core project
- **Spectre.Console** (0.50.0): Rich console UI in Console project

## Development Notes

### Combat Mechanics Implementation
The ranged combat simulation in `SimulationService.cs:42-147` implements Warhammer 40k-style mechanics:
- Hit rolls use Ballistic Skill with auto-hit on 6
- Wound rolls based on Strength vs Toughness ratios (2+, 3+, 4+, 5+, 6+)
- Armor saves consider weapon Armor Piercing values
- Melee combat simulation is incomplete (placeholder at `SimulationService.cs:149-164`)

### Current Limitations
- Melee combat simulation is not implemented
- No unit test coverage
- DiceRoller creates new Random instance per call (should use static/seeded instance)
- No database integration despite SQLite dependency
- Console UI has basic menu but limited simulation configuration options