using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pikeman : Character
{
    public Pikeman(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/Pikeman");
        charName = "Pikeman";
        charId = 3;
        charCost = 25;

        charType = CharVars.char_Type.day;
        charHp = new CharVars.char_Hp(73); // 55
        charExp = new CharVars.char_Exp(32);

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.0f;
        charDef.pierce_resistance = 0.4f;
        charDef.impact_resistance = 0.0f;
        charDef.magic_resistance = 0.2f;

        charMovement.moveType = CharVars.char_moveType.ground;
        charMovement.movePoints_max = 5;

        upgradeList.Add(6);

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack attack1 = new CharVars.char_Attack();
        attack1.attackType = CharVars.char_attackType.Melee;
        attack1.attackDmgType = CharVars.char_attackDmgType.Pierce;
        attack1.attackCount = 3;
        attack1.attackDmg_base = 10;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);
    }
}
