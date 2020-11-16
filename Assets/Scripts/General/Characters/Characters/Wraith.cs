using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wraith : Character
{
    public Wraith(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/Wraith");
        charName = "Wraith";
        charId = 23;
        charCost = 38;

        charType = CharVars.char_Type.night;
        charHp = new CharVars.char_Hp(25);
        charExp = new CharVars.char_Exp(99);

        charDef.dodgeChance = 30;
        charDef.blade_resistance = 0.5f;
        charDef.pierce_resistance = 0.5f;
        charDef.impact_resistance = 0.5f;
        charDef.magic_resistance = -0.1f;

        charMovement.moveType = CharVars.char_moveType.ground;
        charMovement.movePoints_max = 4;

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack attack1 = new CharVars.char_Attack();
        attack1.attackType = CharVars.char_attackType.Melee;
        attack1.attackDmgType = CharVars.char_attackDmgType.Magic;
        attack1.attackCount = 4;
        attack1.attackDmg_base = 6;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        attack1.attackBuff = new ABuff_DrainLife();
        charAttacks.Add(attack1);

        CharVars.char_Attack attack2 = new CharVars.char_Attack();
        attack2.attackType = CharVars.char_attackType.Ranged;
        attack2.attackDmgType = CharVars.char_attackDmgType.Magic;
        attack2.attackCount = 3;
        attack2.attackDmg_base = 4;
        attack2.attackDmg_cur = attack2.attackDmg_base;
        charAttacks.Add(attack2);
    }
}
