using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Character
{
    public Mage(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/HumMage");
		charName = "Mage";
		charId = 10;
		charCost = 20;

		charType = CharVars.char_Type.day;
		charHp = new CharVars.char_Hp(32); // 24
		charExp = new CharVars.char_Exp(27);

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.0f;
		charDef.pierce_resistance = 0.0f;
		charDef.impact_resistance = 0.0f;
		charDef.magic_resistance = 0.2f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 5;

		upgradeList.Add(34);

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Impact;
		char_Attack.attackCount = 1;
		char_Attack.attackDmg_base = 5;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);

		CharVars.char_Attack char_Attack2 = default(CharVars.char_Attack);
		char_Attack2.attackType = CharVars.char_attackType.Ranged;
		char_Attack2.attackDmgType = CharVars.char_attackDmgType.Magic;
		char_Attack2.attackCount = 3;
		char_Attack2.attackDmg_base = 7;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		char_Attack2.attackBuff = new ABuff_Marksman();
		charAttacks.Add(char_Attack2);

		//charSpell_1 = new Flame(8);
		//charSpell_1 = new Heal(4);
		//charSpell_1 = new SummonBat();
		//charSpell_1 = new EarthSpike(6);
		//charSpell_1 = new SummonZombie();
		//charSpell_1 = new Blink(3);
		//charSpell_1 = new SummonFireEmber();
		//charSpell_2 = new MassHeal(3);
	}
}
