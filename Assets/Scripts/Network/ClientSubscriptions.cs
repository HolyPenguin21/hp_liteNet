﻿using System;
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

    public void Replace()
    {
        netProcessor.SubscribeReusable<Replace>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_Blink(data));
        });
    }

    public void AddExp()
    {
        netProcessor.SubscribeReusable<AddExp>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_AddExp(data));
        });
    }

    public void AddMaxHealth()
    {
        netProcessor.SubscribeReusable<AddMaxHealth>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_AddMaxHealth(data));
        });
    }

    public void CharacterDie()
    {
        netProcessor.SubscribeReusable<CharacterDie>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_Die(data));
        });
    }

    public void OpenUpgradeMenu()
    {
        netProcessor.SubscribeReusable<OpenUpgradeMenu>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_OpenUpgradeMenu(data));
        });
    }

    public void ReceiveSpellDmg()
    {
        netProcessor.SubscribeReusable<SpellDamage>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_SpellDamage(data));
        });
    }

    public void ReceivePoisonDmg()
    {
        netProcessor.SubscribeReusable<ReceivePoisonDmg>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_ReceivePoisonDmg(data));
        });
    }

    public void AttackResult()
    {
        netProcessor.SubscribeReusable<AttackResult>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_Attack(data));
        });
    }

    public void CastSpell()
    {
        netProcessor.SubscribeReusable<CastSpell>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_CastSpell(data));
        });
    }

    public void CastItemSpell()
    {
        netProcessor.SubscribeReusable<CastItemSpell>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_CastItemSpell(data));
        });
    }

    public void SpellHeal()
    {
        netProcessor.SubscribeReusable<SpellHeal>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_SpellHeal(data));
        });
    }

    public void BlockActions()
    {
        netProcessor.SubscribeReusable<BlockActions>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_BlockActions(data));
        });
    }

    public void ItemRemove()
    {
        netProcessor.SubscribeReusable<ItemRemove>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_RemoveItem(data));
        });
    }

    public void ItemUse()
    {
        netProcessor.SubscribeReusable<ItemUse>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_UseItem(data));
        });
    }

    public void ItemDrop()
    {
        netProcessor.SubscribeReusable<ItemDrop>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_DropItem(data));
        });
    }

    public void ItemPickup()
    {
        netProcessor.SubscribeReusable<ItemPickup>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_PickupItem(data));
        });
    }

    public void CapVillage()
    {
        netProcessor.SubscribeReusable<CaptureVillage>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_CaptureVillage(data));
        });
    }

    public void Move()
    {
        netProcessor.SubscribeReusable<Move>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_Move(data));
        });
    }

    public void SetCurrentTurn()
    {
        netProcessor.SubscribeReusable<SetCurrentTurn>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_SetCurTurn(data));
        });
    }

    public void EndTurn()
    {
        netProcessor.SubscribeReusable<EndTurn>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_EndTurn(data.playerName));
        });
    }

    public void SetCameraToHero()
    {
        netProcessor.SubscribeReusable<SetCameraToHero>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_SetCamera_ToHero());
        });
    }

    public void UpdData()
    {
        netProcessor.SubscribeReusable<UpdateData>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_UpdateData(data));
        });
    }

    public void ItemCreate()
    {
        netProcessor.SubscribeReusable<ItemCreate>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_CreateItem(data));
        });
    }

    public void CrCharacter()
    {
        netProcessor.SubscribeReusable<CreateCharacter>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_CreateCharacter(data));
        });
    }

    public void SetCharVars()
    {
        netProcessor.SubscribeReusable<SetCharacterVars>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_Character_SetVars(data));
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

    public void HeroChange()
    {
        netProcessor.SubscribeReusable<HeroChange>((data) => {
            client.player.isAvailable = false;

            client.StartCoroutine(GameMain.inst.Client_HeroChange(data));
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
                somePlayer.heroId = 10;
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
