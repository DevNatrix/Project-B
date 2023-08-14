using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public static int team;
	public List<Transform> spawnPoints;

	public int health;
	public TextMeshProUGUI healthText;

	public ServerEvents serverEvents;

    private void Start()
    {
		SetHealth(100);
    }

    public void TakeDamage(int damage)
	{
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

	public void setTeam(int _team)
	{
		team = _team;
		Debug.Log("Joined team " + team);

		serverEvents.sendEventToOtherClients("setClientTeam", new string[] { Client.ID + "", team + "", "false" });
	}
}
