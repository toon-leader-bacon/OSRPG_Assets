/// <summary>
/// Optional interface for conductors that require time advancement (ATB, real-time systems).
/// Conductors that implement this interface need Tick() to be called regularly
/// by the BattleManager or Unity Update loop.
///
/// Turn-based conductors (SimpleTurnConductor, PartyBattleConductor, etc.) do NOT
/// need to implement this interface.
/// </summary>
public interface ITimeDrivenConductor : IBattleConductor
{
  /// <summary>
  /// Advances time-based state by deltaTime seconds.
  /// For ATB systems: fills gauges, checks for ready combatants.
  /// Should be called every frame or on a regular timer.
  /// </summary>
  /// <param name="deltaTime">Time elapsed since last tick, in seconds</param>
  void Tick(float deltaTime);
}
