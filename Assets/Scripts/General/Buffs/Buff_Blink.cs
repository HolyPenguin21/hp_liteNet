using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Blink : Buff
{
    public Buff_Blink()
    {
        base.buffId = 3;
        base.buffName = "Blink";
        base.buffDescription = "Character will change its position to target hex on use";
        base.buffType = Utility.buff_Type.onEquip;
    }

    public override void Buff_Activate(Character character)
    {
        Ingame_Input ingameInput = GameObject.Find("UI").GetComponent<Ingame_Input>();

        if (Utility.IsMyCharacter(character))
        {
            ingameInput.castingSpell = true;
            ingameInput.castItemSpell = true;
            ingameInput.spell_Active = new Blink(3);

            ingameInput.mouseOverUI = false;
        }
    }

    public override void Buff_Remove(Character character)
    {

    }
}
