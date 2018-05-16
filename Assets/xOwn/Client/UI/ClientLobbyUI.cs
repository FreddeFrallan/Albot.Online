using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientUI{
	public class ClientLobbyUI : MonoBehaviour {

		void Awake(){
			if (ClientUIOverlord.hasLoaded)
				Destroy (this.gameObject);
		}
	}

}