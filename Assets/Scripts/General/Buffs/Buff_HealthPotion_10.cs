using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_HealthPotion_10 : Buff
{
    public Buff_HealthPotion_10 ()
    {
        base.buffId = 2;
        base.buffName = "Healh potion 10";
        base.buffDescription = "Heals character for 10 HP.";

        base.buffType = Utility.buff_Type.active;
    }

    public override IEnumerator Buff_Activate(Character character)
    {
        character.charHp.hp_cur += 10;
        GameMain.inst.effectsData.Effect_VillageHeal(character.tr.position, 10);

        if (character.charHp.hp_cur > character.charHp.hp_max)
            character.charHp.hp_cur = character.charHp.hp_max;

        yield return null;
    }

    public override IEnumerator Buff_Remove(Character character)
    {
        yield return null;
    }
}
