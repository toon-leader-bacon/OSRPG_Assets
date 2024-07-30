using System;
using UnityEngine;

public class BattleManager
{
  // private IBattleAI playerAi;
  // private IMonster playerMonster;


  // private IBattleAI computerAi;
  // private IMonster computerMonster;

  BattleTeam playerTeam;
  BattleTeam computerTeam;

  private System.Random random = new System.Random();

  public BattleManager(BattleTeam playerTeam, BattleTeam computerTeam)
  {
    this.playerTeam = playerTeam;
    this.playerTeam._SetTeamID(0);

    this.computerTeam = computerTeam;
    this.computerTeam._SetTeamID(1);
  }

  public void StartBattle()
  {
    Debug.Log("Battle Start");

    while (!IsBattleOver())
    {
      IMove playerMove = playerTeam.BattleAI.GetMove(this, playerTeam.ActiveMonster, computerTeam.ActiveMonster);
      IMove computerMove = computerTeam.BattleAI.GetMove(this, computerTeam.ActiveMonster, playerTeam.ActiveMonster);

      ExecuteTurn(playerMove, computerMove);
    }

    AnnounceWinner();
  }

  private bool IsBattleOver()
  {
    return playerTeam.ActiveMonster.Health <= 0 || computerTeam.ActiveMonster.Health <= 0;
  }

  private void ExecuteTurn(IMove playerMove, IMove computerMove)
  {
    if (playerTeam.ActiveMonster.Speed >= computerTeam.ActiveMonster.Speed)
    {
      ExecuteMove(playerMove, playerTeam.ActiveMonster, computerTeam.ActiveMonster);
      if (!IsBattleOver()) ExecuteMove(computerMove, computerTeam.ActiveMonster, playerTeam.ActiveMonster);
    }
    else
    {
      ExecuteMove(computerMove, computerTeam.ActiveMonster, playerTeam.ActiveMonster);
      if (!IsBattleOver()) ExecuteMove(playerMove, playerTeam.ActiveMonster, computerTeam.ActiveMonster);
    }
  }

  private void ExecuteMove(IMove move, IMonster user, IMonster target)
  {
    Debug.Log($"{user.Nickname} uses {move.Name}!");
    move.Execute(this, user, target);
    Debug.Log($"{target.Nickname}'s HP: {target.Health}/{target.MaxHealth}");
  }

  private void AnnounceWinner()
  {
    Debug.Log("Battle over");
    if (playerTeam.ActiveMonster.Health > 0)
    {
      Debug.Log($"{playerTeam.ActiveMonster.Nickname} wins!");
    }
    else
    {
      Debug.Log($"{computerTeam.ActiveMonster.Nickname} wins!");
    }
  }

  public IMonster[] GetAllMonsters()
  {
    return new IMonster[] { playerTeam.ActiveMonster, computerTeam.ActiveMonster };
  }
}