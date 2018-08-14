using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using System.Linq;

public class SpectatorAuthModule : ServerModuleBehaviour {

	private readonly string ADMIN_PASSWORD = "Lol";
	private List<IPeer> currentAdmins = new List<IPeer> ();
	private static SpectatorAuthModule singleton;
    public AlbotTournamentModule tournamentModule;

	public override void Initialize (IServer server){
		singleton = this;
		server.SetHandler ((short)CustomMasterServerMSG.adminLogin, handleLogin);
		server.SetHandler ((short)CustomMasterServerMSG.adminLogout, handleLogout);
		Debug.LogError ("Spectator Auth Init");
	}


	//Should probebly be encrpyted later on
	private void handleLogin(IIncommingMessage msg){
		string password = msg.AsString ();
		Debug.LogError (password);
		if (password != ADMIN_PASSWORD) {
			msg.Respond ("Wrong Password!", ResponseStatus.Invalid);
			return;
		}

		if(currentAdmins.Any(x => x.Id == msg.Peer.Id)){
			msg.Respond ("This ID is already loged in", ResponseStatus.Error);
			return;
		}
			
		Debug.LogError ("Admin loged in: " + msg.Peer.Id);


		currentAdmins.Add (msg.Peer);
		msg.Respond (ResponseStatus.Success);
	}

	private void handleLogout(IIncommingMessage msg){
		IPeer admin = currentAdmins.Find (x => msg.Peer.Id == x.Id);
        if (admin != null)
            removeAdmin(admin);
        else
            Debug.LogError("Tried to logout a user that was not registred");
	}
    private void removeAdmin(IPeer admin) {
        currentAdmins.Remove(admin);
        tournamentModule.adminLeft(admin);
        Debug.LogError("Admin removed");
    }

	public bool playerDissconnected(IPeer p){
		IPeer adminP = currentAdmins.Find (x => x.Id == p.Id);
		if(adminP == null)
			return false;

        removeAdmin(adminP);
        return true;
	}



	public static bool existsAdmin(IPeer p){return singleton.currentAdmins.Any (x => x.Id == p.Id);}
}
