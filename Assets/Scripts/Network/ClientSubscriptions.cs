using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class ClientSubscriptions
{
    Client client;
    private NetPacketProcessor netProcessor;

    public ClientSubscriptions(Client client, NetPacketProcessor netPacketProcessor)
    {
        this.client = client;
        this.netProcessor = netPacketProcessor;
    }
    public void CrCharacter()
    {
        netProcessor.SubscribeReusable<CreateCharacter>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_CreateCharacter(data));
        });
    }

    public void CrNeutralPlayer()
    {
        netProcessor.SubscribeReusable<CreateNeutralPlayer>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_CreateNeutralPlayer(data));
        });
    }

    public void SceneChange()
    {
        netProcessor.SubscribeReusable<SceneToLoad>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_LoadScene(data));
        });
    }

    public void RaceChange()
    {
        netProcessor.SubscribeReusable<RaceChange>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_RaceChange(data));
        });
    }

    public void ChatMessage()
    {
        netProcessor.SubscribeReusable<ChatMessage>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_ChatMessage(data));
        });
    }

    public void PlayersList()
    {
        netProcessor.SubscribeReusable<PlayersList>((data) => {
            Debug.Log("Client > Players list from Server.");
            string[] sData = data.playersList.Split('|');
            for (int x = 1; x < sData.Length; x++)
            {
                Player somePlayer = new Player();
                somePlayer.name = sData[x];
                if (x == 1) somePlayer.isServer = true;
                client.players.Add(somePlayer);
            }

            GameObject.FindObjectOfType<UI_MainMenu>().Client_OnConnect();
            GameObject.FindObjectOfType<UI_MainMenu>().Setup_RacePicker();
        });
    }
    public void OnLogin()
    {
        netProcessor.SubscribeReusable<LoginData>((data) => {
            Debug.Log("Client > Login request from Server.");

            netProcessor.Send(client.network.GetPeerById(0), new LoginData() { clientId = data.clientId, clientName = client.player.name }, DeliveryMethod.ReliableOrdered);
        });
    }
}
