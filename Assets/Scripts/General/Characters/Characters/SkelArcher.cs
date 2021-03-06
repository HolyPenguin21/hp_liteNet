﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelArcher : Character
{
    public SkelArcher(Transform tr, Player owner, bool isHero)
    {
        Init(tr, owner, isHero);

        charImage = Resources.Load<Sprite>("Images/SkelArcher");
        charName = "Skeleton Archer";
        charId = 16;
        charCost = 14;

        charType = CharVars.char_Type.night;
        charHp = new CharVars.char_Hp(41); // 31
        charExp = new CharVars.char_Exp(17);

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.4f;
        charDef.pierce_resistance = 0.6f;
        charDef.impact_resistance = -0.2f;
        charDef.magic_resistance = -0.5f;

        charMovement.moveType = CharVars.char_moveType.ground;
        charMovement.movePoints_max = 5;

        upgradeList.Add(35);

        charAttacks = new List<CharVars.char_Attack>();
        CharVars.char_Attack char_Attack = default(CharVars.char_Attack);
        char_Attack.attackType = CharVars.char_attackType.Melee;
        char_Attack.attackDmgType = CharVars.char_attackDmgType.Impact;
        char_Attack.attackCount = 2;
        char_Attack.attackDmg_base = 3;
        char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
        charAttacks.Add(char_Attack);

        CharVars.char_Attack char_Attack2 = default(CharVars.char_Attack);
        char_Attack2.attackType = CharVars.char_attackType.Ranged;
        char_Attack2.attackDmgType = CharVars.char_attackDmgType.Pierce;
        char_Attack2.attackCount = 3;
        char_Attack2.attackDmg_base = 6;
        char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
        charAttacks.Add(char_Attack2);
    }
}
