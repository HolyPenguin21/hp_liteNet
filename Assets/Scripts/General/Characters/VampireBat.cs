using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBat : Character
{
    public VampireBat(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/Bat");
        charName = "Vampire Bat";
        charId = 18;
        charCost = 13;

        charType = Utility.char_Type.night;

        charHp.hp_max = 16;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 20;
        charDef.blade_resistance = 0.0f;
        charDef.pierce_resistance = 0.0f;
        charDef.impact_resistance = -0.2f;
        charDef.magic_resistance = 0.2f;

        charExp.exp_cur = 0;
        charExp.exp_max = 11;

        charMovement.moveType = Utility.char_moveType.air;
        charMovement.movePoints_max = 8;
        base.lookRange = 8;

        upgradeList.Add(19);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.Melee;
        attack1.attackDmgType = Utility.char_attackDmgType.Blade;
        attack1.attackCount = 2;
        attack1.attackDmg_base = 4;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);

        charBuffs.Add(new Buff_DrainLife());
    }
}
