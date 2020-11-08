using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteranSpearman : Character
{
	public VeteranSpearman(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/VeteranSpearman");
		charName = "Veteran Spearman";
		charId = 2;
		charCost = 30;

		charType = CharVars.char_Type.day;
		charHp = new CharVars.char_Hp(30);
		charExp = new CharVars.char_Exp(24);

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.2f;
		charDef.pierce_resistance = 0.2f;
		charDef.impact_resistance = 0.0f;
		charDef.magic_resistance = 0.0f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 5;

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Pierce;
		char_Attack.attackCount = 3;
		char_Attack.attackDmg_base = 4;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);

		CharVars.char_Attack char_Attack2 = default(CharVars.char_Attack);
		char_Attack2.attackType = CharVars.char_attackType.Ranged;
		char_Attack2.attackDmgType = CharVars.char_attackDmgType.Pierce;
		char_Attack2.attackCount = 2;
		char_Attack2.attackDmg_base = 4;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		charAttacks.Add(char_Attack2);
	}
}
