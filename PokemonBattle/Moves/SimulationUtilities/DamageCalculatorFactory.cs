public enum PokemonGeneration
{
  Gen1,
  Gen2,
  Gen3,
  Gen4,
  Gen5,
  PokemonGo,
}

public static class DamageCalculatorFactory
{
  public static BaseDamageCalculator CreateCalculator(PokemonGeneration generation)
  {
    return generation switch
    {
      PokemonGeneration.Gen1 => new Gen1DamageCalculator(),
      PokemonGeneration.Gen2 => new Gen2DamageCalculator(),
      PokemonGeneration.Gen3 => new Gen3DamageCalculator(),
      PokemonGeneration.Gen4 => new Gen4DamageCalculator(),
      PokemonGeneration.Gen5 => new Gen5DamageCalculator(),
      PokemonGeneration.PokemonGo => new PokemonGoDamageCalculator(),
      _ => new Gen5DamageCalculator(), // Default to latest main series generation
    };
  }
}
