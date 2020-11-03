using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterBowman : Character
{
	public MasterBowman(Transform tr, Player owner, bool isHero)
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

		charImage = Resources.Load<Sprite>("Images/HumArcher3");
		charName = "Master Bowman";
		charId = 9;
		charCost = 23;

		charType = Utility.char_Type.day;

		charHp.hp_max = 21;
		charHp.hp_cur = charHp.hp_max;

		charDef.dodgeChance = 10;
		charDef.blade_resistance = 0.0f;
		charDef.pierce_resistance = 0.0f;
		charDef.impact_resistance = 0.0f;
		charDef.magic_resistance = 0.2f;

		charExp.exp_cur = 0;
		charExp.exp_max = 50;

		charMovement.moveType = Utility.char_moveType.ground;
		charMovement.movePoints_max = 5;
		base.lookRange = 5;

		charAttacks = new List<Utility.char_Attack>();
		Utility.char_Attack char_Attack = default(Utility.char_Attack);
		char_Attack.attackType = Utility.char_attackType.Melee;
		char_Attack.attackDmgType = Utility.char_attackDmgType.Blade;
		char_Attack.attackCount = 1;
		char_Attack.attackDmg_base = 4;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);

		Utility.char_Attack char_Attack2 = default(Utility.char_Attack);
		char_Attack2.attackType = Utility.char_attackType.Ranged;
		char_Attack2.attackDmgType = Utility.char_attackDmgType.Pierce;
		char_Attack2.attackCount = 4;
		char_Attack2.attackDmg_base = 4;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		charAttacks.Add(char_Attack2);
	}
}
