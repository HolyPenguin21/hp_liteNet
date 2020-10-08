using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell
{
    public int spellId;
    public string spellName;
    public int spellDmg;
    public Utility.spell_Area spellArea;
    public int cooldown_cur;
    public int cooldown_max;
    public int spellCastRange;

    public abstract void Use(Vector3 pos);

    public abstract IEnumerator ResultingEffect(Hex casterHex, Hex hex);

    public void CooldownUpdate()
    {
        if(cooldown_cur > 0)
            cooldown_cur --;
    }
}
