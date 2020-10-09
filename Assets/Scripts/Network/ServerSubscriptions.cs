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

    public void ItemDrop()
    {
        netProcessor.SubscribeReusable<ItemDrop>((data) => {
            Character character = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.coord_x, data.coord_y).hex.character;
            server.StartCoroutine(GameMain.inst.Server_DropItem(character));
        });
    }

    public void ItemPickup()
    {
        netProcessor.SubscribeReusable<ItemPickup>((data) => {
            Character character = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.coord_x, data.coord_y).hex.character;
            server.StartCoroutine(GameMain.inst.Server_PickupItem(character));
        });
    }

    public void Move()
    {
        netProcessor.SubscribeReusable<Move>((data) => {
            List<Hex> somePath = new List<Hex>();
            string[] pathData = data.pathData.Split('|');
            for (int j = 1; j < pathData.Length; j++)
            {
                string[] hexCoords = pathData[j].Split(';');
                int posX = int.Parse(hexCoords[0]);
                int posY = int.Parse(hexCoords[1]);
                somePath.Add(GameMain.inst.gridManager.Get_GridItem_ByCoords(posX, posY).hex);
            }
            server.StartCoroutine(GameMain.inst.Server_Move(somePath));
        });
    }

    public void EndTurn()
    {
        netProcessor.SubscribeReusable<EndTurn>((data) => {
            server.StartCoroutine(GameMain.inst.Server_ChangeTurn());
        });
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

    public void ResponsOnTaskDone()
    {
        netProcessor.SubscribeReusable<TaskDone>((data) => {
            Debug.Log("Server > " + data.playerName + " finished task : " + data.task);

            for (int x = 0; x < server.players.Count; x++)
            {
                Player somePlayer = server.players[x];
                if (somePlayer.isServer) continue;

                if (somePlayer.name == data.playerName)
                    somePlayer.isAvailable = true;
            }

            if (Utility.AreAllPlayersAvailable())
                server.player.isAvailable = true;
        });
    }
}
