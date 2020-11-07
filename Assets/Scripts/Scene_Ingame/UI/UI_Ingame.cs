using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ingame : MonoBehaviour
{
	private Ingame_Input ingameInput;
	public CharactersData charData;

	private Client client;
	private Server server;

	public bool somePanelIsOn = false;

	public GameObject helpMenu;
	public Button save_Button;
	public GameObject turnMessage;

	#region Hex info panel
	[Header("Hex info")]
	public GameObject hexInfo_Canvas;
	public Image hexInfo_Image;
	public Sprite grassHex_Sprite;
	public Sprite villageHex_Sprite;
	public Sprite castleHex_Sprite;
	public Sprite mountainHex_Sprite;
	public Sprite waterHex_Sprite;
	public Text hexInfo_Title_Text;
	public Text hexInfo_VillageOwner_Text;
	public Text hexInfo_MoveCost_Text;
	public Text hexInfo_HexDodge_Text;
	#endregion

	#region Hex item panel
	[Header("Hex item info")]
	public GameObject hexItem_Canvas;
	public Image hItem_Image;
	public Text hItem_Name_Text;
	public Text hItem_Buffs_Text;
	public GameObject hItem_Pickup_Obj;
	#endregion

	#region Character info panel
	[Header("Character info")]
	public GameObject cInfo_Canvas;
	public Image cInfo_Image;
	public Text cInfo_Name_Text;
	public Text cInfo_CanAct_Text;
	public Text cInfo_Hp_Text;
	public Text cInfo_Exp_Text;
	public Text cInfo_MP_Text;
	public Text cInfo_Dodge_Text;
	public Button cInfo_Spell1_Obj;
	public Text cInfo_Spell1_CD_Text;
	public Button cInfo_Spell2_Obj;
	public Text cInfo_Spell2_CD_Text;
	public GameObject cInfo_Spell_Cancel;
	public Text cInfo_Attacks_Text;
	public Text cInfo_Buffs_Text;
	#endregion

	#region Character item panel
	[Header("Character item info")]
	public GameObject cItem_Canvas;
	public Image cItem_Image;
	public Text cItem_Name_Text;
	public Text cItem_Buffs_Text;
	public GameObject cItem_ItemUse_Obj;
	public GameObject cItem_ItemDrop_Obj;
    #endregion

    #region Recruit panel
	[Header("Recruit panel")]
    public GameObject recruit_Canvas;
	public Button closeRecruitMenu_button;
	private Dictionary<Button, Character> recrutable_dict = new Dictionary<Button, Character>();
	public Button recruit_1_button;
	public Button recruit_2_button;
	public Button recruit_3_button;
	public Button recruit_4_button;
	public Button recruit_5_button;
	public Button recruit_6_button;
	public Button recruit_7_button;

	public Image rec_Char_Image;
	public Text rec_CharName_Text;
	public Text rec_CharCost_Text;
	public Text rec_CharHp_Text;
	public Text rec_CharExp_Text;
	public Text rec_CharMP_Text;
	public Text rec_CharDodge_Text;
	public Text rec_CharAttack_Text;

	public Character charToRecruit;
	private Hex recruitHex;
	public Button recruitButton;
	#endregion

	#region Attack panel
	[Header("Attack panel")]
	public GameObject attack_Canvas;
	public Image attackerImage;
	public Image targetImage;
	
	public Button attack1;
	public Image attack1_Image_a;
	public Text attack1_Text_a;
	public Image attack1_Image_t;
	public Text attack1_Text_t;

	public Button attack2;
	public Image attack2_Image_a;
	public Text attack2_Text_a;
	public Image attack2_Image_t;
	public Text attack2_Text_t;

	public Button attack3;
	public Image attack3_Image_a;
	public Text attack3_Text_a;
	public Image attack3_Image_t;
	public Text attack3_Text_t;

	private List<Hex> attackPath;
	private int selectedAttackId;
	public Button attack_button;
	public Button closeAttackPanel;
	#endregion

	#region Upgrade panel	
	private Dictionary<Button, Character> levelup_dict = new Dictionary<Button, Character>();
	[Header("Upgrade panel")]
	public GameObject upgrade_Canvas;
	public Button levelupOption_1;
	public Button levelupOption_2;
	public Button levelupOption_3;
	private Character levelupCharacter;
	private int selectedUpgradeId;

	public Image upg_curChar_Image;
	public Text upg_curName_Text;
	public Text upg_curHp_Text;
	public Text upg_curExp_Text;
	public Text upg_curMP_Text;
	public Text upg_curDodge_Text;

	public Image upg_Char_Image;
	public Text upg_CharName_Text;
	public Text upg_Hp_Text;
	public Text upg_Exp_Text;
	public Text upg_MP_Text;
	public Text upg_Dodge_Text;

	public Button levelupButton;
	#endregion

	#region Player info panel
	[Header("Player info")]
	public GameObject playerInfoPanel;
	public Text pInfo_Gold_Text;
	public Text pInfo_Vilage_Text;
	public Text pInfo_Income_Text;
	public Text pInfo_Daytime_Text;
	#endregion
	
	public Button endTurn;

	private void Awake()
	{
		ingameInput = GetComponent<Ingame_Input>();

		if (Utility.IsServer())
			server = GameMain.inst.server;
		else
			client = GameMain.inst.client;

		hexInfo_Canvas.SetActive(false);
		hexItem_Canvas.SetActive(false);
		cInfo_Canvas.SetActive(false);
		cInfo_Spell_Cancel.SetActive(false);
		cItem_Canvas.SetActive(false);

		recruit_Canvas.SetActive(false);
		attack_Canvas.SetActive(false);
		upgrade_Canvas.SetActive(false);
	}

	private void Update()
	{
		if(UnityEngine.Input.GetKeyDown(KeyCode.F1)) Button_Menu();
		if (UnityEngine.Input.GetKeyDown(KeyCode.F2)) Button_HideFog();
		Update_HexInfo();
	}

	public void Button_EndTurn()
	{
		if (Utility.IsServer())
			StartCoroutine(GameMain.inst.Server_ChangeTurn());
		else
			GameMain.inst.Request_EndTurn(new EndTurn());

		Recruit_CloseMenu();
		Hide_AttackPanel();
		Hide_HexInfo();

		ingameInput.Reset_All();
		ingameInput.mouseOverUI = false;
	}

	public void Update_PlayerInfoPanel()
	{
		Player player;
        if (Utility.IsServer())
            player = Utility.Get_Client_byString(server.player.name, server.players);
        else
            player = Utility.Get_Client_byString(client.player.name, client.players);

        pInfo_Gold_Text.text = "" + player.gold;
        pInfo_Vilage_Text.text = "" + player.villages + " / " + GameMain.inst.gridManager.villages.Count;
        pInfo_Income_Text.text = "+" + player.villages * Utility.villageIncome;
        pInfo_Daytime_Text.text = "" + GameMain.inst.dayTime_cur;
	}

	public void Update_OnTurnUI()
	{
		if (Utility.IsMyTurn())
		{
			endTurn.interactable = true;
			ShowTurnMessage();
		}
		else
			endTurn.interactable = false;
	}

	#region Hex info panel
	private void Update_HexInfo()
	{
		if (ingameInput.selectedHex != null && hexInfo_Canvas.activeInHierarchy)
		{
			Show_HexInfo(ingameInput.selectedHex);
		}
	}

	public void Show_HexInfo(Hex hex)
	{
		hexInfo_Canvas.SetActive(true);
		hexInfo_Image.sprite = grassHex_Sprite;
		if(hex.isStartPoint) hexInfo_Image.sprite = castleHex_Sprite;
		else if(hex.isVillage) hexInfo_Image.sprite = villageHex_Sprite;
		else if(hex.isMountain) hexInfo_Image.sprite = mountainHex_Sprite;
		else if(hex.isWater) hexInfo_Image.sprite = waterHex_Sprite;

		hexInfo_Title_Text.text = "Hex title : " + hex.gameObject.name;
		if (hex.isVillage && hex.villageOwner.name != "") hexInfo_VillageOwner_Text.text = "Village owner : " + hex.villageOwner.name;
		else hexInfo_VillageOwner_Text.text = "";
		hexInfo_MoveCost_Text.text = "Hex move cost : " + hex.moveCost;
		hexInfo_HexDodge_Text.text = "Hex dodge : " + hex.dodge;

		if (hex.item != null && Utility.HexIsVisible(hex))
			Show_HexItemInfo(hex);
		else
			hexItem_Canvas.SetActive(false);

		if (hex.character != null && Utility.HexIsVisible(hex))
		{
			Show_HexCharacterInfo(hex);
		}
		else
		{
			cInfo_Canvas.SetActive(false);
			cItem_Canvas.SetActive(false);
		}
	}

	private void Show_HexItemInfo(Hex hex)
	{
		hexItem_Canvas.SetActive(true);

		Item i = hex.item;
		hItem_Image.sprite = i.itemImage;
		hItem_Name_Text.text = i.itemName;
		hItem_Buffs_Text.text = "Item buffs :";
		if(i.itemActive != null)
			hItem_Buffs_Text.text += "\n - " + i.itemActive.buffName;
		for (int x = 0; x < i.itemBuffs.Count; x++)
		{
			Text text2 = hItem_Buffs_Text;
			text2.text = text2.text + "\n - " + i.itemBuffs[x].buffName;
		}

		if (hex.character != null)
		{
			if (hex.character.charItem != null || !Utility.IsMyCharacter(hex.character))
				hItem_Pickup_Obj.SetActive(false);
			else
				hItem_Pickup_Obj.SetActive(true);
		}
		else
			hItem_Pickup_Obj.SetActive(false);
	}

	private void Show_HexCharacterInfo(Hex hex)
	{
		cInfo_Canvas.SetActive(true);

		Character c = hex.character;
		cInfo_Image.sprite = c.charImage;
		cInfo_Name_Text.text = "" + c.charName;
		cInfo_CanAct_Text.text = "CanAct :" + c.canAct.ToString();
		cInfo_Hp_Text.text = "Health : " + c.charHp.hp_cur + " / " + c.charHp.hp_max;
		cInfo_Exp_Text.text = "Exp : " + c.charExp.exp_cur + " / " + c.charExp.exp_max;
		cInfo_MP_Text.text = "MovePoints : " + c.charMovement.movePoints_cur + " / " + c.charMovement.movePoints_max;
		cInfo_Dodge_Text.text = "Dodge : " + c.charDef.dodgeChance + " + " + hex.dodge;
		cInfo_Attacks_Text.text = "";
		for (int y = 0; y < c.charAttacks.Count; y++)
			cInfo_Attacks_Text.text = cInfo_Attacks_Text.text + Get_AttackTooltip(c.charAttacks[y]) + "\n";
		cInfo_Buffs_Text.text = "";
		for (int y = 0; y < c.charBuffs.Count; y++)
			cInfo_Buffs_Text.text = cInfo_Buffs_Text.text + c.charBuffs[y].buffName + "\n";

		// Spells
		Show_CharacterSpellInfo(c);

		if (c.charItem != null)
		{
			cItem_Canvas.SetActive(true);

			Item i = c.charItem;
			cItem_Image.sprite = i.itemImage;
			cItem_Name_Text.text = i.itemName;
			cItem_Buffs_Text.text = "Item buffs :";
			if(i.itemActive != null)
				cItem_Buffs_Text.text += "\n - " + i.itemActive.buffName;
			for (int x = 0; x < i.itemBuffs.Count; x++)
			{
				Text text = cItem_Buffs_Text;
				text.text = text.text + "\n - " + i.itemBuffs[x].buffName;
			}

			if (Utility.IsMyCharacter(c))
			{
				if(i.itemActive != null)
				{
					cItem_ItemUse_Obj.SetActive(true);
					if(c.canAct)
						cItem_ItemUse_Obj.GetComponent<Button>().interactable = true;
					else
						cItem_ItemUse_Obj.GetComponent<Button>().interactable = false;
				}
				else
					cItem_ItemUse_Obj.SetActive(false);

				if(hex.item == null)
					cItem_ItemDrop_Obj.SetActive(true);
				else
					cItem_ItemDrop_Obj.SetActive(false);
			}
			else
			{
				cItem_ItemUse_Obj.SetActive(false);
				cItem_ItemDrop_Obj.SetActive(false);
			}
		}
		else
		{
			cItem_Canvas.SetActive(false);
		}
	}

	private void Show_CharacterSpellInfo(Character c)
	{
		if(Utility.IsMyCharacter(c) && c.canAct && c.charSpell_1 != null)
		{
			if(!ingameInput.castingSpell && c.charSpell_1.cooldown_cur == 0)
			{
				cInfo_Spell1_Obj.interactable = true;
				cInfo_Spell1_CD_Text.enabled = false;
			}
			else
			{
				cInfo_Spell1_Obj.interactable = false;
				if(c.charSpell_1.cooldown_cur != 0)
				{
					cInfo_Spell1_CD_Text.enabled = true;
					cInfo_Spell1_CD_Text.text = "" + c.charSpell_1.cooldown_cur;
				}
			}
		}
		else
		{
			cInfo_Spell1_Obj.interactable = false;
			cInfo_Spell1_CD_Text.enabled = false;
		}
			
		if(Utility.IsMyCharacter(c) && c.canAct && c.charSpell_2 != null)
		{
			if(!ingameInput.castingSpell && c.charSpell_2.cooldown_cur == 0)
			{
				cInfo_Spell2_Obj.interactable = true;
				cInfo_Spell2_CD_Text.enabled = false;
			}
			else
			{
				cInfo_Spell2_Obj.interactable = false;
				if(c.charSpell_2.cooldown_cur != 0)
				{
					cInfo_Spell2_CD_Text.enabled = true;
					cInfo_Spell2_CD_Text.text = "" + c.charSpell_2.cooldown_cur;
				}
			}
		}
		else
		{
			cInfo_Spell2_Obj.interactable = false;
			cInfo_Spell2_CD_Text.enabled = false;
		}
	}

	public void Button_Item_Pickup()
	{
		if(!Utility.IsMyTurn()) return;

		if (Utility.IsServer())
			StartCoroutine(GameMain.inst.Server_PickupItem(ingameInput.selectedHex.character));
		else
			GameMain.inst.Request_PickupItem(ingameInput.selectedHex.character);

		ingameInput.mouseOverUI = false;
	}

	public void Button_Item_Use()
	{
		if(!Utility.IsMyTurn()) return;
		if(!ingameInput.selectedHex.character.canAct) return;

		if (Utility.IsServer())
			StartCoroutine(GameMain.inst.Server_UseItem_Logic(ingameInput.selectedHex.character));
		else
			GameMain.inst.Request_UseItem(ingameInput.selectedHex.character);

		ingameInput.mouseOverUI = false;
	}

	public void Button_Item_Remove()
	{
		if(!Utility.IsMyTurn()) return;

		if (Utility.IsServer())
			StartCoroutine(GameMain.inst.Server_DropItem(ingameInput.selectedHex.character));
		else
			GameMain.inst.Request_DropItem(ingameInput.selectedHex.character);

		ingameInput.mouseOverUI = false;
	}

	public void Hide_HexInfo()
	{
		hexInfo_Canvas.SetActive(false);
		hexItem_Canvas.SetActive(false);
		cInfo_Canvas.SetActive(false);
		cItem_Canvas.SetActive(false);
	}
	#endregion

	#region Spells
	public void Button_Spell_1()
	{
		cInfo_Spell1_Obj.interactable = false;
		cInfo_Spell2_Obj.interactable = false;

		cInfo_Spell_Cancel.SetActive(true);

		ingameInput.castingSpell = true;
		ingameInput.spell_Active = ingameInput.selectedHex.character.charSpell_1;

		ingameInput.mouseOverUI = false;
	}

	public void Button_Spell_2()
	{
		cInfo_Spell1_Obj.interactable = false;
		cInfo_Spell2_Obj.interactable = false;

		cInfo_Spell_Cancel.SetActive(true);

		ingameInput.castingSpell = true;
		ingameInput.spell_Active = ingameInput.selectedHex.character.charSpell_2;

		ingameInput.mouseOverUI = false;
	}

	public void Button_Cancel_Spell()
	{
		ingameInput.Spellcasting_Cancel();

		ingameInput.mouseOverUI = false;
	}
	#endregion

	#region Recruit panel
	public void Recruit_OpenMenu(Hex recruitHex)
	{
		somePanelIsOn = true;

		recruit_1_button.gameObject.SetActive(false);
		recruit_2_button.gameObject.SetActive(false);
		recruit_3_button.gameObject.SetActive(false);
		recruit_4_button.gameObject.SetActive(false);
		recruit_5_button.gameObject.SetActive(false);
		recruit_6_button.gameObject.SetActive(false);
		recruit_7_button.gameObject.SetActive(false);

		recruit_Canvas.SetActive(true);

		recruitButton.interactable = false;
		this.recruitHex = recruitHex;

		rec_Char_Image.enabled = false;
		rec_CharName_Text.text = "";
		rec_CharCost_Text.text = "";
		rec_CharHp_Text.text = "";
		rec_CharExp_Text.text = "";
		rec_CharMP_Text.text = "";
		rec_CharDodge_Text.text = "";
		rec_CharAttack_Text.text = "";

		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		switch (gameClient.race)
		{
			case 0: // Humans
				recruit_1_button.gameObject.SetActive(true);
				recruit_2_button.gameObject.SetActive(true);
				recruit_3_button.gameObject.SetActive(true);
				recruit_4_button.gameObject.SetActive(true);
				recruit_5_button.gameObject.SetActive(true);
				recruit_6_button.gameObject.SetActive(true);
				recruit_7_button.gameObject.SetActive(true);

				if (!recrutable_dict.ContainsKey(recruit_1_button))
				{
					Character spearman = new Spearman(null, gameClient, false);
					recrutable_dict[recruit_1_button] = spearman;

					GameObject.Find("recruit_1_Char_Image").GetComponent<Image>().sprite = spearman.charImage;
					GameObject.Find("recruit_1_CharName_Text").GetComponent<Text>().text = (spearman.charName ?? "");
					GameObject.Find("recruit_1_CharPrice_Text").GetComponent<Text>().text = string.Concat(spearman.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_2_button))
				{
					Character humArcher = new Bowman(null, gameClient, false);
					recrutable_dict[recruit_2_button] = humArcher;

					GameObject.Find("recruit_2_Char_Image").GetComponent<Image>().sprite = humArcher.charImage;
					GameObject.Find("recruit_2_CharName_Text").GetComponent<Text>().text = (humArcher.charName ?? "");
					GameObject.Find("recruit_2_CharPrice_Text").GetComponent<Text>().text = string.Concat(humArcher.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_3_button))
				{
					Character humMage = new Mage(null, gameClient, false);
					recrutable_dict[recruit_3_button] = humMage;

					GameObject.Find("recruit_3_Char_Image").GetComponent<Image>().sprite = humMage.charImage;
					GameObject.Find("recruit_3_CharName_Text").GetComponent<Text>().text = (humMage.charName ?? "");
					GameObject.Find("recruit_3_CharPrice_Text").GetComponent<Text>().text = string.Concat(humMage.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_4_button))
				{
					Character heavy = new HeavyInfantryman(null, gameClient, false);
					recrutable_dict[recruit_4_button] = heavy;

					GameObject.Find("recruit_4_Char_Image").GetComponent<Image>().sprite = heavy.charImage;
					GameObject.Find("recruit_4_CharName_Text").GetComponent<Text>().text = (heavy.charName ?? "");
					GameObject.Find("recruit_4_CharPrice_Text").GetComponent<Text>().text = string.Concat(heavy.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_5_button))
				{
					Character cavalryman = new Cavalryman(null, gameClient, false);
					recrutable_dict[recruit_5_button] = cavalryman;

					GameObject.Find("recruit_5_Char_Image").GetComponent<Image>().sprite = cavalryman.charImage;
					GameObject.Find("recruit_5_CharName_Text").GetComponent<Text>().text = (cavalryman.charName ?? "");
					GameObject.Find("recruit_5_CharPrice_Text").GetComponent<Text>().text = string.Concat(cavalryman.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_6_button))
				{
					Character horseman = new Horseman(null, gameClient, false);
					recrutable_dict[recruit_6_button] = horseman;

					GameObject.Find("recruit_6_Char_Image").GetComponent<Image>().sprite = horseman.charImage;
					GameObject.Find("recruit_6_CharName_Text").GetComponent<Text>().text = (horseman.charName ?? "");
					GameObject.Find("recruit_6_CharPrice_Text").GetComponent<Text>().text = string.Concat(horseman.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_7_button))
				{
					Character rogue = new Rogue(null, gameClient, false);
					recrutable_dict[recruit_7_button] = rogue;

					GameObject.Find("recruit_7_Char_Image").GetComponent<Image>().sprite = rogue.charImage;
					GameObject.Find("recruit_7_CharName_Text").GetComponent<Text>().text = (rogue.charName ?? "");
					GameObject.Find("recruit_7_CharPrice_Text").GetComponent<Text>().text = string.Concat(rogue.charCost);
				}
				break;

			case 1: // Orcs
				recruit_1_button.gameObject.SetActive(false);
				recruit_2_button.gameObject.SetActive(false);
				recruit_3_button.gameObject.SetActive(false);
				recruit_4_button.gameObject.SetActive(false);
				recruit_5_button.gameObject.SetActive(false);
				recruit_6_button.gameObject.SetActive(false);
				recruit_7_button.gameObject.SetActive(false);
				break;

			case 2: // Undeads
				recruit_1_button.gameObject.SetActive(true);
				recruit_2_button.gameObject.SetActive(true);
				recruit_3_button.gameObject.SetActive(true);
				recruit_4_button.gameObject.SetActive(true);
				recruit_5_button.gameObject.SetActive(true);
				recruit_6_button.gameObject.SetActive(true);
				recruit_7_button.gameObject.SetActive(true);

				if (!recrutable_dict.ContainsKey(recruit_1_button))
				{
					Character zombie = new Ghoul(null, gameClient, false);
					recrutable_dict[recruit_1_button] = zombie;
					GameObject.Find("recruit_1_Char_Image").GetComponent<Image>().sprite = zombie.charImage;
					GameObject.Find("recruit_1_CharName_Text").GetComponent<Text>().text = (zombie.charName ?? "");
					GameObject.Find("recruit_1_CharPrice_Text").GetComponent<Text>().text = string.Concat(zombie.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_2_button))
				{
					Character character = new Skeleton(null, gameClient, isHero: false);
					recrutable_dict[recruit_2_button] = character;
					GameObject.Find("recruit_2_Char_Image").GetComponent<Image>().sprite = character.charImage;
					GameObject.Find("recruit_2_CharName_Text").GetComponent<Text>().text = (character.charName ?? "");
					GameObject.Find("recruit_2_CharPrice_Text").GetComponent<Text>().text = string.Concat(character.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_3_button))
				{
					Character character3 = new SkelArcher(null, gameClient, isHero: false);
					recrutable_dict[recruit_3_button] = character3;
					GameObject.Find("recruit_3_Char_Image").GetComponent<Image>().sprite = character3.charImage;
					GameObject.Find("recruit_3_CharName_Text").GetComponent<Text>().text = (character3.charName ?? "");
					GameObject.Find("recruit_3_CharPrice_Text").GetComponent<Text>().text = string.Concat(character3.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_4_button))
				{
					Character character2 = new DarkAdept(null, gameClient, isHero: false);
					recrutable_dict[recruit_4_button] = character2;
					GameObject.Find("recruit_4_Char_Image").GetComponent<Image>().sprite = character2.charImage;
					GameObject.Find("recruit_4_CharName_Text").GetComponent<Text>().text = (character2.charName ?? "");
					GameObject.Find("recruit_4_CharPrice_Text").GetComponent<Text>().text = string.Concat(character2.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_5_button))
				{
					Character ghost = new Ghost(null, gameClient, isHero: false);
					recrutable_dict[recruit_5_button] = ghost;
					GameObject.Find("recruit_5_Char_Image").GetComponent<Image>().sprite = ghost.charImage;
					GameObject.Find("recruit_5_CharName_Text").GetComponent<Text>().text = (ghost.charName ?? "");
					GameObject.Find("recruit_5_CharPrice_Text").GetComponent<Text>().text = string.Concat(ghost.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_6_button))
				{
					Character bat = new VampireBat(null, gameClient, isHero: false);
					recrutable_dict[recruit_6_button] = bat;
					GameObject.Find("recruit_6_Char_Image").GetComponent<Image>().sprite = bat.charImage;
					GameObject.Find("recruit_6_CharName_Text").GetComponent<Text>().text = (bat.charName ?? "");
					GameObject.Find("recruit_6_CharPrice_Text").GetComponent<Text>().text = string.Concat(bat.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_7_button))
				{
					Character walkingCorpse = new WalkingCorpse(null, gameClient, isHero: false);
					recrutable_dict[recruit_7_button] = walkingCorpse;
					GameObject.Find("recruit_7_Char_Image").GetComponent<Image>().sprite = walkingCorpse.charImage;
					GameObject.Find("recruit_7_CharName_Text").GetComponent<Text>().text = (walkingCorpse.charName ?? "");
					GameObject.Find("recruit_7_CharPrice_Text").GetComponent<Text>().text = string.Concat(walkingCorpse.charCost);
				}
				break;
		}
	}

	public void Recruit_CloseMenu()
	{
		recruit_Canvas.SetActive(false);

		somePanelIsOn = false;
		ingameInput.mouseOverUI = false;
	}

	public void Recruit_Character_1()
	{
		rec_Char_Image.enabled = true;
		Set_RecruitTooltip(recrutable_dict[recruit_1_button]);

		charToRecruit = recrutable_dict[recruit_1_button];

		Player gameClient = (!Utility.IsServer() ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_1_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_2()
	{
		rec_Char_Image.enabled = true;
		Set_RecruitTooltip(recrutable_dict[recruit_2_button]);

		charToRecruit = recrutable_dict[recruit_2_button];

		Player gameClient = (!Utility.IsServer() ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_2_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_3()
	{
		rec_Char_Image.enabled = true;
		Set_RecruitTooltip(recrutable_dict[recruit_3_button]);

		charToRecruit = recrutable_dict[recruit_3_button];

		Player gameClient = (!Utility.IsServer() ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_3_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_4()
	{
		rec_Char_Image.enabled = true;
		Set_RecruitTooltip(recrutable_dict[recruit_4_button]);

		charToRecruit = recrutable_dict[recruit_4_button];

		Player gameClient = (!Utility.IsServer() ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_4_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_5()
	{
		rec_Char_Image.enabled = true;
		Set_RecruitTooltip(recrutable_dict[recruit_5_button]);

		charToRecruit = recrutable_dict[recruit_5_button];

		Player gameClient = (!Utility.IsServer() ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_5_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_6()
	{
		rec_Char_Image.enabled = true;
		Set_RecruitTooltip(recrutable_dict[recruit_6_button]);

		charToRecruit = recrutable_dict[recruit_6_button];

		Player gameClient = (!Utility.IsServer() ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_6_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_7()
	{
		rec_Char_Image.enabled = true;
		Set_RecruitTooltip(recrutable_dict[recruit_7_button]);

		charToRecruit = recrutable_dict[recruit_7_button];

		Player gameClient = (!Utility.IsServer() ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_7_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	private void Set_RecruitTooltip(Character character)
	{
		rec_Char_Image.sprite = character.charImage;
		rec_CharName_Text.text = character.charName;
		rec_CharCost_Text.text = "Cost : " + character.charCost;
		rec_CharHp_Text.text = "Health : " + character.charHp.hp_max;
		rec_CharExp_Text.text = "Exp : " + character.charExp.exp_max;
		rec_CharMP_Text.text = "MovePoints : " + character.charMovement.movePoints_max;
		rec_CharDodge_Text.text = "Dodge : " + character.charDef.dodgeChance;

		rec_CharAttack_Text.text = "";
		for (int y = 0; y < character.charAttacks.Count; y++)
		{
			rec_CharAttack_Text.text = rec_CharAttack_Text.text + Get_AttackTooltip(character.charAttacks[y]) + "\n";
		}
	}

	public void Recruit()
	{
		Recruit_CloseMenu();
		if (Utility.IsServer())
		{
			StartCoroutine(GameMain.inst.Server_Recruit(recruitHex, charToRecruit.charId, server.player.name, charToRecruit.charCost));
		}
		else
		{
			GameMain.inst.Request_Recruit(recruitHex, charToRecruit.charId, client.player.name, charToRecruit.charCost);
		}
	}
	#endregion

	#region Attack panel
	public void Show_AttackPanel(List<Hex> attackPath)
	{
		somePanelIsOn = true;

		attack1.gameObject.SetActive(false);
		attack2.gameObject.SetActive(false);
		attack3.gameObject.SetActive(false);

		attack1.interactable = true;
		attack2.interactable = true;
		attack3.interactable = true;

		attack_button.interactable = false;

		attack_Canvas.SetActive(true);

		this.attackPath = new List<Hex>(attackPath);

		Character attacker = attackPath[0].character;
		Character target = attackPath[attackPath.Count - 1].character;
		List<Utility.char_Attack> a_Attacks = attacker.charAttacks;
		List<Utility.char_Attack> t_Attacks = target.charAttacks;

		attackerImage.sprite = attacker.charImage;
		targetImage.sprite = target.charImage;

		if (a_Attacks.Count > 0 || t_Attacks.Count > 0)
		{
			attack1.gameObject.SetActive(true);
			if (a_Attacks.Count > 0)
			{
				attack1_Image_a.gameObject.SetActive(true);
				attack1_Image_a.sprite = Get_AttackTypeImage(a_Attacks[0].attackDmgType);
				attack1_Text_a.text = Calculate_AttackValue(a_Attacks[0], target);
			}
			else
			{
				attack1_Image_a.gameObject.SetActive(false);
				attack1_Text_a.text = "";
			}

			if (t_Attacks.Count > 0)
			{
				attack1_Image_t.gameObject.SetActive(true);
				attack1_Image_t.sprite = Get_AttackTypeImage(t_Attacks[0].attackDmgType);
				attack1_Text_t.text = Calculate_AttackValue(t_Attacks[0], attacker);
			}
			else
			{
				attack1_Image_t.gameObject.SetActive(false);
				attack1_Text_t.text = "";
			}
		}
		if (a_Attacks.Count > 1 || t_Attacks.Count > 1)
		{
			attack2.gameObject.SetActive(true);
			if (a_Attacks.Count > 1)
			{
				attack2_Image_a.gameObject.SetActive(true);
				attack2_Image_a.sprite = Get_AttackTypeImage(a_Attacks[1].attackDmgType);
				attack2_Text_a.text = Calculate_AttackValue(a_Attacks[1], target);
			}
			else
			{
				attack2_Image_a.gameObject.SetActive(false);
				attack2_Text_a.text = "";
			}

			if (t_Attacks.Count > 1)
			{
				attack2_Image_t.gameObject.SetActive(true);
				attack2_Image_t.sprite = Get_AttackTypeImage(t_Attacks[1].attackDmgType);
				attack2_Text_t.text = Calculate_AttackValue(t_Attacks[1], attacker);
			}
			else
			{
				attack2_Image_t.gameObject.SetActive(false);
				attack2_Text_t.text = "";
			}
		}
		if (a_Attacks.Count > 2 || t_Attacks.Count > 2)
		{
			attack3.gameObject.SetActive(true);
			if (a_Attacks.Count > 2)
			{
				attack3_Image_a.gameObject.SetActive(true);
				attack3_Image_a.sprite = Get_AttackTypeImage(a_Attacks[2].attackDmgType);
				attack3_Text_a.text = Calculate_AttackValue(a_Attacks[2], target);
			}
			else
			{
				attack3_Image_a.gameObject.SetActive(false);
				attack3_Text_a.text = "";
			}

			if (t_Attacks.Count > 2)
			{
				attack3_Image_t.gameObject.SetActive(true);
				attack3_Image_t.sprite = Get_AttackTypeImage(a_Attacks[2].attackDmgType);
				attack3_Text_t.text = Calculate_AttackValue(t_Attacks[2], attacker);
			}
			else
			{
				attack3_Image_t.gameObject.SetActive(false);
				attack3_Text_t.text = "";
			}
		}
	}

	private Sprite Get_AttackTypeImage(Utility.char_attackDmgType attackDmgType)
	{
		Sprite result = null;
		switch (attackDmgType)
		{
			case Utility.char_attackDmgType.Blade:
				result = Resources.Load<Sprite>("DamageType/IronSword");
				break;
			case Utility.char_attackDmgType.Pierce:
				result = Resources.Load<Sprite>("DamageType/Arrow");
				break;
			case Utility.char_attackDmgType.Impact:
				result = Resources.Load<Sprite>("DamageType/Hammer");
				break;
			case Utility.char_attackDmgType.Magic:
				result = Resources.Load<Sprite>("DamageType/RubyStaff");
				break;
		}
		return result;
	}

	private string Get_AttackTooltip(Utility.char_Attack attack)
	{
		if(attack.attackBuff != null)
			return attack.attackType + ", " + attack.attackDmgType + " " + attack.attackDmg_cur + " x " + attack.attackCount + "\n -"+ attack.attackBuff.buffName;
		else
			return attack.attackType + ", " + attack.attackDmgType + " " + attack.attackDmg_cur + " x " + attack.attackCount;
	}

	private string Calculate_AttackValue(Utility.char_Attack attack, Character target)
	{
		int attackDmg_cur = attack.attackDmg_cur;
		int resultDmg = 0;
		switch (attack.attackDmgType)
		{
			case Utility.char_attackDmgType.Blade:
				resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * target.charDef.blade_resistance);
				break;
			case Utility.char_attackDmgType.Pierce:
				resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * target.charDef.pierce_resistance);
				break;
			case Utility.char_attackDmgType.Impact:
				resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * target.charDef.impact_resistance);
				break;
			case Utility.char_attackDmgType.Magic:
				resultDmg = Convert.ToInt32((float)attackDmg_cur - (float)attackDmg_cur * target.charDef.magic_resistance);
				break;
		}

		if (attack.attackBuff != null)
			return attack.attackType + ", " + attack.attackDmgType + " " + resultDmg + " x " + attack.attackCount + "\n -" + attack.attackBuff.buffName;
		else
			return attack.attackType + ", " + attack.attackDmgType + " " + resultDmg + " x " + attack.attackCount;
	}

	public void SelectAttack1()
	{
		selectedAttackId = 0;
		attack_button.interactable = true;
		attack1.interactable = false;
		attack2.interactable = true;
		attack3.interactable = true;
	}

	public void SelectAttack2()
	{
		selectedAttackId = 1;
		attack_button.interactable = true;
		attack1.interactable = true;
		attack2.interactable = false;
		attack3.interactable = true;
	}

	public void SelectAttack3()
	{
		selectedAttackId = 2;
		attack_button.interactable = true;
		attack1.interactable = true;
		attack2.interactable = true;
		attack3.interactable = false;
	}

	public void Attack()
	{
		if (Utility.IsServer())
		{
			StartCoroutine(GameMain.inst.Server_Attack(attackPath, selectedAttackId));
		}
		else
		{
			GameMain.inst.Request_Attack(attackPath, selectedAttackId);
		}
		Hide_AttackPanel();
	}

	public void Hide_AttackPanel()
	{
		attack_Canvas.SetActive(false);

		somePanelIsOn = false;
		ingameInput.mouseOverUI = false;
	}
	#endregion

	#region Level up
	public void Show_LevelupPanel(Character character)
	{
		somePanelIsOn = true;

		endTurn.interactable = false;
		upgrade_Canvas.SetActive(true);
		levelupButton.interactable = false;

		levelupOption_1.gameObject.SetActive(false);
		levelupOption_2.gameObject.SetActive(false);
		levelupOption_3.gameObject.SetActive(false);

		levelupCharacter = character;
		if (levelupCharacter.upgradeList.Count > 0)
		{
			levelupOption_1.gameObject.SetActive(true);
			levelupOption_1.interactable = true;

			levelup_dict[levelupOption_1] = charData.Get_CharacterById(levelupCharacter.upgradeList[0]);

			GameObject.Find("upg1Image").GetComponent<Image>().sprite = levelup_dict[levelupOption_1].charImage;
			GameObject.Find("upg1Name").GetComponent<Text>().text = levelup_dict[levelupOption_1].charName;
		}
		if (levelupCharacter.upgradeList.Count > 1)
		{
			levelupOption_2.gameObject.SetActive(true);
			levelupOption_2.interactable = true;

			levelup_dict[levelupOption_2] = charData.Get_CharacterById(levelupCharacter.upgradeList[1]);

			GameObject.Find("upg2Image").GetComponent<Image>().sprite = levelup_dict[levelupOption_2].charImage;
			GameObject.Find("upg2Name").GetComponent<Text>().text = levelup_dict[levelupOption_2].charName;
		}
		if (levelupCharacter.upgradeList.Count > 2)
		{
			levelupOption_3.gameObject.SetActive(true);
			levelupOption_3.interactable = true;

			levelup_dict[levelupOption_3] = charData.Get_CharacterById(levelupCharacter.upgradeList[2]);

			GameObject.Find("upg3Image").GetComponent<Image>().sprite = levelup_dict[levelupOption_3].charImage;
			GameObject.Find("upg3Name").GetComponent<Text>().text = levelup_dict[levelupOption_3].charName;
		}

		Character someChar = charData.Get_CharacterById(levelupCharacter.charId);
		upg_curChar_Image.sprite = someChar.charImage;
		upg_curName_Text.text = someChar.charName;
		upg_curHp_Text.text = "Health : " + someChar.charHp.hp_max;
		upg_curExp_Text.text = "Exp : " + someChar.charExp.exp_max;
		upg_curMP_Text.text = "MovePoints : " + someChar.charMovement.movePoints_max;
		upg_curDodge_Text.text = "Dodge : " + someChar.charDef.dodgeChance;

		upg_Char_Image.gameObject.SetActive(false);
		upg_CharName_Text.text = "";
		upg_Hp_Text.text = "";
		upg_Exp_Text.text = "";
		upg_MP_Text.text = "";
		upg_Dodge_Text.text = "";
	}

	public void Select_Upgrade1()
	{
		selectedUpgradeId = levelup_dict[levelupOption_1].charId;

		upg_Char_Image.gameObject.SetActive(true);
		Set_UpgradeTooltip(levelup_dict[levelupOption_1]);

		levelupOption_1.interactable = false;
		levelupOption_2.interactable = true;
		levelupOption_3.interactable = true;
		levelupButton.interactable = true;
	}

	public void Select_Upgrade2()
	{
		selectedUpgradeId = levelup_dict[levelupOption_2].charId;

		upg_Char_Image.gameObject.SetActive(true);
		Set_UpgradeTooltip(levelup_dict[levelupOption_2]);

		levelupOption_1.interactable = true;
		levelupOption_2.interactable = false;
		levelupOption_3.interactable = true;
		levelupButton.interactable = true;
	}

	public void Select_Upgrade3()
	{
		selectedUpgradeId = levelup_dict[levelupOption_3].charId;

		upg_Char_Image.gameObject.SetActive(true);
		Set_UpgradeTooltip(levelup_dict[levelupOption_3]);

		levelupOption_1.interactable = true;
		levelupOption_2.interactable = true;
		levelupOption_3.interactable = false;
		levelupButton.interactable = true;
	}

	private void Set_UpgradeTooltip(Character character)
	{
		upg_Char_Image.sprite = character.charImage;
		upg_CharName_Text.text = character.charName;
		upg_Hp_Text.text = "Health : " + character.charHp.hp_max;
		upg_Exp_Text.text = "Exp : " + character.charExp.exp_max;
		upg_MP_Text.text = "MovePoints : " + character.charMovement.movePoints_max;
		upg_Dodge_Text.text = "Dodge : " + character.charDef.dodgeChance;
	}

	public void Button_Upgrade()
	{
		if (Utility.IsServer())
			StartCoroutine(GameMain.inst.Server_UpgradeCharacter(levelupCharacter, selectedUpgradeId));
		else
			GameMain.inst.Request_UpgradeCharacter(levelupCharacter, selectedUpgradeId);

		upgrade_Canvas.SetActive(false);
		endTurn.interactable = true;

		somePanelIsOn = false;
		ingameInput.mouseOverUI = false;
	}
	#endregion

	#region Menu
	public void Button_Menu()
	{
		if (helpMenu.activeInHierarchy)
		{
			helpMenu.SetActive(false);
			ingameInput.mouseOverUI = false;
		}
		else
		{
			helpMenu.SetActive(true);

			if(!Utility.IsServer())
				save_Button.interactable = false;
			else
				save_Button.interactable = true;
		}
	}

	public void Button_HideFog()
	{
		GameMain.inst.fog.Hide_Fog();

		ingameInput.mouseOverUI = false;
	}

	public void Button_Save()
	{
		SaveLoad.Save();

		ingameInput.mouseOverUI = false;
	}

	public void Button_Quit()
	{
		Application.Quit();
	}
	#endregion

	#region On turn message
	public void ShowTurnMessage()
	{
		turnMessage.SetActive(true);
		ingameInput.mouseOverUI = true;
	}
	public void HideTurnMessage()
	{
		turnMessage.SetActive(false);
		ingameInput.mouseOverUI = false;
	}
    #endregion
}
