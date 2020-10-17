using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : Spell
{
    public Blink(int maxRange)
    {
        spellId = 5;
        spellName = "Heal";
        spellArea = Utility.spell_Area.single;
        cooldown_max = 3;
        spellCastRange = maxRange;
    }

    public override void Use(Vector3 pos)
    {
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex targetHex)
    {
        yield return GameMain.inst.Server_Blink(casterHex, targetHex);
    }
}
