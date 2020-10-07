using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : Spell
{
    public Flame()
    {
        spellId = 1;
        spellName = "Flame";
        spellDmg = 10;
        spellArea = Utility.spell_Area.single;
        cooldown_max = 2;
        spellCastRange = 2;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_Flame(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        //yield return GameMain.inst.Server_SpellDamage(casterHex, hex, spellDmg); // Server is blocked
        yield return null;
    }
}
