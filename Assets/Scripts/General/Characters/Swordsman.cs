using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Character
{
    public Swordsman(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/Swordman");
        charName = "Swordsman";
        charId = 4;
        charCost = 25;

        charType = Utility.char_Type.day;

        charHp.hp_max = 55;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 10;
        charDef.blade_resistance = 0.2f;
        charDef.pierce_resistance = 0.0f;
        charDef.impact_resistance = 0.20f;
        charDef.magic_resistance = 0.20f;

        charExp.exp_cur = 0;
        charExp.exp_max = 30;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 5;
        base.lookRange = 5;

        // Upgrades
        upgradeList.Add(5);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.Melee;
        attack1.attackDmgType = Utility.char_attackDmgType.Blade;
        attack1.attackCount = 4;
        attack1.attackDmg_base = 8;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);
    }
}
