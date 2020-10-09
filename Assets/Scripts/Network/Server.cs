using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Server : MonoBehaviour
{
    // Custom
    public Player player = new Player();
    public List<Player> players = new List<Player>();

    private ServerSubscriptions sSubscription;

    // Initial
    public NetManager network;
    public NetPacketProcessor netProcessor;
    EventBasedNetListener listener = new EventBasedNetListener();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        netProcessor = new NetPacketProcessor();
        sSubscription = new ServerSubscriptions(this, netProcessor);
        listener = new EventBasedNetListener();
        network = new NetManager(listener);
    }

    public void Init(string hostName)
    {
        // Add server player
        player.id = 0;
        player.isServer = true;
        player.isAvailable = true;
        player.name = hostName;
        players.Add(player);

        //Debug.Log("Starting server ...");
        network.Start(9050);

        listener.ConnectionRequestEvent += request =>
        {
            request.AcceptIfKey("v0.0.1");
        };

        listener.PeerConnectedEvent += client =>
        {
            Debug.Log("Server > someone is connected, asking to login..." + client.EndPoint);
            //netProcessor.Send(client, new FooPacket() { NumberValue = 1, StringValue = "Test" }, DeliveryMethod.ReliableOrdered);

            Player somePlayer = new Player();
            players.Add(somePlayer);
            int id = players.IndexOf(somePlayer);
            somePlayer.address = client;
            somePlayer.id = id;
            somePlayer.isServer = false;

            netProcessor.Send(client, new LoginData() { clientId = id }, DeliveryMethod.ReliableOrdered);
        };

        listener.PeerDisconnectedEvent += (client, info) =>
        {
            Debug.Log("Server > someone is disconnected");

            int idToRemove = 0;
            for (int x = 0; x < players.Count; x++)
                if (players[x].address == client)
                    idToRemove = x;
            if (idToRemove != 0) players.RemoveAt(idToRemove);

            GameObject.FindObjectOfType<UI_MainMenu>().Update_ConnectedList();
        };

        listener.NetworkReceiveEvent += (someClient, dataReader, deliveryMethod) =>
        {
            netProcessor.ReadAllPackets(dataReader, someClient);
            //dataReader.Recycle();
        };

        // Custom methods
        sSubscription.RaceChange();
        sSubscription.ResponceOnLogin();
        sSubscription.ChatMessage();
        sSubscription.ResponsOnTaskDone();
        sSubscription.EndTurn();
        sSubscription.Move();
        sSubscription.ItemPickup();
        sSubscription.ItemDrop();
        sSubscription.ItemUse();
        sSubscription.RecruitCharacter();
    }

    private void Update()
    {
        network.PollEvents();
    }

    public void StopServer()
    {
        network.Stop();
    }
}
