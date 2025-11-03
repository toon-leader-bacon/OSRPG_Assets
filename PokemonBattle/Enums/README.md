# Battle System Enums

This directory contains all type-safe enums used throughout the battle system, replacing string-based lookups with compile-time checked values.

## Unified Enum Pattern

All enums in this system follow a consistent pattern:

1. **Enum Definition** - Clear enum values
2. **String Mapping Dictionary** - Single source of truth for all string aliases
3. **Parse Methods** - Convert strings to enums (with validation)
4. **ToString Methods** - Convert enums to canonical strings
5. **Helper Methods** - Get aliases, list all valid names

### Usage Example

```csharp
// Parsing (handles all aliases, case-insensitive)
EMonsterAttribute attr = EMonsterAttributeExtensions.ParseAttribute("hp");  // Returns Health
EMonsterAttribute attr2 = EMonsterAttributeExtensions.ParseAttribute("HP");  // Same!

// Safe parsing
if (EMonsterAttributeExtensions.TryParseAttribute("spatk", out var result))
{
    // result is EMonsterAttribute.SpecialAttack
}

// Convert to string
string canonical = EMonsterAttribute.Health.ToAttributeString();  // "health"

// Get all aliases for debugging/help
var aliases = EMonsterAttribute.Health.GetAliases();  // ["health", "hp", "hitpoints"]
```

## Available Enums

### Monster Stats and Attributes

**File:** `EMonsterAttribute.cs`  
**Purpose:** Defines all modifiable monster attributes (health, speed, attack, etc.)

**Enum Values:**

- `Health` - HP, hitpoints
- `Speed` - Movement order
- `Attack` - Physical damage stat
- `Defense` - Physical defense stat
- `SpecialAttack` - Special damage stat
- `SpecialDefense` - Special defense stat
- `Accuracy` - Hit chance modifier
- `Evasion` - Dodge chance modifier
- `CriticalRate` - Critical hit rate

**Common Aliases:**

- Health: `"health"`, `"hp"`, `"hitpoints"`
- Speed: `"speed"`, `"spd"`, `"spe"`
- SpecialAttack: `"specialattack"`, `"spatk"`, `"sp.atk"`

---

### Status Effects

**File:** `EStatusEffect.cs`  
**Purpose:** All status conditions that can affect monsters

**Categories:**

- **Major Status** (persistent): Burn, Freeze, Paralysis, Poison, BadlyPoisoned, Sleep
- **Minor Status** (temporary): Confusion, Flinch, Infatuation, LeechSeed, Curse, Taunt, etc.
- **Stat Modifiers**: AttackUp, AttackDown, DefenseUp, DefenseDown, etc.

**Common Aliases:**

- Burn: `"burn"`, `"brn"`, `"burned"`
- Paralysis: `"paralysis"`, `"par"`, `"paralyzed"`
- BadlyPoisoned: `"badlypoison"`, `"toxic"`, `"tox"`

---

### Side Effects (Team-wide)

**File:** `ESideEffect.cs`  
**Purpose:** Effects that apply to an entire team's side of the field

**Categories:**

- **Protective Screens**: Reflect, LightScreen, AuroraVeil
- **Entry Hazards**: StealthRock, Spikes, ToxicSpikes, StickyWeb
- **Field Conditions**: Mist, Safeguard, Tailwind, LuckyChant

**Common Aliases:**

- Reflect: `"reflect"`, `"physical wall"`
- LightScreen: `"lightscreen"`, `"light screen"`, `"light_screen"`
- StealthRock: `"stealthrock"`, `"stealth rock"`, `"sr"`, `"rocks"`

**Usage Note:** Side effects from the old code used inconsistent casing (`"Reflect"` vs `"reflect"`). The enum system normalizes this.

---

### Field Effects (Battle-wide)

**File:** `EFieldEffect.cs`  
**Purpose:** Effects that influence the entire battle field

**Categories:**

- **Weather**: Points to `BattleWeather` enum (handled separately)
- **Terrains**: ElectricTerrain, GrassyTerrain, MistyTerrain, PsychicTerrain
- **Room Effects**: TrickRoom, MagicRoom, WonderRoom
- **Conditions**: Gravity, MudSport, WaterSport, IonDeluge, FairyLock

**Common Aliases:**

- TrickRoom: `"trickroom"`, `"trick room"`, `"tr"`
- ElectricTerrain: `"electricterrain"`, `"electric terrain"`, `"electric"`

---

### Battle Weather

**File:** `../BattleWeather/BattleWeather.cs` (updated)  
**Purpose:** Weather conditions affecting the battle

**Values:**

- `None` - Clear weather
- `Sunny` - Sun
- `Rainy` - Rain
- `Sandstorm` - Sand
- `Hail` - Hail/Snow
- `HarshSunlight` - Primal Groudon's weather
- `HeavyRain` - Primal Kyogre's weather
- `StrongWinds` - Mega Rayquaza's weather

**Common Aliases:**

- Sunny: `"sunny"`, `"sun"`, `"sunlight"`
- Rainy: `"rainy"`, `"rain"`, `"raining"`

---

### Battle Type (Pokémon Type)

**File:** `../BattleTypes/EBattleType.cs` (updated)  
**Purpose:** Elemental types for monsters and moves

**Values:**

- Gen 1: Normal, Fire, Water, Electric, Grass, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon
- Gen 2+: Dark, Steel
- Gen 6+: Fairy

**Common Aliases:**

- Electric: `"electric"`, `"elec"`, `"lightning"`
- Fighting: `"fighting"`, `"fight"`, `"combat"`
- Psychic: `"psychic"`, `"psy"`

---

### Move Medium (Move Category)

**File:** `../BattleTypes/EMoveMedium.cs` (updated)  
**Purpose:** How a move calculates damage or if it's non-damaging

**Values:**

- `Physical` - Uses Attack/Defense
- `Special` - Uses SpecialAttack/SpecialDefense
- `Status` - No damage, applies effects
- `None` - Unspecified

**Common Aliases:**

- Physical: `"physical"`, `"phys"`, `"attack"`, `"atk"`
- Special: `"special"`, `"spec"`, `"spatk"`
- Status: `"status"`, `"effect"`, `"non-damaging"`

---

## Adding New Enum Values

To extend any enum:

1. **Add to the enum definition**

```csharp
public enum EMonsterAttribute
{
    // ... existing values ...
    Luck  // NEW!
}
```

2. **Add to the string mapping dictionary (ONE place!)**

```csharp
private static readonly Dictionary<string, EMonsterAttribute> StringToEnumMap = new()
{
    // ... existing entries ...
    
    // Luck aliases
    { "luck", EMonsterAttribute.Luck },
    { "lck", EMonsterAttribute.Luck },
    { "fortune", EMonsterAttribute.Luck }
};
```

3. **Done!** All parsing, conversion, and helper methods work automatically.

---

## Migration from String-Based System

### Before (String-based, error-prone)

```csharp
// Typos not caught at compile time
AttributeDeltas["hlth"] = -25;  // Silent error!

// Inconsistent casing
BattleEffects.AddTag("Reflect");
BattleEffects.AddTag("reflect");  // Different tags!

// Manual switch statements
switch (attributeName.ToLower())
{
    case "health":
    case "hp":
        // ...
        break;
}
```

### After (Enum-based, type-safe)

```csharp
// Compile-time checking
AttributeDeltas[EMonsterAttribute.Health] = -25;  // Typos caught!

// Consistent values
SideEffects[ESideEffect.Reflect] = true;  // Always the same

// Exhaustive checking
switch (attribute)
{
    case EMonsterAttribute.Health:
        // ...
        break;
    // Compiler warns if you miss a case!
}
```

---

## Benefits

1. **Type Safety** - Typos caught at compile time
2. **IntelliSense** - IDE autocomplete for all values
3. **Refactor-Friendly** - Rename enum values safely
4. **Exhaustive Checking** - Compiler ensures all cases handled
5. **Alias Support** - Multiple strings map to same enum
6. **Single Source of Truth** - Add enum value + aliases in one place
7. **Better Error Messages** - Parse errors show all valid options

---

## Note on PokemonGeneration Enum

The `PokemonGeneration` enum in `DamageCalculatorFactory.cs` doesn't need string parsing since it's only used internally by the calculator factory. If external parsing is needed, follow the same pattern as above.

---

## Future Considerations

- **Serialization**: Enums serialize as ints by default. Consider custom JSON converters to serialize as strings.
- **Localization**: The canonical string names can be localized by creating a separate mapping dictionary.
- **Validation**: Add `[Flags]` attribute if enums need to be combined (bitwise operations).
