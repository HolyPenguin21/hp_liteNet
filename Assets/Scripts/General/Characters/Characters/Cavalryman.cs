using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cavalryman : Character
{
	public Cavalryman(Transform tr, Player owner, bool isHero)
	{
		Init(tr, owner, isHero);

		charImage = Resources.Load<Sprite>("Images/Сavalryman");
		charName = "Сavalryman";
		charId = 26;
		charCost = 17;

		charType = CharVars.char_Type.day;
		charHp = new CharVars.char_Hp(45); // 34
		charExp = new CharVars.char_Exp(20);

		charDef.dodgeChance = 0;
		charDef.blade_resistance = 0.3f;
		charDef.pierce_resistance = -0.2f;
		charDef.impact_resistance = 0.4f;
		charDef.magic_resistance = 0.2f;

		charMovement.moveType = CharVars.char_moveType.ground;
		charMovement.movePoints_max = 8;

		upgradeList.Add(27);

		charAttacks = new List<CharVars.char_Attack>();
		CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
		char_Attack.attackType = CharVars.char_attackType.Melee;
		char_Attack.attackDmgType = CharVars.char_attackDmgType.Blade;
		char_Attack.attackCount = 3;
		char_Attack.attackDmg_base = 6;
		char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
		charAttacks.Add(char_Attack);
	}
}
