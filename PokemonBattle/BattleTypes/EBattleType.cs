using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines all Pokémon types across generations.
/// </summary>
public enum EBattleType
{
  // Gen 1
  Normal,
  Fire,
  Water,
  Electric,
  Grass,
  Ice,
  Fighting,
  Poison,
  Ground,
  Flying,
  Psychic,
  Bug,
  Rock,
  Ghost,
  Dragon,

  // Gen 2-5
  Dark,
  Steel,

  // Gen 6
  Fairy,

  // Misc:
  None,
}

/// <summary>
/// Extension methods for EBattleType enum providing string parsing and conversion.
/// </summary>
public static class EBattleTypeExtensions
{
  /// <summary>
  /// Single source of truth for all string-to-enum mappings.
  /// Add new types and their aliases here when extending the enum.
  /// </summary>
  private static readonly Dictionary<string, EBattleType> StringToEnumMap = new()
  {
    // None
    { "none", EBattleType.None },
    { "typeless", EBattleType.None },
    // Normal
    { "normal", EBattleType.Normal },
    { "norm", EBattleType.Normal },
    // Fire
    { "fire", EBattleType.Fire },
    // Water
    { "water", EBattleType.Water },
    { "aqua", EBattleType.Water },
    // Electric
    { "electric", EBattleType.Electric },
    { "elec", EBattleType.Electric },
    { "lightning", EBattleType.Electric },
    // Grass
    { "grass", EBattleType.Grass },
    { "plant", EBattleType.Grass },
    // Ice
    { "ice", EBattleType.Ice },
    { "frozen", EBattleType.Ice },
    // Fighting
    { "fighting", EBattleType.Fighting },
    { "fight", EBattleType.Fighting },
    { "combat", EBattleType.Fighting },
    // Poison
    { "poison", EBattleType.Poison },
    { "toxic", EBattleType.Poison },
    // Ground
    { "ground", EBattleType.Ground },
    { "earth", EBattleType.Ground },
    // Flying
    { "flying", EBattleType.Flying },
    { "fly", EBattleType.Flying },
    { "air", EBattleType.Flying },
    // Psychic
    { "psychic", EBattleType.Psychic },
    { "psy", EBattleType.Psychic },
    // Bug
    { "bug", EBattleType.Bug },
    { "insect", EBattleType.Bug },
    // Rock
    { "rock", EBattleType.Rock },
    { "stone", EBattleType.Rock },
    // Ghost
    { "ghost", EBattleType.Ghost },
    { "spirit", EBattleType.Ghost },
    // Dragon
    { "dragon", EBattleType.Dragon },
    { "draco", EBattleType.Dragon },
    // Dark (Gen 2+)
    { "dark", EBattleType.Dark },
    { "evil", EBattleType.Dark },
    // Steel (Gen 2+)
    { "steel", EBattleType.Steel },
    { "metal", EBattleType.Steel },
    // Fairy (Gen 6+)
    { "fairy", EBattleType.Fairy },
    { "fae", EBattleType.Fairy },
  };

  // Lazy-initialized reverse map (enum -> canonical string)
  private static Dictionary<EBattleType, string> _enumToStringMap;
  private static Dictionary<EBattleType, string> EnumToStringMap
  {
    get
    {
      if (_enumToStringMap == null)
      {
        _enumToStringMap = new Dictionary<EBattleType, string>();
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
  public static EBattleType ParseType(string typeName)
  {
    if (string.IsNullOrWhiteSpace(typeName))
      throw new ArgumentException("Type name cannot be null or empty", nameof(typeName));

    string normalized = typeName.ToLower().Trim();

    if (StringToEnumMap.TryGetValue(normalized, out var result))
      return result;

    throw new ArgumentException(
      $"Unknown type: '{typeName}'. Valid values: {string.Join(", ", StringToEnumMap.Keys)}"
    );
  }

  /// <summary>
  /// Try parse string to enum. Returns false if not found.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static bool TryParseType(string typeName, out EBattleType result)
  {
    result = default;

    if (string.IsNullOrWhiteSpace(typeName))
      return false;

    string normalized = typeName.ToLower().Trim();
    return StringToEnumMap.TryGetValue(normalized, out result);
  }

  /// <summary>
  /// Convert enum to its canonical string representation (first alias defined in map).
  /// </summary>
  public static string ToTypeString(this EBattleType type)
  {
    if (EnumToStringMap.TryGetValue(type, out string result))
      return result;

    // Fallback to enum name if not in map
    return type.ToString().ToLower();
  }

  /// <summary>
  /// Get all valid string aliases for an enum value.
  /// </summary>
  public static IEnumerable<string> GetAliases(this EBattleType type)
  {
    return StringToEnumMap.Where(kvp => kvp.Value == type).Select(kvp => kvp.Key);
  }

  /// <summary>
  /// Get all valid type names across all enums.
  /// </summary>
  public static IEnumerable<string> GetAllValidNames()
  {
    return StringToEnumMap.Keys;
  }
}

public static class EBattleType_Util
{
  public static readonly HashSet<EBattleType> GEN1_BATTLE_TYPES = new()
  {
    EBattleType.Normal,
    EBattleType.Fighting,
    EBattleType.Flying,
    EBattleType.Poison,
    EBattleType.Ground,
    EBattleType.Rock,
    EBattleType.Bug,
    EBattleType.Ghost,
    EBattleType.Fighting,
    EBattleType.Water,
    EBattleType.Grass,
    EBattleType.Electric,
    EBattleType.Psychic,
    EBattleType.Ice,
    EBattleType.Dragon,
  };

  public static readonly HashSet<EBattleType> GEN2_5_BATTLE_TYPES = new()
  {
    EBattleType.Normal,
    EBattleType.Fighting,
    EBattleType.Flying,
    EBattleType.Poison,
    EBattleType.Ground,
    EBattleType.Rock,
    EBattleType.Bug,
    EBattleType.Ghost,
    EBattleType.Fighting,
    EBattleType.Water,
    EBattleType.Grass,
    EBattleType.Electric,
    EBattleType.Psychic,
    EBattleType.Ice,
    EBattleType.Dragon,
    EBattleType.Steel,
    EBattleType.Dark,
  };

  public static readonly HashSet<EBattleType> GEN6_BATTLE_TYPES = new()
  {
    EBattleType.Normal,
    EBattleType.Fighting,
    EBattleType.Flying,
    EBattleType.Poison,
    EBattleType.Ground,
    EBattleType.Rock,
    EBattleType.Bug,
    EBattleType.Ghost,
    EBattleType.Fighting,
    EBattleType.Water,
    EBattleType.Grass,
    EBattleType.Electric,
    EBattleType.Psychic,
    EBattleType.Ice,
    EBattleType.Dragon,
    EBattleType.Steel,
    EBattleType.Dark,
    EBattleType.Fairy,
  };
}
