using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AlbotServer {

    public class AlbotPreGameMaintence{
 
        public void cleanupOldPreGames() {
            foreach (PreGame p in AlbotPreGameModule.getAllGames().Where(p => p.getPeers().Count == 0))
                AlbotPreGameModule.removeGame(p, p.specs.roomID);
        }

    }

}