using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Hp_up_5 : Buff
{
    public Buff_Hp_up_5 ()
    {
        base.buffId = 1;
        base.buffName = "HP bonus : + 5";
        base.buffDescription = "Adds 5 HP to character health pull.";

        base.buffType = Utility.buff_Type.onEquip;
    }

    public override IEnumerator Buff_Activate(Character character)
    {
        // Both
        // Put some effect here to represent on both Server and Client

        // Server
        if (!Utility.IsServer()) yield break;

        yield return GameMain.inst.Server_ChangeMaxHealth(character.hex, 5);
    }

    public override IEnumerator Buff_Remove(Character character)
    {
        yield return GameMain.inst.Server_ChangeMaxHealth(character.hex, -5);
    }
}
