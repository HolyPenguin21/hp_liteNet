using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABuff_Charge : ABuff
{
    public ABuff_Charge()
    {
        base.buffId = 3;
        base.buffName = "Charge - Not done";
        base.buffDescription = "Character will double damage on attack.";
    }
}
