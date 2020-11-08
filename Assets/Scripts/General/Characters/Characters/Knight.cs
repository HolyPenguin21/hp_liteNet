using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Character
{
	public Knight(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/MountedKnight");
		charName = "Knight";
		charId = 31;
		charCost = 40;

		charType = CharVars.char_Type.day;
		charHp = new CharVars.char_Hp(58);
		charExp = new CharVars.char_Exp(99);

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.2f;
		charDef.pierce_resistance = -0.2f;
		charDef.impact_resistance = 0.3f;
		charDef.magic_resistance = 0.2f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 8;

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Pierce;
		char_Attack.attackCount = 2;
		char_Attack.attackDmg_base = 14;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		char_Attack.attackBuff = new ABuff_Charge();
		charAttacks.Add(char_Attack);
	}
}
