using UnityEditor;

/// <summary>
/// Instead of editing this script, I would recommend to write your own
/// (or copy and change it). Otherwise, your changes will be overwriten when you
/// update project :)
/// </summary>
public class MsfQuickBuild{
	/// <summary>
	/// Have in mind that if you change it, it might take "a while" 
	/// for the editor to pick up changes  
	/// </summary>
	public static string QuickSetupRoot = "Assets/xOwn/";
	public static BuildTarget TargetPlatform = BuildTarget.StandaloneWindows;
	public static BuildTarget TargetPlatformMac = BuildTarget.StandaloneOSXIntel;


	/// <summary>
	/// Build with "Development" flag, so that we can see the console if something 
	/// goes wrong
	/// </summary>
	public static string PrevPath = null;


	/// <summary>
	/// Creates a build for master server and spawner
	/// </summary>
	/// <param name="path"></param>
	public static void BuildMasterAndSpawner(string path, BuildTarget target, string fileExtension = ".exe", BuildOptions options = BuildOptions.Development){
		var masterScenes = new[]{
			QuickSetupRoot+ "/MasterServer/MasterAndSpawner.unity"
		};

		BuildPipeline.BuildPlayer(masterScenes, path + "/MasterAndSpawner" + fileExtension, target, options);
	}

	/// <summary>
	/// Creates a build for client
	/// </summary>
	/// <param name="path"></param>
	public static void BuildClient(string path, BuildTarget target, string fileExtension = ".exe", BuildOptions options = BuildOptions.Development){
		var clientScenes = new[]{
			QuickSetupRoot+ "/Client/Client.unity",
			QuickSetupRoot+ "/Client/EmptyScene.unity",
            QuickSetupRoot+ "/Admin Client/Tournament/Scene/TournamentScene.unity",
			// Add all the game scenes
			QuickSetupRoot+ "/Game/Games/Connect4/Connect4Game.unity",
			QuickSetupRoot+"/Game/Games/Snake/SnakeGame.unity",
			QuickSetupRoot+"/Game/SinglePlayer/BrickPuzzle/BrickPuzzleGame.unity",
            /*
            QuickSetupRoot + "/Game/Games/Othello/OthelloGame.unity",
			QuickSetupRoot+ "/Game/Games/FallingDebris/FallingDebrisGame.unity",
			QuickSetupRoot+ "/Game/Games/Chess/ChessGame.unity",
			QuickSetupRoot+ "/Game/Games/Battleships/BattleshipsGame.unity",
			QuickSetupRoot+"/Game/Games/Soldiers/Game/SoldiersGame.unity",
			QuickSetupRoot+"/Game/SinglePlayer/Pickup/PickupGame.unity",
			QuickSetupRoot+"/Game/SinglePlayer/2048/2048Game.unity", 
			QuickSetupRoot+"/Game/SinglePlayer/Tower Of Hanoi/TowerOfHanoiGame.unity", 
			QuickSetupRoot+"/Game/Games/Bomberman/Game/BombermanGame.unity",
			QuickSetupRoot+"/Game/Games/Brakethrough/BrakethroughGame.unity",
			QuickSetupRoot+"/Game/SinglePlayer/HeapManager/HeapManager.unity",
			QuickSetupRoot+"/DemoGame/BlockBattleGame.unity",
            QuickSetupRoot+"/Game/Games/SpeedRunner/SpeedRunnerGame.unity"
            */
        };
		BuildPipeline.BuildPlayer(clientScenes, path + "/Albot.Online.exe",  target, options);
	}

	/// <summary>
	/// Creates a build for game server
	/// </summary>
	/// <param name="path"></param>
	public static void BuildGameServer(string path, BuildTarget target, string fileExtension = ".exe", BuildOptions options = BuildOptions.Development){
		var gameServerScenes = new[]{
			QuickSetupRoot+"/GameServer/GameServer.unity",
			QuickSetupRoot+"/Game/Games/FallingDebris/FallingDebrisGameServer.unity",
			QuickSetupRoot+"/Game/Games/Soldiers/Server/SoldiersGameServer.unity",
			QuickSetupRoot+"/Game/Games/Bomberman/Server/BombermanGameServer.unity",
			QuickSetupRoot+"/Game/Games/Snake/GameServer/SnakeGameServer.unity",
            QuickSetupRoot+"/Game/Games/SpeedRunner/GameServer/SpeedRunnerGameServer.unity",
			//QuickSetupRoot+"/DemoGame/BlockBattleGameServer.unity",
		};
		BuildPipeline.BuildPlayer(gameServerScenes, path + "/GameServer" + fileExtension, target, options);
	}

	public static void BuildAdminClient(string path, BuildTarget target, string fileExtension = ".exe", BuildOptions options = BuildOptions.Development){
		var gameServerScenes = new[]{
			QuickSetupRoot+ "/Admin Client/Admin Client.unity",
			QuickSetupRoot+ "/Client/EmptyScene.unity",
            QuickSetupRoot+ "/Admin Client/Tournament/Scene/TournamentScene.unity",
            QuickSetupRoot+"/Game/Games/Brakethrough/Admin/BreakthroughAdmin.unity",
			QuickSetupRoot+"/Game/Games/Snake/SnakeGame.unity"
		};
		BuildPipeline.BuildPlayer(gameServerScenes, path + "/AlbotAdmin" + fileExtension, target, options);
	}


	#region Linux server Deploy menu Builds
	[MenuItem("Tools/Msf/Build Server %&d", false, 0)]
	public static void BuildAllServerVersion(){
		var path = GetPath();
		if (string.IsNullOrEmpty(path))
			return;

		BuildMasterAndSpawner(path, BuildTarget.StandaloneLinux64, ".x86_64", BuildOptions.EnableHeadlessMode);
		BuildGameServer(path, BuildTarget.StandaloneLinux64, ".x86_64", BuildOptions.EnableHeadlessMode);
	}
	#endregion



	#region Win menu Builds
	[MenuItem("Tools/Msf/Build All %&a", false, 0)]
	public static void BuildGame(){
		var path = GetPath();
		if (string.IsNullOrEmpty(path))
			return;

		BuildMasterAndSpawner(path, BuildTarget.StandaloneWindows);
		BuildClient(path, BuildTarget.StandaloneWindows);
		BuildGameServer(path, BuildTarget.StandaloneWindows);
		BuildAdminClient (path, BuildTarget.StandaloneWindows);
	}

	[MenuItem("Tools/Msf/Build Master + Spawner %&m", false, 11)]
	public static void BuildMasterAndSpawnerMenu(){
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
			BuildMasterAndSpawner(path, BuildTarget.StandaloneWindows);
	}

	[MenuItem("Tools/Msf/Build Client %&q", false, 11)]
	public static void BuildClientMenu(){
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
			BuildClient(path, BuildTarget.StandaloneWindows);
	}

	[MenuItem("Tools/Msf/Build Admin Tool %&g", false, 11)]
	public static void BuildGameServerMenu(){
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
			BuildGameServer(path, BuildTarget.StandaloneWindows);
	}

	[MenuItem("Tools/Msf/Build Admin Tool %&s", false, 11)]
	public static void BuildAdminToolMenu(){
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
			BuildAdminClient(path, BuildTarget.StandaloneWindows);
	}
	#endregion



	#region MAC menu Builds
	[MenuItem("Tools/Msf/MAC: Build All", false, 0)]
	public static void BuildGameMac(){
		var path = GetPath();
		if (string.IsNullOrEmpty(path))
			return;

		BuildMasterAndSpawner(path, BuildTarget.StandaloneOSXIntel);
		BuildClient(path, BuildTarget.StandaloneOSXIntel);
		BuildGameServer(path, BuildTarget.StandaloneOSXIntel);
	}

	[MenuItem("Tools/Msf/MAC: Build Master + Spawner", false, 11)]
	public static void BuildMasterAndSpawnerMenuMac(){
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
			BuildMasterAndSpawner(path, BuildTarget.StandaloneOSXIntel);

	}

	[MenuItem("Tools/Msf/MAC: Build Client", false, 11)]
	public static void BuildClientMenuMac(){
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
			BuildClient(path, BuildTarget.StandaloneOSXIntel);

	}

	[MenuItem("Tools/Msf/MAC: Build Game Server", false, 11)]
	public static void BuildGameServerMenuMac(){
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
			BuildGameServer(path, BuildTarget.StandaloneOSXIntel);
	}
	#endregion


	#region Linux menu Builds
	[MenuItem("Tools/Msf/Build Release %&r", false, 11)]
	public static void BuildRelease(){
		var path = GetPath();
		if (!string.IsNullOrEmpty (path)) {
			BuildGameServer (path, BuildTarget.StandaloneLinux64, ".x86_64", BuildOptions.EnableHeadlessMode);
			BuildMasterAndSpawner(path, BuildTarget.StandaloneLinux64, ".x86_64", BuildOptions.EnableHeadlessMode);
		}
	}
	#endregion



	public static string GetPath(){
		var prevPath = EditorPrefs.GetString("msf.buildPath", "");
		string path = EditorUtility.SaveFolderPanel("Choose Location for binaries", prevPath, "");

		if (!string.IsNullOrEmpty(path))
			EditorPrefs.SetString("msf.buildPath", path);

		return path;
	}
}