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

	public Transform player;

	public void spawnBullet(Vector3 position, Vector3 velocity, Transform flashParent = null)
	{
		GameObject newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
		newBullet.GetComponent<Rigidbody>().velocity = velocity;

		GameObject flash = Instantiate(muzzleFlashPrefab, position, Quaternion.identity, flashParent);
		GameObject smoke = Instantiate(smokePrefab, position, Quaternion.identity, flashParent);

		Destroy(flash, muzzleFlashTime);
		Destroy(newBullet, secondsAlive);
	}

	public void createBullet(Vector3 position, Vector3 velocity)
	{
		serverEvents.sendEventToOtherClients("spawnBulletEvent", new string[] { position + "", velocity + "" });

		spawnBullet(position, velocity, player);
	}
}
