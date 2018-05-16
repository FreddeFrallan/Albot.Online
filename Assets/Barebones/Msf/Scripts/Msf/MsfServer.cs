using System;
using Barebones.Networking;

namespace Barebones.MasterServer{
    public class MsfServer : MsfBaseClient{
		public AlbotRoomMasterServer Rooms { get; private set; }
        public MsfSpawnersServer Spawners { get; private set; }

        public MsfDbAccessorFactory DbAccessors;

        public MsfAuthServer Auth { get; private set; }
        public MsfLobbiesServer Lobbies { get; private set; }
        public MsfProfilesServer Profiles { get; private set; }

        public MsfServer(IClientSocket connection) : base(connection){
            DbAccessors = new MsfDbAccessorFactory();
			Rooms = new AlbotRoomMasterServer(connection);
            Spawners = new MsfSpawnersServer(connection);
            Auth = new MsfAuthServer(connection);
            Lobbies = new MsfLobbiesServer(connection);
            Profiles = new MsfProfilesServer(connection);
        }
    }
}