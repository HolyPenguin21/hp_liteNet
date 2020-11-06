using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Character
{
	public Rogue(Transform tr, Player owner, bool isHero)
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

		charImage = Resources.Load<Sprite>("Images/Rogue");
		charName = "Rogue";
		charId = 11;
		charCost = 24;

		charType = Utility.char_Type.night;

		charHp.hp_max = 40;
		charHp.hp_cur = charHp.hp_max;

		charDef.dodgeChance = 20;
		charDef.blade_resistance = -0.3f;
		charDef.pierce_resistance = -0.2f;
		charDef.impact_resistance = -0.2f;
		charDef.magic_resistance = 0.2f;

		charExp.exp_cur = 0;
		charExp.exp_max = 99;

		charMovement.moveType = Utility.char_moveType.ground;
		charMovement.movePoints_max = 6;
		base.lookRange = 6;

		//upgradeList.Add(8);

		charAttacks = new List<Utility.char_Attack>();
		Utility.char_Attack char_Attack = default(Utility.char_Attack);
		char_Attack.attackType = Utility.char_attackType.Melee;
		char_Attack.attackDmgType = Utility.char_attackDmgType.Blade;
		char_Attack.attackCount = 3;
		char_Attack.attackDmg_base = 6;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);

		Utility.char_Attack char_Attack2 = default(Utility.char_Attack);
		char_Attack2.attackType = Utility.char_attackType.Ranged;
		char_Attack2.attackDmgType = Utility.char_attackDmgType.Blade;
		char_Attack2.attackCount = 3;
		char_Attack2.attackDmg_base = 4;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		charAttacks.Add(char_Attack2);
	}
}
