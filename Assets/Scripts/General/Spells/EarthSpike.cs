using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSpike : Spell
{
    public EarthSpike()
    {
        spellId = 2;
        spellName = "Earth spikes";
        spellDmg = 10;
        spellArea = Utility.spell_Area.circle;
        cooldown_max = 3;
        spellCastRange = 3;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_EarthSpike(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellDamage(casterHex, hex, spellDmg);
    }
}
