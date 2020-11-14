using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Character
{
    public Swordsman(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/Swordman");
        charName = "Swordsman";
        charId = 4;
        charCost = 25;

        charType = CharVars.char_Type.day;
        charHp = new CharVars.char_Hp(55);
        charExp = new CharVars.char_Exp(30);

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.2f;
        charDef.pierce_resistance = 0.0f;
        charDef.impact_resistance = 0.20f;
        charDef.magic_resistance = 0.20f;

        charMovement.moveType = CharVars.char_moveType.ground;
        charMovement.movePoints_max = 5;

        upgradeList.Add(5);

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
