using System;
using UnityEngine;

namespace NetworkData
{
    public enum Commands
    {
        NEW_CLIENT,
        UPDATE,
        CLIENT_DROPPED,
        CLIENT_LIST,
        OWN_ID
    }

    [Serializable]
    public class State
    {
        public Commands cmd;
    }

    [Serializable]
	public class Player
	{
        public string id;
        [Serializable]
        public struct receivedColor
        {
            public float R;
            public float G;
            public float B;
        }
        public receivedColor color;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 input;

        public override string ToString()
        {
            string result = "Player : \n";
            result += "id : " + id + "\n";
            result += "R : " + color.R.ToString() + ", ";
            result += "G : " + color.G.ToString() + ", ";
            result += "B : " + color.B.ToString() + "\n";
            result += "position : " + position.ToString() + "\n";
            result += "rotation : " + rotation.ToString() + "\n";
            result += "input : " + input.ToString() + "\n";

            return result;
        }
    }

    [Serializable]
    public class NewPlayer
    {
        public Player[] player;
    }
    [Serializable]
    public class ConnectedPlayer
    {
        public Player[] connect;
    }
    [Serializable]
    public class DisconnectedPlayer
    {
        public Player[] disconnect;
    }
    [Serializable]
    public class UpdatedPlayer
    {
        public Player[] update;
    }
}
