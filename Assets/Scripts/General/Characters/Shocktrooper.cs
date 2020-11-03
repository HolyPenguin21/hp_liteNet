using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shocktrooper : Character
{
	public Shocktrooper(Transform tr, Player owner, bool isHero)
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

		charImage = Resources.Load<Sprite>("Images/Heavyinfantry");
		charName = "Heavy Infantryman";
		charId = 28;
		charCost = 35;

		charType = Utility.char_Type.day;

		charHp.hp_max = 52;
		charHp.hp_cur = charHp.hp_max;

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.5f;
		charDef.pierce_resistance = 0.4f;
		charDef.impact_resistance = 0.1f;
		charDef.magic_resistance = 0.2f;

		charExp.exp_cur = 0;
		charExp.exp_max = 99;

		charMovement.moveType = Utility.char_moveType.ground;
		charMovement.movePoints_max = 4;
		base.lookRange = 4;

		charAttacks = new List<Utility.char_Attack>();
		Utility.char_Attack char_Attack = default(Utility.char_Attack);
		char_Attack.attackType = Utility.char_attackType.Melee;
		char_Attack.attackDmgType = Utility.char_attackDmgType.Impact;
		char_Attack.attackCount = 2;
		char_Attack.attackDmg_base = 18;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);
	}
}
