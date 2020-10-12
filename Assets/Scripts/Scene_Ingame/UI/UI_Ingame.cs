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
	public Text cInfo_SlashResist_Text;
	public Text cInfo_PierceResist_Text;
	public Text cInfo_MagicResist_Text;
	public Button cInfo_Spell1_Obj;
	public Text cInfo_Spell1_CD_Text;
	public Button cInfo_Spell2_Obj;
	public Text cInfo_Spell2_CD_Text;
	public GameObject cInfo_Spell_Cancel;
	public Text cInfo_Attacks_Text;
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
    public GameObject charRecruitPanel;
	public Button closeRecruitMenu_button;
	private Dictionary<Button, Character> recrutable_dict = new Dictionary<Button, Character>();
	public Button recruit_1_button;
	public Button recruit_2_button;
	public Button recruit_3_button;
	public Button recruit_4_button;
	public Button recruit_5_button;
	public Button recruit_6_button;

	public Image rec_Char_Image;
	public Text rec_CharName_Text;
	public Text rec_CharCost_Text;
	public Text rec_CharHp_Text;
	public Text rec_CharExp_Text;
	public Text rec_CharMP_Text;
	public Text rec_CharDodge_Text;
	public Text rec_CharSlashResist_Text;
	public Text rec_CharPierceResist_Text;
	public Text rec_CharMagicResist_Text;

	private Character charToRecruit;
	private Hex recruitHex;
	public Button recruitButton;
	#endregion

	#region Attack panel
	[Header("Attack panel")]
	public GameObject attackPanel;
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
	[Header("Upgrade panel")]
	private Dictionary<Button, Character> levelup_dict = new Dictionary<Button, Character>();
	public GameObject levelupPanel;
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
	public Text upg_curSlashResist_Text;
	public Text upg_curPierceResist_Text;
	public Text upg_curMagicResist_Text;

	public Image upg_Char_Image;
	public Text upg_CharName_Text;
	public Text upg_Hp_Text;
	public Text upg_Exp_Text;
	public Text upg_MP_Text;
	public Text upg_Dodge_Text;
	public Text upg_SlashResist_Text;
	public Text upg_PierceResist_Text;
	public Text upg_MagicResist_Text;

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

		charRecruitPanel.SetActive(value: false);
		attackPanel.SetActive(value: false);
		levelupPanel.SetActive(value: false);
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
        pInfo_Vilage_Text.text = "" + player.villages;
        pInfo_Income_Text.text = "+" + player.villages * Utility.villageIncome;
        pInfo_Daytime_Text.text = "" + GameMain.inst.dayTime_cur;

		if(Utility.IsMyTurn())
			endTurn.interactable = true;
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
		cInfo_SlashResist_Text.text = "SlashRes : " + c.charDef.slash_resistance;
		cInfo_PierceResist_Text.text = "PierceRes : " + c.charDef.pierce_resistance;
		cInfo_MagicResist_Text.text = "MagicRes : " + c.charDef.magic_resistance;
		cInfo_Attacks_Text.text = "";
		for (int y = 0; y < c.charAttacks.Count; y++)
		{
			cInfo_Attacks_Text.text = cInfo_Attacks_Text.text + c.charAttacks[y].attackType + ", " + c.charAttacks[y].attackDmgType + " - " + c.charAttacks[y].attackCount + " x " + c.charAttacks[y].attackDmg_cur + "\n";
		}

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

		charRecruitPanel.SetActive(true);

		recruitButton.interactable = false;
		this.recruitHex = recruitHex;

		rec_Char_Image.enabled = false;
		rec_CharName_Text.text = "";
		rec_CharCost_Text.text = "";
		rec_CharHp_Text.text = "";
		rec_CharExp_Text.text = "";
		rec_CharMP_Text.text = "";
		rec_CharDodge_Text.text = "";
		rec_CharSlashResist_Text.text = "";
		rec_CharPierceResist_Text.text = "";
		rec_CharMagicResist_Text.text = "";

		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		switch (gameClient.race)
		{
			case 0: // Humans
				recruit_1_button.gameObject.SetActive(true);
				recruit_2_button.gameObject.SetActive(true);
				recruit_3_button.gameObject.SetActive(true);
				recruit_4_button.gameObject.SetActive(true);
				recruit_5_button.gameObject.SetActive(false);
				recruit_6_button.gameObject.SetActive(false);

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
					Character swordman = new Swordman(null, gameClient, isHero: false);
					recrutable_dict[recruit_2_button] = swordman;

					GameObject.Find("recruit_2_Char_Image").GetComponent<Image>().sprite = swordman.charImage;
					GameObject.Find("recruit_2_CharName_Text").GetComponent<Text>().text = (swordman.charName ?? "");
					GameObject.Find("recruit_2_CharPrice_Text").GetComponent<Text>().text = string.Concat(swordman.charCost);
				}
				if (!recrutable_dict.ContainsKey(recruit_3_button))
				{
					Character humArcher = new HumArcher(null, gameClient, isHero: false);
					recrutable_dict[recruit_3_button] = humArcher;

					GameObject.Find("recruit_3_Char_Image").GetComponent<Image>().sprite = humArcher.charImage;
					GameObject.Find("recruit_3_CharName_Text").GetComponent<Text>().text = (humArcher.charName ?? "");
					GameObject.Find("recruit_3_CharPrice_Text").GetComponent<Text>().text = string.Concat(humArcher.charCost);
				}
				if (!recrutable_dict.ContainsKey(recruit_4_button))
				{
					Character humMage = new HumMage(null, gameClient, isHero: false);
					recrutable_dict[recruit_4_button] = humMage;

					GameObject.Find("recruit_4_Char_Image").GetComponent<Image>().sprite = humMage.charImage;
					GameObject.Find("recruit_4_CharName_Text").GetComponent<Text>().text = (humMage.charName ?? "");
					GameObject.Find("recruit_4_CharPrice_Text").GetComponent<Text>().text = string.Concat(humMage.charCost);
				}
				// if (!recrutable_dict.ContainsKey(recruit_5_button))
				// {
				// 	Character humMage = new HumMage(null, gameClient, isHero: false);
				// 	recrutable_dict[recruit_5_button] = humMage;

				// 	GameObject.Find("recruit_5_Char_Image").GetComponent<Image>().sprite = humMage.charImage;
				// 	GameObject.Find("recruit_5_CharName_Text").GetComponent<Text>().text = (humMage.charName ?? "");
				// 	GameObject.Find("recruit_5_CharPrice_Text").GetComponent<Text>().text = string.Concat(humMage.charCost);
				// }
				// if (!recrutable_dict.ContainsKey(recruit_6_button))
				// {
				// 	Character knight = new KnightHalberd(null, gameClient, isHero: false);
				// 	recrutable_dict[recruit_6_button] = knight;

				// 	GameObject.Find("recruit_6_Char_Image").GetComponent<Image>().sprite = knight.charImage;
				// 	GameObject.Find("recruit_6_CharName_Text").GetComponent<Text>().text = (knight.charName ?? "");
				// 	GameObject.Find("recruit_6_CharPrice_Text").GetComponent<Text>().text = string.Concat(knight.charCost);
				// }
			break;

			case 1: // Orcs
				recruit_1_button.gameObject.SetActive(false);
				recruit_2_button.gameObject.SetActive(false);
				recruit_3_button.gameObject.SetActive(false);
				recruit_4_button.gameObject.SetActive(false);
				recruit_5_button.gameObject.SetActive(false);
				recruit_6_button.gameObject.SetActive(false);
			break;

			case 2: // Undeads
				recruit_1_button.gameObject.SetActive(true);
				recruit_2_button.gameObject.SetActive(true);
				recruit_3_button.gameObject.SetActive(true);
				recruit_4_button.gameObject.SetActive(true);
				recruit_5_button.gameObject.SetActive(false);
				recruit_6_button.gameObject.SetActive(false);

				if (!recrutable_dict.ContainsKey(recruit_1_button))
				{
					Character character = new Skeleton(null, gameClient, isHero: false);
					recrutable_dict[recruit_1_button] = character;
					GameObject.Find("recruit_1_Char_Image").GetComponent<Image>().sprite = character.charImage;
					GameObject.Find("recruit_1_CharName_Text").GetComponent<Text>().text = (character.charName ?? "");
					GameObject.Find("recruit_1_CharPrice_Text").GetComponent<Text>().text = string.Concat(character.charCost);
				}

				if (!recrutable_dict.ContainsKey(recruit_2_button))
				{
					Character character2 = new SkelMage(null, gameClient, isHero: false);
					recrutable_dict[recruit_2_button] = character2;
					GameObject.Find("recruit_2_Char_Image").GetComponent<Image>().sprite = character2.charImage;
					GameObject.Find("recruit_2_CharName_Text").GetComponent<Text>().text = (character2.charName ?? "");
					GameObject.Find("recruit_2_CharPrice_Text").GetComponent<Text>().text = string.Concat(character2.charCost);
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
					Character character4 = new BatWhite(null, gameClient, isHero: false);
					recrutable_dict[recruit_4_button] = character4;
					GameObject.Find("recruit_4_Char_Image").GetComponent<Image>().sprite = character4.charImage;
					GameObject.Find("recruit_4_CharName_Text").GetComponent<Text>().text = (character4.charName ?? "");
					GameObject.Find("recruit_4_CharPrice_Text").GetComponent<Text>().text = string.Concat(character4.charCost);
				}
				break;
		}
	}

	public void Recruit_CloseMenu()
	{
		charRecruitPanel.SetActive(value: false);

		somePanelIsOn = false;
		ingameInput.mouseOverUI = false;
	}

	public void Recruit_Character_1()
	{
		rec_Char_Image.enabled = true;
		rec_Char_Image.sprite = recrutable_dict[recruit_1_button].charImage;
		rec_CharName_Text.text = recrutable_dict[recruit_1_button].charName;
		rec_CharCost_Text.text = "Cost : " + recrutable_dict[recruit_1_button].charCost;
		rec_CharHp_Text.text = "Health : " + recrutable_dict[recruit_1_button].charHp.hp_max;
		rec_CharExp_Text.text = "Exp : " + recrutable_dict[recruit_1_button].charExp.exp_max;
		rec_CharMP_Text.text = "MovePoints : " + recrutable_dict[recruit_1_button].charMovement.movePoints_max;
		rec_CharDodge_Text.text = "Dodge : " + recrutable_dict[recruit_1_button].charDef.dodgeChance;
		rec_CharSlashResist_Text.text = "SlashRes : " + recrutable_dict[recruit_1_button].charDef.slash_resistance;
		rec_CharPierceResist_Text.text = "PierceRes : " + recrutable_dict[recruit_1_button].charDef.pierce_resistance;
		rec_CharMagicResist_Text.text = "MagicRes : " + recrutable_dict[recruit_1_button].charDef.magic_resistance;

		charToRecruit = recrutable_dict[recruit_1_button];
		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_1_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_2()
	{
		rec_Char_Image.enabled = true;
		rec_Char_Image.sprite = recrutable_dict[recruit_2_button].charImage;
		rec_CharName_Text.text = recrutable_dict[recruit_2_button].charName;
		rec_CharCost_Text.text = "Cost : " + recrutable_dict[recruit_2_button].charCost;
		rec_CharHp_Text.text = "Health : " + recrutable_dict[recruit_2_button].charHp.hp_max;
		rec_CharExp_Text.text = "Exp : " + recrutable_dict[recruit_2_button].charExp.exp_max;
		rec_CharMP_Text.text = "MovePoints : " + recrutable_dict[recruit_2_button].charMovement.movePoints_max;
		rec_CharDodge_Text.text = "Dodge : " + recrutable_dict[recruit_2_button].charDef.dodgeChance;
		rec_CharSlashResist_Text.text = "SlashRes : " + recrutable_dict[recruit_2_button].charDef.slash_resistance;
		rec_CharPierceResist_Text.text = "PierceRes : " + recrutable_dict[recruit_2_button].charDef.pierce_resistance;
		rec_CharMagicResist_Text.text = "MagicRes : " + recrutable_dict[recruit_2_button].charDef.magic_resistance;

		charToRecruit = recrutable_dict[recruit_2_button];
		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_2_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_3()
	{
		rec_Char_Image.enabled = true;
		rec_Char_Image.sprite = recrutable_dict[recruit_3_button].charImage;
		rec_CharName_Text.text = recrutable_dict[recruit_3_button].charName;
		rec_CharCost_Text.text = "Cost : " + recrutable_dict[recruit_3_button].charCost;
		rec_CharHp_Text.text = "Health : " + recrutable_dict[recruit_3_button].charHp.hp_max;
		rec_CharExp_Text.text = "Exp : " + recrutable_dict[recruit_3_button].charExp.exp_max;
		rec_CharMP_Text.text = "MovePoints : " + recrutable_dict[recruit_3_button].charMovement.movePoints_max;
		rec_CharDodge_Text.text = "Dodge : " + recrutable_dict[recruit_3_button].charDef.dodgeChance;
		rec_CharSlashResist_Text.text = "SlashRes : " + recrutable_dict[recruit_3_button].charDef.slash_resistance;
		rec_CharPierceResist_Text.text = "PierceRes : " + recrutable_dict[recruit_3_button].charDef.pierce_resistance;
		rec_CharMagicResist_Text.text = "MagicRes : " + recrutable_dict[recruit_3_button].charDef.magic_resistance;
		
		charToRecruit = recrutable_dict[recruit_3_button];
		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_3_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_4()
	{
		rec_Char_Image.enabled = true;
		rec_Char_Image.sprite = recrutable_dict[recruit_4_button].charImage;
		rec_CharName_Text.text = recrutable_dict[recruit_4_button].charName;
		rec_CharCost_Text.text = "Cost : " + recrutable_dict[recruit_4_button].charCost;
		rec_CharHp_Text.text = "Health : " + recrutable_dict[recruit_4_button].charHp.hp_max;
		rec_CharExp_Text.text = "Exp : " + recrutable_dict[recruit_4_button].charExp.exp_max;
		rec_CharMP_Text.text = "MovePoints : " + recrutable_dict[recruit_4_button].charMovement.movePoints_max;
		rec_CharDodge_Text.text = "Dodge : " + recrutable_dict[recruit_4_button].charDef.dodgeChance;
		rec_CharSlashResist_Text.text = "SlashRes : " + recrutable_dict[recruit_4_button].charDef.slash_resistance;
		rec_CharPierceResist_Text.text = "PierceRes : " + recrutable_dict[recruit_4_button].charDef.pierce_resistance;
		rec_CharMagicResist_Text.text = "MagicRes : " + recrutable_dict[recruit_4_button].charDef.magic_resistance;

		charToRecruit = recrutable_dict[recruit_4_button];
		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_4_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_5()
	{
		rec_Char_Image.enabled = true;
		rec_Char_Image.sprite = recrutable_dict[recruit_5_button].charImage;
		rec_CharName_Text.text = recrutable_dict[recruit_5_button].charName;
		rec_CharCost_Text.text = "Cost : " + recrutable_dict[recruit_5_button].charCost;
		rec_CharHp_Text.text = "Health : " + recrutable_dict[recruit_5_button].charHp.hp_max;
		rec_CharExp_Text.text = "Exp : " + recrutable_dict[recruit_5_button].charExp.exp_max;
		rec_CharMP_Text.text = "MovePoints : " + recrutable_dict[recruit_5_button].charMovement.movePoints_max;
		rec_CharDodge_Text.text = "Dodge : " + recrutable_dict[recruit_5_button].charDef.dodgeChance;
		rec_CharSlashResist_Text.text = "SlashRes : " + recrutable_dict[recruit_5_button].charDef.slash_resistance;
		rec_CharPierceResist_Text.text = "PierceRes : " + recrutable_dict[recruit_5_button].charDef.pierce_resistance;
		rec_CharMagicResist_Text.text = "MagicRes : " + recrutable_dict[recruit_5_button].charDef.magic_resistance;

		charToRecruit = recrutable_dict[recruit_5_button];
		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_5_button].charCost)
		{
			recruitButton.interactable = true;
		}
	}

	public void Recruit_Character_6()
	{
		rec_Char_Image.enabled = true;
		rec_Char_Image.sprite = recrutable_dict[recruit_6_button].charImage;
		rec_CharName_Text.text = recrutable_dict[recruit_6_button].charName;
		rec_CharCost_Text.text = "Cost : " + recrutable_dict[recruit_6_button].charCost;
		rec_CharHp_Text.text = "Health : " + recrutable_dict[recruit_6_button].charHp.hp_max;
		rec_CharExp_Text.text = "Exp : " + recrutable_dict[recruit_6_button].charExp.exp_max;
		rec_CharMP_Text.text = "MovePoints : " + recrutable_dict[recruit_6_button].charMovement.movePoints_max;
		rec_CharDodge_Text.text = "Dodge : " + recrutable_dict[recruit_6_button].charDef.dodgeChance;
		rec_CharSlashResist_Text.text = "SlashRes : " + recrutable_dict[recruit_6_button].charDef.slash_resistance;
		rec_CharPierceResist_Text.text = "PierceRes : " + recrutable_dict[recruit_6_button].charDef.pierce_resistance;
		rec_CharMagicResist_Text.text = "MagicRes : " + recrutable_dict[recruit_6_button].charDef.magic_resistance;

		charToRecruit = recrutable_dict[recruit_6_button];
		Player gameClient = null;
		gameClient = ((!Utility.IsServer()) ? Utility.Get_Client_byString(client.player.name, client.players) : Utility.Get_Client_byString(server.player.name, server.players));
		recruitButton.interactable = false;
		if (gameClient.gold >= recrutable_dict[recruit_6_button].charCost)
		{
			recruitButton.interactable = true;
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

		attack1.gameObject.SetActive(value: false);
		attack2.gameObject.SetActive(value: false);
		attack3.gameObject.SetActive(value: false);
		attack1.interactable = true;
		attack2.interactable = true;
		attack3.interactable = true;
		attack_button.interactable = false;
		attackPanel.SetActive(value: true);
		this.attackPath = new List<Hex>(attackPath);
		List<Utility.char_Attack> charAttacks = attackPath[0].character.charAttacks;
		List<Utility.char_Attack> charAttacks2 = attackPath[attackPath.Count - 1].character.charAttacks;
		attackerImage.sprite = attackPath[0].character.charImage;
		targetImage.sprite = attackPath[attackPath.Count - 1].character.charImage;
		if (charAttacks.Count > 0 || charAttacks2.Count > 0)
		{
			attack1.gameObject.SetActive(value: true);
			if (charAttacks.Count > 0)
			{
				attack1_Image_a.gameObject.SetActive(value: true);
				attack1_Image_a.sprite = Get_AttackTypeImage(charAttacks[0].attackDmgType);
				attack1_Text_a.text = charAttacks[0].attackType + ", " + charAttacks[0].attackDmgType + " " + charAttacks[0].attackCount + " x " + charAttacks[0].attackDmg_cur;
			}
			else
			{
				attack1_Image_a.gameObject.SetActive(value: false);
				attack1_Text_a.text = "";
			}
			if (charAttacks2.Count > 0)
			{
				attack1_Image_t.gameObject.SetActive(value: true);
				attack1_Image_t.sprite = Get_AttackTypeImage(charAttacks2[0].attackDmgType);
				attack1_Text_t.text = charAttacks2[0].attackType + ", " + charAttacks2[0].attackDmgType + " " + charAttacks2[0].attackCount + " x " + charAttacks2[0].attackDmg_cur;
			}
			else
			{
				attack1_Image_t.gameObject.SetActive(value: false);
				GameObject.Find("Attack1").transform.Find("TargetInfo_Text").GetComponent<Text>().text = "";
			}
		}
		if (charAttacks.Count > 1 || charAttacks2.Count > 1)
		{
			attack2.gameObject.SetActive(value: true);
			if (charAttacks.Count > 1)
			{
				attack2_Image_a.gameObject.SetActive(value: true);
				attack2_Image_a.sprite = Get_AttackTypeImage(charAttacks[1].attackDmgType);
				attack2_Text_a.text = charAttacks[1].attackType + ", " + charAttacks[1].attackDmgType + " " + charAttacks[1].attackCount + " x " + charAttacks[1].attackDmg_cur;
			}
			else
			{
				attack2_Image_a.gameObject.SetActive(value: false);
				attack2_Text_a.text = "";
			}
			if (charAttacks2.Count > 1)
			{
				attack2_Image_t.gameObject.SetActive(value: true);
				attack2_Image_t.sprite = Get_AttackTypeImage(charAttacks2[1].attackDmgType);
				attack2_Text_t.text = charAttacks2[1].attackType + ", " + charAttacks2[1].attackDmgType + " " + charAttacks2[1].attackCount + " x " + charAttacks2[1].attackDmg_cur;
			}
			else
			{
				attack2_Image_t.gameObject.SetActive(value: false);
				attack2_Text_t.text = "";
			}
		}
		if (charAttacks.Count > 2 || charAttacks2.Count > 2)
		{
			attack3.gameObject.SetActive(value: true);
			if (charAttacks.Count > 2)
			{
				attack3_Image_a.gameObject.SetActive(value: true);
				attack3_Image_a.sprite = Get_AttackTypeImage(charAttacks[2].attackDmgType);
				attack3_Text_a.text = charAttacks[2].attackType + ", " + charAttacks[2].attackDmgType + " " + charAttacks[2].attackCount + " x " + charAttacks[2].attackDmg_cur;
			}
			else
			{
				attack3_Image_a.gameObject.SetActive(value: false);
				attack3_Text_a.text = "";
			}
			if (charAttacks2.Count > 2)
			{
				attack3_Image_t.gameObject.SetActive(value: true);
				attack3_Image_t.sprite = Get_AttackTypeImage(charAttacks[2].attackDmgType);
				attack3_Text_t.text = charAttacks2[2].attackType + ", " + charAttacks2[2].attackDmgType + " " + charAttacks2[2].attackCount + " x " + charAttacks2[2].attackDmg_cur;
			}
			else
			{
				attack3_Image_t.gameObject.SetActive(value: false);
				attack3_Text_t.text = "";
			}
		}
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
		attackPanel.SetActive(value: false);

		somePanelIsOn = false;
		ingameInput.mouseOverUI = false;
	}
	#endregion

	#region Level up
	public void Show_LevelupPanel(Character character)
	{
		somePanelIsOn = true;

		endTurn.interactable = false;
		levelupPanel.SetActive(true);
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
			levelupOption_2.gameObject.SetActive(value: true);
			levelupOption_2.interactable = true;

			levelup_dict[levelupOption_2] = charData.Get_CharacterById(levelupCharacter.upgradeList[1]);

			GameObject.Find("upg2Image").GetComponent<Image>().sprite = levelup_dict[levelupOption_2].charImage;
			GameObject.Find("upg2Name").GetComponent<Text>().text = levelup_dict[levelupOption_2].charName;
		}
		if (levelupCharacter.upgradeList.Count > 2)
		{
			levelupOption_3.gameObject.SetActive(value: true);
			levelupOption_3.interactable = true;
			levelup_dict[levelupOption_3] = charData.Get_CharacterById(levelupCharacter.upgradeList[2]);
			GameObject.Find("upg3Image").GetComponent<Image>().sprite = levelup_dict[levelupOption_3].charImage;
			GameObject.Find("upg3Name").GetComponent<Text>().text = levelup_dict[levelupOption_3].charName;
		}

		upg_curChar_Image.sprite = levelupCharacter.charImage;
		upg_curName_Text.text = levelupCharacter.charName;
		upg_curHp_Text.text = "Health : " + levelupCharacter.charHp.hp_max;
		upg_curExp_Text.text = "Exp : " + levelupCharacter.charExp.exp_max;
		upg_curMP_Text.text = "MovePoints : " + levelupCharacter.charMovement.movePoints_max;
		upg_curDodge_Text.text = "Dodge : " + levelupCharacter.charDef.dodgeChance;
		upg_curSlashResist_Text.text = "SlashRes : " + levelupCharacter.charDef.slash_resistance;
		upg_curPierceResist_Text.text = "PierceRes : " + levelupCharacter.charDef.pierce_resistance;
		upg_curMagicResist_Text.text = "MagicRes : " + levelupCharacter.charDef.magic_resistance;

		upg_Char_Image.gameObject.SetActive(false);
		upg_CharName_Text.text = "";
		upg_Hp_Text.text = "";
		upg_Exp_Text.text = "";
		upg_MP_Text.text = "";
		upg_Dodge_Text.text = "";
		upg_SlashResist_Text.text = "";
		upg_PierceResist_Text.text = "";
		upg_MagicResist_Text.text = "";
	}

	public void Select_Upgrade1()
	{
		selectedUpgradeId = levelup_dict[levelupOption_1].charId;

		upg_Char_Image.gameObject.SetActive(true);
		upg_Char_Image.sprite = levelup_dict[levelupOption_1].charImage;
		upg_CharName_Text.text = levelup_dict[levelupOption_1].charName;
		upg_Hp_Text.text = "Health : " + levelup_dict[levelupOption_1].charHp.hp_max;
		upg_Exp_Text.text = "Exp : " + levelup_dict[levelupOption_1].charExp.exp_max;
		upg_MP_Text.text = "MovePoints : " + levelup_dict[levelupOption_1].charMovement.movePoints_max;
		upg_Dodge_Text.text = "Dodge : " + levelup_dict[levelupOption_1].charDef.dodgeChance;
		upg_SlashResist_Text.text = "SlashRes : " + levelup_dict[levelupOption_1].charDef.slash_resistance;
		upg_PierceResist_Text.text = "PierceRes : " + levelup_dict[levelupOption_1].charDef.pierce_resistance;
		upg_MagicResist_Text.text = "MagicRes : " + levelup_dict[levelupOption_1].charDef.magic_resistance;

		levelupOption_1.interactable = false;
		levelupOption_2.interactable = true;
		levelupOption_3.interactable = true;
		levelupButton.interactable = true;
	}

	public void Select_Upgrade2()
	{
		selectedUpgradeId = levelup_dict[levelupOption_2].charId;

		upg_Char_Image.gameObject.SetActive(true);
		upg_Char_Image.sprite = levelup_dict[levelupOption_2].charImage;
		upg_CharName_Text.text = levelup_dict[levelupOption_2].charName;
		upg_Hp_Text.text = "Health : " + levelup_dict[levelupOption_2].charHp.hp_max;
		upg_Exp_Text.text = "Exp : " + levelup_dict[levelupOption_2].charExp.exp_max;
		upg_MP_Text.text = "MovePoints : " + levelup_dict[levelupOption_2].charMovement.movePoints_max;
		upg_Dodge_Text.text = "Dodge : " + levelup_dict[levelupOption_2].charDef.dodgeChance;
		upg_SlashResist_Text.text = "SlashRes : " + levelup_dict[levelupOption_2].charDef.slash_resistance;
		upg_PierceResist_Text.text = "PierceRes : " + levelup_dict[levelupOption_2].charDef.pierce_resistance;
		upg_MagicResist_Text.text = "MagicRes : " + levelup_dict[levelupOption_2].charDef.magic_resistance;

		levelupOption_1.interactable = true;
		levelupOption_2.interactable = false;
		levelupOption_3.interactable = true;
		levelupButton.interactable = true;
	}

	public void Select_Upgrade3()
	{
		selectedUpgradeId = levelup_dict[levelupOption_3].charId;

		upg_Char_Image.gameObject.SetActive(true);
		upg_Char_Image.sprite = levelup_dict[levelupOption_3].charImage;
		upg_CharName_Text.text = levelup_dict[levelupOption_3].charName;
		upg_Hp_Text.text = "Health : " + levelup_dict[levelupOption_3].charHp.hp_max;
		upg_Exp_Text.text = "Exp : " + levelup_dict[levelupOption_3].charExp.exp_max;
		upg_MP_Text.text = "MovePoints : " + levelup_dict[levelupOption_3].charMovement.movePoints_max;
		upg_Dodge_Text.text = "Dodge : " + levelup_dict[levelupOption_3].charDef.dodgeChance;
		upg_SlashResist_Text.text = "SlashRes : " + levelup_dict[levelupOption_3].charDef.slash_resistance;
		upg_PierceResist_Text.text = "PierceRes : " + levelup_dict[levelupOption_3].charDef.pierce_resistance;
		upg_MagicResist_Text.text = "MagicRes : " + levelup_dict[levelupOption_3].charDef.magic_resistance;

		levelupOption_1.interactable = true;
		levelupOption_2.interactable = true;
		levelupOption_3.interactable = false;
		levelupButton.interactable = true;
	}

	public void Button_Upgrade()
	{
		if (Utility.IsServer())
			StartCoroutine(GameMain.inst.Server_UpgradeCharacter(levelupCharacter, selectedUpgradeId));
		else
			GameMain.inst.Request_UpgradeCharacter(levelupCharacter, selectedUpgradeId);

		levelupPanel.SetActive(false);
		endTurn.interactable = true;

		somePanelIsOn = false;
		ingameInput.mouseOverUI = false;
	}
	#endregion

	private Sprite Get_AttackTypeImage(Utility.char_attackDmgType attackDmgType)
	{
		Sprite result = null;
		switch (attackDmgType)
		{
		case Utility.char_attackDmgType.slash:
			result = Resources.Load<Sprite>("DamageType/IronSword");
			break;
		case Utility.char_attackDmgType.pierce:
			result = Resources.Load<Sprite>("DamageType/Arrow");
			break;
		case Utility.char_attackDmgType.magic:
			result = Resources.Load<Sprite>("DamageType/RubyStaff");
			break;
		}
		return result;
	}

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
}
