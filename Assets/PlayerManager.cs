using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public int team;
	public List<Transform> spawnPoints;

	public int health;
	public TextMeshProUGUI healthText;

	public ServerEvents serverEvents;

	public void TakeDamage(int damage)
	{
		//Debug.Log("You got damaged: " + damage);
		SetHealth(health - damage);
	}

	public void SetHealth(int _health)
	{
		health = _health;

		if (health <= 0)
		{
			Debug.Log("You Died");

			health = 100;

			respawn();
		}

		healthText.text = health.ToString();

		string[] sendData = { Client.ID + "", health + "" };
		serverEvents.sendEventToOtherClients("setHealth", sendData);
	}

	public void respawn()
	{
		transform.position = spawnPoints[team].position;
	}
}
