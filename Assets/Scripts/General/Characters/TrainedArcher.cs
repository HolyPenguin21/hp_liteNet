﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainedArcher : Character
{
	public TrainedArcher(Transform tr, Player owner, bool isHero)
	{
		base.tr = tr;
		base.owner = owner;

		if (isHero)
			base.heroCharacter = true;
		else
		{
			if (tr != null)
				tr.Find("Hero").gameObject.SetActive(false);
		}

		// Item icon
		if (tr != null) tr.Find("Item").gameObject.SetActive(false);

		charImage = Resources.Load<Sprite>("Images/HumArcher2");
		charName = "TrainedArcher";
		charId = 8;
		charCost = 22;

		charType = Utility.char_Type.day;

		charHp.hp_max = 25;
		charHp.hp_cur = charHp.hp_max;

		charDef.dodgeChance = 15;
		charDef.slash_resistance = 0.1f;
		charDef.pierce_resistance = 0.1f;
		charDef.magic_resistance = 0.0f;

		charExp.exp_cur = 0;
		charExp.exp_max = 25;

		charMovement.moveType = Utility.char_moveType.ground;
		charMovement.movePoints_max = 5;
		base.lookRange = 5;

		//upgradeList.Add(2);

		charAttacks = new List<Utility.char_Attack>();
		Utility.char_Attack char_Attack = default(Utility.char_Attack);
		char_Attack.attackType = Utility.char_attackType.melee;
		char_Attack.attackDmgType = Utility.char_attackDmgType.slash;
		char_Attack.attackCount = 2;
		char_Attack.attackDmg_base = 3;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);

		Utility.char_Attack char_Attack2 = default(Utility.char_Attack);
		char_Attack2.attackType = Utility.char_attackType.ranged;
		char_Attack2.attackDmgType = Utility.char_attackDmgType.pierce;
		char_Attack2.attackCount = 3;
		char_Attack2.attackDmg_base = 5;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		charAttacks.Add(char_Attack2);
	}

	public override IEnumerator AttackAnimation(Hex target, int attackId)
	{
		float t2 = 0f;
		Vector3 attackVector = tr.position + (target.transform.position - tr.position) / 2f;
		while (t2 < 1f)
		{
			tr.position = Vector3.Lerp(tr.position, attackVector, t2);
			t2 += Time.deltaTime * attackAnimationSpeed * 2f;
			yield return null;
		}
		t2 = 0f;
		while (t2 < 1f)
		{
			tr.position = Vector3.Lerp(tr.position, hex.transform.position, t2);
			t2 += Time.deltaTime * attackAnimationSpeed;
			yield return null;
		}
	}
}
