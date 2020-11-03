using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_PoisonTouch : Buff
{
    public Buff_PoisonTouch()
    {
        base.buffId = 5;
        base.buffName = "Poison Touch";
        base.buffDescription = "Character will poison its target on attack.";

        base.buffType = Utility.buff_Type.onAttack;
    }

    public override IEnumerator Buff_Activate(Character character)
    {
        // Both
        // Put some effect here to represent on both Server and Client

        // Server
        if (!Utility.IsServer()) yield break;
        Debug.Log("asd");
        //yield return GameMain.inst.Server_ReceivePoisonDmg(character.hex, Utility.villageHeal);
    }

    public override IEnumerator Buff_Remove(Character character)
    {
        yield return null;
    }
}
