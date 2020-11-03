using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSorcerer : Character
{
    public DarkSorcerer(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/DarkFigure2");
        charName = "Dark Sorcerer";
        charId = 22;
        charCost = 32;

        charType = Utility.char_Type.night;

        charHp.hp_max = 48;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 5;
        charDef.blade_resistance = 0.0f;
        charDef.pierce_resistance = 0.0f;
        charDef.impact_resistance = 0.0f;
        charDef.magic_resistance = 0.2f;

        charExp.exp_cur = 0;
        charExp.exp_max = 45;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 5;
        lookRange = 5;

        upgradeList.Add(17);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.Melee;
        attack1.attackDmgType = Utility.char_attackDmgType.Impact;
        attack1.attackCount = 3;
        attack1.attackDmg_base = 4;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);

        Utility.char_Attack attack2 = new Utility.char_Attack();
        attack2.attackType = Utility.char_attackType.Ranged;
        attack2.attackDmgType = Utility.char_attackDmgType.Magic;
        attack2.attackCount = 2;
        attack2.attackDmg_base = 13;
        attack2.attackDmg_cur = attack2.attackDmg_base;
        charAttacks.Add(attack2);

        charSpell_1 = new Heal(6);
        charSpell_2 = new SummonBat();
    }
}
