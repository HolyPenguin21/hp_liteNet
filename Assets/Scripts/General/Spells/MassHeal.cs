using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassHeal : Spell
{
    private int healValue;

    public MassHeal(int healValue)
    {
        spellId = 3;
        spellName = "Mass heal";
        spellArea = Utility.spell_Area.circle;
        cooldown_max = 5;
        spellCastRange = 1;

        this.healValue = healValue;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_MassHeal(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellHeal(hex, healValue);
    }
}
