using System;

// Unity-compatible pipe interface without INumber<T> (not available in Unity's .NET runtime)
public interface IPipe<T>
{
  T pump(T input);
}

#region Simple Arithmetic Pipes
public class Pipe_AddConstant : IPipe<float>
{
  public float value;

  public Pipe_AddConstant(float value)
  {
    this.value = value;
  }

  public float pump(float input)
  {
    return input + value;
  }
}

public class Pipe_SubtractConstant : IPipe<float>
{
  public float value;

  public Pipe_SubtractConstant(float value)
  {
    this.value = value;
  }

  public float pump(float input)
  {
    return input - value;
  }
}

public class Pipe_MultiplyConstant : IPipe<float>
{
  public float value;

  public Pipe_MultiplyConstant(float value)
  {
    this.value = value;
  }

  public float pump(float input)
  {
    return input * value;
  }
}

public class Pipe_DivideConstant : IPipe<float>
{
  public float value;

  public Pipe_DivideConstant(float value)
  {
    this.value = value;
  }

  public float pump(float input)
  {
    return input / value;
  }
}
#endregion

public class Pipe_Lambda : IPipe<float>
{
  public Func<float, float> lambda;

  public Pipe_Lambda(Func<float, float> lambda)
  {
    this.lambda = lambda;
  }

  public float pump(float input)
  {
    return lambda(input);
  }
}
