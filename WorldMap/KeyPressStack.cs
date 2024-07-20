using System.Collections.Generic;
using UnityEngine;

public class KeyPressStack
{
  private LinkedList<KeyCode> keyStack;

  public KeyPressStack()
  {
    keyStack = new LinkedList<KeyCode>();
  }

  public void Push(KeyCode key)
  {
    if (keyStack.Contains(key))
    {
      keyStack.Remove(key);
    }
    keyStack.AddFirst(key);
  }

  public void Pop(KeyCode key)
  {
    if (keyStack.Contains(key))
    {
      keyStack.Remove(key);
    }
  }

  public KeyCode? GetTopOfStack()
  {
    if (keyStack.Count > 0)
    {
      return keyStack.First.Value;
    }
    return null;
  }
}
