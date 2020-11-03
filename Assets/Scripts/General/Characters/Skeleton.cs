using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Character
{
    public Skeleton(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/Skeleton");
        charName = "Skeleton";
        charId = 12;
        charCost = 15;

        charType = Utility.char_Type.night;

        charHp.hp_max = 34;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 0;
        charDef.blade_resistance = 0.4f;
        charDef.pierce_resistance = 0.6f;
        charDef.impact_resistance = -0.2f;
        charDef.magic_resistance = -0.5f;

        charExp.exp_cur = 0;
        charExp.exp_max = 17;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 5;
        lookRange = 5;

        upgradeList.Add(13);
        upgradeList.Add(25);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.Melee;
        attack1.attackDmgType = Utility.char_attackDmgType.Blade;
        attack1.attackCount = 3;
        attack1.attackDmg_base = 7;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);
    }
}
