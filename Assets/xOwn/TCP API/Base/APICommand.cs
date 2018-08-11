using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API { 

    public class TCPFields {
        public const string action = "action";
    }

    public abstract class TCPCommand {
        public string player;
        public string action;
    }
}