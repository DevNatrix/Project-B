using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject bulletPrefab;
	public float secondsAlive;
	public ServerEvents serverEvents;

	public void spawnBullet(Vector3 position, Vector3 velocity)
	{
		GameObject newBullet;
		newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
		newBullet.GetComponent<Rigidbody>().velocity = velocity;

		Destroy(newBullet, secondsAlive);
	}

	public void createBullet(Vector3 position, Vector3 velocity)
	{
		serverEvents.sendEventToOtherClients("spawnBulletEvent", new string[] { position + "", velocity + "" });

		spawnBullet(position, velocity);
	}
}
