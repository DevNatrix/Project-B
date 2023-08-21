using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
	public static float damageMult = 1;
	public static float healthMult = 1;
	public static float fireRateMult = 1;
	public static float speedMult = 1;
	public static float dashMult = 1;
	public static float reloadMult = 1;
	public static float clipSizeMult = 1;

	public float damageStep = 1;
	public float healthStep = 1;
	public float fireRateStep = 1;
	public float speedStep = 1;
	public float dashStep = 1;
	public float reloadStep = 1;
	public float clipSizeStep = 1;

    public void damage()
	{
		damageMult += damageStep;
	}
	public void health()
	{
		healthMult += healthStep;
	}
	public void fireRate()
	{
		fireRateMult += fireRateStep;
	}
	public void speed()
	{
		speedMult += speedStep;
	}
	public void dash()
	{
		dashMult += dashStep;
	}
	public void reload()//do later
	{
		reloadMult += reloadStep;
	}
	public void clipSize()
	{
		clipSizeMult += clipSizeStep;
	}

	public void resetUpgrades()
	{
		damageMult = 1;
		healthMult = 1;
		fireRateMult = 1;
		speedMult = 1;
		dashMult = 1;
		reloadMult = 1;
		clipSizeMult = 1;
	}
}
