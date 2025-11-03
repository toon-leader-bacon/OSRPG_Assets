using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines the category of a move (how damage is calculated or if it's a status move).
/// </summary>
public enum EMoveMedium
{
  Physical,
  Special,
  Status,

  // Misc.
  None,
}

/// <summary>
/// Extension methods for EMoveMedium enum providing string parsing and conversion.
/// </summary>
public static class EMoveMediumExtensions
{
  /// <summary>
  /// Single source of truth for all string-to-enum mappings.
  /// Add new move categories and their aliases here when extending the enum.
  /// </summary>
  private static readonly Dictionary<string, EMoveMedium> StringToEnumMap = new()
  {
    // Physical
    { "physical", EMoveMedium.Physical },
    { "phys", EMoveMedium.Physical },
    { "attack", EMoveMedium.Physical },
    { "atk", EMoveMedium.Physical },
    // Special
    { "special", EMoveMedium.Special },
    { "spec", EMoveMedium.Special },
    { "spatk", EMoveMedium.Special },
    // Status
    { "status", EMoveMedium.Status },
    { "effect", EMoveMedium.Status },
    { "non-damaging", EMoveMedium.Status },
    // None
    { "none", EMoveMedium.None },
    { "unknown", EMoveMedium.None },
  };

  // Lazy-initialized reverse map (enum -> canonical string)
  private static Dictionary<EMoveMedium, string> _enumToStringMap;
  private static Dictionary<EMoveMedium, string> EnumToStringMap
  {
    get
    {
      if (_enumToStringMap == null)
      {
        _enumToStringMap = new Dictionary<EMoveMedium, string>();
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
  public static EMoveMedium ParseMedium(string mediumName)
  {
    if (string.IsNullOrWhiteSpace(mediumName))
      throw new ArgumentException("Move medium name cannot be null or empty", nameof(mediumName));

    string normalized = mediumName.ToLower().Trim();

    if (StringToEnumMap.TryGetValue(normalized, out var result))
      return result;

    throw new ArgumentException(
      $"Unknown move medium: '{mediumName}'. Valid values: {string.Join(", ", StringToEnumMap.Keys)}"
    );
  }

  /// <summary>
  /// Try parse string to enum. Returns false if not found.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static bool TryParseMedium(string mediumName, out EMoveMedium result)
  {
    result = default;

    if (string.IsNullOrWhiteSpace(mediumName))
      return false;

    string normalized = mediumName.ToLower().Trim();
    return StringToEnumMap.TryGetValue(normalized, out result);
  }

  /// <summary>
  /// Convert enum to its canonical string representation (first alias defined in map).
  /// </summary>
  public static string ToMediumString(this EMoveMedium medium)
  {
    if (EnumToStringMap.TryGetValue(medium, out string result))
      return result;

    // Fallback to enum name if not in map
    return medium.ToString().ToLower();
  }

  /// <summary>
  /// Get all valid string aliases for an enum value.
  /// </summary>
  public static IEnumerable<string> GetAliases(this EMoveMedium medium)
  {
    return StringToEnumMap.Where(kvp => kvp.Value == medium).Select(kvp => kvp.Key);
  }

  /// <summary>
  /// Get all valid move medium names across all enums.
  /// </summary>
  public static IEnumerable<string> GetAllValidNames()
  {
    return StringToEnumMap.Keys;
  }
}
