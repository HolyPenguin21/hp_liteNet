﻿using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour
{
    public static GameMain inst;
    public bool loadGame = false;

    [HideInInspector] public Server server;
    [HideInInspector] public Client client;

    #region Game data vars
    public Player currentTurn;
    public List<Character> allCharacters = new List<Character>();
    // Daytime
    private Daytime daytime;
    public Utility.dayTime dayTime_cur = Utility.dayTime.night2;
    #endregion

    public TaskManager taskManager;
    //private ServerOrders sOrders;
    public GridManager gridManager;
    private UI_Ingame uiIngame;

    public CharactersData charactersData;
    public EffectsData effectsData;
    public SpellData spellData;
    public Pathfinding pathfinding;
    private AiNeutrals aiNeutrals;
    public Fog fog;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        inst = this;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene_Map_Test")
        {
            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            uiIngame = GameObject.Find("UI").GetComponent<UI_Ingame>();
            //sOrders.gridManager = gridManager;
            //sOrders.server = server;
            //Setup_Fog();
            //Setup_Pathfinding();
            //Setup_PostProcessing();
            //Setup_CharactersData();
            //Setup_EffectsData();
            //Setup_SpellData();

            if (Utility.IsServer())
            {
                //server.serverMessenger.gridManager = gridManager;
                //aiNeutrals = new AiNeutrals(this);

                //if (load)
                //    StartCoroutine(Load());
                //else
                //    StartCoroutine(StartNewGame());
            }
            else
            {
                //client.clientMessenger.gridManager = gridManager;
            }
        }
        else
        {
            gridManager = null;
            daytime.postProcess = null;
        }
    }

    #region Race change
    public void Request_RaceChange(RaceChange raceChange)
    {
        client.netProcessor.Send(client.network.GetPeerById(0), raceChange, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_RaceChange(RaceChange raceChange)
    {
        yield return raceChange.Implementation();

        for (int x = 0; x < server.players.Count; x++)
        {
            if (server.players[x].isServer) continue;
            yield return server.netProcessor.Send(server.players[x].address, raceChange, DeliveryMethod.ReliableOrdered);
        }
    }

    public IEnumerator Client_RaceChange(RaceChange raceChange)
    {
        yield return raceChange.Implementation();
    }
    #endregion

    #region Chat message()
    public void Request_ChatMessage(ChatMessage chatMessage)
    {
        client.netProcessor.Send(client.network.GetPeerById(0), chatMessage, DeliveryMethod.ReliableOrdered);
    }
    public IEnumerator Server_ChatMessage(ChatMessage chatMessage)
    {
        yield return chatMessage.Implementation();

        for (int x = 0; x < server.players.Count; x++)
        {
            if (server.players[x].isServer) continue;
            yield return server.netProcessor.Send(server.players[x].address, chatMessage, DeliveryMethod.ReliableOrdered);
        }
    }
    public IEnumerator Client_ChatMessage(ChatMessage chatMessage)
    {
        yield return chatMessage.Implementation();
    }
    #endregion

    #region Load game scene
    public IEnumerator Server_LoadScene(string sceneName)
    {
        SceneToLoad sceneToLoad = new SceneToLoad();
        sceneToLoad.sceneToLoad = sceneName;

        for (int x = 0; x < server.players.Count; x++)
        {
            if (server.players[x].isServer) continue;
            yield return server.netProcessor.Send(server.players[x].address, sceneToLoad, DeliveryMethod.ReliableOrdered);
        }

        sceneToLoad.Implementation();
        yield return null;
    }

    public IEnumerator Client_LoadScene(SceneToLoad sceneToLoad)
    {
        sceneToLoad.Implementation();
        yield return null;
    }
    #endregion
}
