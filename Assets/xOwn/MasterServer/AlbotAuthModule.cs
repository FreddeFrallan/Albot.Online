using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using AlbotDB;


//Framework starts from 32000, so we can use anything from 0 - 32000 ^^
public enum CustomMasterServerMSG{
	login = 0,
	spectateStatus = 1,
	requestSpectatorGames = 2,
	requestFullGameLog = 3,
	spectateLogUpdate = 4,
	startSpectate = 5,
	stopSpectate = 6,
	adminLogin = 7,
	adminLogout = 8,
}

public class AlbotAuthModule : ServerModuleBehaviour {
	
	public delegate void AuthEventHandler(IUserExtension account);
	public event AuthEventHandler LoggedIn;
	private SpectatorAuthModule spectatorModule;

	public override void Initialize (IServer server){
		Debug.LogError ("Init costum AuthModule");

		AlbotDBManager.initDB ();
		AlbotDBManager.updateLoginInfo (new UserLoginInformation{ username = "Zully", password = "apa", isLoggedIn = false});
		AlbotDBManager.updateGameStat (new UserInfo{ username = "Zully", profilePic = "6", roomActions = "0", logins = "0" });
		AlbotDBManager.updateLoginInfo (new UserLoginInformation{ username = "Pingul", password = "orm", isLoggedIn = false});
		AlbotDBManager.updateGameStat (new UserInfo{ username = "Pingul", profilePic = "3", roomActions = "0", logins = "0" });
		AlbotDBManager.clearActiveUsers ();
		AlbotDBManager.resetLoggedInUsers ();

		server.SetHandler ((short)CustomMasterServerMSG.login, handleLoginMsg);
		server.SetHandler((short)MsfOpCodes.GetPeerAccountInfo, HandleGetPeerAccountInfo);
		Component.FindObjectOfType<MasterServerBehaviour> ().PeerDisconnected += handleDissconnectedPeer;
		spectatorModule = Component.FindObjectOfType<SpectatorAuthModule> ();
	}

	//If player dissconnects we remove it from loged in users, but not if the user was a Admin.
	private void handleDissconnectedPeer(IPeer peer){
		if (spectatorModule.playerDissconnected (peer))
			return;
		AlbotDBManager.onPlayerDissconnected (peer.Id);
	}
		
	private void handleLoginMsg(IIncommingMessage msg){
		//Validate incoming message to make sure connection is safe
		Dictionary<string, string> data = new Dictionary<string, string> ();
		if (validateDecryptMsg (msg, out data) == false) {
			msg.Respond("Insecure request".ToBytes(), ResponseStatus.Unauthorized);
			return;
		}

		string errorMsg = "";
		if (alphaLegitCredentials (data, out errorMsg) == false) {
			msg.Respond(errorMsg.ToBytes(), ResponseStatus.Failed);
			return;
		}
			
		UserLoginInformation user = new UserLoginInformation () {
			username = data ["Username"].Trim(),
			password = data ["Password"],
			isLoggedIn = true
		};
		AlbotDBManager.updateLoginInfo (user);

		UserInfo storedInfo;
		if (AlbotDBManager.getUserData (user.username, out storedInfo) == false) {
			storedInfo = new UserInfo () {
				username = user.username,
				logins = "0",
				roomActions = "0",
				profilePic = (Random.Range (1, 100)).ToString ()
			};
		}
		else
			storedInfo.logins = (int.Parse (storedInfo.logins) + 1).ToString ();

		AlbotDBManager.updateGameStat (storedInfo);


		//Generate account data
		IAccountData accountData = null;
		if (generateAcountData (storedInfo, out accountData) == false) {
			msg.Respond("Error in generating accountData", ResponseStatus.Unauthorized);
			return;
		}
			
		// Setup auth extension
		var extension = msg.Peer.AddExtension(new UserExtension((msg.Peer)));
		extension.Load(accountData);
		var infoPacket = extension.CreateInfoPacket();

		// Finalize login
		FinalizeLogin(extension, user);
		msg.Respond(infoPacket.ToBytes(), ResponseStatus.Success);
	}

	private bool generateAcountData(UserInfo storedInfo, out IAccountData accountData){
		var db = Msf.Server.DbAccessors.GetAccessor<IAuthDatabase>();
		accountData = db.CreateAccountObject();

		accountData.Username = storedInfo.username;
		accountData.IsGuest = true;
		accountData.IsAdmin = false;
		accountData.Properties = new Dictionary<string, string> ();
		accountData.Properties.Add("icon", storedInfo.profilePic.ToString());

		if (accountData == null)
			return false;
		return true;
	}

	private bool validateDecryptMsg(IIncommingMessage msg, out Dictionary<string, string> data){
		var encryptedData = msg.AsBytes();
		var securityExt = msg.Peer.GetExtension<PeerSecurityExtension>();
		var aesKey = securityExt.AesKey;

		data = new Dictionary<string, string> ();
		if (aesKey == null)
			return false;

		var decrypted = Msf.Security.DecryptAES(encryptedData, aesKey);
		data = new Dictionary<string, string>().FromBytes(decrypted);
		return true;
	}

	private bool legitLoginCredentials(Dictionary<string, string> data, out string errorMsg){
		errorMsg = "";
		if (data.ContainsKey ("Username") == false || data.ContainsKey ("Password") == false){
			errorMsg = "Something went wrong in login";
			return false;
		}
		string username = data ["Username"];
		string password = data["Password"];

		UserLoginInformation user = new UserLoginInformation();
		if (AlbotDBManager.getLoginInfo (username, out user) == false)
			errorMsg = "Could not find registred acount";
		else if (password != user.password)
			errorMsg = "Incorrect password";
		else if (user.isLoggedIn)
			errorMsg = "Current acount is already loged in!";

		if (errorMsg != "")
			return false;
		return true;
	}

	private bool alphaLegitCredentials(Dictionary<string, string> data, out string errorMsg){
		errorMsg = "";
		if (data.ContainsKey ("Username") == false || data.ContainsKey ("Password") == false){
			errorMsg = "Something went wrong in login";
			return false;
		}
		string username = data ["Username"].Trim();
		string password = data["Password"];

		UserLoginInformation user = new UserLoginInformation();
		AlbotDBManager.getLoginInfo (username, out user);
		if (user != null && user.isLoggedIn)
			errorMsg = "Current acount is already loged in!";
		else if (username [0] == '<' && username [username.Length - 1] == '>')
			errorMsg = "Invalid name format, please don't use the \"<\" or \">\" sign in your name.";
		else if(username == Game.LocalTrainingBots.StandardTrainingBotInfo.username)
			errorMsg = "Please don't choose \""+username+"\" as your username";
		
		if (errorMsg != "")
			return false;
		return true;
	}


	private void FinalizeLogin(IUserExtension extension, UserLoginInformation newUser){
		AlbotDBManager.onUserLogedIn (newUser, extension.Peer.Id);
		if (LoggedIn != null)
			LoggedIn.Invoke(extension);
	}


	private void HandleGetPeerAccountInfo(IIncommingMessage message){
		int peerId = message.AsInt();
		UserInfo user = new UserInfo ();

		if (AlbotDBManager.getActiveUsersInfo(peerId, ref user) == false){
			message.Respond("Peer with a given ID is not in the game", ResponseStatus.Error);
			return;
		}
			
		var packet = new PeerAccountInfoPacket(){
			PeerId = peerId,
			Properties = new Dictionary<string, string>(){{"Icon", user.profilePic}},
			Username = user.username
		};

		message.Respond(packet, ResponseStatus.Success);
	}
}


public class UserAcount{
	public string username;
	public string password;
	private bool isLoggedIn;

	public UserAcount(string un, string pw){
		username = un;
		password = pw;
	}
}
