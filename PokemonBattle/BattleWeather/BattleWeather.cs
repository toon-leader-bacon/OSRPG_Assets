using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines all weather conditions that can affect the battle field.
/// </summary>
public enum BattleWeather
{
  None,
  Sunny,
  Rainy,
  Sandstorm,
  Hail,
  HarshSunlight,
  HeavyRain,
  StrongWinds,
}

/// <summary>
/// Extension methods for BattleWeather enum providing string parsing and conversion.
/// </summary>
public static class BattleWeatherExtensions
{
  /// <summary>
  /// Single source of truth for all string-to-enum mappings.
  /// Add new weather conditions and their aliases here when extending the enum.
  /// </summary>
  private static readonly Dictionary<string, BattleWeather> StringToEnumMap = new()
  {
    // None
    { "none", BattleWeather.None },
    { "clear", BattleWeather.None },
    { "normal", BattleWeather.None },
    // Sunny aliases
    { "sunny", BattleWeather.Sunny },
    { "sun", BattleWeather.Sunny },
    { "sunlight", BattleWeather.Sunny },
    { "harsh sunlight", BattleWeather.Sunny },
    // Rainy aliases
    { "rainy", BattleWeather.Rainy },
    { "rain", BattleWeather.Rainy },
    { "raining", BattleWeather.Rainy },
    // Sandstorm aliases
    { "sandstorm", BattleWeather.Sandstorm },
    { "sand", BattleWeather.Sandstorm },
    // Hail aliases
    { "hail", BattleWeather.Hail },
    { "hailing", BattleWeather.Hail },
    { "snow", BattleWeather.Hail },
    // Harsh Sunlight (Primal Groudon) aliases
    { "harshsunlight", BattleWeather.HarshSunlight },
    { "harsh sunlight", BattleWeather.HarshSunlight },
    { "extreme sun", BattleWeather.HarshSunlight },
    { "desolate land", BattleWeather.HarshSunlight },
    // Heavy Rain (Primal Kyogre) aliases
    { "heavyrain", BattleWeather.HeavyRain },
    { "heavy rain", BattleWeather.HeavyRain },
    { "extreme rain", BattleWeather.HeavyRain },
    { "primordial sea", BattleWeather.HeavyRain },
    // Strong Winds (Mega Rayquaza) aliases
    { "strongwinds", BattleWeather.StrongWinds },
    { "strong winds", BattleWeather.StrongWinds },
    { "delta stream", BattleWeather.StrongWinds },
    { "air currents", BattleWeather.StrongWinds },
  };

  // Lazy-initialized reverse map (enum -> canonical string)
  private static Dictionary<BattleWeather, string> _enumToStringMap;
  private static Dictionary<BattleWeather, string> EnumToStringMap
  {
    get
    {
      if (_enumToStringMap == null)
      {
        _enumToStringMap = new Dictionary<BattleWeather, string>();
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
  public static BattleWeather ParseWeather(string weatherName)
  {
    if (string.IsNullOrWhiteSpace(weatherName))
      throw new ArgumentException("Weather name cannot be null or empty", nameof(weatherName));

    string normalized = weatherName.ToLower().Trim();

    if (StringToEnumMap.TryGetValue(normalized, out var result))
      return result;

    throw new ArgumentException(
      $"Unknown weather: '{weatherName}'. Valid values: {string.Join(", ", StringToEnumMap.Keys)}"
    );
  }

  /// <summary>
  /// Try parse string to enum. Returns false if not found.
  /// Case-insensitive and trims whitespace.
  /// </summary>
  public static bool TryParseWeather(string weatherName, out BattleWeather result)
  {
    result = default;

    if (string.IsNullOrWhiteSpace(weatherName))
      return false;

    string normalized = weatherName.ToLower().Trim();
    return StringToEnumMap.TryGetValue(normalized, out result);
  }

  /// <summary>
  /// Convert enum to its canonical string representation (first alias defined in map).
  /// </summary>
  public static string ToWeatherString(this BattleWeather weather)
  {
    if (EnumToStringMap.TryGetValue(weather, out string result))
      return result;

    // Fallback to enum name if not in map
    return weather.ToString().ToLower();
  }

  /// <summary>
  /// Get all valid string aliases for an enum value.
  /// </summary>
  public static IEnumerable<string> GetAliases(this BattleWeather weather)
  {
    return StringToEnumMap.Where(kvp => kvp.Value == weather).Select(kvp => kvp.Key);
  }

  /// <summary>
  /// Get all valid weather names across all enums.
  /// </summary>
  public static IEnumerable<string> GetAllValidNames()
  {
    return StringToEnumMap.Keys;
  }
}
