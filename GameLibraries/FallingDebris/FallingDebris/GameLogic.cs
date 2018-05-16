using System;
using System.Diagnostics;
using Main;
using System.Threading;

namespace FallingDebris
{
	public class GameLogic
	{

		public Action<string> print;
		private CommProtocol protocol = new CommProtocol ();
		public int thePlayerID;

		public static Object dataLock = new object ();
		private Stopwatch watch = new Stopwatch();
		private bool gameRunning = true;
		private MovingObject player = new MovingObject (new double[2]{ 0, 0 }, new double[2]{ 0, 0 }, new double[2]{ 0, 0 }, 5);

		private double lastTime = 0;
		private double dt;
		private Thread gameThread, sendThread;


		public void startGameLogic(){
			watch.Start ();
			gameThread = new Thread (new ThreadStart (update));
			gameThread.Start ();
			print ("Starting game!!!!");
		}



		private void update(){
			while (gameRunning) {
				calcDT ();
				lock (dataLock) {
					player.moveObject (dt);
				}

			//	Thread.Sleep (10);
			}
		}



		public void setTargetpos(int newTarget){
			lock(dataLock)
				player.setNewTarget(new double[2]{newTarget, 0} );
		}

		public double[] getPlayerState(){
			lock (dataLock)
				return new double[2]{ player.pos [0], player.target [0] };
		}

		private void calcDT(){
			double tempTime = watch.ElapsedMilliseconds;
			dt = (tempTime - lastTime)/1000;
			lastTime = tempTime;
		}

		public void gameOver(){
			gameRunning = false;
		}
	}
}

