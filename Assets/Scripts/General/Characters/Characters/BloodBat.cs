using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBat : Character
{
    public BloodBat(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/Bat");
        charName = "Blood Bat";
        charId = 19;
        charCost = 21;

        charType = CharVars.char_Type.night;
        charHp = new CharVars.char_Hp(36); // 27
        charExp = new CharVars.char_Exp(99);

        charDef.dodgeChance = 20;
        charDef.blade_resistance = 0.0f;
        charDef.pierce_resistance = 0.0f;
        charDef.impact_resistance = -0.2f;
        charDef.magic_resistance = 0.2f;

        charMovement.moveType = CharVars.char_moveType.air;
        charMovement.movePoints_max = 9;

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack attack1 = new CharVars.char_Attack();
        attack1.attackType = CharVars.char_attackType.Melee;
        attack1.attackDmgType = CharVars.char_attackDmgType.Blade;
        attack1.attackCount = 3;
        attack1.attackDmg_base = 5;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        attack1.attackBuff = new ABuff_DrainLife();
        charAttacks.Add(attack1);
    }
}
