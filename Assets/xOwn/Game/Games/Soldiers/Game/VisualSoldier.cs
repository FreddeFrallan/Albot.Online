using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VisualSoldier : MonoBehaviour {

	public NavMeshAgent agent;
	private List<UpdatePoint> points = new List<UpdatePoint>();
	private UpdatePoint targetPoint;
	private float nextKeynote;
	private bool playing = false;
	private float teleportDist = 2;
	private float startTime, deathTime;
	private bool isDead = false;
	public float extraSpeed = 1;
	
	// Update is called once per frame
	void Update () {
		if (isDead && Time.time >= deathTime)
			Destroy (gameObject);

		if (playing == false)
			return;
		
		if (Time.time >= nextKeynote)
			selectNextKeynote ();
	}


	public void startMoving(){
		startTime = Time.time - points [0].timeStamp;
		selectNextKeynote ();
		playing = true;
	}


	public void death(float time){
		isDead = true;
		deathTime = startTime + time;
	}

	private void selectNextKeynote(){
		if (points.Count < 2)
			return;

		UpdatePoint prevPoint = points [0];
		if(Vector3.Distance(transform.position, prevPoint.position) >= teleportDist)
			agent.Warp(prevPoint.position);
		
		points.RemoveAt (0);
		targetPoint = points [0];

		float currentTime = Time.time;
		float deltaTime = targetPoint.timeStamp - prevPoint.timeStamp;
		float dist = Vector3.Distance (prevPoint.position, targetPoint.position);

		if (dist == 0)
			return;

		float neededSpeed = dist / deltaTime + extraSpeed;

		agent.speed = neededSpeed;
		agent.Resume ();
		agent.SetDestination (targetPoint.position);

		nextKeynote = startTime + targetPoint.timeStamp;
	}

	private float getLastTimeStamp(){
		return points [points.Count - 1].timeStamp;
	}

	public void addNewPosition(float time, Vector3 pos){
		points.Add(new UpdatePoint(){position = pos, timeStamp = time});
	}

	private struct UpdatePoint{
		public Vector3 position;
		public float timeStamp;
	}
}

