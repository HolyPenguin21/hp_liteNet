using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonFireEmber : Spell
{
    public SummonFireEmber()
    {
        spellId = 8;
        spellName = "Summon fire ember";
        description = "Creates fire ember character in target hex.";
        spellArea = Utility.spell_Area.single;
        cooldown_max = 4;
        spellCastRange = 3;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_Flame(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellSummon(casterHex, hex, 24);
    }
}
