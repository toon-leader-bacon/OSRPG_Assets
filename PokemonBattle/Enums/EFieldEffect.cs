using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines all field-wide effects that affect the entire battle field.
/// These effects influence all monsters regardless of team.
/// </summary>
public enum EFieldEffect
{
  None,

  // Weather is handled separately by BattleWeather enum
  Weather,

  // Terrains
  ElectricTerrain,
  GrassyTerrain,
  MistyTerrain,
  PsychicTerrain,

  // Room effects
  TrickRoom,
  MagicRoom,
  WonderRoom,

  // Field conditions
  Gravity,
  MudSport,
  WaterSport,
  IonDeluge,
  FairyLock,
}

/// <summary>
/// Extension methods for EFieldEffect enum providing string parsing and conversion.
/// </summary>
public static class EFieldEffectExtensions
{
  /// <summary>
  /// Single source of truth for all string-to-enum mappings.
  /// Add new field effects and their aliases here when extending the enum.
  /// </summary>
  private static readonly Dictionary<string, EFieldEffect> StringToEnumMap = new()
  {
    // None
    { "none", EFieldEffect.None },
    // Weather (points to BattleWeather enum for actual value)
    { "weather", EFieldEffect.Weather },
    // Electric Terrain aliases
    { "electricterrain", EFieldEffect.ElectricTerrain },
    { "electric terrain", EFieldEffect.ElectricTerrain },
    { "electric_terrain", EFieldEffect.ElectricTerrain },
    { "electric", EFieldEffect.ElectricTerrain },
    // Grassy Terrain aliases
    { "grassyterrain", EFieldEffect.GrassyTerrain },
    { "grassy terrain", EFieldEffect.GrassyTerrain },
    { "grassy_terrain", EFieldEffect.GrassyTerrain },
    { "grassy", EFieldEffect.GrassyTerrain },
    { "grass terrain", EFieldEffect.GrassyTerrain },
    // Misty Terrain aliases
    { "mistyterrain", EFieldEffect.MistyTerrain },
    { "misty terrain", EFieldEffect.MistyTerrain },
    { "misty_terrain", EFieldEffect.MistyTerrain },
    { "misty", EFieldEffect.MistyTerrain },
    // Psychic Terrain aliases
    { "psychicterrain", EFieldEffect.PsychicTerrain },
    { "psychic terrain", EFieldEffect.PsychicTerrain },
    { "psychic_terrain", EFieldEffect.PsychicTerrain },
    { "psychic", EFieldEffect.PsychicTerrain },
    // Trick Room aliases
    { "trickroom", EFieldEffect.TrickRoom },
    { "trick room", EFieldEffect.TrickRoom },
    { "trick_room", EFieldEffect.TrickRoom },
    { "tr", EFieldEffect.TrickRoom },
    // Magic Room aliases
    { "magicroom", EFieldEffect.MagicRoom },
    { "magic room", EFieldEffect.MagicRoom },
    { "magic_room", EFieldEffect.MagicRoom },
    // Wonder Room aliases
    { "wonderroom", EFieldEffect.WonderRoom },
    { "wonder room", EFieldEffect.WonderRoom },
    { "wonder_room", EFieldEffect.WonderRoom },
    // Gravity aliases
    { "gravity", EFieldEffect.Gravity },
    { "intense gravity", EFieldEffect.Gravity },
    // Mud Sport aliases
    { "mudsport", EFieldEffect.MudSport },
    { "mud sport", EFieldEffect.MudSport },
    { "mud_sport", EFieldEffect.MudSport },
    // Water Sport aliases
    { "watersport", EFieldEffect.WaterSport },
    { "water sport", EFieldEffect.WaterSport },
    { "water_sport", EFieldEffect.WaterSport },
    // Ion Deluge aliases
    { "iondeluge", EFieldEffect.IonDeluge },
    { "ion deluge", EFieldEffect.IonDeluge },
    { "ion_deluge", EFieldEffect.IonDeluge },
    // Fairy Lock aliases
    { "fairylock", EFieldEffect.FairyLock },
    { "fairy lock", EFieldEffect.FairyLock },
    { "fairy_lock", EFieldEffect.FairyLock },
  };

  // Lazy-initialized reverse map (enum -> canonical string)
  private static Dictionary<EFieldEffect, string> _enumToStringMap;
  private static Dictionary<EFieldEffect, string> EnumToStringMap
  {
    get
    {
      if (_enumToStringMap == null)
      {
        _enumToStringMap = new Dictionary<EFieldEffect, string>();
        foreach (var kvp in StringToEnumMap)
        {
          if (!_enumToStringMap.ContainsKey(kvp.Value))
          {
            _enumToStringMap[kvp.Value] = kvp.Key;
          }
        }
      }
      return _enumToStringMap;
    }
  }

  /// <summary>
  /// Parse string to enum. Handles all aliases defined in StringToEnumMap.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static EFieldEffect ParseFieldEffect(string effectName)
  {
    if (string.IsNullOrWhiteSpace(effectName))
      throw new ArgumentException("Field effect name cannot be null or empty", nameof(effectName));

    string normalized = effectName.ToLower().Trim();

    if (StringToEnumMap.TryGetValue(normalized, out var result))
      return result;

    throw new ArgumentException(
      $"Unknown field effect: '{effectName}'. Valid values: {string.Join(", ", StringToEnumMap.Keys)}"
    );
  }

  /// <summary>
  /// Try parse string to enum. Returns false if not found.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static bool TryParseFieldEffect(string effectName, out EFieldEffect result)
  {
    result = default;

    if (string.IsNullOrWhiteSpace(effectName))
      return false;

    string normalized = effectName.ToLower().Trim();
    return StringToEnumMap.TryGetValue(normalized, out result);
  }

  /// <summary>
  /// Convert enum to its canonical string representation (first alias defined in map).
  /// </summary>
  public static string ToEffectString(this EFieldEffect effect)
  {
    if (EnumToStringMap.TryGetValue(effect, out string result))
      return result;

    // Fallback to enum name if not in map
    return effect.ToString().ToLower();
  }

  /// <summary>
  /// Get all valid string aliases for an enum value.
  /// </summary>
  public static IEnumerable<string> GetAliases(this EFieldEffect effect)
  {
    return StringToEnumMap.Where(kvp => kvp.Value == effect).Select(kvp => kvp.Key);
  }

  /// <summary>
  /// Get all valid field effect names across all enums.
  /// </summary>
  public static IEnumerable<string> GetAllValidNames()
  {
    return StringToEnumMap.Keys;
  }
}
