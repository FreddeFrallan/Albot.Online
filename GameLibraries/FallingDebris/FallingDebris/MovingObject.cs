using System;

namespace FallingDebris
{
	public class MovingObject
	{
		public double[] pos, vel, target;
		double moveSpeed;
		private bool moving = false;

		public MovingObject (double[] startPos, double[] startVel, double[] startTarget, double moveSpeed){
			this.pos = startPos; this.vel = startVel; this.target = startTarget; this.moveSpeed = moveSpeed;
			moving = vel[0] != 0 || vel[1] != 0;
		}

		public void setNewTarget(double[] newTarget){
			moving = newTarget [0] != pos [0] || newTarget [1] != pos [1];
			target = newTarget;

			double[] deltaPos = calcDeltaVec2 (target, pos);
			double hyp = calcHyp (deltaPos);
			double cosV = hyp / Math.Abs(deltaPos[0]);
			double sinV = Math.Abs (deltaPos [1]) / hyp;


			vel [0] = moveSpeed * cosV * (deltaPos [0] >= 0 ? 1 : -1);
			vel [1] = moveSpeed * sinV * (deltaPos [1] >= 0 ? 1 : -1);


		}



		public void moveObject(double dt){
			if (moving == false)
				return;

			pos [0] += vel[0] * dt;
			pos [1] += vel[1] * dt;

			if (vel [0] != 0) {
				if (vel [0] > 0 && pos [0] >= target [0])
					reachedTarget ();
				else if (vel [0] < 0 && pos [0] <= target [0])
					reachedTarget ();
			}
			if (vel [1] != 0) {
				if (vel [1] > 0 && pos [1] >= target [1])
					reachedTarget ();
				else if (vel [1] < 0 && pos [1] <= target [1])
					reachedTarget ();
			}
	
		}

		private void reachedTarget(){
			pos = (double[])target.Clone ();
			moving = false;
		}

		private double calcHyp(double[] a){return Math.Sqrt( Math.Pow (a [0], 2) + Math.Pow (a [1], 2));}
		private double[] calcDeltaVec2(double[] a, double[] b){return new double[2]{a[0] -b[0], b[1] - b[1]};}
	}
}

