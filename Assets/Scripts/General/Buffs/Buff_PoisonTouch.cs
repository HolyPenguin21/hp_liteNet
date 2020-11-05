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
        yield return null;
    }

    public override IEnumerator Buff_Remove(Character character)
    {
        yield return null;
    }
}
