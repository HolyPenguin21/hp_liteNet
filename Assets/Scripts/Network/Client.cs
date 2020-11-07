using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Client : MonoBehaviour
{
    // Custom
    public Player player = new Player();
    public List<Player> players = new List<Player>();

    private string loginKey = "v0.0.1";
    private ClientSubscriptions cSubscription;

    // Initial
    public NetManager network;
    public NetPacketProcessor netProcessor;
    EventBasedNetListener listener;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        netProcessor = new NetPacketProcessor();
        cSubscription = new ClientSubscriptions(this, netProcessor);
        listener = new EventBasedNetListener();
        network = new NetManager(listener);
    }

    public void Connect(string playerName, string ipAddress)
    {
        // Client player
        player.isServer = false;
        player.isAvailable = true;
        player.name = playerName;
        player.heroId = 10;

        network.Start();

        network.Connect(ipAddress /* host ip or name */, 9050 /* port */, loginKey /* text key or NetDataWriter */);

        listener.NetworkReceiveEvent += (server, dataReader, deliveryMethod) =>
        {
            netProcessor.ReadAllPackets(dataReader, server);
            //dataReader.Recycle();
        };

        // Custom methods
        cSubscription.RaceChange();
        cSubscription.HeroChange();
        cSubscription.OnLogin();
        cSubscription.PlayersList();
        cSubscription.ChatMessage();
        cSubscription.SceneChange();
        cSubscription.CrNeutralPlayer();
        cSubscription.CrCharacter();
        cSubscription.UpdData();
        cSubscription.SetCameraToHero();
        cSubscription.EndTurn();
        cSubscription.SetCurrentTurn();
        cSubscription.Move();
        cSubscription.CapVillage();
        cSubscription.ItemCreate();
        cSubscription.ItemPickup();
        cSubscription.ItemDrop();
        cSubscription.ItemUse();
        cSubscription.ItemRemove();
        cSubscription.BlockActions();
        cSubscription.SetCharVars();
        cSubscription.CastSpell();
        cSubscription.CastItemSpell();
        cSubscription.SpellHeal();
        cSubscription.AttackResult();
        cSubscription.ReceivePoisonDmg();
        cSubscription.ReceiveSpellDmg();
        cSubscription.CharacterDie();
        cSubscription.AddExp();
        cSubscription.AddMaxHealth();
        cSubscription.OpenUpgradeMenu();
        cSubscription.Replace();
    }

    private void Update()
    {
        network.PollEvents();
    }

    public void StopClient()
    {
        network.Stop();
    }
}
