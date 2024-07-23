using System.Diagnostics;

public enum CardinalDirection
{
  North = 1,
  East = 2,
  South = 3,
  West = 4,
}

public static class CardinalDirection_Util
{
  public static CardinalDirection opposite(CardinalDirection direction)
  {
    switch (direction)
    {
      case CardinalDirection.North: return CardinalDirection.South;
      case CardinalDirection.South: return CardinalDirection.North;
      case CardinalDirection.East: return CardinalDirection.West;
      case CardinalDirection.West: return CardinalDirection.East;
      default: return CardinalDirection.North;
    }
  }
}
