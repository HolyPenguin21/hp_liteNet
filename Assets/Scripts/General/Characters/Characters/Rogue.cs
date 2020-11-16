using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Character
{
	public Rogue(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/Rogue");
		charName = "Rogue";
		charId = 11;
		charCost = 24;

		charType = CharVars.char_Type.night;
		charHp = new CharVars.char_Hp(53); // 40
		charExp = new CharVars.char_Exp(99);

		charDef.dodgeChance = 20;
		charDef.blade_resistance = -0.3f;
		charDef.pierce_resistance = -0.2f;
		charDef.impact_resistance = -0.2f;
		charDef.magic_resistance = 0.2f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 6;

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Blade;
		char_Attack.attackCount = 3;
		char_Attack.attackDmg_base = 6;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);

		CharVars.char_Attack char_Attack2 = default(CharVars.char_Attack);
		char_Attack2.attackType = CharVars.char_attackType.Ranged;
		char_Attack2.attackDmgType = CharVars.char_attackDmgType.Blade;
		char_Attack2.attackCount = 3;
		char_Attack2.attackDmg_base = 4;
		char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
		charAttacks.Add(char_Attack2);

		charSpell_1 = new Blink(5);
	}
}
