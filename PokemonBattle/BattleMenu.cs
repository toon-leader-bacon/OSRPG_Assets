using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
  protected UINavigationGrid buttons;

  void Start()
  {
    buttons = new UINavigationGrid(GetComponentsInChildren<Button>(), wrapAround: true, cols: 2);
    buttons.IndicateButton(0, 0);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      buttons.IndicateUp();
    }
    else if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      buttons.IndicateDown();
    }
    else if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      buttons.IndicateRight();
    }
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      buttons.IndicateLeft();
    }
    else if (Input.GetKeyDown(KeyCode.Return))
    {
      buttons.SelectCurrent();
    }
  }
}
