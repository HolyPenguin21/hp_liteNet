using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necrophage : Character
{
    public Necrophage(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/Necrophage");
        charName = "Necrophage";
        charId = 15;
        charCost = 27;

        charType = CharVars.char_Type.night;
        charHp = new CharVars.char_Hp(62); // 47
        charExp = new CharVars.char_Exp(99);

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.1f;
        charDef.pierce_resistance = 0.3f;
        charDef.impact_resistance = 0.0f;
        charDef.magic_resistance = 0.2f;

        charMovement.moveType = CharVars.char_moveType.ground;
        charMovement.movePoints_max = 5;

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack attack1 = new CharVars.char_Attack();
        attack1.attackType = CharVars.char_attackType.Melee;
        attack1.attackDmgType = CharVars.char_attackDmgType.Blade;
        attack1.attackCount = 3;
        attack1.attackDmg_base = 7;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        attack1.attackBuff = new ABuff_PoisonTouch();
        charAttacks.Add(attack1);
    }
}
