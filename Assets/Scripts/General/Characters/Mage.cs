using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Character
{
    public Mage(Transform tr, Player owner, bool isHero)
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

		charImage = Resources.Load<Sprite>("Images/HumMage");
		charName = "Mage";
		charId = 10;
		charCost = 20;

		charType = Utility.char_Type.day;

		charHp.hp_max = 24;
		charHp.hp_cur = charHp.hp_max;

		charDef.dodgeChance = 5;
		charDef.blade_resistance = 0.0f;
		charDef.pierce_resistance = 0.0f;
		charDef.impact_resistance = 0.0f;
		charDef.magic_resistance = 0.2f;

		charExp.exp_cur = 0;
		charExp.exp_max = 27;

		charMovement.moveType = Utility.char_moveType.ground;
		charMovement.movePoints_max = 5;
		base.lookRange = 5;

		// upgradeList.Add(2);

		charAttacks = new List<Utility.char_Attack>();
		Utility.char_Attack char_Attack = default(Utility.char_Attack);
		char_Attack.attackType = Utility.char_attackType.Melee;
		char_Attack.attackDmgType = Utility.char_attackDmgType.Impact;
		char_Attack.attackCount = 1;
		char_Attack.attackDmg_base = 5;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		char_Attack.attackBuff = new ABuff_DrainLife();
		charAttacks.Add(char_Attack);

		Utility.char_Attack char_Attack2 = default(Utility.char_Attack);
		char_Attack2.attackType = Utility.char_attackType.Ranged;
		char_Attack2.attackDmgType = Utility.char_attackDmgType.Magic;
		char_Attack2.attackCount = 3;
		char_Attack2.attackDmg_base = 7;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		char_Attack2.attackBuff = new ABuff_PoisonTouch();
		charAttacks.Add(char_Attack2);

		//charSpell_1 = new Flame(8);
		//charSpell_1 = new Heal(4);
		//charSpell_1 = new SummonBat();
		//charSpell_1 = new EarthSpike(6);
		//charSpell_1 = new SummonZombie();
		//charSpell_1 = new Blink(3);
		charSpell_1 = new SummonFireEmber();
		charSpell_2 = new MassHeal(3);
	}
}
