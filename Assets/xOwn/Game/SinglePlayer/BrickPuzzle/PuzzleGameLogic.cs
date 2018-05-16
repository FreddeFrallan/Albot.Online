using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleGameLogic : MonoBehaviour {

	public PuzzleGameMaster gameMaster;
	public GameObject gameTile;
	public GameObject canvas;
	public GameObject bgTile;

	private float squareSize = 70;
	private Transform[,] bgTiles;
	private BrickTile[,] vizGrid;
	private int[,] scoreGrid;
	private Vector3 topLeftCorner = new Vector3 (-100, 100, 0);

	private bool canPlayMove = true;
	private bool hasInit = false;
	private int[] zeroPos;

	private int boardSize = 4;
	private int shuffleTimes = 300;

	private void initLists(){
		vizGrid = new BrickTile[boardSize,boardSize];
		scoreGrid = new int[boardSize, boardSize];
		bgTiles = new Transform[boardSize, boardSize];
	}
	private void destroyOldBoard(){
		iterateOverBoard ((int x, int y) => {
			if(bgTiles[x, y] != null)
				Destroy(bgTiles[x, y].gameObject);
			if(vizGrid[x, y] != null)
				Destroy(vizGrid[x, y].gameObject);
		});
	}

	public int[,] restartGame (int size, int shuffleAmount){
		if (hasInit)
			destroyOldBoard ();
		boardSize = size;
		shuffleTimes = shuffleAmount;
		initLists ();

		generateBoardBackground ();
		generateGameBoard ();
		canPlayMove = true;
		hasInit = true;
		return scoreGrid;
	}

	public void playMove(int move){
		if (canPlayMove == false)
			return;
		canPlayMove = false;

		List<BrickTile> moveSquares = getMoveSquares ();
		if (moveSquares [move] == null) {
			finishedRound ();
			return;
		}

		updateGrids (moveSquares [move].pos);
		int x = zeroPos [0], y = zeroPos [1];
		moveSquares [move].startSlide (bgTiles[x, y], x, y);
	}

	private void updateGrids(int[] pos){
		scoreGrid [zeroPos [0], zeroPos [1]] = scoreGrid [pos [0], pos [1]];
		scoreGrid [pos [0], pos [1]] = 0;

		vizGrid [zeroPos [0], zeroPos [1]] = vizGrid [pos [0], pos [1]];
		vizGrid [pos [0], pos [1]] = null;
	}


	private List<BrickTile> getMoveSquares(){
		List<BrickTile> moveSquares = new List<BrickTile> ();
		iterateOverBoard ((int x, int y) => {
			if(scoreGrid[x, y] != 0)
				return;

			zeroPos = new int[]{x, y};
			moveSquares.Add(x == 0 ? null : vizGrid[x-1, y]);
			moveSquares.Add(y == boardSize-1 ? null : vizGrid[x, y+1]);
			moveSquares.Add(x == boardSize-1 ? null : vizGrid[x+1, y]);
			moveSquares.Add(y == 0 ? null : vizGrid[x, y-1]);
		});
		return moveSquares;
	}


	private void generateBoardBackground(){
		float factor = (float)boardSize / 2;
		topLeftCorner.x = -squareSize * factor;
		topLeftCorner.y = squareSize * factor;

		iterateOverBoard ((int x, int y) => {
			vizGrid [x, y] = null;
			Vector2 spawnPos = topLeftCorner + new Vector3 (x, -y, 0) * squareSize;
			bgTiles[x, y] = Instantiate (bgTile, spawnPos, Quaternion.identity, canvas.transform).transform;
			bgTiles[x, y].transform.localPosition = spawnPos;
		});
	}

	private void generateGameBoard(){
		iterateOverBoard ((int x, int y) => {scoreGrid[x, y] = x + y* boardSize;});

		//Shuffle board
		for (int i = 0; i < shuffleTimes; i++) {
			int[] zeroPos = getZeroPos (scoreGrid);
			List<int> possMoves = getPossibleMoves (zeroPos);
			simulateMove (zeroPos, possMoves [UnityEngine.Random.Range (0, possMoves.Count)]);
		}
			
		iterateOverBoard ((int x, int y) => {
			if(scoreGrid[x, y] != 0) {
				vizGrid[x, y] = Instantiate(gameTile, bgTiles[x, y].position, Quaternion.identity, canvas.transform).GetComponent<BrickTile>();
				vizGrid[x, y].init(scoreGrid[x, y], x, y, this);
			}
		});
	}

	private int[] getZeroPos(int[,] grid){
		int[] zeroPos = new int[2];
		iterateOverBoard((int x, int y) => {if(grid[x, y] == 0) zeroPos = new int[]{x, y};});
		return zeroPos;
	}

	private List<int> getPossibleMoves(int[] zeroPos){
		List<int> moves = new List<int> ();
		if (zeroPos [0] > 0)moves.Add (0);
		if (zeroPos [1] > 0)moves.Add (1);
		if (zeroPos [0] < boardSize-1)moves.Add (2);
		if (zeroPos [1] < boardSize-1)moves.Add (3);
		return moves;
	}
	private void simulateMove(int[] zeroPos, int move){
		int[] targetPos = new int[]{ zeroPos [0], zeroPos [1] };
		if (move == 0)targetPos [0]--;
		if (move == 1)targetPos [1]--;
		if (move == 2)targetPos [0]++;
		if (move == 3)targetPos [1]++;

		scoreGrid [zeroPos [0], zeroPos [1]] = scoreGrid [targetPos [0], targetPos [1]];
		scoreGrid [targetPos [0], targetPos [1]] = 0;
	}


	private void iterateOverBoard(Action<int, int> a){
		for (int x = 0; x < boardSize; x++)
			for (int y = 0; y < boardSize; y++)
				a (x, y);
	}

	public bool checkForVictory(){
		bool victory = true;
		iterateOverBoard ((int x, int y) => {
			if(x == 0 && y == 0 && scoreGrid[0, 0] != 0)
				victory = false;
			else if(scoreGrid[x, y] != y * boardSize + x)
				victory = false;
		});
		return victory;
	}


	private void printBoard(){
		for (int y = 0; y < boardSize; y++) {
			string s = "";
			for (int x = 0; x < boardSize; x++)
				s += scoreGrid [x, y].ToString () + "\t";
			print (s);
		}
	}
	public void finishedRound(){
		canPlayMove = true;
		if (checkForVictory ()) 
			gameMaster.victory ();
		else
			gameMaster.roundFinished ();
	}
}
