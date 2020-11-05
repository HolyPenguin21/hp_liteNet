﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wraith : Character
{
    public Wraith(Transform tr, Player owner, bool isHero)
    {
        base.tr = tr;
        base.owner = owner;

        if (isHero)
            base.heroCharacter = true;
        else
        {
            if (tr != null)
                tr.Find("Hero").gameObject.SetActive(false);
        }

        // Item icon
        if (tr != null) tr.Find("Item").gameObject.SetActive(false);

        charImage = Resources.Load<Sprite>("Images/DarkFigure4");
        charName = "Wraith";
        charId = 23;
        charCost = 38;

        charType = Utility.char_Type.night;

        charHp.hp_max = 25;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 40;
        charDef.blade_resistance = 0.5f;
        charDef.pierce_resistance = 0.5f;
        charDef.impact_resistance = 0.5f;
        charDef.magic_resistance = -0.1f;

        charExp.exp_cur = 0;
        charExp.exp_max = 99;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 4;
        base.lookRange = 4;

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.Melee;
        attack1.attackDmgType = Utility.char_attackDmgType.Magic;
        attack1.attackCount = 4;
        attack1.attackDmg_base = 6;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);

        Utility.char_Attack attack2 = new Utility.char_Attack();
        attack2.attackType = Utility.char_attackType.Ranged;
        attack2.attackDmgType = Utility.char_attackDmgType.Magic;
        attack2.attackCount = 3;
        attack2.attackDmg_base = 4;
        attack2.attackDmg_cur = attack2.attackDmg_base;
        charAttacks.Add(attack2);
    }
}