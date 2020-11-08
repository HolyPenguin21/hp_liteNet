using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalGuard : Character
{
	public RoyalGuard(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/Knight");
		charName = "RoyalGuard";
		charId = 5;
		charCost = 43;

		charType = CharVars.char_Type.day;
		charHp = new CharVars.char_Hp(74);
		charExp = new CharVars.char_Exp(99);

		charDef.dodgeChance = 10;
		charDef.blade_resistance = 0.2f;
		charDef.pierce_resistance = 0.0f;
		charDef.impact_resistance = 0.2f;
		charDef.magic_resistance = 0.2f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 6;

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Blade;
		char_Attack.attackCount = 4;
		char_Attack.attackDmg_base = 11;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);
	}
}
