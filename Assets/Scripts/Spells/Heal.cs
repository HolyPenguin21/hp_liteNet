using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Spell
{
    public Heal()
    {
        spellId = 4;
        spellName = "Heal";
        spellDmg = 8;
        spellArea = Utility.spell_Area.single;
        cooldown_max = 3;
        spellCastRange = 2;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_Heal(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        //yield return GameMain.inst.Server_SpellHeal(hex, spellDmg); // Server is blocked
        yield return null;
    }
}
