using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public LayerMask groundMask;

	private void OnTriggerEnter(Collider otherColl)
	{
		if(otherColl.gameObject.isStatic)
		{
			Destroy(this.gameObject);
		}
	}

	public static bool IsLayerInMask(LayerMask mask, int layerIndex)
	{
		return (mask.value & (1 << layerIndex)) != 0;
	}
}
