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
    public bool isNeutral;
    public bool isAvailable;
    public string name;
    public int race;
    public int gold;
    public int villages;
    public Character heroCharacter;
}
