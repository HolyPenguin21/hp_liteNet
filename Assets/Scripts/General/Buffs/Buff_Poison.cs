using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Poison : Buff
{
    public Buff_Poison()
    {
        base.buffId = 3;
        base.buffName = "-Poisoned";
        base.buffDescription = "Character is poisoned and will lose hp on turn.";

        base.buffType = Utility.buff_Type.onTurn;
    }

    public override IEnumerator Buff_Activate(Character character)
    {
        // Both
        // Put some effect here to represent on both Server and Client

        // Server
        if (!Utility.IsServer()) yield break;

        yield return GameMain.inst.Server_ReceivePoisonDmg(character.hex);
    }

    public override IEnumerator Buff_Remove(Character character)
    {
        yield return null;
    }
}
