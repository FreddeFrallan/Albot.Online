using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster2048 : MonoBehaviour {


	public static float slideSpeed;
	public Slider moveSpeedSlider;
	public Text scoreText;
	public GameLogic2048 logic;
	private int[,] currentBoard;
	private List<int> moveQ = new List<int>();
	private bool playing = false;

	void Start(){
        TCPLocalConnection.subscribeToTCPStatus(TCPStatusChanged);
        TCPMessageQueue.readMsgInstant = takeInput;
        TCPLocalConnection.startServer(4000);
        //SinglePlayerGameMaster.init (takeInput, restartGame);
        logic.init (gameOver, roundFinished, updateScoreText);
		slideSpeed = moveSpeedSlider.value;
		//restartGame ();
	}


	public void takeInput(ReceivedLocalMessage msg) {
        try {
            MainThread.fireEventAtMainThread(() => {
                logic.playMove(int.Parse(msg.message.Trim()));
                }
            );
        } catch {
            Debug.LogError("Could not handle move: " + msg.message);
        }
		//playMove ();
	}

	private void playMove(){
		if (moveQ.Count == 0) 
			TCPLocalConnection.sendMessage (logic.formatBoard ());
		else {
			int tempMove = moveQ [0];
			moveQ.RemoveAt (0);
			logic.playMove (tempMove);
		}
	}

	public void roundFinished(){playMove ();}
	private IEnumerator sendBoardToPlayer(){
		yield return new WaitForSeconds (1);
		TCPLocalConnection.sendMessage (logic.formatBoard ());
	}

	private bool charToMove(char c, ref int move){
		if (char.IsDigit (c) == false)
			return false;

		move = int.Parse(c.ToString());
		return move >= 0 && move < 4;
	}


	public void restartGame(){
		logic.startNewGame ();
		playing = true;
		StartCoroutine (sendBoardToPlayer ());
	}
	public void gameOver(){
		moveQ.Clear ();
		playing = false;
        string gameOverString = TCP_API.APIStandardConstants.Fields.gameOver;
        TCPLocalConnection.sendMessage (gameOverString + ": " + logic.getCurrentScore ());
		//restartGame ();
	}

	public void restartButtonPressed(){
        TCPLocalConnection.restartServer();
		moveQ.Clear ();
		restartGame ();
	}

	public void updateScoreText(int newScore){scoreText.text = newScore.ToString ();}
	public void speedValueChanged(int t){slideSpeed = moveSpeedSlider.value;}


    private void TCPStatusChanged(ConnectionStatus status) {
        print(status);
        if (status == ConnectionStatus.Connected) {
            print("Connecrted");
            restartGame();
        }
        else if (status == ConnectionStatus.Connecting)
            print("Connecting");
        else
            print("Not Connecteed");
    }
    void OnDestroy() { TCPLocalConnection.unSubscribeToTCPStatus(TCPStatusChanged); }
}
