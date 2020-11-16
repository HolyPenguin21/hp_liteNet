using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shocktrooper : Character
{
	public Shocktrooper(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/Heavyinfantry");
		charName = "Heavy Infantryman";
		charId = 28;
		charCost = 35;

		charType = CharVars.char_Type.day;
		charHp = new CharVars.char_Hp(69); // 52
		charExp = new CharVars.char_Exp(99);

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.5f;
		charDef.pierce_resistance = 0.4f;
		charDef.impact_resistance = 0.1f;
		charDef.magic_resistance = 0.2f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 4;

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Impact;
		char_Attack.attackCount = 2;
		char_Attack.attackDmg_base = 18;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);
	}
}
