using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigationGrid
{
  public Button[,] buttons;
  public bool wrapAround = false;

  private int rows;
  private int cols;
  private int selectedRow = 0;
  private int selectedCol = 0;

  public UINavigationGrid(Button[,] buttons, bool wrapAround = false)
  {
    this.buttons = buttons;
    this.wrapAround = wrapAround;
    rows = buttons.GetLength(0);
    cols = buttons.GetLength(1);

    this.selectedRow = 0;
    this.selectedCol = 0;
    if (rows > 0 && cols > 0)
    {
      IndicateButton(selectedRow, selectedCol);
    }
  }

  public UINavigationGrid(Button[] buttons, bool wrapAround = false, int cols = 2)
    : this(new List<Button>(buttons), wrapAround, cols) { }

  public UINavigationGrid(List<Button> buttonList, bool wrapAround = false, int cols = 2)
  {
    this.cols = cols;
    int buttonCount = buttonList.Count;
    this.rows = Mathf.CeilToInt((float)buttonCount / cols);
    this.wrapAround = wrapAround;

    buttons = new Button[rows, cols];
    for (int i = 0; i < buttonCount; i++)
    {
      int row = i / cols;
      int col = i % cols;
      buttons[row, col] = buttonList[i];
    }

    this.selectedRow = 0;
    this.selectedCol = 0;
    if (rows > 0 && cols > 0)
    {
      IndicateButton(selectedRow, selectedCol);
    }
  }

  public void IndicateKey(KeyCode key)
  {
    switch (key)
    {
      case KeyCode.UpArrow:
        IndicateUp();
        break;
      case KeyCode.DownArrow:
        IndicateDown();
        break;
      case KeyCode.LeftArrow:
        IndicateLeft();
        break;
      case KeyCode.RightArrow:
        IndicateRight();
        break;
      default:
        Debug.LogWarning($"Unsupported key: {key}");
        break;
    }
  }

  public void IndicateUp()
  {
    IndicateDirection(ref selectedRow, -1, rows);
  }

  public void IndicateDown()
  {
    IndicateDirection(ref selectedRow, 1, rows);
  }

  public void IndicateLeft()
  {
    IndicateDirection(ref selectedCol, -1, cols);
  }

  public void IndicateRight()
  {
    IndicateDirection(ref selectedCol, 1, cols);
  }

  private void IndicateDirection(ref int index, int delta, int limit)
  {
    buttons[selectedRow, selectedCol].interactable = true; // Turn the current selection off
    index = wrapAround ? (index + delta + limit) % limit : Mathf.Clamp(index + delta, 0, limit - 1);
    IndicateButton(selectedRow, selectedCol); // Turn the new selection on
  }

  public void IndicateButton(int row, int col)
  {
    buttons[row, col].Select();
    buttons[row, col].interactable = false;
  }

  public void SelectCurrent()
  {
    buttons[selectedRow, selectedCol].onClick.Invoke();
    Debug.Log($"Selected: {buttons[selectedRow, selectedCol].name}");
  }
}
