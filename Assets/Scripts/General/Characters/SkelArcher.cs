using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelArcher : Character
{
    public SkelArcher(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/SkelArcher");
        charName = "Skel Archer";
        charId = 12;
        charCost = 15;

        charType = Utility.char_Type.night;

        charHp.hp_max = 18;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 0;
        charDef.slash_resistance = 0.0f;
        charDef.pierce_resistance = 0.4f;
        charDef.magic_resistance = 0.0f;

        charExp.exp_cur = 0;
        charExp.exp_max = 15;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 5;
        base.lookRange = 4;

        // Upgrades
        //upgradeList.Add(7);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack char_Attack = default(Utility.char_Attack);
        char_Attack.attackType = Utility.char_attackType.melee;
        char_Attack.attackDmgType = Utility.char_attackDmgType.slash;
        char_Attack.attackCount = 1;
        char_Attack.attackDmg_base = 3;
        char_Attack.attackDmg_cur = char_Attack.attackDmg_base;
        charAttacks.Add(char_Attack);

        Utility.char_Attack char_Attack2 = default(Utility.char_Attack);
        char_Attack2.attackType = Utility.char_attackType.ranged;
        char_Attack2.attackDmgType = Utility.char_attackDmgType.pierce;
        char_Attack2.attackCount = 3;
        char_Attack2.attackDmg_base = 4;
        char_Attack2.attackDmg_cur = char_Attack2.attackDmg_base;
        charAttacks.Add(char_Attack2);
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
