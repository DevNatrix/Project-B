using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject smokePrefab;
	public float secondsAlive;
	public ServerEvents serverEvents;
	public float muzzleFlashTime;
	public Transform weaponHolder;

	public void spawnBullet(Vector3 position, Vector3 velocity, Transform effectParent = null)
	{
		GameObject newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
		newBullet.GetComponent<Rigidbody>().velocity = velocity;
		Destroy(newBullet, secondsAlive);

		if(effectParent != null)
		{
			GameObject flash = Instantiate(muzzleFlashPrefab, position, Quaternion.identity, effectParent);
			Instantiate(smokePrefab, position, Quaternion.identity, effectParent);

			Destroy(flash, muzzleFlashTime);
		}
		else
		{
			GameObject flash = Instantiate(muzzleFlashPrefab, position, Quaternion.identity);
			Instantiate(smokePrefab, position, Quaternion.identity);

			Destroy(flash, muzzleFlashTime);
		}
	}

	public void createBullet(Vector3 position, Vector3 velocity)
	{
		serverEvents.sendEventToOtherClients("spawnBulletEvent", new string[] { position + "", velocity + "" });

		spawnBullet(position, velocity, weaponHolder);
	}
}
