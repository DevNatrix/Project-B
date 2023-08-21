using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
	public static int damageMult = 1;
	public static int healthMult = 1;
	public static int fireRateMult = 1;
	public static int speedMult = 1;
	public static int dashMult = 1;
	public static int reloadMult = 1;
	public static int clipSizeMult = 1;

	public int step = 1;

    public void damage()
	{
		damageMult += step;
	}
	public void health()
	{
		healthMult += step;
	}
	public void fireRate()
	{
		fireRateMult += step;
	}
	public void speed()
	{
		speedMult += step;
	}
	public void dash()
	{
		dashMult += step;
	}
	public void reload()//do later
	{
		reloadMult += step;
	}
	public void clipSize()
	{
		clipSizeMult += step;
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
