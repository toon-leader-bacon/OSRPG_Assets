using UnityEngine;

/// <summary>
/// Specialized conductor for Pokemon-style Double Battles (2v2).
/// Inherits PartyBattleConductor but validates that both teams have exactly
/// two active monsters. All party battle rules still apply.
/// </summary>
public class DoubleBattleConductor : PartyBattleConductor
{
  /// <summary>
  /// Validates that both teams are configured for 2 active monsters.
  /// </summary>
  public override void Initialize(BattleModel model)
  {
    base.Initialize(model);

    ValidateActiveCount(model.playerTeam, "Player team");
    ValidateActiveCount(model.computerTeam, "Computer team");
  }

  private void ValidateActiveCount(BattleTeam team, string teamLabel)
  {
    if (team.ActiveCount != 2)
    {
      Debug.LogWarning(
        $"{teamLabel} activeCount is {team.ActiveCount}, but DoubleBattleConductor expects exactly 2. "
          + "Battle will still run, but consider setting activeCount: 2 for accurate double battle behavior."
      );
    }
  }
}
