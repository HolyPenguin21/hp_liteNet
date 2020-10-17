using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Spell
{
    private int healValue;

    public Heal(int healValue)
    {
        spellId = 4;
        spellName = "Heal";
        description = "Heals character in target hex.";
        spellArea = Utility.spell_Area.single;
        cooldown_max = 3;
        spellCastRange = 2;

        this.healValue = healValue;
    }

    public override void Use(Vector3 pos)
    {
        GameMain.inst.effectsData.Effect_Heal(pos);
        cooldown_cur = cooldown_max;
    }

    public override IEnumerator ResultingEffect(Hex casterHex, Hex hex)
    {
        yield return GameMain.inst.Server_SpellHeal(hex, healValue);
    }
}
