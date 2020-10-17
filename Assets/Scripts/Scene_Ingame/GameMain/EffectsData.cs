using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsData : MonoBehaviour
{
    public GameObject effectVillageHeal;
	public GameObject effectDamage;
	public GameObject effectLightning;
	public GameObject effectEarthMedium;
	public GameObject effectEarthWeak;
	public GameObject effectEarthSpike;
	public GameObject effectFlame;
	public GameObject effectMassHeal;
	public GameObject effectHeal;
	public GameObject effectDarkPortal;

    public void Effect_Lightning(Vector3 pos)
	{
		Instantiate(effectLightning, pos, Quaternion.identity);
	}

	public void Effect_VillageHeal(Vector3 pos, int value)
	{
		Instantiate(effectVillageHeal, pos, Quaternion.identity).GetComponent<DamageDone>().Init(value);
	}

	public void Effect_Damage(Vector3 pos, int value)
	{
		Instantiate(effectDamage, pos, Quaternion.identity).GetComponent<DamageDone>().Init(value);
	}

	public void Effect_EarthMedium(Vector3 pos)
	{
		Instantiate(effectEarthMedium, pos, Quaternion.identity);
	}

	public void Effect_EarthSpike(Vector3 pos)
	{
		Instantiate(effectEarthSpike, pos, Quaternion.identity);
	}

	public void Effect_EarthWeak(Vector3 pos)
	{
		Instantiate(effectEarthWeak, pos, Quaternion.identity);
	}

	public void Effect_Flame(Vector3 pos)
	{
		Instantiate(effectFlame, pos, Quaternion.identity);
	}

	public void Effect_MassHeal(Vector3 pos)
	{
		Instantiate(effectMassHeal, pos, Quaternion.identity);
	}

	public void Effect_Heal(Vector3 pos)
	{
		Instantiate(effectHeal, pos, Quaternion.identity);
	}

	public void Effect_DarkPortal(Vector3 pos, Transform parent)
	{
		Instantiate(effectDarkPortal, pos, Quaternion.identity, parent);
	}
}
