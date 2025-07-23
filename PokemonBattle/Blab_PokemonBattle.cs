using UnityEngine;

public class Blab_PokemonBattle : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    var playerMon = new BirdMon(
      speed: 50,
      maxHealth: 100,
      defense: 10,
      attack: 15,
      nickname: "Tweety"
    );
    playerMon.Moves.Add(new BasicAttackMove());
    playerMon.Moves.Add(new SlickRainMove());
    IBattleAI playerAi = new BattleAI_Random();
    BattleTeam playerTeam = new(playerMon, playerAi);

    var computerMon = new CatMon(
      speed: 40,
      maxHealth: 120,
      defense: 12,
      attack: 18,
      nickname: "Whiskers"
    );
    computerMon.Moves.Add(new BasicAttackMove());
    computerMon.Moves.Add(new SlickRainMove());
    IBattleAI computerAi = new BattleAI_Random();
    BattleTeam computerTeam = new(computerMon, computerAi);

    BattleModel bm = new BattleModel(playerTeam: playerTeam, computerTeam: computerTeam);
    var battleManager = new BattleManager(bm);
    battleManager.StartBattle();
  }

  // Update is called once per frame
  void Update() { }
}
