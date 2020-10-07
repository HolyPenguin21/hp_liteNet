using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.UI;

public class Orders
{
    Server server;
    Client client;
    private NetPacketProcessor netProcessor;

    public Orders(Server server, Client client, NetPacketProcessor netPacketProcessor)
    {
        this.server = server;
        this.client = client;
        this.netProcessor = netPacketProcessor;
    }    
}
