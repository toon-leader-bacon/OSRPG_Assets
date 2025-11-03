using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines all side-wide effects that apply to an entire team during battle.
/// These effects remain active until removed or the battle ends.
/// </summary>
public enum ESideEffect
{
  None,

  // Protective screens
  Reflect,
  LightScreen,
  AuroraVeil,

  // Entry hazards
  StealthRock,
  Spikes,
  ToxicSpikes,
  StickyWeb,

  // Field conditions
  Mist,
  Safeguard,
  Tailwind,
  LuckyChant,

  // Other side effects
  WideGuard,
  QuickGuard,
  MatBlock,
  CraftyShield,
}

/// <summary>
/// Extension methods for ESideEffect enum providing string parsing and conversion.
/// </summary>
public static class ESideEffectExtensions
{
  /// <summary>
  /// Single source of truth for all string-to-enum mappings.
  /// Add new side effects and their aliases here when extending the enum.
  /// </summary>
  private static readonly Dictionary<string, ESideEffect> StringToEnumMap = new()
  {
    // None
    { "none", ESideEffect.None },
    // Reflect aliases
    { "reflect", ESideEffect.Reflect },
    { "physical wall", ESideEffect.Reflect },
    // Light Screen aliases
    { "lightscreen", ESideEffect.LightScreen },
    { "light screen", ESideEffect.LightScreen },
    { "light_screen", ESideEffect.LightScreen },
    { "special wall", ESideEffect.LightScreen },
    // Aurora Veil aliases
    { "auroraveil", ESideEffect.AuroraVeil },
    { "aurora veil", ESideEffect.AuroraVeil },
    { "aurora_veil", ESideEffect.AuroraVeil },
    { "veil", ESideEffect.AuroraVeil },
    // Stealth Rock aliases
    { "stealthrock", ESideEffect.StealthRock },
    { "stealth rock", ESideEffect.StealthRock },
    { "stealth_rock", ESideEffect.StealthRock },
    { "sr", ESideEffect.StealthRock },
    { "rocks", ESideEffect.StealthRock },
    // Spikes aliases
    { "spikes", ESideEffect.Spikes },
    { "spike", ESideEffect.Spikes },
    // Toxic Spikes aliases
    { "toxicspikes", ESideEffect.ToxicSpikes },
    { "toxic spikes", ESideEffect.ToxicSpikes },
    { "toxic_spikes", ESideEffect.ToxicSpikes },
    { "tspikes", ESideEffect.ToxicSpikes },
    // Sticky Web aliases
    { "stickyweb", ESideEffect.StickyWeb },
    { "sticky web", ESideEffect.StickyWeb },
    { "sticky_web", ESideEffect.StickyWeb },
    { "web", ESideEffect.StickyWeb },
    // Mist aliases
    { "mist", ESideEffect.Mist },
    // Safeguard aliases
    { "safeguard", ESideEffect.Safeguard },
    { "safe guard", ESideEffect.Safeguard },
    { "safe_guard", ESideEffect.Safeguard },
    // Tailwind aliases
    { "tailwind", ESideEffect.Tailwind },
    { "tail wind", ESideEffect.Tailwind },
    { "tail_wind", ESideEffect.Tailwind },
    { "wind", ESideEffect.Tailwind },
    // Lucky Chant aliases
    { "luckychant", ESideEffect.LuckyChant },
    { "lucky chant", ESideEffect.LuckyChant },
    { "lucky_chant", ESideEffect.LuckyChant },
    // Wide Guard aliases
    { "wideguard", ESideEffect.WideGuard },
    { "wide guard", ESideEffect.WideGuard },
    { "wide_guard", ESideEffect.WideGuard },
    // Quick Guard aliases
    { "quickguard", ESideEffect.QuickGuard },
    { "quick guard", ESideEffect.QuickGuard },
    { "quick_guard", ESideEffect.QuickGuard },
    // Mat Block aliases
    { "matblock", ESideEffect.MatBlock },
    { "mat block", ESideEffect.MatBlock },
    { "mat_block", ESideEffect.MatBlock },
    // Crafty Shield aliases
    { "craftyshield", ESideEffect.CraftyShield },
    { "crafty shield", ESideEffect.CraftyShield },
    { "crafty_shield", ESideEffect.CraftyShield },
  };

  // Lazy-initialized reverse map (enum -> canonical string)
  private static Dictionary<ESideEffect, string> _enumToStringMap;
  private static Dictionary<ESideEffect, string> EnumToStringMap
  {
    get
    {
      if (_enumToStringMap == null)
      {
        _enumToStringMap = new Dictionary<ESideEffect, string>();
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
  public static ESideEffect ParseSideEffect(string effectName)
  {
    if (string.IsNullOrWhiteSpace(effectName))
      throw new ArgumentException("Side effect name cannot be null or empty", nameof(effectName));

    string normalized = effectName.ToLower().Trim();

    if (StringToEnumMap.TryGetValue(normalized, out var result))
      return result;

    throw new ArgumentException(
      $"Unknown side effect: '{effectName}'. Valid values: {string.Join(", ", StringToEnumMap.Keys)}"
    );
  }

  /// <summary>
  /// Try parse string to enum. Returns false if not found.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static bool TryParseSideEffect(string effectName, out ESideEffect result)
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
  public static string ToEffectString(this ESideEffect effect)
  {
    if (EnumToStringMap.TryGetValue(effect, out string result))
      return result;

    // Fallback to enum name if not in map
    return effect.ToString().ToLower();
  }

  /// <summary>
  /// Get all valid string aliases for an enum value.
  /// </summary>
  public static IEnumerable<string> GetAliases(this ESideEffect effect)
  {
    return StringToEnumMap.Where(kvp => kvp.Value == effect).Select(kvp => kvp.Key);
  }

  /// <summary>
  /// Get all valid side effect names across all enums.
  /// </summary>
  public static IEnumerable<string> GetAllValidNames()
  {
    return StringToEnumMap.Keys;
  }
}
