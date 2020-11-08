using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revenant : Character
{
    public Revenant(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/Skeleton2");
        charName = "Revenant";
        charId = 13;
        charCost = 28;

        charType = CharVars.char_Type.night;
        charHp = new CharVars.char_Hp(47);
        charExp = new CharVars.char_Exp(99);

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.4f;
        charDef.pierce_resistance = 0.6f;
        charDef.impact_resistance = -0.2f;
        charDef.magic_resistance = -0.5f;

        charMovement.moveType = CharVars.char_moveType.ground;
        charMovement.movePoints_max = 5;

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack attack1 = new CharVars.char_Attack();
        attack1.attackType = CharVars.char_attackType.Melee;
        attack1.attackDmgType = CharVars.char_attackDmgType.Blade;
        attack1.attackCount = 4;
        attack1.attackDmg_base = 8;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);
    }
}
