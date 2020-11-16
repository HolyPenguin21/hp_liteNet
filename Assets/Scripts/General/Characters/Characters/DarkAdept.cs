using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkAdept : Character
{
    public DarkAdept(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/DarkFigure");
        charName = "Dark Adept";
        charId = 21;
        charCost = 16;

        charType = CharVars.char_Type.night;
        charHp = new CharVars.char_Hp(37); // 28
        charExp = new CharVars.char_Exp(24);

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.0f;
        charDef.pierce_resistance = 0.0f;
        charDef.impact_resistance = 0.0f;
        charDef.magic_resistance = 0.2f;

        charMovement.moveType = CharVars.char_moveType.ground;
        charMovement.movePoints_max = 5;

        upgradeList.Add(22);

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack attack1 = new CharVars.char_Attack();
        attack1.attackType = CharVars.char_attackType.Melee;
        attack1.attackDmgType = CharVars.char_attackDmgType.Impact;
        attack1.attackCount = 1;
        attack1.attackDmg_base = 3;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);

        CharVars.char_Attack attack2 = new CharVars.char_Attack();
        attack2.attackType = CharVars.char_attackType.Ranged;
        attack2.attackDmgType = CharVars.char_attackDmgType.Magic;
        attack2.attackCount = 2;
        attack2.attackDmg_base = 10;
        attack2.attackDmg_cur = attack2.attackDmg_base;
        attack2.attackBuff = new ABuff_Marksman();
        charAttacks.Add(attack2);
    }
}
