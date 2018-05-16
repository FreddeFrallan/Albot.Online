using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace ClientUI{
	
	public class UsernameGenerator{
		
		private static string[] partOne = new string[] {
			"Red", "Blue", "Black", "Orange", "Purple", "Green",
			"Dangerous", "Cool", "Fierce", "Hot", "Cunning", "Smart", "Epic", "Cold", "Liquid", "Solid", "Plasma",
			"Strong", "Angry", "Psyco", "Flammable", "Smoking", "Huge", "Silent", "Electric", "Sunshine", "Moonlight",
			"Tasty", "Ugly", "Beautiful", "Sexy", "Slow", "Fast", "Instant", "Freezing"
		};
		
		
		private static string[] partTwo = new string[] {
			"Velvet", "Angel", "Demon", "Devil", "Tiger", "Gorilla", "Programmer", "Bot", "Hammer", "Eye", "Fist",
			"Hippo", "Pokémon", "Digimon", "Grasshopper", "Raccoon", "Dancer", "Theif", "Thug", "Lawyer", "Music",
			"Computer", "Omega", "Alpha", "Runner", "Nerd", "Geek", "Player", "NPC", "Cluster", "Smoke", "Water", "Ocean",
			"Forest", "Sand", "Stone", "Air", "Space", "Sun", "Star", "Asteroid",
		};
		
		
		
		public static string generateName(){
			System.Random r = new System.Random ();
			string p1 = partOne [r.Next (0, partOne.Length)];
			string p2 = partTwo [r.Next (0, partTwo.Length)];
			return p1 + p2;
		}
	}

}