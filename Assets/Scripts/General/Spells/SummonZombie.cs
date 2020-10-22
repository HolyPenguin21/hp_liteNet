using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonZombie : Spell
{
    public SummonZombie()
    {
        spellId = 6;
        spellName = "Summon zombie";
        description = "Creates zombie character in target hex.";
        spellArea = Utility.spell_Area.single;
        cooldown_max = 5;
        spellCastRange = 3;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_Flame(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellSummon(casterHex, hex, 14);
    }
}
