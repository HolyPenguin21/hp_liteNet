using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkFigure : Character
{
    public DarkFigure(Transform tr, Player owner, bool isHero)
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

        charImage = Resources.Load<Sprite>("Images/DarkFigure");
        charName = "Dark Figure";
        charId = 21;
        charCost = 24;

        charType = Utility.char_Type.night;

        charHp.hp_max = 20;
        charHp.hp_cur = charHp.hp_max;

        charDef.dodgeChance = 5;
        charDef.slash_resistance = 0.0f;
        charDef.pierce_resistance = 0.0f;
        charDef.magic_resistance = 0.3f;

        charExp.exp_cur = 0;
        charExp.exp_max = 10;

        charMovement.moveType = Utility.char_moveType.ground;
        charMovement.movePoints_max = 4;
        lookRange = 4;

        upgradeList.Add(20);
        upgradeList.Add(22);
        upgradeList.Add(23);

        charAttacks = new List<Utility.char_Attack>();
        Utility.char_Attack attack1 = new Utility.char_Attack();
        attack1.attackType = Utility.char_attackType.melee;
        attack1.attackDmgType = Utility.char_attackDmgType.blunt;
        attack1.attackCount = 2;
        attack1.attackDmg_base = 3;
        attack1.attackDmg_cur = attack1.attackDmg_base;
        charAttacks.Add(attack1);

        Utility.char_Attack attack2 = new Utility.char_Attack();
        attack2.attackType = Utility.char_attackType.ranged;
        attack2.attackDmgType = Utility.char_attackDmgType.magic;
        attack2.attackCount = 2;
        attack2.attackDmg_base = 3;
        attack2.attackDmg_cur = attack2.attackDmg_base;
        charAttacks.Add(attack2);

        charSpell_1 = new Heal(4);
        charSpell_2 = new SummonBat();
    }

    public override IEnumerator AttackAnimation(Hex target, int attackId)
    {
        if (attackId == 1)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * attackAnimationSpeed * 4;
                yield return null;
            }

            if (base.tr.gameObject.activeInHierarchy)
                GameMain.inst.effectsData.Effect_Lightning(target.transform.position);

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * attackAnimationSpeed * 0.5f;
                yield return null;
            }
        }
        else
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
}
