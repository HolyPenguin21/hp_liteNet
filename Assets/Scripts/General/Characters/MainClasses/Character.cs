using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
	public Transform tr;

	public Hex hex;
	public Player owner;
	public bool heroCharacter;

	public Sprite charImage;
	public string charName;
	public int charId;
	public int charCost;
	public bool canAct;

	public CharVars.char_Type charType;
	public CharVars.char_Hp charHp;
	public CharVars.char_Defence charDef;
	public CharVars.char_Exp charExp;
	public CharVars.char_Move charMovement;
	public List<CharVars.char_Attack> charAttacks;

	public List<int> upgradeList = new List<int>();

	public Item charItem;

	public Spell charSpell_1;
	public Spell charSpell_2;

	public List<Buff> charBuffs = new List<Buff>();

	public float moveAnimationSpeed = 3f;
	public float attackAnimationSpeed = 3f;

	public void Init(Transform tr_, Player owner_, bool isHero_)
	{
		tr = tr_;
		owner = owner_;

		if (isHero_)
			heroCharacter = true;
		else
		{
			if (tr != null)
				tr.Find("Hero").gameObject.SetActive(false);
		}

		// Item icon
		if (tr != null) tr.Find("Item").gameObject.SetActive(false);
	}

    #region Updates
    public IEnumerator OnMyTurnUpdate()
	{
		charMovement.movePoints_cur = charMovement.movePoints_max;
		canAct = true;
		if (tr != null) tr.Find("canMove").gameObject.SetActive(true);

		if (charSpell_1 != null) charSpell_1.CooldownUpdate();
		if (charSpell_2 != null) charSpell_2.CooldownUpdate();

		AttackUpdateOnDayChange();
		VillageHeal();

		if (charItem != null) charItem.Item_OnTurn(this);

		for (int x = 0; x < charBuffs.Count; x++)
		{
			Buff someBuff = charBuffs[x];
			if (someBuff.buffType != Utility.buff_Type.onTurn) continue;

			yield return charBuffs[x].Buff_Activate(this);
		}

		yield return null;
	}

	public IEnumerator OnEnemyTurnUpdate()
	{
		charMovement.movePoints_cur = 0;
		canAct = false;
		if (tr != null) tr.Find("canMove").gameObject.SetActive(false);

		AttackUpdateOnDayChange();

		yield return null;
	}

	private void AttackUpdateOnDayChange()
	{
		GameMain inst = GameMain.inst;
		switch (charType)
		{
			case CharVars.char_Type.day:
				if (inst.dayTime_cur == Utility.dayTime.day1 || inst.dayTime_cur == Utility.dayTime.day2)
				{
					for (int j = 0; j < charAttacks.Count; j++)
					{
						CharVars.char_Attack value2 = charAttacks[j];
						value2.attackDmg_cur = Convert.ToInt32((float)charAttacks[j].attackDmg_base + (float)charAttacks[j].attackDmg_base * 0.25f);
						charAttacks[j] = value2;
					}
				}
				else if (inst.dayTime_cur == Utility.dayTime.night1 || inst.dayTime_cur == Utility.dayTime.night2)
				{
					for (int k = 0; k < charAttacks.Count; k++)
					{
						CharVars.char_Attack value3 = charAttacks[k];
						value3.attackDmg_cur = Convert.ToInt32((float)charAttacks[k].attackDmg_base - (float)charAttacks[k].attackDmg_base * 0.25f);
						charAttacks[k] = value3;
					}
				}
				else
				{
					for (int l = 0; l < charAttacks.Count; l++)
					{
						CharVars.char_Attack value4 = charAttacks[l];
						value4.attackDmg_cur = Convert.ToInt32(charAttacks[l].attackDmg_base);
						charAttacks[l] = value4;
					}
				}
				break;
			case CharVars.char_Type.night:
				if (inst.dayTime_cur == Utility.dayTime.day1 || inst.dayTime_cur == Utility.dayTime.day2)
				{
					for (int m = 0; m < charAttacks.Count; m++)
					{
						CharVars.char_Attack value5 = charAttacks[m];
						value5.attackDmg_cur = Convert.ToInt32((float)charAttacks[m].attackDmg_base - (float)charAttacks[m].attackDmg_base * 0.25f);
						charAttacks[m] = value5;
					}
				}
				else if (inst.dayTime_cur == Utility.dayTime.night1 || inst.dayTime_cur == Utility.dayTime.night2)
				{
					for (int n = 0; n < charAttacks.Count; n++)
					{
						CharVars.char_Attack value6 = charAttacks[n];
						value6.attackDmg_cur = Convert.ToInt32((float)charAttacks[n].attackDmg_base + (float)charAttacks[n].attackDmg_base * 0.25f);
						charAttacks[n] = value6;
					}
				}
				else
				{
					for (int num = 0; num < charAttacks.Count; num++)
					{
						CharVars.char_Attack value7 = charAttacks[num];
						value7.attackDmg_cur = Convert.ToInt32(charAttacks[num].attackDmg_base);
						charAttacks[num] = value7;
					}
				}
				break;
			case CharVars.char_Type.neutral:
				for (int i = 0; i < charAttacks.Count; i++)
				{
					CharVars.char_Attack value = charAttacks[i];
					value.attackDmg_cur = Convert.ToInt32(charAttacks[i].attackDmg_base);
					charAttacks[i] = value;
				}
				break;
		}
	}
	#endregion

	#region Health actions
	public void Set_Health(int hp_target)
	{
		charHp.hp_cur = hp_target;
	}

	private void VillageHeal()
	{
		if (!hex.isVillage) return;

		Buff existBuff = charBuffs.Find(i => i.buffId == 4); // Poisoned
		if (existBuff != null)
		{
			charBuffs.Remove(existBuff);
		}
		else if (charHp.hp_cur < charHp.hp_max)
		{
			charHp.hp_cur += Utility.villageHeal;

			if (tr.gameObject.activeInHierarchy)
				GameMain.inst.effectsData.Effect_VillageHeal(hex.transform.position, Utility.villageHeal);

			if (charHp.hp_cur > charHp.hp_max)
				charHp.hp_cur = charHp.hp_max;
		}

	}

	public void RecieveHeal(int amount)
	{
		if (amount == 0) return;

		charHp.hp_cur += amount;

		if (charHp.hp_cur > charHp.hp_max)
			charHp.hp_cur = charHp.hp_max;

		if (tr.gameObject.activeInHierarchy)
			GameMain.inst.effectsData.Effect_VillageHeal(hex.transform.position, amount);
	}
    #endregion

    #region Movement
    public IEnumerator Move(List<Hex> somePath)
	{
		if (somePath != null)
		{
			List<Hex> path = new List<Hex>(somePath);
			while (path.Count > 0)
			{
				hex.character = null;
				hex = path[0];
				path[0].character = this;
				yield return ActualMove(path[0]);
				path.RemoveAt(0);
			}
		}

		if (Utility.IsMyCharacter(this))
			GameObject.Find("UI").GetComponent<Ingame_Input>().SelectHex(hex);

		if (charMovement.movePoints_cur == 0)
			GameMain.inst.fog.UpdateFog_PlayerView();
	}
	private IEnumerator ActualMove(Hex hexToMove)
	{
		float t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime * moveAnimationSpeed;
			t2 = Mathf.Clamp(t2 + Time.deltaTime * 0.01f, 0f, 1f);
			tr.position = Vector3.Lerp(tr.position, hexToMove.transform.position, t2);
			yield return null;
		}

		GameMain.inst.fog.UpdateFog_PlayerView();
	}
	public void Replace(Hex replaceTo)
	{
		hex.character = null;
		hex = replaceTo;
		replaceTo.character = this;

		tr.position = replaceTo.transform.position;

		GameMain.inst.fog.UpdateFog_PlayerView();

		if (Utility.IsMyCharacter(this))
			GameObject.Find("UI").GetComponent<Ingame_Input>().SelectHex(hex);
	}
    #endregion

    #region Item actions
    public void Item_PickUp(Hex hexWithItem)
	{
		Item item = hexWithItem.item;
		hexWithItem.Remove_Item();
		charItem = item;
		if (tr != null) tr.Find("Item").gameObject.SetActive(true);
		// update UI
		item.Item_OnEquip(this);
	}
	public void Item_Drop()
	{
		Item item = charItem;
		if (item == null) return;
		item.Item_OnRemove(this);

		charItem = null;
		if (tr != null) tr.Find("Item").gameObject.SetActive(false);
		hex.Add_Item(item);
	}
	public void Item_Use()
	{
		if (charItem == null) return;
		if (charItem.itemActive == null) return;

		GameMain.inst.StartCoroutine(charItem.itemActive.Buff_Activate(this));
	}
	public void Item_Remove()
	{
		charItem = null;
		if (tr != null) tr.Find("Item").gameObject.SetActive(false);
	}
	#endregion

	public IEnumerator AttackAnimation(Hex target, int attackId)
	{
		if (attackId == 1)
		{
			float t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * attackAnimationSpeed * 4;
				yield return null;
			}

			switch (charId)
			{
				case 10:
				case 34:
				case 17:
				case 22:
				case 20:
				case 23:
					GameMain.inst.effectsData.Effect_Lightning(target.transform.position);
					break;
			}

			t = 0f;
			while (t < 1f)
			{
				t += Time.deltaTime * attackAnimationSpeed * 0.5f;
				yield return null;
			}
		}
		else
		{
			// attack move
			float t = 0f;
			Vector3 attackVector = tr.position + (target.transform.position - tr.position) / 2; // A+(B-A)/2 - vector middle
			while (t < 1f)
			{
				tr.position = Vector3.Lerp(tr.position, attackVector, t);
				t += Time.deltaTime * attackAnimationSpeed * 2;
				yield return null;
			}
			// return move
			t = 0f;
			while (t < 1f)
			{
				tr.position = Vector3.Lerp(tr.position, hex.transform.position, t);
				t += Time.deltaTime * attackAnimationSpeed;
				yield return null;
			}
		}
	}

	public void Add_Exp(int expToAdd)
	{
		charExp.exp_cur += expToAdd;
	}

	public void Recieve_ABuff_AsAttacker(ABuff aBuff, int hp_target)
	{
		switch (aBuff.buffId)
		{
			case 1: // Drain
				int drain = hp_target - charHp.hp_cur;
				RecieveHeal(drain);
				break;
		}
	}

	public void Recieve_ABuff_AsTarget(ABuff aBuff)
	{
		switch (aBuff.buffId)
		{
			case 2: // Poison touch
				Buff someBuff = charBuffs.Find(i => i.buffId == 4);
				if(someBuff == null) charBuffs.Add(new Buff_Poison());
				break;
		}
	}
}
