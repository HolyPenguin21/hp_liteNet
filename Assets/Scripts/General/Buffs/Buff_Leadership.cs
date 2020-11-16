using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Leadership : Buff
{
    public Buff_Leadership()
    {
        base.buffId = 6;
        base.buffName = "-Leadership";
        base.buffDescription = "Character will increase attack of adjacent units.";

        base.buffType = Utility.buff_Type.onAttack;
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
