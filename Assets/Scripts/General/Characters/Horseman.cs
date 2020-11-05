using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horseman : Character
{
	public Horseman(Transform tr, Player owner, bool isHero)
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

		charImage = Resources.Load<Sprite>("Images/Horseman");
		charName = "Horseman";
		charId = 30;
		charCost = 23;

		charType = Utility.char_Type.day;

		charHp.hp_max = 38;
		charHp.hp_cur = charHp.hp_max;

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.2f;
		charDef.pierce_resistance = -0.2f;
		charDef.impact_resistance = 0.3f;
		charDef.magic_resistance = 0.2f;

		charExp.exp_cur = 0;
		charExp.exp_max = 22;

		charMovement.moveType = Utility.char_moveType.ground;
		charMovement.movePoints_max = 8;
		base.lookRange = 8;

		upgradeList.Add(31);

		charAttacks = new List<Utility.char_Attack>();
		Utility.char_Attack char_Attack = default(Utility.char_Attack);
		char_Attack.attackType = Utility.char_attackType.Melee;
		char_Attack.attackDmgType = Utility.char_attackDmgType.Pierce;
		char_Attack.attackCount = 2;
		char_Attack.attackDmg_base = 9;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);
	}
}
