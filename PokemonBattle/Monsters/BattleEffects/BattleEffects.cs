using System;
using System.Collections.Generic;

public class BattleEffects
{
  /**
   * Represents a collection of temporary values that are added (or tagged) during the course
   * of a battle. Things like Burn status, or speed bonuses etc. 
   *
   * NOTE: All tags will always be lowercase!
   */
  public BattleEffects() { }


  #region Tags
  protected HashSet<string> tags = new();
  public bool ContainsTag(string tagName) { return this.tags.Contains(tagName.ToLower()); }
  public bool AddTag(string tagName) { return this.tags.Add(tagName.ToLower()); }
  public bool RemoveTag(string tagName) { return this.tags.Remove(tagName.ToLower()); }
  #endregion

  #region Tagged Bools
  protected Dictionary<string, bool> booleanTags = new();
  public bool? SetTaggedBool(string tagName, bool newValue)
  {
    tagName = tagName.ToLower();
    bool? result = booleanTags.ContainsKey(tagName) ? booleanTags[tagName] : null;
    booleanTags[tagName] = newValue;
    return result;
  }
  public bool ContainsTaggedBool(string tagName) { return this.booleanTags.ContainsKey(tagName.ToLower()); }
  public bool GetTaggedBool(string tagName) { return this.booleanTags[tagName.ToLower()]; }
  public bool RemoveTaggedBool(string tagName) { return this.booleanTags.Remove(tagName.ToLower()); }
  #endregion Tagged Bools

  #region Tagged Ints
  protected Dictionary<string, int> intTags = new();
  public int? SetTaggedInt(string tagName, int newValue)
  {
    tagName = tagName.ToLower();
    int? result = intTags.ContainsKey(tagName) ? intTags[tagName] : null;
    intTags[tagName] = newValue;
    return result;
  }
  public bool ContainsTaggedInt(string tagName) { return this.intTags.ContainsKey(tagName.ToLower()); }
  public int GetTaggedInt(string tagName) { return this.intTags[tagName.ToLower()]; }
  public bool RemoveTaggedInt(string tagName) { return this.intTags.Remove(tagName.ToLower()); }
  #endregion Tagged Ints

  #region Tagged Float
  protected Dictionary<string, float> floatTags = new();
  public float? SetTaggedFloat(string tagName, float newValue)
  {
    tagName = tagName.ToLower();
    float? result = floatTags.ContainsKey(tagName) ? floatTags[tagName] : null;
    floatTags[tagName] = newValue;
    return result;
  }
  public bool ContainsTaggedFloat(string tagName) { return this.floatTags.ContainsKey(tagName.ToLower()); }
  public float GetTaggedFloat(string tagName) { return this.floatTags[tagName.ToLower()]; }
  public bool RemoveTaggedFloat(string tagName) { return this.floatTags.Remove(tagName.ToLower()); }
  #endregion Tagged Float

  #region Tagged String
  protected Dictionary<string, string> stringTags = new();
#nullable enable
  public string? SetTag(string tagName, string newValue)
  {
    tagName = tagName.ToLower();
    string? result = stringTags.ContainsKey(tagName) ? stringTags[tagName] : null;
    stringTags[tagName] = newValue;
    return result;
  }
#nullable disable

  public bool ContainsTag_string(string tagName) { return this.stringTags.ContainsKey(tagName.ToLower()); }
  public string GetTaggedString(string tagName) { return this.stringTags[tagName.ToLower()]; }
  public bool RemoveTaggedString(string tagName) { return this.stringTags.Remove(tagName.ToLower()); }
  #endregion Tagged String
}