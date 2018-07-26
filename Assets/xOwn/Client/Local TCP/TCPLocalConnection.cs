using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using Barebones.Networking;
using System.Collections;
using System.Net.NetworkInformation;
using System.Linq;

public enum LocalConnectionStatus{
	connecting,
	portIsBusy,
	otherPortError
}
	
class TCPLocalConnection{
	public static bool isReady = false;
	private static TcpListener server;

	private static List<Action<ConnectionStatus>> TCPStatusSubs = new List<Action<ConnectionStatus>>();
	public static void subscribeToTCPStatus(Action<ConnectionStatus> sub){if(!TCPStatusSubs.Contains(sub))TCPStatusSubs.Add (sub);}
	public static void unSubscribeToTCPStatus(Action<ConnectionStatus> sub){if(TCPStatusSubs.Contains(sub))TCPStatusSubs.Remove (sub);}
	public static ConnectionStatus currentState;

	private static TcpRemoteClient client;
	private static TcpClient connectionClient;
	private static int checkCooldown = 500;
	private static int lastUsedPort = 0;
	private static Thread monitorThread;

    private static int sendBufferSize = 2048;


    public static void init(){
		if (isReady)return;
		ClientUI.ClientUIOverlord.onUIStateChanged += (ClientUI.ClientUIStates newState) => {if(newState == ClientUI.ClientUIStates.GameLobby || newState == ClientUI.ClientUIStates.LoginMenu) stopServer();};
		monitorThread = new Thread( new ThreadStart(monitorTCPConnection));
		monitorThread.Start ();
		isReady = true;
	}


	public static void OnApplicationQuit(){
		dissconnect ();
		if(monitorThread != null)
			monitorThread.Abort ();
	}

	private static void dissconnect(){
		if (client != null)
			client.killClient ();

		if(server != null)
			server.Stop ();
	}
	public static void stopServer(){
		dissconnect ();
		fireTCPStatusChange (ConnectionStatus.Disconnected);
	}


	//Reworked so it only fires on main thread
	public static void fireTCPStatusChange(ConnectionStatus newStatus){
		currentState = newStatus;
		foreach (Action<ConnectionStatus> s in TCPStatusSubs) {
			try{MainThread.fireEventAtMainThread(() => { s.Invoke (newStatus);});}
			catch(Exception e){Debug.LogError (e.Message);}
		}
	}


	public static void sendMessage(string message){client.sendMessage (message + "\n");}
	public static void restartServer(){MainThread.createTimedAction(() => {startServer (lastUsedPort);}, 0f);}
	public static LocalConnectionStatus startServer(int port){
		lastUsedPort = port;
		return initServer (port);
	}

	public static LocalConnectionStatus initServer(int port){
		try{
			if(server != null){
				server.Stop();
			}
			IPAddress localAddr = IPAddress.Parse("127.0.0.1");
			server = new TcpListener (localAddr, port);
            server.Start ();
		}
		catch(SocketException e){
			if (e.ErrorCode == 10048)
				return LocalConnectionStatus.portIsBusy;
			return LocalConnectionStatus.otherPortError;
		}
		catch{return LocalConnectionStatus.otherPortError;}

		client = new TcpRemoteClient (server, 1);
		return LocalConnectionStatus.connecting;
	}


	#region monitor TCP
	public static void clientConnected(TcpClient c) {
        connectionClient = c;
        c.SendBufferSize = sendBufferSize;
    }
	private static void monitorTCPConnection(){
		while (true) {
			if (currentState == ConnectionStatus.Connected) 
				checkCurrentConnection ();
			Thread.Sleep (checkCooldown);
		}
	}
	private static void checkCurrentConnection(){
		try{
			if(connectionClient == null || client == null){
				Debug.Log("No good TCP connection");
				stopServer();
				return;
			}
			if(client.IsConnected == false){
				Debug.Log ("Stop because Cool");
				stopServer();
				return;
			}
			if(connectionClient.Connected == false){
				Debug.Log ("Stop because null ref");
				stopServer();
				return;
			}
		}
		catch{
			Debug.Log ("Stop because Crash");
			stopServer();
		}
	}
	#endregion
}




public class TcpRemoteClient{
	private TcpClient client;
	private TcpListener server;
	private Thread connectThread, listenThread;
	private NetworkStream stream;

	public bool clientConnected = false;
	public bool closeClient = false;
	public bool isHuman = false;
	public bool isWaitingForConnection = false;
	public int id;

	public TcpRemoteClient(TcpListener server, int id){
		this.id = id;
		this.server = server;
		initThreads ();
	}

	private void initThreads(){
		connectThread = new Thread( new ThreadStart(waitForConnection));
		connectThread.IsBackground = true;
		connectThread.Start();

		listenThread = new Thread( new ThreadStart(listenForMessage));
		listenThread.IsBackground = true;
		listenThread.Start();
	}


	private void waitForConnection(){
		isWaitingForConnection = true;
		TCPLocalConnection.fireTCPStatusChange (ConnectionStatus.Connecting);
        client = server.AcceptTcpClient();
		stream = client.GetStream ();

		clientConnected = true;
		TCPLocalConnection.clientConnected (client);
		isWaitingForConnection = false;
        TCPLocalConnection.fireTCPStatusChange (ConnectionStatus.Connected);
	}

	private void listenForMessage(){
        while (clientConnected == false) 
            Thread.Sleep (1000);
		
		Byte[] bytes = new Byte[2048*10];
		String data = null;

        while (closeClient == false) {
			int i;
			data = null;

			while((i = stream.Read(bytes, 0, bytes.Length))!=0) {   
				data = Encoding.ASCII.GetString(bytes, 0, i);
				TCPMessageQueue.addMessage (data, id);
            }

			Thread.Sleep (100);
		}
	}

	public void sendMessage(string message){
        Debug.Log("TCP MSG: " + message);
		if (clientConnected == false)
			return;

		byte[] msg = Encoding.ASCII.GetBytes(message);
		try{
			stream.Write(msg, 0, msg.Length);
                
        }
		catch{
			clientConnected = false;
		}
	}

	public void sendMessage(byte[] msg){
		if (clientConnected == false || isHuman)
			return;

		stream.Write(msg, 0, msg.Length);
	}

	public void killClient(){
		closeClient = true;
		clientConnected = false;
		isWaitingForConnection = false;

		if (connectThread != null)
			connectThread.Abort ();

		if(listenThread != null)
			listenThread.Abort ();

		if (client != null)
			client.Close ();
	}



	public bool IsConnected{
		get{
			try{
				if (client != null && client.Client != null && client.Client.Connected){
					/* pear to the documentation on Poll:
                * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                * -or- true if data is available for reading; 
                * -or- true if the connection has been closed, reset, or terminated; 
                * otherwise, returns false
                */

					// Detect if client disconnected
					if (client.Client.Poll(0, SelectMode.SelectRead)){
						byte[] buff = new byte[1];
						if (client.Client.Receive(buff, SocketFlags.Peek) == 0){
							// Client disconnected
							return false;
						}
						else
							return true;
					}
					return true;
				}
				else
					return false;
			}
			catch{
				return false;
			}
		}
	}
}
