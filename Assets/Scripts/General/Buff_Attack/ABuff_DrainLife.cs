using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABuff_DrainLife : ABuff
{
    public ABuff_DrainLife()
    {
        base.buffId = 1;
        base.buffName = "Drain Life";
        base.buffDescription = "Character will drain its target health on attack.";
    }
}
