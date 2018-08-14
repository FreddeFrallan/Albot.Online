using System.Collections.Generic;
using System.Linq;
using Barebones.Networking;
using UnityEngine;
using AlbotServer;
using Tournament.Server;

namespace Barebones.MasterServer{
	
    public class MatchmakerModule : ServerModuleBehaviour{
		
        protected HashSet<IGamesProvider> GameProviders;

        protected virtual void Awake(){
            AddOptionalDependency<RoomsModule>();
            AddOptionalDependency<LobbiesModule>();
        }

        public override void Initialize(IServer server){
            base.Initialize(server);
            GameProviders = new HashSet<IGamesProvider>();

            if (server.GetModule<RoomsModule>() != null)
                AddProvider(server.GetModule<RoomsModule>());

            if (server.GetModule<LobbiesModule>() != null)
                AddProvider(server.GetModule<LobbiesModule>());

            server.SetHandler((short) MsfOpCodes.FindGames, HandleFindGames);
        }

        public void AddProvider(IGamesProvider provider){
            GameProviders.Add(provider);
        }

        private void HandleFindGames(IIncommingMessage message){
            List<GameInfoPacket> gameList = getCurrentGameInfos();
            var filters = new Dictionary<string, string>().FromBytes(message.AsBytes());

            var bytes = gameList.Select(l => (ISerializablePacket)l).ToBytes();
            message.Respond(bytes, ResponseStatus.Success);

			int currentGamesCounter = 0;
			foreach (var provider in GameProviders)
				currentGamesCounter += provider.GetPublicGames (message.Peer, filters).ToList().Count;
			message.Peer.SendMessage ((short)ServerCommProtocl.LobbyGameStats, new LobbyGameStatsMsg () {currentActiveGames = currentGamesCounter, totalGamesPlayed = GamesData.totallGamesPlayed});
        }

        public List<GameInfoPacket> getCurrentGameInfos() {
			List<GameInfoPacket> gameList = new List<GameInfoPacket>();

            foreach (PreGame p in AlbotPreGameModule.getCurrentJoinableGames())
                gameList.Add(p.convertToGameInfoPacket());
            foreach (PreTournamentGame t in AlbotTournamentModule.getCurrentTournaments())
                gameList.Add(t.convertToGameInfoPacket());

            return gameList;
        }
    }
}