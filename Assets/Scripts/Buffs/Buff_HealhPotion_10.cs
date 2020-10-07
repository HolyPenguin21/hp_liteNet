using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_HealhPotion_10 : Buff
{
    public Buff_HealhPotion_10 ()
    {
        base.buffId = 1;
        base.buffName = "Heals for 10 HP";
        base.buffDescription = "Heals character for 10 HP.";
        base.buffType = Utility.buff_Type.onEquip;
    }

    public override void Buff_Activate(Character character)
    {
        character.charHp.hp_cur += 10;

        if(character.charHp.hp_cur > character.charHp.hp_max)
            character.charHp.hp_cur = character.charHp.hp_max;
    }

    public override void Buff_Remove(Character character)
    {

    }
}
