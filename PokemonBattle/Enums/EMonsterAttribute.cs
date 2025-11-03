using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines all monster attributes that can be modified during battle.
/// </summary>
public enum EMonsterAttribute
{
  Health,
  Speed,
  Attack,
  Defense,
  SpecialAttack,
  SpecialDefense,

  // Future extensions
  Accuracy,
  Evasion,
  CriticalRate,
}

/// <summary>
/// Extension methods for EMonsterAttribute enum providing string parsing and conversion.
/// </summary>
public static class EMonsterAttributeExtensions
{
  /// <summary>
  /// Single source of truth for all string-to-enum mappings.
  /// Add new attributes and their aliases here when extending the enum.
  /// </summary>
  private static readonly Dictionary<string, EMonsterAttribute> StringToEnumMap = new()
  {
    // Health aliases
    { "health", EMonsterAttribute.Health },
    { "hp", EMonsterAttribute.Health },
    { "hitpoints", EMonsterAttribute.Health },
    // Speed aliases
    { "speed", EMonsterAttribute.Speed },
    { "spd", EMonsterAttribute.Speed },
    { "spe", EMonsterAttribute.Speed },
    // Attack aliases
    { "attack", EMonsterAttribute.Attack },
    { "atk", EMonsterAttribute.Attack },
    { "att", EMonsterAttribute.Attack },
    // Defense aliases
    { "defense", EMonsterAttribute.Defense },
    { "def", EMonsterAttribute.Defense },
    // Special Attack aliases
    { "specialattack", EMonsterAttribute.SpecialAttack },
    { "spatk", EMonsterAttribute.SpecialAttack },
    { "sp.atk", EMonsterAttribute.SpecialAttack },
    { "spcatk", EMonsterAttribute.SpecialAttack },
    { "special attack", EMonsterAttribute.SpecialAttack },
    // Special Defense aliases
    { "specialdefense", EMonsterAttribute.SpecialDefense },
    { "spdef", EMonsterAttribute.SpecialDefense },
    { "sp.def", EMonsterAttribute.SpecialDefense },
    { "spcdef", EMonsterAttribute.SpecialDefense },
    { "special defense", EMonsterAttribute.SpecialDefense },
    // Accuracy aliases
    { "accuracy", EMonsterAttribute.Accuracy },
    { "acc", EMonsterAttribute.Accuracy },
    // Evasion aliases
    { "evasion", EMonsterAttribute.Evasion },
    { "eva", EMonsterAttribute.Evasion },
    { "evade", EMonsterAttribute.Evasion },
    // Critical Rate aliases
    { "criticalrate", EMonsterAttribute.CriticalRate },
    { "critrate", EMonsterAttribute.CriticalRate },
    { "crit", EMonsterAttribute.CriticalRate },
    { "critical", EMonsterAttribute.CriticalRate },
  };

  // Lazy-initialized reverse map (enum -> canonical string)
  private static Dictionary<EMonsterAttribute, string> _enumToStringMap;
  private static Dictionary<EMonsterAttribute, string> EnumToStringMap
  {
    get
    {
      if (_enumToStringMap == null)
      {
        _enumToStringMap = new Dictionary<EMonsterAttribute, string>();
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
  public static EMonsterAttribute ParseAttribute(string attributeName)
  {
    if (string.IsNullOrWhiteSpace(attributeName))
      throw new ArgumentException("Attribute name cannot be null or empty", nameof(attributeName));

    string normalized = attributeName.ToLower().Trim();

    if (StringToEnumMap.TryGetValue(normalized, out var result))
      return result;

    throw new ArgumentException(
      $"Unknown attribute: '{attributeName}'. Valid values: {string.Join(", ", StringToEnumMap.Keys)}"
    );
  }

  /// <summary>
  /// Try parse string to enum. Returns false if not found.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static bool TryParseAttribute(string attributeName, out EMonsterAttribute result)
  {
    result = default;

    if (string.IsNullOrWhiteSpace(attributeName))
      return false;

    string normalized = attributeName.ToLower().Trim();
    return StringToEnumMap.TryGetValue(normalized, out result);
  }

  /// <summary>
  /// Convert enum to its canonical string representation (first alias defined in map).
  /// </summary>
  public static string ToAttributeString(this EMonsterAttribute attribute)
  {
    if (EnumToStringMap.TryGetValue(attribute, out string result))
      return result;

    // Fallback to enum name if not in map
    return attribute.ToString().ToLower();
  }

  /// <summary>
  /// Get all valid string aliases for an enum value.
  /// Useful for debugging, help text, or validation messages.
  /// </summary>
  public static IEnumerable<string> GetAliases(this EMonsterAttribute attribute)
  {
    return StringToEnumMap.Where(kvp => kvp.Value == attribute).Select(kvp => kvp.Key);
  }

  /// <summary>
  /// Get all valid attribute names across all enums.
  /// Useful for error messages, help text, or autocomplete.
  /// </summary>
  public static IEnumerable<string> GetAllValidNames()
  {
    return StringToEnumMap.Keys;
  }
}
