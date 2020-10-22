using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellData : MonoBehaviour
{
    public Spell Get_Spell_ById(int spellId)
	{
		Spell spell = null;

		switch (spellId)
		{
			case 1:
				spell = new Flame(0);
				break;
			case 2:
				spell = new EarthSpike(0);
				break;
            case 3:
				spell = new MassHeal(0);
				break;
            case 4:
				spell = new Heal(0);
				break;
            case 5:
                spell = new Blink(0);
                break;
            case 6:
                spell = new SummonZombie();
                break;
            case 7:
                spell = new SummonBat();
                break;
            case 8:
                spell = new SummonFireEmber();
                break;
        }

		return spell;
	}

    public List<Hex> Get_ConcernedHexes(Hex targetHex, int spellId)
    {
        Spell spell = Get_Spell_ById(spellId);
        List<Hex> concernedHexes = new List<Hex>();

        switch (spell.spellArea)
        {
            case Utility.spell_Area.single:
                concernedHexes.Add(targetHex);
            break;

            case Utility.spell_Area.circle:
                concernedHexes.Add(targetHex);
                foreach(Hex h in targetHex.neighbors)
                    concernedHexes.Add(h);
            break;

            case Utility.spell_Area.cone:

            break;
        }

        return concernedHexes;
    }

    public Spell Get_Spell_ById(Character c, int spellId)
    {
        if(c.charSpell_1 != null && c.charSpell_1.spellId == spellId)
			return c.charSpell_1;
		if(c.charSpell_2 != null && c.charSpell_2.spellId == spellId)
			return c.charSpell_2;
        
        return null;
    }

    public bool InRange(Hex selectedHex, Hex someHex, Spell someSpell)
    {
        float dist = Vector3.Distance(selectedHex.transform.position, someHex.transform.position);
        if(dist <= Utility.distHexes * someSpell.spellCastRange)
            return true;

        return false;
    }
}
