using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABuff_Marksman : ABuff
{
    public ABuff_Marksman()
    {
        base.buffId = 4;
        base.buffName = "Marksman";
        base.buffDescription = "Character will have better chance to attack.";
    }
}
