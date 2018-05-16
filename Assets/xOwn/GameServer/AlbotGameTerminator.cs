using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using UnityEngine;
using UnityEngine.Networking;


public class AlbotGameTerminator : MonoBehaviour{
	[SerializeField]
	public AlbotGameRoom Room;
	private static AlbotGameTerminator singleton;

    [Tooltip("Terminates server if first player doesn't join in a given number of seconds")]
    public float FirstPlayerTimeoutSecs = 25;

    [Tooltip("Terminates if room is not registered in a given number of seconds")]
    public float RoomRegistrationTimeoutSecs = 15;

    [Tooltip("Once every given number of seconds checks if the room is empty." +
             " If it is - terminates it")]
    public float TerminateEmptyOnIntervals = 60;

    [Tooltip("Each second, will check if connected to master. If not - quits the application")]
    public bool TerminateIfNotConnected = true;

    private bool _hasFirstPlayerShowedUp = false;

	void Awake(){
		singleton = this;
		AlbotServer.AlbotNetworkManager.onClientLeft += OnClientLeft;
		AlbotServer.ConnectionManager.onPlayerJoined += OnPlayerJoined;
	}

    // Use this for initialization
    void Start () {
        if (!Msf.Args.IsProvided(Msf.Args.Names.SpawnCode)){
            // If this game server was not spawned by a spawner
            Destroy(gameObject);
            return;
        }

	    if (Room == null){
	        Logs.Error("Room is not set");
	        return;
	    }


        if (RoomRegistrationTimeoutSecs > 0)
            StartCoroutine(StartStartedTimeout(RoomRegistrationTimeoutSecs));

        if (FirstPlayerTimeoutSecs > 0)
            StartCoroutine(StartFirstPlayerTimeout(FirstPlayerTimeoutSecs));

        if (TerminateIfNotConnected)
            StartCoroutine(StartWaitingForConnectionLost());

		StartCoroutine (StartRandomPlayerChecks ());
    }

	private void OnPlayerJoined(ConnectedPlayer p){
		Debug.Log ("Invoked on player joined");
        _hasFirstPlayerShowedUp = true;
    }

	private void OnClientLeft(ConnectedClient c){
		if (GameRoomClients.currentClientCount() == 0)
            Application.Quit();
    }

	public static void onGameFinished(){
		singleton.StartCoroutine (singleton.exitGameOnDelay (3));
	}

	public IEnumerator exitGameOnDelay(float delay){
		yield return new WaitForSeconds (delay);
		Application.Quit ();
	}

    /// <summary>
    ///     Each second checks if we're still connected, and if we are not,
    ///     terminates game server
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartWaitingForConnectionLost(){
        // Wait at least 5 seconds until first check
        yield return new WaitForSeconds(5);

        while (true){
            yield return new WaitForSeconds(1);
            if (!Msf.Connection.IsConnected){
                Logs.Error("Terminating game server, no connection");
                Application.Quit();
            }
        }
    }


	private IEnumerator StartRandomPlayerChecks(){
		// Wait at least 5 seconds until first check
		yield return new WaitForSeconds(30);

		while (true){
			yield return new WaitForSeconds(20);
			if (GameRoomClients.currentClientCount() == 0){
				Logs.Error("Terminating game server, no players!");
				Application.Quit();
			}
		}
	}

    /// <summary>
    ///     Waits a number of seconds, and checks if the game room was registered
    ///     If not - terminates the application
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartStartedTimeout(float timeout){
        yield return new WaitForSeconds(timeout);
        if (Room == null || !Room.IsRoomRegistered)
            Application.Quit();
    }

    private IEnumerator StartFirstPlayerTimeout(float timeout){
        yield return new WaitForSeconds(timeout);
        if (!_hasFirstPlayerShowedUp){
            Logs.Error("Terminated game server because first player didn't show up");
         	Application.Quit();
        }
    }
}
