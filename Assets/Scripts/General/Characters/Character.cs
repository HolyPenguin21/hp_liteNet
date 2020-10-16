﻿using System;
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
	public int lookRange;

	public Utility.char_Type charType;
	public Utility.char_Hp charHp;
	public Utility.char_Defence charDef;
	public Utility.char_Exp charExp;
	public Utility.char_Move charMovement;
	public List<Utility.char_Attack> charAttacks;

	public List<int> upgradeList = new List<int>();

	public Item charItem;

	public Spell charSpell_1;
	public Spell charSpell_2;

	public float moveAnimationSpeed = 3f;
	public float attackAnimationSpeed = 3f;

	public void OnMyTurnUpdate()
	{
		charMovement.movePoints_cur = charMovement.movePoints_max;
		canAct = true;
		if (tr != null) tr.Find("canMove").gameObject.SetActive(true);

		if (charSpell_1 != null) charSpell_1.CooldownUpdate();
		if(charSpell_2 != null) charSpell_2.CooldownUpdate();

		AttackUpdateOnDayChange();
		VillageHeal();
	}

	public void OnEnemyTurnUpdate()
	{
		charMovement.movePoints_cur = 0;
		canAct = false;
		if (tr != null) tr.Find("canMove").gameObject.SetActive(false);

		AttackUpdateOnDayChange();
	}

	private void AttackUpdateOnDayChange()
	{
		GameMain inst = GameMain.inst;
		switch (charType)
		{
			case Utility.char_Type.day:
				if (inst.dayTime_cur == Utility.dayTime.day1 || inst.dayTime_cur == Utility.dayTime.day2)
				{
					for (int j = 0; j < charAttacks.Count; j++)
					{
						Utility.char_Attack value2 = charAttacks[j];
						value2.attackDmg_cur = Convert.ToInt32((float)charAttacks[j].attackDmg_base + (float)charAttacks[j].attackDmg_base * 0.25f);
						charAttacks[j] = value2;
					}
				}
				else if (inst.dayTime_cur == Utility.dayTime.night1 || inst.dayTime_cur == Utility.dayTime.night2)
				{
					for (int k = 0; k < charAttacks.Count; k++)
					{
						Utility.char_Attack value3 = charAttacks[k];
						value3.attackDmg_cur = Convert.ToInt32((float)charAttacks[k].attackDmg_base - (float)charAttacks[k].attackDmg_base * 0.25f);
						charAttacks[k] = value3;
					}
				}
				else
				{
					for (int l = 0; l < charAttacks.Count; l++)
					{
						Utility.char_Attack value4 = charAttacks[l];
						value4.attackDmg_cur = Convert.ToInt32(charAttacks[l].attackDmg_base);
						charAttacks[l] = value4;
					}
				}
				break;
			case Utility.char_Type.night:
				if (inst.dayTime_cur == Utility.dayTime.day1 || inst.dayTime_cur == Utility.dayTime.day2)
				{
					for (int m = 0; m < charAttacks.Count; m++)
					{
						Utility.char_Attack value5 = charAttacks[m];
						value5.attackDmg_cur = Convert.ToInt32((float)charAttacks[m].attackDmg_base - (float)charAttacks[m].attackDmg_base * 0.25f);
						charAttacks[m] = value5;
					}
				}
				else if (inst.dayTime_cur == Utility.dayTime.night1 || inst.dayTime_cur == Utility.dayTime.night2)
				{
					for (int n = 0; n < charAttacks.Count; n++)
					{
						Utility.char_Attack value6 = charAttacks[n];
						value6.attackDmg_cur = Convert.ToInt32((float)charAttacks[n].attackDmg_base + (float)charAttacks[n].attackDmg_base * 0.25f);
						charAttacks[n] = value6;
					}
				}
				else
				{
					for (int num = 0; num < charAttacks.Count; num++)
					{
						Utility.char_Attack value7 = charAttacks[num];
						value7.attackDmg_cur = Convert.ToInt32(charAttacks[num].attackDmg_base);
						charAttacks[num] = value7;
					}
				}
				break;
			case Utility.char_Type.neutral:
				for (int i = 0; i < charAttacks.Count; i++)
				{
					Utility.char_Attack value = charAttacks[i];
					value.attackDmg_cur = Convert.ToInt32(charAttacks[i].attackDmg_base);
					charAttacks[i] = value;
				}
				break;
		}
	}

	private void VillageHeal()
	{
		if (!hex.isVillage) return;

		if (charHp.hp_cur < charHp.hp_max)
		{
			charHp.hp_cur += Utility.villageHeal;

			if (tr.gameObject.activeInHierarchy)
				GameMain.inst.effectsData.Effect_VillageHeal(hex.transform.position, Utility.villageHeal);

			if (charHp.hp_cur > charHp.hp_max)
				charHp.hp_cur = charHp.hp_max;
		}

	}

	public IEnumerator Move(List<Hex> somePath)
	{
		if (somePath != null)
		{
			List<Hex> path = new List<Hex>(somePath);
			path.RemoveAt(0);
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

		if(charMovement.movePoints_cur == 0)
			GameMain.inst.fog.Update_Fog();
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

		GameMain.inst.fog.Update_Fog();
	}

	public void Replace(Hex replaceTo)
	{
		hex.character = null;
		hex = replaceTo;
		replaceTo.character = this;

		tr.position = replaceTo.transform.position;

		GameMain.inst.fog.Update_Fog();

		if (Utility.IsMyCharacter(this))
			GameObject.Find("UI").GetComponent<Ingame_Input>().SelectHex(hex);
	}

	public void StatsUp()
	{
		charHp.hp_max += 5;
		charHp.hp_cur = charHp.hp_max;
		charDef.magic_resistance += 0.1f;
		charExp.exp_cur = 0;
		charExp.exp_max += 5;
		charMovement.movePoints_max++;
		for (int i = 0; i < charAttacks.Count; i++)
		{
			Utility.char_Attack value = charAttacks[i];
			value.attackCount++;
			value.attackDmg_base++;
			charAttacks[i] = value;
		}
	}

	public abstract IEnumerator AttackAnimation(Hex target, int attackId);

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
		if(item == null) return;
		item.Item_OnRemove(this);

		charItem = null;
		if (tr != null) tr.Find("Item").gameObject.SetActive(false);
		hex.Add_Item(item);
	}
	public void Item_Use()
	{
		if(charItem == null) return;
		if(charItem.itemActive == null) return;
		
		charItem.itemActive.Buff_Activate(this);
	}
	public void Item_Remove()
	{
		charItem = null;
		if (tr != null) tr.Find("Item").gameObject.SetActive(false);
	}

	public void Add_Exp(int expToAdd)
	{
		charExp.exp_cur += expToAdd;
	}

	public void RecieveDmg(int dmgToRecieve)
	{
		charHp.hp_cur -= dmgToRecieve;

		if (tr.gameObject.activeInHierarchy)
			GameMain.inst.effectsData.Effect_Damage(hex.transform.position, dmgToRecieve);
	}

	public void RecieveHeal(int amount)
	{
		charHp.hp_cur += amount;

		if(charHp.hp_cur > charHp.hp_max)
			charHp.hp_cur = charHp.hp_max;

		if (tr.gameObject.activeInHierarchy)
			GameMain.inst.effectsData.Effect_VillageHeal(hex.transform.position, amount);
	}
}
