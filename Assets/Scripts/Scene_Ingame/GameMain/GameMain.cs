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

        Hex hex2 = gridManager.Get_GridItem_ByCoords(3, 6).hex;
        yield return Server_CreateItem(hex2, 1); // Server is blocked

        hex2 = gridManager.Get_GridItem_ByCoords(1, 19).hex;
        yield return Server_CreateItem(hex2, 2); // Server is blocked

        hex2 = gridManager.Get_GridItem_ByCoords(6, 24).hex;
        yield return Server_CreateItem(hex2, 1); // Server is blocked

        hex2 = gridManager.Get_GridItem_ByCoords(8, 10).hex;
        yield return Server_CreateItem(hex2, 2); // Server is blocked

        yield return Server_UpdateData();

        yield return Server_ChangeTurn();

        yield return Server_SetCamera_ToHero();
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1f);

        yield return Server_CreateNeutralPlayer();

        yield return SaveLoad.Load();

        yield return Server_UpdateData();

        yield return null;
    }

    #region Capture village
    public IEnumerator Server_Village_SetOwner(Hex someHex, Player owner)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        if (someHex.villageOwner.name == "")
        {
            someHex.villageOwner = owner;
            Utility.Get_Client_byString(owner.name, server.players).villages++;
        }
        else
        {
            Utility.Get_Client_byString(someHex.villageOwner.name, server.players).villages--;
            someHex.villageOwner = owner;
            Utility.Get_Client_byString(owner.name, server.players).villages++;
        }

        Utility.Set_OwnerColor(someHex.transform, owner);

        CaptureVillage captureVillage = new CaptureVillage();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(someHex);
        captureVillage.coord_x = gridCoord.coord_x;
        captureVillage.coord_y = gridCoord.coord_y;
        captureVillage.ownerName = someHex.villageOwner.name;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, captureVillage, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    private IEnumerator Server_CaptureVillage(Hex someHex)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Character character = someHex.character;
        character.charMovement.movePoints_cur = 0;

        if (Utility.IsMyCharacter(character))
            GameObject.Find("UI").GetComponent<Ingame_Input>().SelectHex(someHex);

        if (someHex.villageOwner.name == "")
        {
            someHex.villageOwner = character.owner;
            Utility.Get_Client_byString(character.owner.name, server.players).villages++;
        }
        else
        {
            Utility.Get_Client_byString(someHex.villageOwner.name, server.players).villages--;
            someHex.villageOwner = character.owner;
            Utility.Get_Client_byString(character.owner.name, server.players).villages++;
        }

        Utility.Set_OwnerColor(someHex.transform, character.owner);

        CaptureVillage captureVillage = new CaptureVillage();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(someHex);
        captureVillage.coord_x = gridCoord.coord_x;
        captureVillage.coord_y = gridCoord.coord_y;
        captureVillage.ownerName = someHex.villageOwner.name;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, captureVillage, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_CaptureVillage(CaptureVillage capVillage)
    {
        Hex someHex = gridManager.Get_GridItem_ByCoords(capVillage.coord_x, capVillage.coord_y).hex;
        if (someHex.character != null)
        {
            someHex.character.charMovement.movePoints_cur = 0;

            if (Utility.IsMyCharacter(someHex.character))
                GameObject.Find("UI").GetComponent<Ingame_Input>().SelectHex(someHex);
        }

        someHex.villageOwner = Utility.Get_Client_byString(capVillage.ownerName, client.players);
        Utility.Set_OwnerColor(someHex.transform, someHex.villageOwner);

        yield return Reply_TaskDone("Village owner changed");
    }
    #endregion

    #region Move
    public void Request_Move(List<Hex> somePath)
    {
        if (somePath == null) return;
        if (somePath.Count == 0) return;

        Character character = somePath[0].character;
        if (!character.canAct || character.charMovement.movePoints_cur < 1) return;

        Move move = new Move();
        List<Utility.GridCoord> list = gridManager.Get_CoordPath(somePath);
        for (int i = 0; i < list.Count; i++)
        {
            move.pathData += "|";
            move.pathData += list[i].coord_x + ";";
            move.pathData += list[i].coord_y;
        }

        client.netProcessor.Send(client.network.GetPeerById(0), move, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_Move(List<Hex> somePath)
    {
        if (somePath == null) yield break;

        Character someCharacter = somePath[0].character;
        yield return On_Move(someCharacter, somePath);

        if (someCharacter.hex.isVillage && someCharacter.hex.villageOwner != someCharacter.owner)
        {
            yield return Server_CaptureVillage(someCharacter.hex);
            yield return Server_UpdateData(); // Server is blocked
        }

        if (someCharacter.hex.item != null && someCharacter.charItem == null)
        {
            yield return Server_PickupItem(someCharacter); // Server is blocked
        }
    }

    private IEnumerator On_Move(Character someCharacter, List<Hex> somePath)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        List<Hex> realPath = pathfinding.Get_RealPath(somePath);
        someCharacter.charMovement.movePoints_cur -= pathfinding.Get_PathCost_FromNext(realPath);

        Move move = new Move();
        move.mpLeft = someCharacter.charMovement.movePoints_cur;
        List<Utility.GridCoord> list = gridManager.Get_CoordPath(somePath);
        for (int i = 0; i < list.Count; i++)
        {
            move.pathData += "|";
            move.pathData += list[i].coord_x + ";";
            move.pathData += list[i].coord_y;
        }

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, move, DeliveryMethod.ReliableOrdered);
        }

        yield return someCharacter.Move(realPath);
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_Move(int mpLeft, List<Hex> somePath)
    {
        Character character = somePath[0].character;
        character.charMovement.movePoints_cur = mpLeft;
        yield return character.Move(somePath);

        yield return Reply_TaskDone("Character move");
    }
    #endregion

    #region End turn
    public void Request_EndTurn(EndTurn endTurn)
    {
        client.netProcessor.Send(client.network.GetPeerById(0), endTurn, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_ChangeTurn()
    {
        yield return Server_EndTurn();

        yield return NeutralsTurn();
    }

    private IEnumerator Server_EndTurn()
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        if (currentTurn.name == "")
        {
            currentTurn = server.players[Random.Range(0, server.players.Count)];
        }
        else
        {
            int curTurnPlayerId = server.players.FindIndex((Player x) => x.name == currentTurn.name);
            curTurnPlayerId++;
            if (curTurnPlayerId > server.players.Count - 1)
            {
                curTurnPlayerId = 0;
            }
            currentTurn = server.players[curTurnPlayerId];

            int incomeOnTurn = server.players[curTurnPlayerId].villages * Utility.villageIncome;
            server.players[curTurnPlayerId].gold += incomeOnTurn;

            yield return Server_UpdateData();
        }

        yield return End_Turn(currentTurn.name);

        if (server.players.Count > 2) server.player.isAvailable = false;

        EndTurn endTurn = new EndTurn();
        endTurn.playerName = currentTurn.name;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, endTurn, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_EndTurn(string clientName)
    {
        yield return End_Turn(clientName);

        yield return Reply_TaskDone("End turn");
    }

    private IEnumerator End_Turn(string clientName)
    {
        if (Utility.IsServer())
        {
            currentTurn = Utility.Get_Client_byString(clientName, server.players);
            if (currentTurn == server.players[0])
                daytime.Update_DayTime();
        }
        else
        {
            currentTurn = Utility.Get_Client_byString(clientName, client.players);
            if (currentTurn == client.players[0])
                daytime.Update_DayTime();
        }

        uiIngame.Update_PlayerInfoPanel();

        for (int i = 0; i < allCharacters.Count; i++)
        {
            Character character = allCharacters[i];
            if (character.owner == currentTurn)
            {
                character.OnMyTurnUpdate();
            }
            else
            {
                character.OnEnemyTurnUpdate();
            }
        }

        yield return null;
    }

    private IEnumerator NeutralsTurn()
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        if (currentTurn.name != "Neutrals") yield break;

        yield return aiNeutrals.Ai_Logic();
        yield return Server_EndTurn();

        yield return new WaitUntil(() => server.player.isAvailable);
    }
    #endregion

    #region Set camera
    private IEnumerator Server_SetCamera_ToHero()
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Vector3 camPos_Hero = server.players[0].heroCharacter.hex.transform.position;
        GameObject.Find("Main Camera").transform.position = camPos_Hero;

        SetCameraToHero camToHero = new SetCameraToHero();
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, camToHero, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_SetCamera_ToHero()
    {
        Vector3 camPos_Hero = client.players[1].heroCharacter.hex.transform.position;
        GameObject.Find("Main Camera").transform.position = camPos_Hero;

        yield return Reply_TaskDone("Set camera to hero");
    }
    #endregion

    #region Update data
    public IEnumerator Server_UpdateData()
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        uiIngame.Update_PlayerInfoPanel();
        fog.Update_Fog();

        UpdateData updateData = new UpdateData();
        for (int i = 0; i < server.players.Count; i++)
        {
            Player gameClient = server.players[i];
            updateData.data += "|";
            updateData.data += gameClient.name + ";";
            updateData.data += gameClient.race + ";";
            updateData.data += gameClient.gold + ";";
            updateData.data += gameClient.villages;
        }

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, updateData, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_UpdateData(UpdateData updateData)
    {
        List<Player> updatedPlayersData = new List<Player>();
        string[] playerData = updateData.data.Split('|');
        for (int x = 1; x < playerData.Length; x++)
        {
            string[] playerVars = playerData[x].Split(';');
            Player player = new Player();
            player.name = playerVars[0];
            player.race = int.Parse(playerVars[1]);
            player.gold = int.Parse(playerVars[2]);
            player.villages = int.Parse(playerVars[3]);
            updatedPlayersData.Add(player);
        }

        for (int i = 0; i < updatedPlayersData.Count; i++)
        {
            Player gameClient = Utility.Get_Client_byString(updatedPlayersData[i].name, client.players);
            gameClient.race = updatedPlayersData[i].race;
            gameClient.gold = updatedPlayersData[i].gold;
            gameClient.villages = updatedPlayersData[i].villages;
        }

        uiIngame.Update_PlayerInfoPanel();
        fog.Update_Fog();

        yield return Reply_TaskDone("Data updated");
    }
    #endregion

    #region Item - Create
    public IEnumerator Server_CreateItem(Hex createAt, int itemId)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        createAt.Add_Item(Get_Item_ById(itemId));

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(createAt);
        ItemCreate someItem = new ItemCreate();
        someItem.coord_x = gridCoord.coord_x;
        someItem.coord_y = gridCoord.coord_y;
        someItem.itemId = itemId;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, someItem, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_CreateItem(ItemCreate someItem)
    {
        Hex createAt = gridManager.Get_GridItem_ByCoords(someItem.coord_x, someItem.coord_y).hex;

        createAt.Add_Item(Get_Item_ById(someItem.itemId));

        yield return Reply_TaskDone("Create item");
    }
    #endregion

    #region Item - Pickup
    public void Request_PickupItem(Character character)
    {
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        ItemPickup pickItem = new ItemPickup();
        pickItem.coord_x = gridCoord.coord_x;
        pickItem.coord_y = gridCoord.coord_y;

        client.netProcessor.Send(client.network.GetPeerById(0), pickItem, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_PickupItem(Character character)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        character.Item_PickUp(character.hex);

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        ItemPickup pickItem = new ItemPickup();
        pickItem.coord_x = gridCoord.coord_x;
        pickItem.coord_y = gridCoord.coord_y;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, pickItem, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_PickupItem(ItemPickup pickItem)
    {
        Character character = gridManager.Get_GridItem_ByCoords(pickItem.coord_x, pickItem.coord_y).hex.character;
        character.Item_PickUp(character.hex);

        yield return Reply_TaskDone("Item picked up");
    }
    #endregion

    #region Item - Drop
    public void Request_DropItem(Character character)
    {
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        ItemDrop itemDrop = new ItemDrop();
        itemDrop.coord_x = gridCoord.coord_x;
        itemDrop.coord_y = gridCoord.coord_y;

        client.netProcessor.Send(client.network.GetPeerById(0), itemDrop, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_DropItem(Character character)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        character.Item_Drop();

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        ItemDrop itemDrop = new ItemDrop();
        itemDrop.coord_x = gridCoord.coord_x;
        itemDrop.coord_y = gridCoord.coord_y;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, itemDrop, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_DropItem(ItemDrop itemDrop)
    {
        Character character = gridManager.Get_GridItem_ByCoords(itemDrop.coord_x, itemDrop.coord_y).hex.character;
        character.Item_Drop();

        yield return Reply_TaskDone("Item droped");
    }
    #endregion

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

    private IEnumerator Reply_TaskDone(string message)
    {
        TaskDone taskDone = new TaskDone();
        taskDone.playerName = client.player.name;
        taskDone.task = message;

        yield return client.netProcessor.Send(client.network.GetPeerById(0), taskDone, DeliveryMethod.ReliableOrdered);
        client.player.isAvailable = true;
    }

    private Item Get_Item_ById(int itemId)
    {
        Item item = null;

        switch (itemId)
        {
            case 1:
                item = new Item_Belt();
                break;

            case 2:
                item = new Item_HealthPotion();
                break;
        }

        return item;
    }

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
