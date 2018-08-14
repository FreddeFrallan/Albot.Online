using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tournament.Client {
    public interface TournamentUIObject{

        void onLeftClick();
        void onRightClick();
    }
}