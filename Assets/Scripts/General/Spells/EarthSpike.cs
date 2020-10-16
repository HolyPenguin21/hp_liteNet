using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSpike : Spell
{
    private int dmgValue;

    public EarthSpike(int dmgValue)
    {
        spellId = 2;
        spellName = "Earth spikes";
        spellArea = Utility.spell_Area.circle;
        cooldown_max = 3;
        spellCastRange = 3;

        this.dmgValue = dmgValue;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_EarthSpike(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellDamage(casterHex, hex, dmgValue);
    }
}
