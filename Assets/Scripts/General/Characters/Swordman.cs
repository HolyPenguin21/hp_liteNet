using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordman : Character
{
    public Swordman(Transform tr, Player owner, bool isHero)
    {
        base.tr = tr;
        base.owner = owner;
        
        if(isHero) 
            base.heroCharacter = true;
        else
        {
            if(tr != null)
                tr.Find("Hero").gameObject.SetActive(false);
        }

        // Item icon
        if (tr != null) tr.Find("Item").gameObject.SetActive(false);

        charImage = Resources.Load<Sprite>("Images/Swordman");
        charName = "Swordman";
        charId = 4;
        charCost = 15;

        charType = Utility.char_Type.day;

        charHp.hp_max = 20;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 15;
        charDef.slash_resistance = 0.2f;
        charDef.pierce_resistance = 0.1f;
        charDef.magic_resistance = 0.0f;

        charExp.exp_cur = 0;
        charExp.exp_max = 15;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 4;
        base.lookRange = 3;

        // Upgrades
        upgradeList.Add(5);
        upgradeList.Add(6);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.melee;
        attack1.attackDmgType = Utility.char_attackDmgType.slash;
        attack1.attackCount = 2;
        attack1.attackDmg_base = 4;
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
