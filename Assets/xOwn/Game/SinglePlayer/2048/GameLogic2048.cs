using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameLogic2048 : MonoBehaviour {

	public CanvasScaler canvasScaler;
	public GameObject gameTile;
	public GameObject canvas;
	public GameObject bgTile;

	private float squareSize = 70;
	private GameObject[,] gridPos = new GameObject[4,4];
	private Tile2048[,] vizGrid = new Tile2048[4, 4];
	private Vector3 topLeftCorner = new Vector3 (-100, 100, 0);
	private int roundCounter = 0, scoreCounter = 0;
	private bool canPlayMove = true;
	private bool hasInit = false;
	private bool isMoving = false;
	private Action gameOver, finishRound;
	private Action<int> updateScore;

	public void init(Action gameOver, Action finishedRound, Action<int> updateScore){
		this.gameOver = gameOver;
		this.finishRound = finishedRound;
		this.updateScore = updateScore;
	}
		
	void Start(){
		startNewGame ();
	}

	public void startNewGame(){
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
		scoreCounter = 0;
		generateBoardBackground ();
		createNewTile ();
		createNewTile ();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
	}


	private void generateBoardBackground(){
		iterateOverBoard ((int x, int y) => {
			roundCounter = 0;
			if (vizGrid [x, y] != null)
				Destroy (vizGrid [x, y].gameObject);
			vizGrid [x, y] = null;

			if (hasInit == false) {
				Vector2 spawnPos = canvas.transform.position + topLeftCorner + new Vector3 (x, -y, 0) * squareSize;
				gridPos[x, y] = Instantiate (bgTile, spawnPos, Quaternion.identity, canvas.transform);
			}
		});
		hasInit = true;
	}

	public void spawnNewTile(int x, int y, int value){
		vizGrid[x, y] = Instantiate (gameTile, gridPos[x, y].transform.position, Quaternion.identity, canvas.transform).GetComponent<Tile2048>();
		vizGrid [x, y].init (value, new int[]{ x, y }, this);
	}
	private void createNewTile(){
		List<int[]> points = new List<int[]> ();
		iterateOverBoard((int x, int y) => points.Add(new int[]{x, y}));

		while(points.Count > 0){
			int[] p = points[UnityEngine.Random.Range(0, points.Count-1)];
			if (vizGrid [p[0], p[1]] == null) {
				int value = UnityEngine.Random.Range (0, 100) >= 90 ? 4 : 2;
				spawnNewTile (p[0], p[1], value);
				break;
			}
			points.Remove (p);
		}
	}

		
	public void playMove(int move){
		if (canPlayMove  && checkIfMovePossible (move) ) {
			canPlayMove = false;
			moveBricks (move);
			setMergeTargets (move);
			StartCoroutine(untilMove(() => mergeBricks(move)));
		}
		else
			finishRound();
	}

	private void moveBricks(int move){
		if (move == 0) slideBricks (0);
		else slideBricks (4 - move);
		isMoving = true;
	}

	private IEnumerator untilMove(Action finish){
		while (isMoving) {
			isMoving = false;
			iterateOverBoard ((int x, int y) => {if (vizGrid[x, y] != null && vizGrid [x, y].isMoving)isMoving = true;});
			yield return new WaitForEndOfFrame ();
		}
		finish ();
	}


	private void mergeBricks(int move){
		iterateOverBoard ((int x, int y) => {if (vizGrid [x, y] != null){
				if(vizGrid [x, y].merge())
					vizGrid[x, y] = null;
			}});
		moveBricks (move);
		StartCoroutine(untilMove( () => {createNewTile(); finishedRound();}));
	}

	private void setMergeTargets(int dir){
		Tile2048[,] rotatedBoard = rotateBoard(vizGrid, 4 -dir);
		iterateOverBoardFromRight ((int x, int y) => {if (rotatedBoard [x, y] != null) rotatedBoard [x, y].findMergeTargets (new int[]{ x, y }, rotatedBoard);});
	}


	private void slideBricks(int dir){
		Tile2048[,] rotatedBoard = rotateBoard(vizGrid, dir);
		iterateOverBoardFromRight((int x, int y) => {
				if (rotatedBoard [x, y] != null) {
					Tile2048 temp = rotatedBoard [x, y];
					int[] targetPos = temp.calcNewPos (new int[]{x, y}, rotatedBoard);
					rotatedBoard[x, y] = null;
					rotatedBoard [targetPos [0], targetPos [1]] = temp;
			}});
				
		rotatedBoard = rotateBoard (rotatedBoard, 4 - dir);
		vizGrid = copyBoard (rotatedBoard);

		//Slide all bricks
		iterateOverBoard((int x, int y) => {if (vizGrid [x, y] != null) { vizGrid [x, y].startSlide (gridPos [x, y], new int[]{x, y}); }});
	}
		
	private void finishedRound(){
		canPlayMove = true;
		roundCounter++;
		if (checkIfGameOver () == false)
			gameOver ();
		else
			finishRound ();
	}

	#region utils

	private Tile2048[,] rotateBoard(Tile2048[,] board, int amount){
		Tile2048[,] rotBoard = copyBoard (board);
		for (int i = 0; i < amount; i++) {
			Tile2048[,] tempBoard = new Tile2048[4, 4];

			iterateOverBoardFromRight((int x, int y) =>tempBoard [y, 3 - x] = rotBoard [x, y]);
			rotBoard = copyBoard (tempBoard);
		}

		return rotBoard;
	}

	private Tile2048[,] copyBoard(Tile2048[,] a){
		Tile2048[,] tempboard = new Tile2048[4, 4];
		iterateOverBoard((int x, int y) => tempboard[x, y] = a[x, y]);
		return tempboard;
	}
		

	public string formatBoard(){
		string s = "";
		for (int y = 0; y < 4; y++)
			for (int x = 0; x < 4; x++) 
				s += vizGrid [x, y] == null ? " - " : vizGrid [x, y].theText.text + " ";
		return s;
	}

	public int getCurrentScore(){return scoreCounter;}
	public void addToScore(int x){scoreCounter += x * 2; updateScore (scoreCounter);}

	private void iterateOverBoard(Action<int, int> a){
		for (int x = 0; x < 4; x++)
			for (int y = 0; y < 4; y++)
				a (x, y);
	}
	private void iterateOverBoardFromRight(Action<int, int> a){
		for (int x = 3; x >= 0; x--)
			for (int y = 0; y < 4; y++)
				a (x, y);
	}
	#endregion


	private bool checkIfMovePossible(int dir){
		if (dir < 0 || dir > 3)
			return false;
		
		int[,] numberBoard = new int[4, 4];
		iterateOverBoard((int x, int y) => {numberBoard[x, y] = vizGrid[x, y] != null ? vizGrid[x, y].currentScore : 0;});
		if (dir == 0)return BoardLogic.moveInXDirection (1, numberBoard);
		if (dir == 1)return BoardLogic.moveInYDirection (-1, numberBoard);
		if (dir == 2)return BoardLogic.moveInXDirection (-1, numberBoard);
		return BoardLogic.moveInYDirection (1, numberBoard);
	}

	private bool checkIfGameOver(){
		int[,] numberBoard = new int[4, 4];
		iterateOverBoard((int x, int y) => {numberBoard[x, y] = vizGrid[x, y] != null ? vizGrid[x, y].currentScore : 0;});
		return BoardLogic.checkIfGameOver (numberBoard);
	}
}
