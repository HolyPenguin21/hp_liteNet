﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necrophage : Character
{
    public Necrophage(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/ZombieArmored");
        charName = "Necrophage";
        charId = 15;
        charCost = 27;

        charType = Utility.char_Type.night;

        charHp.hp_max = 47;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.1f;
        charDef.pierce_resistance = 0.3f;
        charDef.impact_resistance = 0.0f;
        charDef.magic_resistance = 0.2f;

        charExp.exp_cur = 0;
        charExp.exp_max = 99;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 5;
        base.lookRange = 5;

        //upgradeList.Add(7);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.Melee;
        attack1.attackDmgType = Utility.char_attackDmgType.Blade;
        attack1.attackCount = 3;
        attack1.attackDmg_base = 7;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);

        charBuffs.Add(new Buff_PoisonTouch());
    }
}