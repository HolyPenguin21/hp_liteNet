using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public NetPeer address;
    public int id;
    public bool isServer;
    public string name;
    public int race;
}
