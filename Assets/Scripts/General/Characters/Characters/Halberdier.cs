using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Halberdier : Character
{
    public Halberdier(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/Knight_2");
		charName = "Halberdier";
		charId = 6;
		charCost = 44;

		charType = CharVars.char_Type.day;
		charHp = new CharVars.char_Hp(72);
		charExp = new CharVars.char_Exp(99);

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.0f;
		charDef.pierce_resistance = 0.4f;
		charDef.impact_resistance = 0.0f;
		charDef.magic_resistance = 0.2f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 5;

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Pierce;
		char_Attack.attackCount = 3;
		char_Attack.attackDmg_base = 15;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);
	}
}
