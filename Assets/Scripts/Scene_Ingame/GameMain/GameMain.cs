﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [HideInInspector] public Utility.dayTime dayTime_cur = Utility.dayTime.night2;
    #endregion

    public GridManager gridManager;
    private UI_Ingame ui_Ingame;
    [HideInInspector]  public IngameUI_Input ui_Input;

    [HideInInspector] public CharactersData charactersData;
    [HideInInspector] public EffectsData effectsData;
    [HideInInspector] public SpellData spellData;
    [HideInInspector] public ABuffData aBuffData;
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
            Utility.set_InputType();

            gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            ui_Ingame = GameObject.Find("UI").GetComponent<UI_Ingame>();
            ui_Input = GameObject.Find("UI").GetComponent<IngameUI_Input>();

            Setup_Fog();
            Setup_Pathfinding();
            Setup_PostProcessing();
            Setup_CharactersData();
            Setup_EffectsData();
            Setup_SpellData();
            Setup_ABuffData();

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
            Player somePlayer = server.players[x];
            if (somePlayer.isNeutral) continue;

            somePlayer.gold = 50;

            Hex hex = gridManager.Get_StartGridItem().hex;
            yield return Server_CreateCharacter(hex, somePlayer.heroId, somePlayer.name, true);
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

    #region Attack
    public void Try_Attack(Hex a_Hex, Hex t_Hex)
    {
        Character attacker = a_Hex.character;
        Character target = t_Hex.character;

        List<Hex> attackPath = new List<Hex>(pathfinding.Get_Path(a_Hex, t_Hex));
        if (attackPath.Count <= 1 || attacker.charMovement.movePoints_cur >= attackPath[0].moveCost)
            ui_Ingame.Show_AttackPanel(attacker, target);
    }

    public void Request_Attack(Character a_character, int a_attackId, Character t_character, int t_attackId)
    {
        AttackRequest aRequest = new AttackRequest();
        aRequest.Setup(a_character, a_attackId, t_character, t_attackId);

        client.netProcessor.Send(client.network.GetPeerById(0), aRequest, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_Attack(Character a_character, int a_attackId, Character t_character, int t_attackId)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        List<Hex> attackPath = new List<Hex>(pathfinding.Get_Path(a_character.hex, t_character.hex));
        attackPath.RemoveAt(attackPath.Count - 1); // last hex does not need - target is there

        if (attackPath.Count > 0) yield return Server_Move(a_character, attackPath[attackPath.Count - 1]);
        if (!Utility.InAttackRange(a_character.hex, t_character.hex)) yield break;

        yield return Server_BlockActions(a_character);

        AttackResult attackResult = new AttackResult();
        yield return attackResult.Setup(a_character, a_attackId, t_character, t_attackId);
        attackResult.AttackData_Calculation(a_character, t_character);
        
        // Send
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, attackResult, DeliveryMethod.ReliableOrdered);
        }

        yield return attackResult.Implementation();
        
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_Attack(AttackResult attackResult)
    {
        yield return attackResult.Implementation();
        yield return Reply_TaskDone("Attack done");
    }
    #endregion

    #region Level up
    public IEnumerator Server_LevelUp(Character character)
    {
        if (character.owner.name == "Neutrals" || character.owner != currentTurn || character.upgradeList.Count < 2)
        {
            if (character.upgradeList.Count >= 2)
            {
                yield return Server_UpgradeCharacter(character, character.upgradeList[UnityEngine.Random.Range(0, character.upgradeList.Count)]);
            }
            else if (character.upgradeList.Count == 1)
            {
                yield return Server_UpgradeCharacter(character, character.upgradeList[0]);
            }
            else
            {
                yield return 
                    
                    (character, character.charId);
            }
        }
        else if (character.owner.name == server.player.name)
        {
            ui_Ingame.Show_LevelupPanel(character);
            yield return null;
        }
        else
        {
            if (server.players.Count > 2) server.player.isAvailable = false;

            OpenUpgradeMenu upgradeMenu = new OpenUpgradeMenu();
            Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
            upgradeMenu.coord_x = gridCoord.coord_x;
            upgradeMenu.coord_y = gridCoord.coord_y;
            for (int x = 0; x < server.players.Count; x++)
            {
                Player somePlayer = server.players[x];
                if (somePlayer.isServer || somePlayer.isNeutral) continue;

                somePlayer.isAvailable = false;
                yield return server.netProcessor.Send(server.players[x].address, upgradeMenu, DeliveryMethod.ReliableOrdered);
            }

            yield return new WaitUntil(() => server.player.isAvailable);
        }
    }

    public IEnumerator Server_UpgradeCharacter(Character character, int upgradeId)
    {
        string ownerName = character.owner.name;
        Hex someHex = character.hex;
        Item charItem = character.charItem;

        yield return Server_Die(someHex);
        yield return Server_CreateCharacter(someHex, upgradeId, ownerName, character.heroCharacter);

        if (charItem == null) yield break;

        yield return Server_CreateItem(someHex, charItem.itemId);
        yield return Server_PickupItem(someHex.character);
    }

    public IEnumerator Client_OpenUpgradeMenu(OpenUpgradeMenu upgradeMenu)
    {
        Character character = gridManager.Get_GridItem_ByCoords(upgradeMenu.coord_x, upgradeMenu.coord_y).hex.character;

        ui_Ingame.Show_LevelupPanel(character);

        yield return Reply_TaskDone("Upgrade menu opened");
    }

    public void Request_UpgradeCharacter(Character character, int upgradeId)
    {
        UpgradeCharacter upgrade = new UpgradeCharacter();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        upgrade.coord_x = gridCoord.coord_x;
        upgrade.coord_y = gridCoord.coord_y;
        upgrade.upgId = upgradeId;

        client.netProcessor.Send(client.network.GetPeerById(0), upgrade, DeliveryMethod.ReliableOrdered);
    }
    #endregion

    #region Spellcasting
    public void Request_CastSpell(Hex charactersHex, Hex spellTargetHex, int spellId)
    {
        Utility.GridCoord characterCoords = gridManager.Get_GridCoord_ByHex(charactersHex);
        Utility.GridCoord targetCoords = gridManager.Get_GridCoord_ByHex(spellTargetHex);

        CastSpell castSpell = new CastSpell();
        castSpell.casterCoord_x = characterCoords.coord_x;
        castSpell.casterCoord_y = characterCoords.coord_y;
        castSpell.targetCoord_x = targetCoords.coord_x;
        castSpell.targetCoord_y = targetCoords.coord_y;
        castSpell.spellId = spellId;

        client.netProcessor.Send(client.network.GetPeerById(0), castSpell, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_CastSpell(Hex charactersHex, Hex spellTargetHex, int spellId)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Character character = charactersHex.character;
        Spell spell = spellData.Get_Spell_ById(character, spellId);
        spell.Use(spellTargetHex.transform.position);

        Utility.GridCoord characterCoords = gridManager.Get_GridCoord_ByHex(charactersHex);
        Utility.GridCoord targetCoords = gridManager.Get_GridCoord_ByHex(spellTargetHex);
        CastSpell castSpell = new CastSpell();
        castSpell.casterCoord_x = characterCoords.coord_x;
        castSpell.casterCoord_y = characterCoords.coord_y;
        castSpell.targetCoord_x = targetCoords.coord_x;
        castSpell.targetCoord_y = targetCoords.coord_y;
        castSpell.spellId = spellId;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, castSpell, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);

        yield return Server_BlockActions(character);

        List<Hex> concernedHexes = spellData.Get_ConcernedHexes(spellTargetHex, spellId);
        for (int x = 0; x < concernedHexes.Count; x++)
        {
            yield return spell.ResultingEffect(charactersHex, concernedHexes[x]);
        }
    }

    public IEnumerator Client_CastSpell(CastSpell castSpell)
    {
        Hex casterHex = gridManager.Get_GridItem_ByCoords(castSpell.casterCoord_x, castSpell.casterCoord_y).hex;
        Hex targetHex = gridManager.Get_GridItem_ByCoords(castSpell.targetCoord_x, castSpell.targetCoord_y).hex;

        spellData.Get_Spell_ById(casterHex.character, castSpell.spellId).Use(targetHex.transform.position);

        yield return Reply_TaskDone("Cast spell");
    }
    #endregion

    #region Cast Item spell
    public void Request_CastItemSpell(Hex charactersHex, Hex spellTargetHex, int spellId)
    {
        Utility.GridCoord characterCoords = gridManager.Get_GridCoord_ByHex(charactersHex);
        Utility.GridCoord targetCoords = gridManager.Get_GridCoord_ByHex(spellTargetHex);

        CastItemSpell castItemSpell = new CastItemSpell();
        castItemSpell.casterCoord_x = characterCoords.coord_x;
        castItemSpell.casterCoord_y = characterCoords.coord_y;
        castItemSpell.targetCoord_x = targetCoords.coord_x;
        castItemSpell.targetCoord_y = targetCoords.coord_y;
        castItemSpell.spellId = spellId;

        client.netProcessor.Send(client.network.GetPeerById(0), castItemSpell, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_CastItemSpell(Hex charactersHex, Hex spellTargetHex, int spellId)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Character character = charactersHex.character;
        Spell spell = spellData.Get_Spell_ById(spellId);
        spell.Use(spellTargetHex.transform.position);

        Utility.GridCoord characterCoords = gridManager.Get_GridCoord_ByHex(charactersHex);
        Utility.GridCoord targetCoords = gridManager.Get_GridCoord_ByHex(spellTargetHex);
        CastItemSpell castItemSpell = new CastItemSpell();
        castItemSpell.casterCoord_x = characterCoords.coord_x;
        castItemSpell.casterCoord_y = characterCoords.coord_y;
        castItemSpell.targetCoord_x = targetCoords.coord_x;
        castItemSpell.targetCoord_y = targetCoords.coord_y;
        castItemSpell.spellId = spellId;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, castItemSpell, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);

        yield return Server_BlockActions(character);

        List<Hex> concernedHexes = spellData.Get_ConcernedHexes(spellTargetHex, spellId);
        for (int x = 0; x < concernedHexes.Count; x++)
        {
            yield return spell.ResultingEffect(charactersHex, concernedHexes[x]);
        }
    }

    public IEnumerator Client_CastItemSpell(CastItemSpell castSpell)
    {
        Hex casterHex = gridManager.Get_GridItem_ByCoords(castSpell.casterCoord_x, castSpell.casterCoord_y).hex;
        Hex targetHex = gridManager.Get_GridItem_ByCoords(castSpell.targetCoord_x, castSpell.targetCoord_y).hex;

        spellData.Get_Spell_ById(castSpell.spellId).Use(targetHex.transform.position);

        yield return Reply_TaskDone("Cast spell");
    }
    #endregion

    #region Spell resulting effect
    public IEnumerator Server_SpellDamage(Hex casterHex, Hex targetHex, int amount)
    {
        if (targetHex.character == null) yield break;
        if (server.players.Count > 2) server.player.isAvailable = false;

        SpellDamage spellDamage = new SpellDamage();
        yield return spellDamage.Setup(targetHex, amount);
        yield return spellDamage.Implementation();

        // Send
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, spellDamage, DeliveryMethod.ReliableOrdered);
        }

        yield return new WaitUntil(() => server.player.isAvailable);

        if (targetHex.character.charHp.hp_cur <= 0)
        {
            yield return Server_Die(targetHex); // Server is blocked
            yield return Server_AddExp(casterHex, 3); // Server is blocked
        }
    }

    public IEnumerator Client_SpellDamage(SpellDamage spellDamage)
    {
        yield return spellDamage.Implementation();
        yield return Reply_TaskDone("Spell damage is done");
    }

    public IEnumerator Server_SpellHeal(Hex targetHex, int amount)
    {
        if (targetHex.character == null) yield break;

        if (server.players.Count > 2) server.player.isAvailable = false;

        Character c = targetHex.character;
        c.RecieveHeal(amount);

        SpellHeal spellHeal = new SpellHeal();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(targetHex);
        spellHeal.coord_x = gridCoord.coord_x;
        spellHeal.coord_y = gridCoord.coord_y;
        spellHeal.amount = amount;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, spellHeal, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_SpellHeal(SpellHeal spellHeal)
    {
        Character c = gridManager.Get_GridItem_ByCoords(spellHeal.coord_x, spellHeal.coord_y).hex.character;

        c.RecieveHeal(spellHeal.amount);

        yield return Reply_TaskDone("Spell healing is done");
    }

    public IEnumerator Server_SpellSummon(Hex casterHex, Hex targetHex, int summonId)
    {
        if (targetHex.character != null) yield break;

        yield return Server_CreateCharacter(targetHex, summonId, casterHex.character.owner.name, false);
    }
    #endregion

    #region Blink spell
    public IEnumerator Server_Blink(Hex casterHex, Hex targetHex)
    {
        if (casterHex.character == null) yield break;

        if (server.players.Count > 2) server.player.isAvailable = false;

        Character c = casterHex.character;
        effectsData.Effect_DarkPortal(c.tr.position, c.tr);
        c.Replace(targetHex);

        yield return Server_AfterMoveCheck(c);

        Utility.GridCoord casterCoord = gridManager.Get_GridCoord_ByHex(casterHex);
        Utility.GridCoord targetCoord = gridManager.Get_GridCoord_ByHex(targetHex);
        Replace replace = new Replace();
        replace.caster_coord_x = casterCoord.coord_x;
        replace.caster_coord_y = casterCoord.coord_y;
        replace.target_coord_x = targetCoord.coord_x;
        replace.target_coord_y = targetCoord.coord_y;

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, replace, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_Blink(Replace replace)
    {
        Character c = gridManager.Get_GridItem_ByCoords(replace.caster_coord_x, replace.caster_coord_y).hex.character;
        Hex targetHex = gridManager.Get_GridItem_ByCoords(replace.target_coord_x, replace.target_coord_y).hex;

        effectsData.Effect_DarkPortal(c.tr.position, c.tr);
        c.Replace(targetHex);

        yield return Reply_TaskDone("Blink done");
    }
    #endregion

    #region Add exp
    public IEnumerator Server_AddExp(Hex charactersHex, int expAmount)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;
        if (charactersHex == null) yield break;
        if (charactersHex.character == null) yield break;

        charactersHex.character.Add_Exp(expAmount);

        AddExp addExp = new AddExp();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(charactersHex);
        addExp.coord_x = gridCoord.coord_x;
        addExp.coord_y = gridCoord.coord_y;
        addExp.amount = expAmount;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, addExp, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_AddExp(AddExp addExp)
    {
        Character c = gridManager.Get_GridItem_ByCoords(addExp.coord_x, addExp.coord_y).hex.character;

        c.Add_Exp(addExp.amount);

        yield return Reply_TaskDone("Exp added");
    }
    #endregion

    #region Change max health
    public IEnumerator Server_ChangeMaxHealth(Hex charHex, int amount)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Character someChar = charHex.character;
        someChar.charHp.hp_cur += amount;
        someChar.charHp.hp_max += amount;

        if (someChar.charHp.hp_cur <= 0) someChar.charHp.hp_cur = 1;

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(charHex);
        AddMaxHealth addMaxHealth = new AddMaxHealth();
        addMaxHealth.coord_x = gridCoord.coord_x;
        addMaxHealth.coord_y = gridCoord.coord_y;
        addMaxHealth.curHealth = someChar.charHp.hp_cur;
        addMaxHealth.maxHealth = someChar.charHp.hp_max;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, addMaxHealth, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_AddMaxHealth(AddMaxHealth addMaxHealth)
    {
        Character someChar = gridManager.Get_GridItem_ByCoords(addMaxHealth.coord_x, addMaxHealth.coord_y).hex.character;
        someChar.charHp.hp_cur = addMaxHealth.curHealth;
        someChar.charHp.hp_max = addMaxHealth.maxHealth;

        yield return Reply_TaskDone("Health added");
    }
    #endregion

    #region Die
    public IEnumerator Server_Die(Hex dieAtHex)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Character character = dieAtHex.character;
        if (character.charItem != null)
        {
            yield return Server_DropItem(character);
        }

        allCharacters.Remove(character);
        character.hex = null;
        dieAtHex.character = null;
        Destroy(character.tr.gameObject);

        CharacterDie characterDie = new CharacterDie();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(dieAtHex);
        characterDie.coord_x = gridCoord.coord_x;
        characterDie.coord_y = gridCoord.coord_y;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, characterDie, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_Die(CharacterDie characterDie)
    {
        Hex dieAtHex = gridManager.Get_GridItem_ByCoords(characterDie.coord_x, characterDie.coord_y).hex;
        Character character = dieAtHex.character;

        allCharacters.Remove(character);
        character.hex = null;
        dieAtHex.character = null;
        Destroy(character.tr.gameObject);

        yield return Reply_TaskDone("Character died");
    }
    #endregion

    #region Receive poison dmg
    public IEnumerator Server_ReceivePoisonDmg(Hex charHex)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        ReceivePoisonDmg poison = new ReceivePoisonDmg();
        yield return poison.Setup(charHex);
        yield return poison.Implementation();

        // Send
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, poison, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_ReceivePoisonDmg(ReceivePoisonDmg poison)
    {
        yield return poison.Implementation();
        yield return Reply_TaskDone("Poison dmg is done");
    }
    #endregion

    #region Recruit
    public void Request_Recruit(Hex createAt, int characterId, string ownerName, int cost)
    {
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(createAt);
        RecruitCharacter recruit = new RecruitCharacter();
        recruit.coord_x = gridCoord.coord_x;
        recruit.coord_y = gridCoord.coord_y;
        recruit.characterId = characterId;
        recruit.ownerName = ownerName;
        recruit.characterCost = cost;

        client.netProcessor.Send(client.network.GetPeerById(0), recruit, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_Recruit(Hex createAt, int characterId, string ownerName, int cost)
    {
        yield return Server_RemoveGold(ownerName, cost);
        yield return Server_CreateCharacter(createAt, characterId, ownerName, isHero: false);
    }
    #endregion

    #region Remove gold
    public IEnumerator Server_RemoveGold(string clientName, int amount)
    {
        Utility.Get_Client_byString(clientName).gold -= amount;
        yield return Server_UpdateData();
    }
    #endregion

    #region Capture village
    public IEnumerator Server_Village_SetOwner(Hex someHex, Player owner)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        if (someHex.villageOwner.name == "")
        {
            someHex.villageOwner = owner;
            Utility.Get_Client_byString(owner.name).villages++;
        }
        else
        {
            Utility.Get_Client_byString(someHex.villageOwner.name).villages--;
            someHex.villageOwner = owner;
            Utility.Get_Client_byString(owner.name).villages++;
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
            ui_Input.SelectHex(someHex);

        if (someHex.villageOwner.name == "")
        {
            someHex.villageOwner = character.owner;
            Utility.Get_Client_byString(character.owner.name).villages++;
        }
        else
        {
            Utility.Get_Client_byString(someHex.villageOwner.name).villages--;
            someHex.villageOwner = character.owner;
            Utility.Get_Client_byString(character.owner.name).villages++;
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
                ui_Input.SelectHex(someHex);
        }

        someHex.villageOwner = Utility.Get_Client_byString(capVillage.ownerName);
        Utility.Set_OwnerColor(someHex.transform, someHex.villageOwner);

        yield return Reply_TaskDone("Village owner changed");
    }
    #endregion

    #region Move
    public void Request_Move(Character character, Hex destination)
    {
        MoveRequest moveRequest = new MoveRequest();
        moveRequest.Setup(character, destination);

        client.netProcessor.Send(client.network.GetPeerById(0), moveRequest, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_Move(Character character, Hex destination)
    {
        List<Hex> path = pathfinding.Get_Path(character.hex, destination);
        if (path == null) yield break;

        yield return On_Move(character, path);

        yield return Server_AfterMoveCheck(character);
    }

    private IEnumerator On_Move(Character character, List<Hex> somePath)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        List<Hex> realPath = pathfinding.Get_RealPath(character, somePath);
        if (realPath == null) yield break;
        //if (realPath.Count == 0) yield break;

        Move move = new Move();
        move.Setup(character, realPath);

        // Send
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, move, DeliveryMethod.ReliableOrdered);
        }

        yield return move.Server_Implementation(character, realPath);

        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Server_AfterMoveCheck(Character someCharacter)
    {
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

    public IEnumerator Client_Move(Move move)
    {
        yield return move.Client_Implementation(move);        

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
            currentTurn = server.players[UnityEngine.Random.Range(0, server.players.Count)];
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
        currentTurn = Utility.Get_Client_byString(clientName);

        if (Utility.IsServer())
        {
            if (currentTurn == server.players[0])
                daytime.Update_DayTime();
        }
        else
        {
            if (currentTurn == client.players[0])
                daytime.Update_DayTime();
        }

        ui_Ingame.Update_OnTurnUI();

        for (int i = 0; i < allCharacters.Count; i++)
        {
            Character character = allCharacters[i];
            if (character.owner == currentTurn)
            {
                StartCoroutine(character.OnMyTurnUpdate());
            }
            else
            {
                StartCoroutine(character.OnEnemyTurnUpdate());
            }
        }

        yield return null;
    }

    private IEnumerator NeutralsTurn()
    {
        if (currentTurn.name != "Neutrals") yield break;

        yield return aiNeutrals.Ai_Logic();
        yield return Server_EndTurn();
    }
    #endregion

    #region Set current turn
    public IEnumerator Server_SetCurTurn()
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        SetCurrentTurn setTurn = new SetCurrentTurn();
        setTurn.name = currentTurn.name;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, setTurn, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_SetCurTurn(SetCurrentTurn setTurn)
    {
        currentTurn = Utility.Get_Client_byString(setTurn.name);

        yield return Reply_TaskDone("Current turn is setted.");
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

        ui_Ingame.Update_PlayerInfoPanel();
        //fog.Update_Fog();

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
            Player gameClient = Utility.Get_Client_byString(updatedPlayersData[i].name);
            gameClient.race = updatedPlayersData[i].race;
            gameClient.gold = updatedPlayersData[i].gold;
            gameClient.villages = updatedPlayersData[i].villages;
        }

        ui_Ingame.Update_PlayerInfoPanel();
        //fog.Update_Fog();

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

    #region Item - Use
    public void Request_UseItem(Character character)
    {
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        ItemUse itemUse = new ItemUse();
        itemUse.coord_x = gridCoord.coord_x;
        itemUse.coord_y = gridCoord.coord_y;

        client.netProcessor.Send(client.network.GetPeerById(0), itemUse, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_UseItem_Logic(Character character)
    {
        yield return Server_UseItem(character);

        yield return Server_BlockActions(character);

        if (character.charItem.itemOneTime)
            yield return Server_RemoveItem(character);

        yield return null;
    }

    private IEnumerator Server_UseItem(Character character)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        yield return character.Item_Use();

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        ItemUse itemUse = new ItemUse();
        itemUse.coord_x = gridCoord.coord_x;
        itemUse.coord_y = gridCoord.coord_y;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, itemUse, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_UseItem(ItemUse itemUse)
    {
        Character character = gridManager.Get_GridItem_ByCoords(itemUse.coord_x, itemUse.coord_y).hex.character;
        yield return character.Item_Use();

        yield return Reply_TaskDone("Item used");
    }
    #endregion

    #region Item - Remove
    private IEnumerator Server_RemoveItem(Character character)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        character.Item_Remove();

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(character.hex);
        ItemRemove itemRemove = new ItemRemove();
        itemRemove.coord_x = gridCoord.coord_x;
        itemRemove.coord_y = gridCoord.coord_y;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, itemRemove, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_RemoveItem(ItemRemove itemRemove)
    {
        Character character = gridManager.Get_GridItem_ByCoords(itemRemove.coord_x, itemRemove.coord_y).hex.character;
        character.Item_Remove();

        yield return Reply_TaskDone("Item removed");
    }
    #endregion

    #region Block actions
    public IEnumerator Server_BlockActions(Character someCharacter)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        someCharacter.canAct = false;
        someCharacter.charMovement.movePoints_cur = 0;
        if (someCharacter.tr != null) someCharacter.tr.Find("canMove").gameObject.SetActive(false);

        if (Utility.IsMyCharacter(someCharacter))
            fog.UpdateFog_CharacterView(someCharacter);

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(someCharacter.hex);
        BlockActions blockActions = new BlockActions();
        blockActions.coord_x = gridCoord.coord_x;
        blockActions.coord_y = gridCoord.coord_y;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, blockActions, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_BlockActions(BlockActions blockActions)
    {
        Character someCharacter = gridManager.Get_GridItem_ByCoords(blockActions.coord_x, blockActions.coord_y).hex.character;

        someCharacter.canAct = false;
        someCharacter.charMovement.movePoints_cur = 0;

        if (Utility.IsMyCharacter(someCharacter))
            fog.UpdateFog_CharacterView(someCharacter);

        yield return Reply_TaskDone("Character actions are blocked");
    }
    #endregion

    #region Create character
    public IEnumerator Server_CreateCharacter(Hex createAt, int characterId, string ownerName, bool isHero)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        yield return charactersData.CreateCharacter(createAt, characterId, ownerName, isHero);
        fog.UpdateFog_PlayerView();

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
        fog.UpdateFog_PlayerView();

        yield return Reply_TaskDone("Create character");
    }
    #endregion

    #region Character - Set Vars
    public IEnumerator Server_Character_SetVars(Hex hex, bool canAct, int hp, int mp, int exp)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        Character character = hex.character;
        character.canAct = canAct;
        character.charHp.hp_cur = hp;
        character.charMovement.movePoints_cur = mp;
        character.charExp.exp_cur = exp;

        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(hex);
        SetCharacterVars setVars = new SetCharacterVars();
        setVars.coord_x = gridCoord.coord_x;
        setVars.coord_y = gridCoord.coord_y;
        setVars.canAct = canAct;
        setVars.curHp = hp;
        setVars.curMp = mp;
        setVars.curExp = exp;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, setVars, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_Character_SetVars(SetCharacterVars setVars)
    {
        Character character = gridManager.Get_GridItem_ByCoords(setVars.coord_x, setVars.coord_y).hex.character;

        character.canAct = setVars.canAct;
        character.charHp.hp_cur = setVars.curHp;
        character.charMovement.movePoints_cur = setVars.curMp;
        character.charExp.exp_cur = setVars.curExp;

        yield return Reply_TaskDone("Character stats are set");
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

        yield return raceChange.Implementation(GameObject.Find("UI").GetComponent<UI_MainMenu>());

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
        yield return raceChange.Implementation(GameObject.Find("UI").GetComponent<UI_MainMenu>());

        yield return Reply_TaskDone("Race change");
    }
    #endregion

    #region Hero change
    public void Request_HeroChange(HeroChange heroChange)
    {
        client.netProcessor.Send(client.network.GetPeerById(0), heroChange, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_HeroChange(HeroChange heroChange)
    {
        if (server.players.Count > 1) server.player.isAvailable = false;

        yield return heroChange.Implementation(GameObject.Find("UI").GetComponent<UI_MainMenu>());

        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, heroChange, DeliveryMethod.ReliableOrdered);
        }

        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_HeroChange(HeroChange heroChange)
    {
        yield return heroChange.Implementation(GameObject.Find("UI").GetComponent<UI_MainMenu>());

        yield return Reply_TaskDone("Hero change");
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

        ui_Ingame.charData = charactersData;
    }

    private void Setup_EffectsData()
    {
        effectsData = GetComponent<EffectsData>();
    }

    private void Setup_SpellData()
    {
        spellData = GetComponent<SpellData>();
        ui_Input.spData = spellData;
    }

    private void Setup_ABuffData()
    {
        aBuffData = GetComponent<ABuffData>();
    }

    private void Setup_Fog()
    {
        fog = new Fog(server, client, gridManager);
    }
    #endregion
}
