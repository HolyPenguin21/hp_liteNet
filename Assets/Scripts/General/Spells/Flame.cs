using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : Spell
{
    private int dmgValue;

    public Flame(int dmgValue)
    {
        spellId = 1;
        spellName = "Flame";
        spellArea = Utility.spell_Area.single;
        cooldown_max = 2;
        spellCastRange = 2;

        this.dmgValue = dmgValue;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_Flame(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellDamage(casterHex, hex, dmgValue);
    }
}
