using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using Game;
using UnityEngine;
using System.Linq;
using AlbotServer;
using Barebones.Networking;


namespace ClientUI{

    public class PreGameLobby : PreGameBaseLobby {

        private List<PreGameSlotType> currentSettings;
        private PreGameSlotInfo p2Slot;
        private PlayerInfo localInfo;

        //Called when we enter a preGame 
        void Start() {
            if (hasInit)
                return;
            hasInit = true;
            ClientUIOverlord.onUIStateChanged += (ClientUIStates newState) => { if (newState != ClientUIStates.PreGame) resetPanel(); };
            TCPLocalConnection.subscribeToTCPStatus(localBotStatusChanged);

            p2Settings.onValueChanged.AddListener(newp2SettingSelected);
            gameCreator.setCurrentPreLobby(this);
        }

        public override void initPreGameLobby(Sprite gameSprite, PreGameRoomMsg roomInfo) {
            base.initPreGameLobby(gameSprite, roomInfo);
            handlers.Add(Msf.Connection.SetHandler((short)ServerCommProtocl.PreGameKick, handlePreGameKickMsg));
            initPlayerInfos();
            initP2Settings(roomInfo.specs.type);
        }

        private void initPlayerInfos() {
            AccountInfoPacket ac = ClientUIOverlord.getCurrentAcountInfo();
            localInfo = new PlayerInfo() { username = ac.Username, iconNumber = int.Parse(ac.Properties["icon"]) };
            p2Slot = new PreGameSlotInfo() { slotID = 1, playerInfo = localInfo };
        }

        private void initP2Settings(GameType type) {
            currentSettings = PreGameSlotTypes.getSlotOptions(type);
            p2Settings.ClearOptions();
            p2Settings.AddOptions(currentSettings.Select(s => s.ToString()).ToList());
            newp2SettingSelected(0);
        }


        public void newp2SettingSelected(int value) {
            if (isAdmin == false) //Should never happen
                return;

            p2Slot.type = currentSettings[value];
            if (p2Slot.type == PreGameSlotType.TrainingBot)
                p2Slot.playerInfo = LocalTrainingBots.StandardTrainingBotInfo;
            else if (p2Slot.type == PreGameSlotType.SelfClone)
                p2Slot.playerInfo = PreGameSlotTypes.getSelfInfo(localInfo);
            else if (p2Slot.type == PreGameSlotType.Human)
                p2Slot.playerInfo = PreGameSlotTypes.getHumanInfo(localInfo);

            sendChangeSlot();
        }

        private void handlePreGameKickMsg(IIncommingMessage message) { ClientUIStateManager.requestGotoGameLobby(); }
        private void sendChangeSlot() {
            Msf.Connection.SendMessage((short)ServerCommProtocl.SlotTypeChanged, new PreGameSlotSTypeMsg() { roomID = roomId, slot = p2Slot });
        }
    }

	public enum Player2Setting{
		Opponent = 0,
		Computer = 1,
		Self = 2,
		Human = 3,
	}
}