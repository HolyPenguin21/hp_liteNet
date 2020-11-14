using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Berserk : Buff
{
    public Buff_Berserk()
    {
        base.buffId = 5;
        base.buffName = "-Berserk";
        base.buffDescription = "Character`s attack damage is increasing when he lose health.";

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
