using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Connect4LocAlbot : MonoBehaviour {

	private static Connect4LocAlbot singleton;
	void Awake(){
		singleton = this;
	}

	private static Connect4Bot.Connect4Bot Albot;

	public static void requstMove(object msg, Action<object, ConnectedPlayer> playFunc, ConnectedPlayer AlbotPlayer){
		int playedMove = Albot.requestMove ((msg as Game.CommProtocol.StringMessage).msg);
		singleton.playMove (playFunc, playedMove, AlbotPlayer);
	}


	public void playMove(Action<object, ConnectedPlayer> playFunc, int playedMove, ConnectedPlayer AlbotPlayer){
		StartCoroutine (playMoveIem (playFunc, playedMove, AlbotPlayer));
	}

	IEnumerator playMoveIem(Action<object, ConnectedPlayer> playFunc, int playedMove, ConnectedPlayer AlbotPlayer){
		yield return new WaitForSeconds (0.5f);
		playFunc (playedMove.ToString(), AlbotPlayer);
	}
}
