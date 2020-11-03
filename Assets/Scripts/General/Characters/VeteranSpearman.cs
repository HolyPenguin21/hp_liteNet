using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteranSpearman : Character
{
	public VeteranSpearman(Transform tr, Player owner, bool isHero)
	{
		base.tr = tr;
		base.owner = owner;

		if (isHero)
		{
			base.heroCharacter = true;
		}
		else
		{
			if (tr != null)
				tr.Find("Hero").gameObject.SetActive(false);
		}

		// Item icon
		if (tr != null) tr.Find("Item").gameObject.SetActive(false);

		charImage = Resources.Load<Sprite>("Images/VeteranSpearman");
		charName = "Veteran Spearman";
		charId = 2;
		charCost = 30;

		charType = Utility.char_Type.day;

		charHp.hp_max = 30;
		charHp.hp_cur = charHp.hp_max;

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.2f;
		charDef.pierce_resistance = 0.2f;
		charDef.impact_resistance = 0.0f;
		charDef.magic_resistance = 0.0f;

		charExp.exp_cur = 0;
		charExp.exp_max = 50;

		charMovement.moveType = Utility.char_moveType.ground;
		charMovement.movePoints_max = 5;
		base.lookRange = 5;

		charAttacks = new List<Utility.char_Attack>();
		Utility.char_Attack char_Attack = default(Utility.char_Attack);
		char_Attack.attackType = Utility.char_attackType.Melee;
		char_Attack.attackDmgType = Utility.char_attackDmgType.Pierce;
		char_Attack.attackCount = 3;
		char_Attack.attackDmg_base = 4;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);

		Utility.char_Attack char_Attack2 = default(Utility.char_Attack);
		char_Attack2.attackType = Utility.char_attackType.Ranged;
		char_Attack2.attackDmgType = Utility.char_attackDmgType.Pierce;
		char_Attack2.attackCount = 2;
		char_Attack2.attackDmg_base = 4;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		charAttacks.Add(char_Attack2);
	}
}
