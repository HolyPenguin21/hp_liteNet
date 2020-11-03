﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelArcher : Character
{
    public SkelArcher(Transform tr, Player owner, bool isHero)
    {
        base.tr = tr;
        base.owner = owner;

        if (isHero)
        {
            base.heroCharacter = true;
        }
        else
        {
            if (tr != null)
                tr.Find("Hero").gameObject.SetActive(false);
        }

        // Item icon
        if (tr != null) tr.Find("Item").gameObject.SetActive(false);

        charImage = Resources.Load<Sprite>("Images/SkelArcher");
        charName = "Skeleton Archer";
        charId = 16;
        charCost = 14;

        charType = Utility.char_Type.night;

        charHp.hp_max = 31;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.4f;
        charDef.pierce_resistance = 0.6f;
        charDef.impact_resistance = -0.2f;
        charDef.magic_resistance = -0.5f;

        charExp.exp_cur = 0;
        charExp.exp_max = 99;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 5;
        lookRange = 5;

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack char_Attack = default(Utility.char_Attack);
        char_Attack.attackType = Utility.char_attackType.Melee;
        char_Attack.attackDmgType = Utility.char_attackDmgType.Impact;
        char_Attack.attackCount = 2;
        char_Attack.attackDmg_base = 3;
        char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
        charAttacks.Add(char_Attack);

        Utility.char_Attack char_Attack2 = default(Utility.char_Attack);
        char_Attack2.attackType = Utility.char_attackType.Ranged;
        char_Attack2.attackDmgType = Utility.char_attackDmgType.Pierce;
        char_Attack2.attackCount = 3;
        char_Attack2.attackDmg_base = 6;
        char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
        charAttacks.Add(char_Attack2);
    }
}
