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

    public void UpgradeCharacter()
    {
        netProcessor.SubscribeReusable<UpgradeCharacter>((data) => {
            Character character = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.coord_x, data.coord_y).hex.character;

            server.StartCoroutine(GameMain.inst.Server_UpgradeCharacter(character, data.upgId));
        });
    }

    public void AttackRequest()
    {
        netProcessor.SubscribeReusable<AttackRequest>((data) => {
            Character a_character = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.a_coord_x, data.a_coord_y).hex.character;
            int a_attackId = data.a_attackId;
            Character t_character = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.t_coord_x, data.t_coord_y).hex.character;
            int t_attackId = data.t_attackId;

            server.StartCoroutine(GameMain.inst.Server_Attack(a_character, a_attackId, t_character, t_attackId));
        });
    }

    public void CastSpell()
    {
        netProcessor.SubscribeReusable<CastSpell>((data) => {
            Hex charactersHex = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.casterCoord_x, data.casterCoord_y).hex;
            Hex spellTargetHex = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.targetCoord_x, data.targetCoord_y).hex;

            server.StartCoroutine(GameMain.inst.Server_CastSpell(charactersHex, spellTargetHex, data.spellId));
        });
    }

    public void CastItemSpell()
    {
        netProcessor.SubscribeReusable<CastItemSpell>((data) => {
            Hex charactersHex = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.casterCoord_x, data.casterCoord_y).hex;
            Hex spellTargetHex = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.targetCoord_x, data.targetCoord_y).hex;

            server.StartCoroutine(GameMain.inst.Server_CastItemSpell(charactersHex, spellTargetHex, data.spellId));
        });
    }

    public void RecruitCharacter()
    {
        netProcessor.SubscribeReusable<RecruitCharacter>((data) => {
            Hex createAt = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.coord_x, data.coord_y).hex;
            server.StartCoroutine(GameMain.inst.Server_Recruit(createAt, data.characterId, data.ownerName, data.characterCost));
        });
    }

    public void ItemUse()
    {
        netProcessor.SubscribeReusable<ItemUse>((data) => {
            Character character = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.coord_x, data.coord_y).hex.character;
            server.StartCoroutine(GameMain.inst.Server_UseItem_Logic(character));
        });
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

    public void MoveRequest()
    {
        netProcessor.SubscribeReusable<MoveRequest>((data) => {
            Character character = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.c_coord_x, data.c_coord_y).hex.character;
            Hex destination = GameMain.inst.gridManager.Get_GridItem_ByCoords(data.d_coord_x, data.d_coord_y).hex;

            server.StartCoroutine(GameMain.inst.Server_Move(character, destination));
        });
    }

    public void EndTurn()
    {
        netProcessor.SubscribeReusable<EndTurn>((data) => {
            server.StartCoroutine(GameMain.inst.Server_ChangeTurn());
        });
    }

    public void HeroChange()
    {
        netProcessor.SubscribeReusable<HeroChange>((data) => {
            server.StartCoroutine(GameMain.inst.Server_HeroChange(data));
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
