using System;
using System.Collections.Generic;
using Battleship;

namespace BattleshipGame
{
	public class GameConstants{
		public const char UNEXPLOREDCHAR = '0', MISSCHAR = '-', HITCHAR = 'X', SUNKCHAR = 'S', EMPTYCHAR = '0';

		public static List<Ship> ships = new List<Ship> () {
			new Ship (ShipType.carrier, '5', 5),
			new Ship (ShipType.battle, '4', 4),
			new Ship (ShipType.transport, '3', 3),
			new Ship (ShipType.submarine, '2', 3),
			new Ship (ShipType.scout, '1', 2),
		};
		
	}

	public class Ship{
		public string boardString = "";
		public ShipType type;
		public char sign;
		public int size;
		public Ship(ShipType type, char sign, int size){
			this.sign = sign;
			this.size = size;
			this.type = type;
			for (int i = 0; i < size; i++)
				boardString += sign;
		}
	}

	/*
	public enum ShipType{
		carrier,
		battle,
		transport,
		submarine,
		scout
	}
	*/
}

