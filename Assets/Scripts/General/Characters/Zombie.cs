using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Character
{
    public Zombie(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/Zombie");
        charName = "Zombie";
        charId = 14;
        charCost = 10;

        charType = Utility.char_Type.night;

        charHp.hp_max = 13;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 0;
        charDef.slash_resistance = 0.0f;
        charDef.pierce_resistance = 0.0f;
        charDef.blunt_resistance = 0.2f;
        charDef.magic_resistance = -0.1f;

        charExp.exp_cur = 0;
        charExp.exp_max = 8;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 3;
        base.lookRange = 3;

        upgradeList.Add(15);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.melee;
        attack1.attackDmgType = Utility.char_attackDmgType.slash;
        attack1.attackCount = 2;
        attack1.attackDmg_base = 3;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);
    }

    public override IEnumerator AttackAnimation(Hex target, int attackId)
    {
        // attack move
        float t = 0f;
        Vector3 attackVector = base.tr.position + (target.transform.position - base.tr.position) / 2; // A+(B-A)/2 - vector middle
        while (t < 1f)
        {
            tr.position = Vector3.Lerp(base.tr.position, attackVector, t);
            t += Time.deltaTime * attackAnimationSpeed * 2;
            yield return null;
        }

        // return move
        t = 0f;
        while (t < 1f)
        {
            tr.position = Vector3.Lerp(base.tr.position, hex.transform.position, t);
            t += Time.deltaTime * attackAnimationSpeed;
            yield return null;
        }
    }
}
