using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soldiers{

	public class SoldiersGameStateUpdater : MonoBehaviour{
		public static List<Soldier> deaths = new List<Soldier>();
		public List<Soldier> team1 = new List<Soldier>(), team2 = new List<Soldier>(), allTeams = new List<Soldier>();

		private bool gamePlaying;
		private float refreshRate = 0.2f;
		private List<ConnectedPlayer> players;

		public SoldiersProtocol protocol;
		public SoldiersGameMaster gameMaster;
		private uint updateCounter = 0;

		public void startGame(List<ConnectedPlayer> players){
			protocol = gameMaster.protocol;
			gamePlaying = true;
			this.players = players;
			team1 = GameOverlord.getEnemyList (2);
			team2 = GameOverlord.getEnemyList (1);
			StartCoroutine (updateGameStatus ());


			allTeams.AddRange (team1);
			allTeams.AddRange (team2);
			broadcastBoard(new BoardUpdate (allTeams, new List<Soldier>(), 0, Time.time), true);
		}



		private IEnumerator updateGameStatus(){
			while (gamePlaying) {
				yield return new WaitForSeconds (refreshRate);
				allTeams.Clear ();
				allTeams.AddRange (team1);
				allTeams.AddRange (team2);
				broadcastBoard (new BoardUpdate (allTeams, deaths, updateCounter++, Time.time));

				deaths.Clear ();

				if (checkIfGameOver())
					break;
			}
		}

		private void broadcastBoard(BoardUpdate b, bool startBoard = false){
			foreach (ConnectedPlayer p in players) {
				b.color = p.color;
				try{
					if(startBoard)
						protocol.sendStartBoard (gameMaster.getMatchingPlayer(p.color).client.peerID, b);
					else
						protocol.sendBoard (gameMaster.getMatchingPlayer(p.color).client.peerID, b);
				}catch{}
			}
		}

		private bool checkIfGameOver(){
			if (team1.Count != 0 && team2.Count != 0)
				return false;
			
			Game.PlayerColor winColor = Game.PlayerColor.None;
			if (team1.Count > team2.Count)
				winColor = Game.PlayerColor.Blue;
			else
				winColor = Game.PlayerColor.Red;
			setGameOver (winColor);

			return true;
		}

		public void setGameOver(Game.PlayerColor winColor){
			gamePlaying = false;
			foreach (ConnectedPlayer p in players) {
				GameInfo infoMsg = new GameInfo (p.username, p.color, true, winColor);
				try{protocol.sendGameInfo (gameMaster.getMatchingPlayer(p.color).client.peerID, infoMsg);}
				catch{}
			}
			gameMaster.startShutdownServer ();
		}
	}
}