using System;
using UnityEngine;

public class BattleManager
{
  // private IBattleAI playerAi;
  // private IMonster playerMonster;

  // private IBattleAI computerAi;
  // private IMonster computerMonster;

  // BattleTeam playerTeam;
  // BattleTeam computerTeam;
  // public BattleWeather currentWeather = BattleWeather.None;

  protected BattleModel battleModel;

  private System.Random random = new System.Random();

  // public BattleManager(BattleTeam playerTeam, BattleTeam computerTeam)
  // {
  //   this.playerTeam = playerTeam;
  //   this.battleModel.playerTeam._SetTeamID(0);

  //   this.computerTeam = computerTeam;
  //   this.battleModel.computerTeam._SetTeamID(1);
  // }

  public BattleManager(BattleModel model)
  {
    model.playerTeam._SetTeamID(BattleModel.PLAYER_TEAM_ID);
    model.computerTeam._SetTeamID(BattleModel.COMPUTER_TEAM_ID);
    this.battleModel = model;
  }

  public void StartBattle()
  {
    Debug.Log("Battle Start");

    // Simple pokemon battle loop: Ask each team for their move. Then execute the move in the engine.
    // The engine will handle the battle logic, including the order of moves, the effects of moves, and the
    // status of the monsters.
    while (!IsBattleOver())
    {
      var playerAI = battleModel.playerTeam.BattleAI;
      var computerAI = battleModel.computerTeam.BattleAI;

      var playerMonster = battleModel.playerTeam.ActiveMonster;
      var computerMonster = battleModel.computerTeam.ActiveMonster;

      var playerMove = playerAI.GetMove(this, playerMonster, computerMonster);
      var computerMove = computerAI.GetMove(this, computerMonster, playerMonster);

      ExecuteTurn(playerMove, computerMove);
    }

    AnnounceWinner();
  }

  private bool IsBattleOver()
  {
    return battleModel.playerTeam.ActiveMonster.Health <= 0
      || battleModel.computerTeam.ActiveMonster.Health <= 0;
  }

  private void ExecuteTurn(IMove playerMove, IMove computerMove)
  {
    var playerMonster = battleModel.playerTeam.ActiveMonster;
    var computerMonster = battleModel.computerTeam.ActiveMonster;
    // Determine the order of moves.
    // Ties go to the player (more fun that way :D)
    if (playerMonster.Speed >= computerMonster.Speed)
    {
      // Player goes first.
      ExecuteMove(playerMove, playerMonster, computerMonster);
      if (!IsBattleOver())
      {
        // Computer goes second.
        ExecuteMove(computerMove, computerMonster, playerMonster);
      }
    }
    else
    {
      // Computer goes first.
      ExecuteMove(computerMove, computerMonster, playerMonster);
      if (!IsBattleOver())
      {
        // Player goes second.
        ExecuteMove(playerMove, playerMonster, computerMonster);
      }
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
    if (battleModel.playerTeam.ActiveMonster.Health > 0)
    {
      Debug.Log($"{battleModel.playerTeam.ActiveMonster.Nickname} wins!");
    }
    else
    {
      Debug.Log($"{battleModel.computerTeam.ActiveMonster.Nickname} wins!");
    }
  }

  public IMonster[] GetAllMonsters()
  {
    return new IMonster[]
    {
      battleModel.playerTeam.ActiveMonster,
      battleModel.computerTeam.ActiveMonster,
    };
  }
}
