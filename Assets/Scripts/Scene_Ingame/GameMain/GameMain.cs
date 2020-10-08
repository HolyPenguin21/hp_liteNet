using System.Collections;
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

    //private ServerOrders sOrders;
    public GridManager gridManager;
    private UI_Ingame uiIngame;

    [HideInInspector] public CharactersData charactersData;
    [HideInInspector] public EffectsData effectsData;
    [HideInInspector] public SpellData spellData;
    public Pathfinding pathfinding;
    private AiNeutrals aiNeutrals;
    public Fog fog;

    // Prefabs > SceneObjects
    public GameObject movePathVisual;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        inst = this;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene_Map_Test")
        {
            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            uiIngame = GameObject.Find("UI").GetComponent<UI_Ingame>();

            Setup_Fog();
            Setup_Pathfinding();
            Setup_PostProcessing();
            Setup_CharactersData();
            Setup_EffectsData();
            Setup_SpellData();

            if (Utility.IsServer())
            {
                aiNeutrals = new AiNeutrals(this);

                if (loadGame)
                    StartCoroutine(Load());
                else
                    StartCoroutine(StartNewGame());
            }
        }
        else
        {
            if (gridManager != null) gridManager = null;
            if (daytime != null) daytime.postProcess = null;
        }
    }

    private IEnumerator StartNewGame()
    {
        yield return Server_CreateNeutralPlayer();

        for (int x = 0; x < server.players.Count; x++)
        {
            Player gameClient = server.players[x];
            if (gameClient.isNeutral) continue;

            gameClient.gold = 50;

            Hex hex = gridManager.Get_StartGridItem().hex;
            switch (gameClient.race)
            {
                case 0: // Humans
                    yield return Server_CreateCharacter(hex, 9, gameClient.name, true);
                    break;
                case 1: // Orcs
                    yield return Server_CreateCharacter(hex, 5, gameClient.name, true);
                    break;
                case 2: // Undeads
                    yield return Server_CreateCharacter(hex, 7, gameClient.name, true);
                    break;
            }
        }

        yield return null;
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1f);

        yield return Server_CreateNeutralPlayer();

        yield return SaveLoad.Load();

        //yield return Server_UpdateData();

        yield return null;
    }

    #region Create character
    public IEnumerator Server_CreateCharacter(Hex createAt, int characterId, string ownerName, bool isHero)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        yield return charactersData.CreateCharacter(createAt, characterId, ownerName, isHero);
        fog.Update_Fog();

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(createAt);
        CreateCharacter someCharacter = new CreateCharacter();
        someCharacter.coord_x = gridCoord.coord_x;
        someCharacter.coord_y = gridCoord.coord_y;
        someCharacter.characterId = characterId;
        someCharacter.ownerName = ownerName;
        someCharacter.isHero = isHero;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, someCharacter, DeliveryMethod.ReliableOrdered);
        }

        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_CreateCharacter(CreateCharacter someCharacter)
    {
        Hex createAt = gridManager.Get_GridItem_ByCoords(someCharacter.coord_x, someCharacter.coord_y).hex;

        yield return charactersData.CreateCharacter(createAt, someCharacter.characterId, someCharacter.ownerName, someCharacter.isHero);
        fog.Update_Fog();

        yield return Reply_TaskDone("Create character");
    }
    #endregion

    #region Create neutral player
    private IEnumerator Server_CreateNeutralPlayer()
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Player neutralPlayer = new Player();
        neutralPlayer.id = server.players.Count;
        neutralPlayer.isNeutral = true;
        neutralPlayer.name = "Neutrals";

        server.players.Add(neutralPlayer);

        CreateNeutralPlayer crNeutral = new CreateNeutralPlayer();
        crNeutral.id = neutralPlayer.id;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, crNeutral, DeliveryMethod.ReliableOrdered);
        }

        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_CreateNeutralPlayer(CreateNeutralPlayer crNeutral)
    {
        Player neutralPlayer = new Player();
        neutralPlayer.id = crNeutral.id;
        neutralPlayer.isNeutral = true;
        neutralPlayer.name = "Neutrals";

        client.players.Add(neutralPlayer);

        yield return Reply_TaskDone("Create neutral player");
    }
    #endregion

    #region Race change
    public void Request_RaceChange(RaceChange raceChange)
    {
        client.netProcessor.Send(client.network.GetPeerById(0), raceChange, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_RaceChange(RaceChange raceChange)
    {
        if (server.players.Count > 1) server.player.isAvailable = false;

        yield return raceChange.Implementation();

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, raceChange, DeliveryMethod.ReliableOrdered);
        }

        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_RaceChange(RaceChange raceChange)
    {
        yield return raceChange.Implementation();

        yield return Reply_TaskDone("Race change");
    }
    #endregion

    private IEnumerator Reply_TaskDone(string message)
    {
        TaskDone taskDone = new TaskDone();
        taskDone.playerName = client.player.name;
        taskDone.task = message;

        yield return client.netProcessor.Send(client.network.GetPeerById(0), taskDone, DeliveryMethod.ReliableOrdered);
        client.player.isAvailable = true;
    }

    #region Chat message
    public void Request_ChatMessage(ChatMessage chatMessage)
    {
        client.netProcessor.Send(client.network.GetPeerById(0), chatMessage, DeliveryMethod.ReliableOrdered);
    }
    public IEnumerator Server_ChatMessage(ChatMessage chatMessage)
    {
        if (server.players.Count > 1) server.player.isAvailable = false;

        yield return chatMessage.Implementation();

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(somePlayer.address, chatMessage, DeliveryMethod.ReliableOrdered);
        }

        yield return new WaitUntil(() => server.player.isAvailable);
    }
    public IEnumerator Client_ChatMessage(ChatMessage chatMessage)
    {
        yield return chatMessage.Implementation();

        yield return Reply_TaskDone("Chat message");
    }
    #endregion

    #region Load game scene
    public IEnumerator Server_LoadScene(string sceneName)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        SceneToLoad sceneToLoad = new SceneToLoad();
        sceneToLoad.sceneToLoad = sceneName;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, sceneToLoad, DeliveryMethod.ReliableOrdered);
        }

        sceneToLoad.Implementation();
        yield return null;
    }

    public IEnumerator Client_LoadScene(SceneToLoad sceneToLoad)
    {
        sceneToLoad.Implementation();

        // Reply about finished task
        TaskDone taskDone = new TaskDone();
        taskDone.playerName = client.player.name;
        taskDone.task = "Scene load.";

        yield return client.netProcessor.Send(client.network.GetPeerById(0), taskDone, DeliveryMethod.ReliableOrdered);
        client.player.isAvailable = true;
    }
    #endregion

    #region Setup scene
    private void Setup_Pathfinding()
    {
        List<Transform> pathVisuals = new List<Transform>();
        for (int x = 0; x < 40; x++) // TODO : count how many are actualy needed
            pathVisuals.Add(Instantiate(movePathVisual, transform).transform);
        foreach (Transform pv in pathVisuals)
            pv.gameObject.SetActive(false);

        pathfinding = new Pathfinding(pathVisuals);
    }

    private void Setup_PostProcessing()
    {
        daytime = new Daytime();
        daytime.postProcess = GameObject.Find("PostProcessing").GetComponent<PostProcessVolume>();
        daytime.postProcess.profile.TryGetSettings(out daytime.colorGradingLayer);
    }

    private void Setup_CharactersData()
    {
        charactersData = GetComponent<CharactersData>();
        charactersData.Init(this);

        uiIngame.charData = charactersData;
    }

    private void Setup_EffectsData()
    {
        effectsData = GetComponent<EffectsData>();
    }

    private void Setup_SpellData()
    {
        spellData = GetComponent<SpellData>();
    }

    private void Setup_Fog()
    {
        fog = new Fog(gridManager, server, client);
    }
    #endregion
}
