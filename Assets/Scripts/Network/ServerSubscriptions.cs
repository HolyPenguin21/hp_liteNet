using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class ServerSubscriptions
{
    Server server;
    private NetPacketProcessor netProcessor;

    public ServerSubscriptions(Server server, NetPacketProcessor netPacketProcessor)
    {
        this.server = server;
        this.netProcessor = netPacketProcessor;
    }

    public void RaceChange()
    {
        netProcessor.SubscribeReusable<RaceChange>((data) => {
            server.StartCoroutine(GameMain.inst.Server_RaceChange(data));
        });
    }

    public void ResponceOnLogin()
    {
        netProcessor.SubscribeReusable<LoginData>((data) => {
            Debug.Log("Server > Client has logged in : " + data.clientName);

            // Log in player
            Player loggedInPlayer = server.players[data.clientId];
            loggedInPlayer.name = data.clientName;

            GameObject.FindObjectOfType<UI_MainMenu>().Update_ConnectedList();
            GameObject.FindObjectOfType<UI_MainMenu>().Setup_RacePicker();

            // Send players list to all players
            string pListData = "";
            for (int x = 0; x < server.players.Count; x++)
            {
                Player somePlayer = server.players[x];
                pListData += "|";
                pListData += somePlayer.name;
            }

            for (int x = 0; x < server.players.Count; x++)
            {
                if (server.players[x].isServer) continue;
                
                netProcessor.Send(server.players[x].address, new PlayersList() { playersList = pListData }, DeliveryMethod.ReliableOrdered);
            }
        });
    }

    public void ChatMessage()
    {
        netProcessor.SubscribeReusable<ChatMessage>((data) => {
            //Debug.Log("Server > message from Client : ChatMessage : cName=" + data.name + ", cMessage=" + data.message);
            server.StartCoroutine(GameMain.inst.Server_ChatMessage(data));
        });
    }
}
