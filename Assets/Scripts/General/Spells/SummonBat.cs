using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBat : Spell
{
    public SummonBat()
    {
        spellId = 7;
        spellName = "Summon Bat";
        description = "Creates bat character in target hex.";
        spellArea = Utility.spell_Area.single;
        cooldown_max = 6;
        spellCastRange = 1;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_Flame(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellSummon(casterHex, hex, 18);
    }
}
