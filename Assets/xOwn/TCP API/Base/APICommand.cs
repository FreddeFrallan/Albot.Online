using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCP_API { 

    public class TCPFields {
        public const string action = "Action";
    }

    public abstract class TCPCommand {
        public string player;
        public string action;
    }
}