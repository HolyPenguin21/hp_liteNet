using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBat : Character
{
    public VampireBat(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/Bat");
        charName = "Vampire Bat";
        charId = 18;
        charCost = 13;

        charType = CharVars.char_Type.night;
        charHp = new CharVars.char_Hp(16);
        charExp = new CharVars.char_Exp(11);

        charDef.dodgeChance = 20;
        charDef.blade_resistance = 0.0f;
        charDef.pierce_resistance = 0.0f;
        charDef.impact_resistance = -0.2f;
        charDef.magic_resistance = 0.2f;

        charMovement.moveType = CharVars.char_moveType.air;
        charMovement.movePoints_max = 8;

        upgradeList.Add(19);

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack attack1 = new CharVars.char_Attack();
        attack1.attackType = CharVars.char_attackType.Melee;
        attack1.attackDmgType = CharVars.char_attackDmgType.Blade;
        attack1.attackCount = 2;
        attack1.attackDmg_base = 4;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        attack1.attackBuff = new ABuff_DrainLife();
        charAttacks.Add(attack1);
    }
}
