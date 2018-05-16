using System.Collections;
using System.Collections.Generic;
using System;

namespace Game{
	
	public class DiceRoller{
		public List<Dice> dices = new List<Dice>();
		public int lastValue;

		public DiceRoller(int amountOfDices){
			for (int i = 0; i < amountOfDices; i++)
				dices.Add (new Dice ());
		}
			
		public DiceSum roll(){
			lastValue = 0;
			List<int> values = new List<int> ();
			foreach (Dice d in dices) {
				lastValue += d.RollDice ();
				values.Add (d.rolledValue);
			}
			return new DiceSum (lastValue, values);
		}

	}

	public class DiceSum{
		public int totalSum;
		public List<int> values = new List<int>();
		public DiceSum(int totalSum, List<int> dices){
			this.totalSum = totalSum;
			this.values = dices;
		}
	}
	
	public class Dice  {
		public int rolledValue;
		private Random rand = new Random ();
		public int RollDice(){
			rolledValue = rand.Next (1, 6);
			return rolledValue;
		}
	}
	

}