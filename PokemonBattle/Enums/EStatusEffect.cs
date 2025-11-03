using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines all status effects that can be applied to monsters during battle.
/// Includes both major (persistent) and minor (temporary) status conditions.
/// </summary>
public enum EStatusEffect
{
  None,

  // Major Status (persistent until cured)
  Burn,
  Freeze,
  Paralysis,
  Poison,
  BadlyPoisoned,
  Sleep,

  // Minor Status (temporary conditions)
  Confusion,
  Flinch,
  Infatuation,
  LeechSeed,
  Curse,
  Taunt,
  Torment,
  Encore,
  Disable,
  Yawn,

  // Stat modification effects (alternative to attribute deltas)
  AttackUp,
  AttackDown,
  DefenseUp,
  DefenseDown,
  SpeedUp,
  SpeedDown,
  AccuracyUp,
  AccuracyDown,
  EvasionUp,
  EvasionDown,
}

/// <summary>
/// Extension methods for EStatusEffect enum providing string parsing and conversion.
/// </summary>
public static class EStatusEffectExtensions
{
  /// <summary>
  /// Single source of truth for all string-to-enum mappings.
  /// Add new status effects and their aliases here when extending the enum.
  /// </summary>
  private static readonly Dictionary<string, EStatusEffect> StringToEnumMap = new()
  {
    // None
    { "none", EStatusEffect.None },
    { "normal", EStatusEffect.None },
    // Burn aliases
    { "burn", EStatusEffect.Burn },
    { "brn", EStatusEffect.Burn },
    { "burned", EStatusEffect.Burn },
    { "burning", EStatusEffect.Burn },
    // Freeze aliases
    { "freeze", EStatusEffect.Freeze },
    { "frz", EStatusEffect.Freeze },
    { "frozen", EStatusEffect.Freeze },
    { "ice", EStatusEffect.Freeze },
    // Paralysis aliases
    { "paralysis", EStatusEffect.Paralysis },
    { "par", EStatusEffect.Paralysis },
    { "paralyzed", EStatusEffect.Paralysis },
    { "paralyze", EStatusEffect.Paralysis },
    // Poison aliases
    { "poison", EStatusEffect.Poison },
    { "psn", EStatusEffect.Poison },
    { "poisoned", EStatusEffect.Poison },
    // Badly Poisoned aliases
    { "badlypoison", EStatusEffect.BadlyPoisoned },
    { "badlypoisoned", EStatusEffect.BadlyPoisoned },
    { "toxic", EStatusEffect.BadlyPoisoned },
    { "tox", EStatusEffect.BadlyPoisoned },
    // Sleep aliases
    { "sleep", EStatusEffect.Sleep },
    { "slp", EStatusEffect.Sleep },
    { "asleep", EStatusEffect.Sleep },
    { "sleeping", EStatusEffect.Sleep },
    // Confusion aliases
    { "confusion", EStatusEffect.Confusion },
    { "confused", EStatusEffect.Confusion },
    { "confuse", EStatusEffect.Confusion },
    // Flinch aliases
    { "flinch", EStatusEffect.Flinch },
    { "flinched", EStatusEffect.Flinch },
    // Infatuation aliases
    { "infatuation", EStatusEffect.Infatuation },
    { "infatuated", EStatusEffect.Infatuation },
    { "attract", EStatusEffect.Infatuation },
    { "attracted", EStatusEffect.Infatuation },
    { "love", EStatusEffect.Infatuation },
    // Leech Seed aliases
    { "leechseed", EStatusEffect.LeechSeed },
    { "leech seed", EStatusEffect.LeechSeed },
    { "seeded", EStatusEffect.LeechSeed },
    // Curse aliases
    { "curse", EStatusEffect.Curse },
    { "cursed", EStatusEffect.Curse },
    // Taunt aliases
    { "taunt", EStatusEffect.Taunt },
    { "taunted", EStatusEffect.Taunt },
    // Torment aliases
    { "torment", EStatusEffect.Torment },
    { "tormented", EStatusEffect.Torment },
    // Encore aliases
    { "encore", EStatusEffect.Encore },
    { "encored", EStatusEffect.Encore },
    // Disable aliases
    { "disable", EStatusEffect.Disable },
    { "disabled", EStatusEffect.Disable },
    // Yawn aliases
    { "yawn", EStatusEffect.Yawn },
    { "drowsy", EStatusEffect.Yawn },
    // Stat modification effects
    { "attackup", EStatusEffect.AttackUp },
    { "attack up", EStatusEffect.AttackUp },
    { "atk+", EStatusEffect.AttackUp },
    { "attackdown", EStatusEffect.AttackDown },
    { "attack down", EStatusEffect.AttackDown },
    { "atk-", EStatusEffect.AttackDown },
    { "defenseup", EStatusEffect.DefenseUp },
    { "defense up", EStatusEffect.DefenseUp },
    { "def+", EStatusEffect.DefenseUp },
    { "defensedown", EStatusEffect.DefenseDown },
    { "defense down", EStatusEffect.DefenseDown },
    { "def-", EStatusEffect.DefenseDown },
    { "speedup", EStatusEffect.SpeedUp },
    { "speed up", EStatusEffect.SpeedUp },
    { "spd+", EStatusEffect.SpeedUp },
    { "speeddown", EStatusEffect.SpeedDown },
    { "speed down", EStatusEffect.SpeedDown },
    { "spd-", EStatusEffect.SpeedDown },
    { "accuracyup", EStatusEffect.AccuracyUp },
    { "accuracy up", EStatusEffect.AccuracyUp },
    { "acc+", EStatusEffect.AccuracyUp },
    { "accuracydown", EStatusEffect.AccuracyDown },
    { "accuracy down", EStatusEffect.AccuracyDown },
    { "acc-", EStatusEffect.AccuracyDown },
    { "evasionup", EStatusEffect.EvasionUp },
    { "evasion up", EStatusEffect.EvasionUp },
    { "eva+", EStatusEffect.EvasionUp },
    { "evasiondown", EStatusEffect.EvasionDown },
    { "evasion down", EStatusEffect.EvasionDown },
    { "eva-", EStatusEffect.EvasionDown },
  };

  // Lazy-initialized reverse map (enum -> canonical string)
  private static Dictionary<EStatusEffect, string> _enumToStringMap;
  private static Dictionary<EStatusEffect, string> EnumToStringMap
  {
    get
    {
      if (_enumToStringMap == null)
      {
        _enumToStringMap = new Dictionary<EStatusEffect, string>();
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
  public static EStatusEffect ParseStatusEffect(string statusName)
  {
    if (string.IsNullOrWhiteSpace(statusName))
      throw new ArgumentException("Status effect name cannot be null or empty", nameof(statusName));

    string normalized = statusName.ToLower().Trim();

    if (StringToEnumMap.TryGetValue(normalized, out var result))
      return result;

    throw new ArgumentException(
      $"Unknown status effect: '{statusName}'. Valid values: {string.Join(", ", StringToEnumMap.Keys)}"
    );
  }

  /// <summary>
  /// Try parse string to enum. Returns false if not found.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static bool TryParseStatusEffect(string statusName, out EStatusEffect result)
  {
    result = default;

    if (string.IsNullOrWhiteSpace(statusName))
      return false;

    string normalized = statusName.ToLower().Trim();
    return StringToEnumMap.TryGetValue(normalized, out result);
  }

  /// <summary>
  /// Convert enum to its canonical string representation (first alias defined in map).
  /// </summary>
  public static string ToStatusString(this EStatusEffect status)
  {
    if (EnumToStringMap.TryGetValue(status, out string result))
      return result;

    // Fallback to enum name if not in map
    return status.ToString().ToLower();
  }

  /// <summary>
  /// Get all valid string aliases for an enum value.
  /// </summary>
  public static IEnumerable<string> GetAliases(this EStatusEffect status)
  {
    return StringToEnumMap.Where(kvp => kvp.Value == status).Select(kvp => kvp.Key);
  }

  /// <summary>
  /// Get all valid status effect names across all enums.
  /// </summary>
  public static IEnumerable<string> GetAllValidNames()
  {
    return StringToEnumMap.Keys;
  }
}
