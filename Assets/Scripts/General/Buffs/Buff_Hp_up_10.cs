using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Hp_up_10 : Buff
{
    public Buff_Hp_up_10 ()
    {
        base.buffId = 1;
        base.buffName = "HP bonus : + 10";
        base.buffDescription = "Adds 10 HP to character health pull.";
        base.buffType = Utility.buff_Type.onEquip;
    }

    public override void Buff_Activate(Character character)
    {
        character.charHp.hp_cur += 10;
        character.charHp.hp_max += 10;
    }

    public override void Buff_Remove(Character character)
    {
        character.charHp.hp_cur -= 10;
        character.charHp.hp_max -= 10;

        if(character.charHp.hp_cur <= 0)
            character.charHp.hp_cur = 1;
    }
}
