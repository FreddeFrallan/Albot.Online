﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TCP_API.Snake;
using Game;

namespace Snake{

	public class SnakeTCPFormater{

		private int team;
		private List<Position2D> freshCoords = new List<Position2D> (), freshCoords2 = new List<Position2D>(), oldCoords = new List<Position2D> ();
		public SnakeTCPFormater(int team){
            this.team = team;
        }


		public void addNewUpdate(List<Position2D> coords, int dir, int enemyDir, Position2D playerPos, Position2D enemyPos){
			foreach (Position2D c in coords) {
				if (freshCoords.Any((p) => GameUtils.comparePos(c, p)) || oldCoords.Any((p) => GameUtils.comparePos(c, p)))
                    continue;
				freshCoords.Add (c);
			}
				
			Game.RealtimeTCPController.gotNewBoard (team, formatBoard(dir, enemyDir, playerPos, enemyPos));
		}


		private string formatBoard(int playerDir, int enemyDir, Position2D playerPos, Position2D enemyPos){
			SnakePlayer player = decodeSnakePlayer(playerPos, playerDir);
            SnakePlayer enemy = decodeSnakePlayer(enemyPos, enemyDir);
            SnakePlayer[] players = new SnakePlayer[]{player, enemy};
            List<Position2D> blockedCoords = getCurrentBlockedCoords(playerPos, enemyPos);

            return SnakeProtocolEncoder.compressBoard(blockedCoords, players).Print();
        }

        private List<Position2D> getCurrentBlockedCoords(Position2D playerPos, Position2D enemyPos) {
            List<Position2D> blockedCoords = new List<Position2D>();
            blockedCoords.AddRange(freshCoords);
            blockedCoords.AddRange(freshCoords2);
            blockedCoords = removePlayerFromFresh(playerPos, enemyPos, blockedCoords);
            return blockedCoords;
        }


        public void newBoardSent(){
			oldCoords.AddRange (freshCoords2);
			freshCoords2.Clear ();
			freshCoords2.AddRange (freshCoords);
			freshCoords.Clear ();
		}


		private List<Position2D> removePlayerFromFresh(Position2D playerPos, Position2D enemyPos, List<Position2D> blockedCoords){
			blockedCoords = blockedCoords.Where ((p) => GameUtils.comparePos(p, playerPos) == false && GameUtils.comparePos(p, enemyPos) == false).ToList();
			return blockedCoords;
		}
			
        private SnakePlayer decodeSnakePlayer(Position2D pos, int dir) {
            return new SnakePlayer() {
                x = pos.x, y = pos.y,
                dir = dirToString(dir)
            };
        }

        private int[] decodeBlockedPos(int pos) {
            return new int[] { pos % Constants.BOARD_WIDTH, Constants.BOARD_WIDTH - (pos / Constants.BOARD_WIDTH) - 1 };
        }


		private static string dirToString(int dir){
			if (dir == 0)return "right";
			if (dir == 1)return "up";
			if (dir == 2)return "left";
			else return "down";
		}
	}

}