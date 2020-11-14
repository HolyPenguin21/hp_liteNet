using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Defensive : Buff
{
    public Buff_Defensive()
    {
        base.buffId = 6;
        base.buffName = "Defensive";
        base.buffDescription = "Character will recieve less damage than should.";

        base.buffType = Utility.buff_Type.onEquip;
    }

    public override IEnumerator Buff_Activate(Character character)
    {
        // Both
        // Put some effect here to represent on both Server and Client

        // Server
        if (!Utility.IsServer()) yield break;

        //yield return GameMain.inst.Server_ReceivePoisonDmg(character.hex);
    }

    public override IEnumerator Buff_Remove(Character character)
    {
        yield return null;
    }
}
