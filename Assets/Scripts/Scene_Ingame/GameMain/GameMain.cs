using System;
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
                    yield return Server_CreateCharacter(hex, 10, gameClient.name, true);
                    break;
                case 1: // Orcs
                    yield return Server_CreateCharacter(hex, 5, gameClient.name, true);
                    break;
                case 2: // Undeads
                    yield return Server_CreateCharacter(hex, 17, gameClient.name, true);
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

    #region Attack
    public void Try_Attack(Hex attacker, Hex target)
    {
        List<Hex> list = new List<Hex>(pathfinding.Get_Path(attacker, target));
        Character character = attacker.character;
        if (list.Count <= 2 || (character.charMovement.movePoints_cur != 0 && character.charMovement.movePoints_cur >= list[1].moveCost))
        {
            uiIngame.Show_AttackPanel(list);
        }
    }

    public void Request_Attack(List<Hex> somePath, int attackId)
    {
        Attack attack = new Attack();
        attack.attackId = attackId;
        List<Utility.GridCoord> coordPath = gridManager.Get_CoordPath(somePath);
        for (int i = 0; i < coordPath.Count; i++)
        {
            attack.path += "|";
            attack.path += coordPath[i].coord_x + ";";
            attack.path += coordPath[i].coord_y;
        }

        client.netProcessor.Send(client.network.GetPeerById(0), attack, DeliveryMethod.ReliableOrdered);
    }

    public IEnumerator Server_Attack(List<Hex> somePath, int attackId)
    {
        Character a_Character = somePath[0].character;
        List<Utility.char_Attack> a_Attack = a_Character.charAttacks;

        Character t_Character = somePath[somePath.Count - 1].character;
        List<Utility.char_Attack> t_Attack = t_Character.charAttacks;

        somePath.RemoveAt(somePath.Count - 1);
        if (somePath.Count > 1)
        {
            yield return Server_Move(somePath);
        }

        if (!Utility.InAttackRange(a_Character.hex, t_Character.hex)) yield break;

        yield return Server_BlockActions(a_Character);

        int attacksCount_attacker = 0;
        if (a_Attack.Count > attackId)
            attacksCount_attacker = a_Attack[attackId].attackCount;

        int attacksCount_target = 0;
        if (t_Attack.Count > attackId)
            attacksCount_target = t_Attack[attackId].attackCount;

        while (attacksCount_attacker > 0 || attacksCount_target > 0)
        {
            if (attacksCount_attacker > 0)
            {
                attacksCount_attacker--;
                yield return Server_AttackAnim(a_Character.hex, t_Character.hex, attackId);
                yield return Server_AttackResult(a_Character, t_Character, attackId);

                if (t_Character.charHp.hp_cur <= 0)
                {
                    yield return Server_Die(t_Character.hex);
                    yield return Server_AddExp(a_Character.hex, 3);
                    break;
                }
            }

            if (attacksCount_target > 0)
            {
                attacksCount_target--;
                yield return Server_AttackAnim(t_Character.hex, a_Character.hex, attackId);
                yield return Server_AttackResult(t_Character, a_Character, attackId);

                if (a_Character.charHp.hp_cur <= 0)
                {
                    yield return Server_Die(a_Character.hex);
                    yield return Server_AddExp(t_Character.hex, 3);
                    break;
                }
            }
            yield return null;
        }

        if (a_Character.charHp.hp_cur > 0)
        {
            yield return Server_AddExp(a_Character.hex, 1);

            if (a_Character.charExp.exp_cur >= a_Character.charExp.exp_max)
                yield return Server_LevelUp(a_Character);
        }
        if (t_Character.charHp.hp_cur > 0)
        {
            yield return Server_AddExp(t_Character.hex, 1);

            if (t_Character.charExp.exp_cur >= t_Character.charExp.exp_max)
                yield return Server_LevelUp(t_Character);
        }
        yield return null;
    }
    #endregion

    #region Attack anim
    private IEnumerator Server_AttackAnim(Hex attackerHex, Hex targetHex, int attackId)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        AttackAnimation attackAnimation = new AttackAnimation();
        Utility.GridCoord aCoords = gridManager.Get_GridCoord_ByHex(attackerHex);
        Utility.GridCoord tCoords = gridManager.Get_GridCoord_ByHex(targetHex);
        attackAnimation.a_coord_x = aCoords.coord_x;
        attackAnimation.a_coord_y = aCoords.coord_y;
        attackAnimation.t_coord_x = tCoords.coord_x;
        attackAnimation.t_coord_y = tCoords.coord_y;
        attackAnimation.attackId = attackId;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, attackAnimation, DeliveryMethod.ReliableOrdered);
        }

        yield return attackerHex.character.AttackAnimation(targetHex, attackId);
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_AttackAnim(AttackAnimation attackAnimation)
    {
        Character a_Character = gridManager.Get_GridItem_ByCoords(attackAnimation.a_coord_x, attackAnimation.a_coord_y).hex.character;
        Hex tHex = gridManager.Get_GridItem_ByCoords(attackAnimation.t_coord_x, attackAnimation.t_coord_y).hex;

        yield return a_Character.AttackAnimation(tHex, attackAnimation.attackId);

        yield return Reply_TaskDone("Attack animation");
    }
    #endregion

    #region Level up
    private IEnumerator Server_LevelUp(Character character)
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
                yield return Server_UpgradeCharacter(character, character.charId);
            }
        }
        else if (character.owner.name == server.player.name)
        {
            uiIngame.Show_LevelupPanel(character);
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
        int itemId = 0;
        if (character.charItem != null)
        {
            itemId = character.charItem.itemId;
        }
        if (character.charId != upgradeId)
        {
            yield return Server_Die(someHex);
            yield return Server_CreateCharacter(someHex, upgradeId, ownerName, character.heroCharacter);
            if (itemId != 0)
            {
                yield return Server_CreateItem(someHex, itemId);
                yield return Server_PickupItem(someHex.character);
            }
        }
        else
        {
            yield return Server_StatsUp(someHex); // Server is blocked
        }
        yield return null;
    }

    public IEnumerator Client_OpenUpgradeMenu(OpenUpgradeMenu upgradeMenu)
    {
        Character character = gridManager.Get_GridItem_ByCoords(upgradeMenu.coord_x, upgradeMenu.coord_y).hex.character;

        uiIngame.Show_LevelupPanel(character);

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

    #region Stats up
    private IEnumerator Server_StatsUp(Hex hex)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        hex.character.StatsUp();

        StatsUp statsUp = new StatsUp();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(hex);
        statsUp.coord_x = gridCoord.coord_x;
        statsUp.coord_y = gridCoord.coord_y;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, statsUp, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_StatsUp(StatsUp statsUp)
    {
        Character character = gridManager.Get_GridItem_ByCoords(statsUp.coord_x, statsUp.coord_y).hex.character;

        character.StatsUp();

        yield return Reply_TaskDone("Character stats are up");
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

    #region Spell resulting effect
    public IEnumerator Server_SpellDamage(Hex casterHex, Hex targetHex, int amount)
    {
        if (targetHex.character == null) yield break;

        if (server.players.Count > 2) server.player.isAvailable = false;

        Character c = targetHex.character;
        int resultDmg = Convert.ToInt32((float)amount - (float)amount * c.charDef.magic_resistance);
        c.RecieveDmg(resultDmg);

        AttackResult attackResult = new AttackResult();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(targetHex);
        attackResult.coord_x = gridCoord.coord_x;
        attackResult.coord_y = gridCoord.coord_y;
        attackResult.amount = resultDmg;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, attackResult, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);

        if (c.charHp.hp_cur <= 0)
        {
            yield return Server_Die(targetHex); // Server is blocked
            yield return Server_AddExp(casterHex, 3); // Server is blocked
        }
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

        yield return Reply_TaskDone("Healing done");
    }

    public IEnumerator Server_Blink(Hex casterHex, Hex targetHex)
    {
        if (casterHex.character == null) yield break;
        if (targetHex.character != null) yield break;

        if (server.players.Count > 2) server.player.isAvailable = false;
        
        Character c = casterHex.character;
        effectsData.Effect_DarkPortal(c.tr.position, c.tr);
        yield return new WaitForSeconds(0.25f);
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
        yield return new WaitForSeconds(0.25f);
        c.Replace(targetHex);

        yield return Reply_TaskDone("Blink done");
    }
    #endregion

    #region Add exp
    private IEnumerator Server_AddExp(Hex charactersHex, int expAmount)
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

    #region Die
    private IEnumerator Server_Die(Hex dieAtHex)
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

    #region Attack result
    private IEnumerator Server_AttackResult(Character a_Character, Character t_Character, int attackId)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        int attackDmg_cur = a_Character.charAttacks[attackId].attackDmg_cur;
        int resultDmg = 0;
        int dodge = t_Character.charDef.dodgeChance + t_Character.hex.dodge;
        if (UnityEngine.Random.Range(0, 101) > dodge)
        {
            resultDmg = attackDmg_cur;
            switch (a_Character.charAttacks[attackId].attackDmgType)
            {
                case Utility.char_attackDmgType.slash:
                    resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * t_Character.charDef.slash_resistance);
                    break;
                case Utility.char_attackDmgType.pierce:
                    resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * t_Character.charDef.pierce_resistance);
                    break;
                case Utility.char_attackDmgType.blunt:
                    resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * t_Character.charDef.blunt_resistance);
                    break;
                case Utility.char_attackDmgType.magic:
                    resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * t_Character.charDef.magic_resistance);
                    break;
            }
        }
        t_Character.RecieveDmg(resultDmg);

        AttackResult attackResult = new AttackResult();
        Utility.GridCoord gridCoord = gridManager.Get_GridCoord_ByHex(t_Character.hex);
        attackResult.coord_x = gridCoord.coord_x;
        attackResult.coord_y = gridCoord.coord_y;
        attackResult.amount = resultDmg;
        for (int x = 0; x < server.players.Count; x++)
        {
            Player somePlayer = server.players[x];
            if (somePlayer.isServer || somePlayer.isNeutral) continue;

            somePlayer.isAvailable = false;
            yield return server.netProcessor.Send(server.players[x].address, attackResult, DeliveryMethod.ReliableOrdered);
        }
        yield return new WaitUntil(() => server.player.isAvailable);
    }

    public IEnumerator Client_AttackResult(AttackResult attackResult)
    {
        Character t_Character = gridManager.Get_GridItem_ByCoords(attackResult.coord_x, attackResult.coord_y).hex.character;

        t_Character.RecieveDmg(attackResult.amount);

        yield return Reply_TaskDone("Attack result");
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
        Utility.Get_Client_byString(clientName, server.players).gold -= amount;
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

        yield return Server_AfterMoveCheck(someCharacter);
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

    private IEnumerator On_Move(Character someCharacter, List<Hex> somePath)
    {
        if (server.players.Count > 2) server.player.isAvailable = false;

        List<Hex> realPath = pathfinding.Get_RealPath(somePath);
        someCharacter.charMovement.movePoints_cur -= pathfinding.Get_PathCost_FromNext(realPath);

        Move move = new Move();
        move.mpLeft = someCharacter.charMovement.movePoints_cur;
        List<Utility.GridCoord> list = gridManager.Get_CoordPath(realPath);
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
        currentTurn = Utility.Get_Client_byString(setTurn.name, client.players);

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

        character.Item_Use();

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
        character.Item_Use();

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
            fog.Show_MoveHexes(someCharacter);

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
            fog.Show_MoveHexes(someCharacter);

        yield return Reply_TaskDone("Character actions are blocked");
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
