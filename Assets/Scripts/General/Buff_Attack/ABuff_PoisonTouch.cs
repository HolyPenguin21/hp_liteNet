using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABuff_PoisonTouch : ABuff
{
    public ABuff_PoisonTouch()
    {
        base.buffId = 2;
        base.buffName = "Poison Touch";
        base.buffDescription = "Character will poison its target on attack.";
    }
}
